namespace apiprac
{
    public interface IVillaFilterBuilder
    {
        IVillaFilterBuilder setRate(int rate);

        IVillaFilterBuilder setSqft(int sqft);

        IVillaFilterBuilder setName(string name);

        Villa Build();
    }
}
