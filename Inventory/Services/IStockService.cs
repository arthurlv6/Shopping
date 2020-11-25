using Microsoft.AspNetCore.Http;
using Shared;
using TSP.Shared;

namespace Inventory
{
    public interface IStockService
    {
        Maybe<StockDto> NewOrder(HttpRequest req);
    }
}