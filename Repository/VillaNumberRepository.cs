using Microsoft.EntityFrameworkCore;

namespace apiprac
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDBContext _db;

        public VillaNumberRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public async Task<VillaNumber> UpdateAsync(Guid id, VillaNumber entity)
        {
            var villaNumber = await _db.VillaNumbers.FindAsync(id);

            villaNumber.VillaNo = entity.VillaNo;
            villaNumber.SpecialDetails = entity.SpecialDetails;

            await _db.SaveChangesAsync();

            return villaNumber;
        }

        public async Task<List<VillaNumber>> FindByCriteriaAsync(VillaNumber filter, int page, int PageSize)
        {
            IQueryable<VillaNumber> query = dbSet;

            if (filter.SpecialDetails != null)
            {
                query = query.Where(villaNumber => villaNumber.SpecialDetails == filter.SpecialDetails);
            }

            if (filter.VillaNo != null && filter.VillaNo != Guid.Empty)
            {
                query = query.Where(villaNumber => villaNumber.VillaNo == filter.VillaNo);
            }

            if (filter.VillaId != null && filter.VillaId != Guid.Empty)
            {
                query = query.Where(villaNumber => villaNumber.VillaId == filter.VillaId);
            }

            query = query.Skip((page - 1) * PageSize).Take(PageSize);

            return await query.ToListAsync();
        }
    }
}

