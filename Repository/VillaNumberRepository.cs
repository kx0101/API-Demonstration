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
    }
}

