using AutoMapper;
using sms.Models;
using sms.Models.Dto;

namespace sms
{
    public class mappingconfig : Profile
    {
        public mappingconfig()
        {
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<Student, StudentUpdateDTO>().ReverseMap();
            CreateMap<StudentDTO, StudentUpdateDTO>().ReverseMap();
        }
    }
}
