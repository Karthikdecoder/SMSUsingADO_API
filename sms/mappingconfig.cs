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

            //CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<LoginRequestDTO, UserDTO>().ReverseMap();
            CreateMap<LoginResponseDTO, UserDTO>().ReverseMap();

            CreateMap<Student, StudentGetDTO>().ReverseMap();

            CreateMap<UserTable, UserTableDTO>().ReverseMap();

            CreateMap<ClassMaster, ClassMasterDTO>().ReverseMap();
            CreateMap<ClassMaster, ClassMasterCUDTO>().ReverseMap();

            CreateMap<RoleMaster, RoleMasterDTO>().ReverseMap();
            CreateMap<RoleMaster, RoleMasterCUDTO>().ReverseMap();
        }
    }
}
