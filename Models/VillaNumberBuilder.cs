namespace apiprac
{
    public class VillaNumberFilterBuilder : IVillaNumberFilterBuilder
    {
        private string _specialDetails;
        private Guid _villaNo;
        private Guid _villaId;

        public IVillaNumberFilterBuilder setSpecialDetails(string SpecialDetails)
        {
            _specialDetails = SpecialDetails;
            return this;
        }

        public IVillaNumberFilterBuilder setVillaId(Guid id)
        {
            _villaId = id;
            return this;
        }

        public IVillaNumberFilterBuilder setVillaNo(Guid villaNo)
        {
            _villaNo = villaNo;
            return this;
        }

        public VillaNumber Build()
        {
            return new VillaNumber()
            {
                SpecialDetails = _specialDetails,
                VillaNo = _villaNo,
                VillaId = _villaId,
            };
        }
    }
}
