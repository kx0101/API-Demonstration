namespace apiprac
{
    public interface IVillaNumberFilterBuilder
    {
        IVillaNumberFilterBuilder setSpecialDetails(string SpecialDetails);

        IVillaNumberFilterBuilder setVillaNo(Guid villaNo);

        IVillaNumberFilterBuilder setVillaId(Guid id);

        VillaNumber Build();
    }
}
