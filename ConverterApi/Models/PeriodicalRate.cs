namespace ConverterApi.Models
{
    public class PeriodicalRate
    {
        public string Base { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
    }
}
