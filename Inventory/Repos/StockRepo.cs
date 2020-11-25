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
        public async Task<Maybe<StockDto>> AddAsync(StockDto model)
        {
            try
            {
                Stock detail = mapper.Map<Stock>(model);
                var addedEntity = dBContext.Stocks.Add(detail);
                await dBContext.SaveChangesAsync();
                return Maybe.Ok(addedEntity.Entity.ToModel<StockDto>(mapper));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<StockDto>("Can't do AddAsync.", ex);
            }
        }
        public async Task<Maybe<StockDto>> UpdateAsync(StockDto model)
        {
            try
            {
                var existingEntity = await dBContext.Stocks.FirstOrDefaultAsync(d => d.Name == model.Name);
                existingEntity.Quantity = existingEntity.Quantity + model.Quantity;
                await dBContext.SaveChangesAsync();
                return Maybe.Ok<StockDto>(existingEntity.ToModel<StockDto>(mapper));
            }
            catch (Exception ex)
            {
                return Maybe.Fail<StockDto>("Can't do UpdateAsync.", ex);
            }
        }
        public async Task<Maybe<bool>> IsExistAsync(string name)
        {
            try
            {
                var stock = await dBContext.Stocks.FirstOrDefaultAsync(d => d.Name == name);
                if (stock == null)
                    return Maybe.Ok(false);
                return Maybe.Ok(true);
            }
            catch (Exception ex)
            {
                return Maybe.Fail<bool>("Can't check IsExistAsync.", ex);
            }
        }
    }
}
