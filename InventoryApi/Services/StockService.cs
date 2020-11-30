using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Shared;

namespace InventoryApi
{
    public class StockService : IStockService
    {
        private readonly IStockRepo _stockRepo;

        public StockService(IStockRepo stockRepo)
        {
            _stockRepo = stockRepo;
        }
        public Maybe<StockDto> NewOrder(CartDto req)
        {
            return req.TryGetMaybeObject(r =>
            {
                var temp = r.Lines.FirstOrDefault();
                return new StockDto() { Name = temp.Name, Quantity = temp.Quantity };
            })
            .Railway(dto =>
            {
                var isExist = _stockRepo.IsExistAsync(dto.Value.Name).Result;
                if (isExist.IsSuccess && isExist.Value)
                {
                    return Maybe.Ok(_stockRepo.UpdateAsync(dto.Value).Result.Value);
                }
                if (isExist.IsSuccess && !isExist.Value)
                {
                    return Maybe.Ok(_stockRepo.AddAsync(dto.Value).Result.Value);
                }
                return Maybe.Fail<StockDto>(isExist.Error);
            });
        }
    }
}
