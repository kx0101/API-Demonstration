namespace apiprac
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(Guid id, VillaNumber entity);

        Task<List<VillaNumber>> FindByCriteriaAsync(VillaNumber entity, int page, int pageSize);
    }
}
