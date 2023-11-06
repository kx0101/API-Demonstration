namespace apiprac
{
    public class VillaFilterBuilder : IVillaFilterBuilder
    {
        private int _rate;
        private int _sqft;
        private string _name;

        public IVillaFilterBuilder setName(string name)
        {
            _name = name;
            return this;
        }

        public IVillaFilterBuilder setRate(int rate)
        {
            _rate = rate;
            return this;
        }

        public IVillaFilterBuilder setSqft(int sqft)
        {
            _sqft = sqft;
            return this;
        }

        public Villa Build()
        {
            return new Villa()
            {
                Rate = _rate,
                Sqft = _sqft,
                Name = _name,
            };
        }
    }
}
