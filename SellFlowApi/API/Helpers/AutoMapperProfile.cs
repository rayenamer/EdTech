using System;
using API.Dtos;
using API.entities;
using API.Entities;
using AutoMapper;
using Google.Apis.Drive.v3.Data;
using Microsoft.Win32;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, AppUserDto>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<AdminAndModeratorDto, AppUser>();

        // For reading from DB → sending to client
        CreateMap<UniProgram, UniProgramDto>();

        // For inserting/updating → mapping DTO to entity
        CreateMap<UniProgramDto, UniProgram>();

        CreateMap<ApplicationDto, Application>()
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));
            
        CreateMap<Document, DocumentDto>();
        CreateMap<DocumentDto, Document>()
            .ForMember(dest => dest.ApplicationId, opt => opt.Ignore()); // Ignore ApplicationId as it will be set by EF

        CreateMap<Application, ApplicationDto>()
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));
    }
}
