using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using fundaconsole.Model;
using fundaconsole.Abstractions;

namespace fundaconsole.Services
{
    public class FeedService : IFeedService
    {
        FeedServiceClient client;
        public FeedService()
        {
            this.client = new FeedServiceClient();
        }

        private IEnumerable<Makelaar> MapReduceMakelaars(List<RealEstate> realEstates, int top = 10)
        {
            IEnumerable<Makelaar> makelaars = from realEstate in realEstates
                            group realEstate by new {
                                MakelaarId = realEstate.MakelaarId,
                                MakelaarNaam = realEstate.MakelaarNaam
                            } into grp
                            let count = grp.Count()
                            orderby count descending
                            select new Makelaar
                            {
                                NumberOfItems = count,
                                MakelaarId = grp.Key.MakelaarId,
                                MakelaarNaam = grp.Key.MakelaarNaam
                            };

            return makelaars;
        }

        public async Task<IEnumerable<Makelaar>> GetMakelaars(bool onlyHasGarden, int top=10)
        {
            var realEstates = new List<RealEstate>();
            int pageSize = 20;
            int page = 1;

            while (page != 0)
            {
                var feedServiceResponse = await client.GetRealEstates(onlyHasGarden, page, pageSize);
                realEstates.AddRange(feedServiceResponse.Objects);

                page++;

                if (page > feedServiceResponse.Paging.AantalPaginas)
                    page = 0;

            }

            return MapReduceMakelaars(realEstates, top);

        }
    }
}
