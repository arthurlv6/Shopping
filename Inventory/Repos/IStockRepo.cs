using Shared;
using System.Threading.Tasks;
using TSP.Shared;

namespace Inventory
{
    public interface IStockRepo
    {
        Task<Maybe<StockDto>> AddAsync(StockDto model);
        Task<Maybe<bool>> IsExistAsync(string name);
        Task<Maybe<StockDto>> UpdateAsync(StockDto model);
    }
}