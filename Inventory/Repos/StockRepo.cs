using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSP.Shared;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Inventory
{
    public class StockRepo: BaseRepo
    {
        public StockRepo(AppDbContext _dBContext, IMapper _mapper) : base(_dBContext, _mapper)
        {
        }
        public async Task<StockDto> AddAsync(StockDto model)
        {
            Stock detail = mapper.Map<Stock>(model);
            var addedEntity = dBContext.Stocks.Add(detail);
            try
            {
                await dBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return addedEntity.Entity.ToModel<StockDto>(mapper);
        }
    }
}
