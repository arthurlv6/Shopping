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

namespace Inventory
{
    public class StockService : IStockService
    {
        private readonly IStockRepo _stockRepo;

        public StockService(IStockRepo stockRepo)
        {
            _stockRepo = stockRepo;
        }
        public Maybe<StockDto> NewOrder(HttpRequest req)
        {
            return req.TryGetMaybeObject(r =>
            {
                return new StreamReader(r.Body).ReadToEndAsync().Result
                    .Map(s => JsonConvert.DeserializeObject<CartDto>(s))
                    .Map(d => d.Lines.FirstOrDefault())
                    .Map(l => new StockDto() { Name = l.Name, Quantity = l.Quantity });
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
