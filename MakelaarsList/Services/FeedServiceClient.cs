#region Used Libraries
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using MakelaarsList.Helper;
using MakelaarsList.Abstractions;
using MakelaarsList.Model;
#endregion

namespace MakelaarsList.Services
{
    public class FeedServiceClient : IFeedServiceClient
    {

        #region Private Properties

        private readonly IConfigurationRoot config;
        private readonly ILogger<FeedServiceClient> logger;

        private readonly object counterLock = new object();
        private HttpClient httpClient;
        private DateTime lastRequestTime;
        private Uri baseUri;
        private int counter;
        private int treshold;
        private int tresholdTimeAsSecond;

        #endregion

        #region Constructor

        public FeedServiceClient(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {

            this.config = config;
            this.logger = loggerFactory.CreateLogger<FeedServiceClient>();

            this.httpClient = new HttpClient();

            string baseUrl = config.GetValue<string>("FeedService:BaseUrl");
            string apiKey = config.GetValue<string>("FeedService:ApiKey");
            var baseUri = new Uri(baseUrl);
            this.baseUri = new Uri(baseUri, apiKey);

            this.treshold = config.GetValue<int>("FeedService:Treshold");
            this.tresholdTimeAsSecond = config.GetValue<int>("FeedService:TresholdTimeAsSecond");
            
        }

        #endregion

        #region Private Methods

        private void CheckTreshold()
        {
            if (lastRequestTime.Year == 1)
            {
                lastRequestTime = DateTime.Now;
            }

            var currentTime = DateTime.Now;
            TimeSpan timeSpan = currentTime - lastRequestTime;

            if (counter >= treshold && timeSpan.TotalSeconds <= tresholdTimeAsSecond)
            {
                throw new Exception("Too many API requests in a short period!");
            }

            lock (counterLock)
            {
                if (timeSpan.TotalSeconds > tresholdTimeAsSecond)
                {
                    counter = 0;
                    lastRequestTime = DateTime.Now;
                }

                counter++;
            }
        }

        private Uri getRequestUri(bool onlyHasGarden, int page, int pageSize)
        {
            NameValueCollection queryStrings = new NameValueCollection();

            queryStrings.Add("type", "koop");
            queryStrings.Add("zo", "/amsterdam" + (onlyHasGarden ? "/tuin" : ""));
            queryStrings.Add("page", page.ToString());
            queryStrings.Add("pagesize", pageSize.ToString());

            return new Uri(this.baseUri, QueryStringHelper.ToQueryString(queryStrings));

        }

        #endregion

        #region Public Methods / Implementations

        public async Task<FeedServiceResponse> GetRealEstates(bool onlyHasGarden, int page, int pageSize = 10)
        {
            CheckTreshold();

            Uri requestUri = getRequestUri(onlyHasGarden, page, pageSize);

            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            FeedServiceResponse feedServiceResponse = JsonConvert.DeserializeObject<FeedServiceResponse>(json);

            return feedServiceResponse;

        }

        #endregion

    }
}
