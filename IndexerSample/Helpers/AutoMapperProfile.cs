
using AutoMapper;
using IndexerSample.Entities;
using IndexerSample.Models;

namespace ExoduswhalesService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Token, InvertedIndex>()
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Term));
            CreateMap<Token, string>().
                ConvertUsing(src => src.Term.ToString());
        }
    }
}
