namespace ConverterApi.Models
{
    public class ExchangeRate
    {
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
