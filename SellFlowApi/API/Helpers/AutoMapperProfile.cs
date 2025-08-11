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
    }
}
