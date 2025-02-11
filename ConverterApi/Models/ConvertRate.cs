namespace ConverterApi.Models
{
    public class ConvertRate
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }

        public Dictionary<string, decimal> Rates { get; set; }
        public string Date { get; set; }
    }
}
