using System;
using System.Threading.Tasks;
using fundaconsole.Model;

namespace fundaconsole.Abstractions
{
    public interface IFeedServiceClient
    {
        Task<FeedServiceResponse> GetRealEstates(bool onlyHasGarden, int page, int pageSize);
    }
}