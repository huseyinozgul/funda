using System;
namespace fundaconsole.Model
{
    public class RealEstate
    {
        public string Id { get; set; }
        public int MakelaarId { get; set; }
        public string MakelaarNaam { get; set; }
        public int? AantalKamers { get; set; }
        public Double? Koopprijs { get; set; }
        public string WoonplaatsAmsterdam { get; set; }
    }
}
