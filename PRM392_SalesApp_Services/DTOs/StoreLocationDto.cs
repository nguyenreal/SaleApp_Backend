namespace PRM392.SalesApp.Services.DTOs
{
    public class StoreLocationDto
    {
        public int LocationID { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}