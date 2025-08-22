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
        CreateMap<AdminAndModeratorDto, AppUser>();

        // For reading from DB → sending to client
        CreateMap<UniProgram, UniProgramDto>();

        // For inserting/updating → mapping DTO to entity
        CreateMap<UniProgramDto, UniProgram>();
        CreateMap<PersonalInformationDto, UserData>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.LinkedinLink, opt => opt.MapFrom(src => src.LinkedinLink));
        CreateMap<PersonalStatementsDto, UserData>()
            .ForMember(dest => dest.Motivation, opt => opt.MapFrom(src => src.Motivation))
            .ForMember(dest => dest.LifeOutSide, opt => opt.MapFrom(src => src.LifeOutSide));
        CreateMap<EducationBackgroundDto, UserData>()
            .ForMember(dest => dest.BaccalaureatDegree, opt => opt.MapFrom(src => src.BaccalaureatDegree))
            .ForMember(dest => dest.BaccalaureatInstitution, opt => opt.MapFrom(src => src.BaccalaureatInstitution))
            .ForMember(dest => dest.BaccalaureatDate, opt => opt.MapFrom(src => src.BaccalaureatDate))
            .ForMember(dest => dest.BachelorDegree, opt => opt.MapFrom(src => src.BachelorDegree))
            .ForMember(dest => dest.BachelorInstitution, opt => opt.MapFrom(src => src.BachelorInstitution))
            .ForMember(dest => dest.BachelorDate, opt => opt.MapFrom(src => src.BachelorDate))
            .ForMember(dest => dest.MasterDegree, opt => opt.MapFrom(src => src.MasterDegree))
            .ForMember(dest => dest.MasterInstitution, opt => opt.MapFrom(src => src.MasterInstitution))
            .ForMember(dest => dest.MasterDate, opt => opt.MapFrom(src => src.MasterDate))
            .ForMember(dest => dest.EngDegree, opt => opt.MapFrom(src => src.EngDegree))
            .ForMember(dest => dest.EngInstitution, opt => opt.MapFrom(src => src.EngInstitution))
            .ForMember(dest => dest.EngDate, opt => opt.MapFrom(src => src.EngDate));

        CreateMap<WorkExperienceDto, UserData>()
            .ForMember(dest => dest.WorkExperience, opt => opt.MapFrom(src => src.WorkExperience));


        CreateMap<UserDataDto, UserData>();
        CreateMap<Document, DocumentDto>();

        CreateMap<UserData, UserDataDto>();
        CreateMap<Application, ApplicationDto>();
    }
}
