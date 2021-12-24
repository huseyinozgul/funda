using System.Collections.Generic;
using System.Threading.Tasks;
using MakelaarsList.Model;

namespace MakelaarsList.Abstractions
{
    public interface IFeedService
    {
        Task ShowMakelaars(bool onlyHasGarden, int top);

    }
}
