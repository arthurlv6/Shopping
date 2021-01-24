using Microsoft.AspNetCore.Http;
using Shared;
using System.Threading.Tasks;
using TSP.Shared;

namespace Inventory
{
    public interface IStockService
    {
        Maybe<StockDto> NewOrder(HttpRequest req);
        Task<Maybe<StockDto>> NewOrderTemp(HttpRequest req);
    }
}