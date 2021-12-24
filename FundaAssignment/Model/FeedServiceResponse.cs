using System.Collections.Generic;

namespace fundaconsole.Model
{
    public class FeedServiceResponse
    {
        public int AccountStatus { get; set; }
        public bool EmailNotConfirmed { get; set; }
        public bool ValidationFailed { get; set; }
        public string ValidationReport { get; set; }
        public int Website { get; set; }
        public Metadata Metadata { get; set; }
        public Paging Paging { get; set; }
        public int TotaalAantalObjecten { get; set; }
        public List<RealEstate> Objects { get; set; }
    }
}
