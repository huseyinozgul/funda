using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using fundaconsole.Model;
using System.Collections.Specialized;
using Newtonsoft.Json;
using fundaconsole.Helper;
using fundaconsole.Abstractions;

namespace fundaconsole.Services
{
    public class FeedServiceClient : IFeedServiceClient
    {

        #region Private Properties

        private readonly object counterLock = new object();
        private HttpClient httpClient;
        private DateTime lastRequestTime;
        private Uri baseUri;
        private int counter;

        #endregion

        #region Constructor

        public FeedServiceClient()
        {
            this.httpClient = new HttpClient();
            var appSettings = ConfigurationManager.AppSettings;

            string servicePath = appSettings.Get("feedServicePath");
            string apiKey = appSettings.Get("feedServiceApiKey");

            var baseUri = new Uri(servicePath);
            this.baseUri = new Uri(baseUri, apiKey);

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

            if (counter >= 100 && timeSpan.TotalSeconds <= 60)
            {
                throw new Exception("Too many API requests in a short period!");
            }

            lock (counterLock)
            {
                if (timeSpan.TotalSeconds > 60)
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
