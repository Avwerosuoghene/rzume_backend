using System;
using AutoMapper;
using RzumeAPI.Models;
using RzumeAPI.Models.DO;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Configurations
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<OtpDTO, Otp>().ReverseMap();

            CreateMap<UserFileDTO, UserFile>().ReverseMap();

            CreateMap<EducationDTO, Education>().ReverseMap();

            CreateMap<ExperienceDTO, Experience>().ReverseMap();

            CreateMap<CountryDTO, Country>().ReverseMap();
        }
    }
}

