using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sms.Models;
using sms.Models.Dto;
using sms.Repository.IRepository;
using System.Net;

namespace sms.Controllers
{
   
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public StudentAPIController(IStudentRepository studentRepository, IMapper mapper)
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
            IEnumerable<Student> studentListDTO = null;

            try
            {
                studentListDTO = await _studentRepository.GetAll();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<List<Student>>(studentListDTO);
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
        public async Task<ActionResult<APIResponse>> GetStudent(int RollNo, int Class) 
        {
            if (RollNo == 0 && Class == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var student = await _studentRepository.Get(RollNo, Class);

                if (student == null) // not checking for null!!!!!!!!!!!!!!!!!!!!!
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Student record not found" }; 
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<Student>(student);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

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

                if (check == true)
                {
                    ModelState.AddModelError("ErrorMessages", "Student detail already Exists!");
                    return BadRequest(ModelState); 
                }
                else
                {
                    Student studentObj2 = await _studentRepository.Create(studentObj1);

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

        [HttpDelete]
        [Route("api/StudentAPI/RemoveStudent")] 
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> RemoveStudent(int RollNo, int Class)
        {
            if (RollNo == 0 && Class == 0) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                bool flag = _studentRepository.RemoveCheck(RollNo, Class); // rows will not be affected

                if (flag == false) 
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "There is no record" }; // should not expose the soft delete to client
                    return _response;
                }

                await _studentRepository.Remove(RollNo, Class);

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

        [HttpPut]
        [Route("api/StudentAPI/UpdateStudent")] 
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> UpdateStudent(int RollNo, int Class, [FromBody] StudentUpdateDTO studentUpdateDTO)
        {
            if (RollNo == 0 && Class == 0) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                bool flag = _studentRepository.UpdateCheck(RollNo, Class);

                if (flag == false)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Student record not found" }; 
                    return NotFound(_response);
                }

                Student student = _mapper.Map<Student>(studentUpdateDTO);

                await _studentRepository.Update(RollNo, Class, student);

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
