namespace apiprac
{
    public interface IVillaRepository : IRepository<Villa>
    {
        Task<Villa> UpdateAsync(Guid id, Villa entity);

        Task<List<Villa>> FindByCriteriaAsync(Villa entity);
    }
}
