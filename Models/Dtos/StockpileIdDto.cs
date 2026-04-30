namespace FoxholeToolsAPI.Models.Dtos
{
    public class StockpileIdDto
    {
        public string Id;
        public string name;
        public string region;
        public string location;
        public string passcode;

        public StockpileIdDto(StockpileModel stockpile)
        {
            Id = stockpile._id;
            name = stockpile.name;
            region = stockpile.region;
            location = stockpile.location;
            passcode = stockpile.passcode;
        }
    }
}
