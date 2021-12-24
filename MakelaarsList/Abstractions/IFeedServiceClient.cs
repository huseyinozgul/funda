using System;
using System.Threading.Tasks;
using MakelaarsList.Model;

namespace MakelaarsList.Abstractions
{
    public interface IFeedServiceClient
    {
        Task<FeedServiceResponse> GetRealEstates(bool onlyHasGarden, int page, int pageSize);
    }
}