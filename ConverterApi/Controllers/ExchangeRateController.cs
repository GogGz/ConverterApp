using ConverterApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConverterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClient;

        public ExchangeRateController(IHttpClientFactory httpClient) 
        {
            _httpClient = httpClient;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency = "EUR", [FromQuery] string symbols = "", [FromQuery] DateTime date = default)
        {
            var client = _httpClient.CreateClient("exchangeapi");

            string url = $"{client.BaseAddress}latest?base={baseCurrency.ToUpper()}";

            if (!string.IsNullOrWhiteSpace(symbols))
            {
                url += $"&symbols={symbols.ToUpper()}"; 
            }

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error fetching exchange rates for {baseCurrency}.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var exchangeRate = JsonSerializer.Deserialize<ExchangeRate>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(exchangeRate);
        }

        [HttpGet("historical/{date}")]
        public async Task<IActionResult> GetHistoricalRates(
       string date,
       [FromQuery] string baseCurrency = "EUR",
       [FromQuery] string symbols = "")
        {
            var client = _httpClient.CreateClient("exchangeapi");

            string url = $"{client.BaseAddress}{date}?base={baseCurrency.ToUpper()}";


            if (!string.IsNullOrWhiteSpace(symbols))
            {
                url += $"&symbols={symbols.ToUpper()}";
            }

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error fetching exchange rates for {baseCurrency}.");
            }

            var json = await response.Content.ReadAsStringAsync();

            var historicalRate = JsonSerializer.Deserialize<ExchangeRate>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(historicalRate);

        }

        [HttpGet("periodical/{startDate}..{endDate}")]
        public async Task<IActionResult> GetPeriodicalRates(
       string startDate,
       string endDate,
       [FromQuery] string baseCurrency = "EUR")
        {
            var client = _httpClient.CreateClient("exchangeapi"); 

            string url = $"{client.BaseAddress}{startDate}..{endDate}?base={baseCurrency.ToUpper()}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error fetching exchange rates for {baseCurrency}.");
            }

            var json = await response.Content.ReadAsStringAsync();

            var periodicalRate = JsonSerializer.Deserialize<PeriodicalRate>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(periodicalRate);

        }

        [HttpGet("present/{startDate}")]
        public async Task<IActionResult> GetToPresentRates(
       string startDate,
       [FromQuery] string baseCurrency = "EUR",
       [FromQuery] string symbols = "")
        {
            var client = _httpClient.CreateClient("exchangeapi");

            string url = $"{client.BaseAddress}{startDate}..?base={baseCurrency.ToUpper()}";

            if (!string.IsNullOrWhiteSpace(symbols))
            {
                url += $"&symbols={symbols.ToUpper()}";
            }

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error fetching exchange rates for {baseCurrency}.");
            }

            var json = await response.Content.ReadAsStringAsync();

            var periodicalRate = JsonSerializer.Deserialize<PeriodicalRate>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            periodicalRate.StartDate = startDate;
            periodicalRate.EndDate = DateTime.Now.ToShortDateString();

            return Ok(periodicalRate);
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetSupportedCurrencies()
        {
            var client = _httpClient.CreateClient("exchangeapi");

            string url = $"{client.BaseAddress}currencies";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error fetching supported currencies.");
            }

            var json = await response.Content.ReadAsStringAsync();

            var currencies = JsonSerializer.Deserialize<Dictionary<string,string>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(currencies);
        } 
        
        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var client = _httpClient.CreateClient("exchangeapi");

            string url = $"{client.BaseAddress}latest?base={from.ToUpper()}&symbols={to.ToUpper()}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error fetching supported currencies.");
            }

            var json = await response.Content.ReadAsStringAsync();

            var exchange = JsonSerializer.Deserialize<ConvertRate>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            exchange.From = from.ToUpper();
            exchange.To = to.ToUpper();
            exchange.Amount = amount;
            exchange.ConvertedAmount = amount * exchange.Rates[to.ToUpper()];
            exchange.Date = DateTime.Now.ToShortDateString();

            var result = $"{exchange.Amount}{exchange.From} = {exchange.ConvertedAmount}{exchange.To}";

            return Ok(exchange);
        }
    }
}
