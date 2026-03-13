using AutoMapper;
using FileService.Application.DTOs.BlobDTOs;
using FileService.Domain.Models;

namespace FileService.Application.Mapper;

public class FileMappingProfile : Profile
{
    public FileMappingProfile()
    {
        CreateMap<UserFileMetadata, FileResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Url, opt => opt.Ignore());
    }
}