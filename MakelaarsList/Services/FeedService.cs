#region Used Libraries
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MakelaarsList.Model;
using MakelaarsList.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
#endregion

namespace MakelaarsList.Services
{
    public class FeedService : IFeedService
    {
        #region Private Properties

        private IFeedServiceClient feedServiceClient;
        private readonly IConfigurationRoot config;
        private readonly ILogger<FeedService> logger;

        #endregion

        #region Constructor

        public FeedService(IConfigurationRoot config, ILoggerFactory loggerFactory, IFeedServiceClient feedServiceClient)
        {
            this.config = config;
            this.logger = loggerFactory.CreateLogger<FeedService>();
            this.feedServiceClient = feedServiceClient;
        }

        #endregion

        #region Private Methods

        private IEnumerable<Makelaar> MapReduceMakelaars(List<RealEstate> realEstates, int top)
        {
            IEnumerable<Makelaar> makelaars = (from realEstate in realEstates
                                              group realEstate by new
                                              {
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
                                              }).Take(top);

            return makelaars;
        }

        #endregion

        #region Public Methods / Implementations

        public async Task ShowMakelaars(bool onlyHasGarden, int top = 10)
        {
            var realEstates = new List<RealEstate>();
            int pageSize = 20;
            int page = 1;

            while (page != 0)
            {
                var feedServiceResponse = await feedServiceClient.GetRealEstates(onlyHasGarden, page, pageSize);
                realEstates.AddRange(feedServiceResponse.Objects);

                page++;

                if (page > feedServiceResponse.Paging.AantalPaginas)
                    page = 0;

            }

            var makelaars = MapReduceMakelaars(realEstates, top);

            this.logger.LogInformation("Top {0} Makelaars {1}", top, onlyHasGarden ? " - Which RealEstates have a garden" : "");
            foreach(Makelaar makelaar in makelaars)
            {
                this.logger.LogInformation(makelaar.ToString());
            }

        }

        #endregion
    }
}
