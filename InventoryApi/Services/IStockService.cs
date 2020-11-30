using Microsoft.AspNetCore.Http;
using Shared;
using TSP.Shared;

namespace InventoryApi
{
    public interface IStockService
    {
        Maybe<StockDto> NewOrder(CartDto req);
    }
}