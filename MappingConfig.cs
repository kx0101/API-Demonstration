using AutoMapper;

namespace apiprac
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>();

            CreateMap<VillaDTO, Villa>();

            CreateMap<VillaCreateDTO, Villa>();

            CreateMap<VillaUpdateDTO, Villa>();
        }
    }
}
