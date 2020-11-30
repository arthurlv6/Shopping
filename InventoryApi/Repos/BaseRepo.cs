using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace InventoryApi
{
    public class BaseRepo
    {
        protected readonly AppDbContext dBContext;
        protected readonly IMapper mapper;
        public BaseRepo(AppDbContext _dBContext, IMapper _mapper)
        {
            dBContext = _dBContext;
            mapper = _mapper;
        }
        public virtual async Task<IEnumerable<M>> GetAll<T, M>(int page=1,int size=20,string keyword = "") where T : BaseEntity where M : BaseDto
        {
            return await dBContext.Set<T>()
                .Where(d=>d.Name.Contains(keyword))
                .OrderBy(d=>d.Id)
                .Skip((page -1)*size)
                .Take(size)
                .Select(d => d.ToModel<M>(mapper))
                .ToListAsync();
        }
        public async Task<M> GetOneAsync<T,M>(int id) where T : BaseEntity where M : BaseDto
        {
            var requirement = await dBContext.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
            if (requirement != null)
                return requirement.ToModel<M>(mapper);
            else
                return null;
        }
        public async Task UpdateAsync<T, M>(M m) where T : BaseEntity where M : BaseDto
        {
            try
            {
                var entity = dBContext.Set<T>().First(d => d.Id == m.Id);
                mapper.Map(m, entity);
                await dBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Tuple<IEnumerable<M>, double>> GetPageData<T, M>(int page = 1, int size = 20, string keyword = "") where M : BaseDto where T : BaseEntity
        {
            var queryable = dBContext.Set<T>().OrderBy(d => d.Id).AsQueryable();
            Expression<Func<T, bool>> nameExpected = d => true;
            if (!string.IsNullOrEmpty(keyword))
            {
                nameExpected = d => d.Name.Contains(keyword);
            }

            double count = await queryable.Where(nameExpected).CountAsync();
            double pagesQuantity = Math.Ceiling(count / size);
            var pagination = new PaginationModel() { Page = page, QuantityPerPage = size };
            return new Tuple<IEnumerable<M>, double>(await queryable.Where(nameExpected).Paginate(pagination).Select(d => d.ToModel<M>(mapper)).ToListAsync(), pagesQuantity);
        }
    }
}
