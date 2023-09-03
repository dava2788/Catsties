using AutoMapper;
using Contracts;
using SerachService.Models;

namespace SerachService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }

}
