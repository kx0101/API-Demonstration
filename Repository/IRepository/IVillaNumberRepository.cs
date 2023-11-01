namespace apiprac
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(Guid id, VillaNumber entity);
    }
}
