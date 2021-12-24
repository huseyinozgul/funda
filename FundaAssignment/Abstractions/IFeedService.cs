using System.Collections.Generic;
using System.Threading.Tasks;
using fundaconsole.Model;

namespace fundaconsole.Abstractions
{
    public interface IFeedService
    {
        Task<IEnumerable<Makelaar>> GetMakelaars(bool onlyHasGarden, int top);

    }
}
