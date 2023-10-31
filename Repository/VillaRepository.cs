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
    }
}
