using Microsoft.EntityFrameworkCore;

namespace apiprac
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDBContext _db;

        public VillaRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Villa> UpdateAsync(Guid id, Villa entity)
        {
            var villa = await _db.Villas.FindAsync(id);

            villa.Name = entity.Name;
            villa.Rate = entity.Rate;
            villa.Sqft = entity.Sqft;

            await _db.SaveChangesAsync();

            return villa;
        }

        public async Task<List<Villa>> FindByCriteriaAsync(Villa filter, int page, int PageSize)
        {
            IQueryable<Villa> query = dbSet;

            if (filter.Name != null)
            {
                query = query.Where(v => v.Name == filter.Name);
            }

            if (filter.Rate > 0)
            {
                query = query.Where(v => v.Rate == filter.Rate);
            }

            if (filter.Sqft > 0)
            {
                query = query.Where(v => v.Sqft == filter.Sqft);
            }

            query = query.Skip((page - 1) * PageSize).Take(PageSize);

            return await query.ToListAsync();
        }

    }
}
