using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sms.Models;
using sms.Models.Dto;
using sms.Repository.IRepository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace sms.Controllers
{
   
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public StudentController(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _response = new();
        }


        
        [HttpGet]
        [Route("api/StudentAPI/GetStudents")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetStudents()
        {
            IEnumerable<StudentGetDTO> studentListDTO = null;

            try
            {
                studentListDTO = await _studentRepository.GetAll();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<List<StudentGetDTO>>(studentListDTO);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }

            return _response;
        }

        [HttpGet]
        [Route("api/StudentAPI/GetStudent")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetStudent(int StudentId) 
        {
            if (StudentId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var student = await _studentRepository.Get(StudentId);

                if (student == null) 
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Student record not found" }; 
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<StudentGetDTO>(student);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [Authorize(Roles = "1,2")]
        [HttpPost]
        [Route("api/StudentAPI/CreateStudent")]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> CreateStudent([FromBody] StudentDTO studentDTO)
        {
            if (studentDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "You entered empty input" };
                return BadRequest(_response);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Student studentObj1 = _mapper.Map<Student>(studentDTO);

                bool check =  _studentRepository.CreateCheck(studentObj1);

                if (check == false)
                {
                    ModelState.AddModelError("ErrorMessages", "Student detail already Exists!");
                    return BadRequest(ModelState); 
                }
                else
                {

                    string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                    Student studentObj2 = await _studentRepository.Create(studentObj1, username);

                    Student result = _mapper.Map<Student>(studentObj2);

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = result;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [Authorize(Roles = "1")]
        [HttpDelete]
        [Route("api/StudentAPI/RemoveStudent")] 
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> RemoveStudent(int StudentId)
        {
            if (StudentId == 0) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                bool flag = _studentRepository.RemoveCheck(StudentId); // rows will not be affected

                if (flag == true) 
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "There is no record" }; // should not expose the soft delete to client
                    return _response;
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _studentRepository.Remove(StudentId, username);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;

        }

        [Authorize(Roles = "1")]
        [HttpPut]
        [Route("api/StudentAPI/UpdateStudent")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> UpdateStudent([FromBody] StudentDTO studentDTO) 
        {
            if (studentDTO.StudentId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (studentDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "You entered empty input" };
                return BadRequest(_response);
            }

            int RollNo = int.Parse(studentDTO.RollNo);

            bool flag2 = _studentRepository.RollNoCheck(RollNo, studentDTO.StudentId);

            if (flag2 == false)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "Roll No already exists" };
                return BadRequest(_response);
            }

            try
            {
                bool flag1 = _studentRepository.UpdateCheck(studentDTO.StudentId);

                if (flag1 == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Student record not found" };
                    return NotFound(_response);
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _studentRepository.Update(studentDTO.StudentId, studentDTO, username);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

    }
}
