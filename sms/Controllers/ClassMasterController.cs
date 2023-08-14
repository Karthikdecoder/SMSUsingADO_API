using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sms.Models;
using sms.Models.Dto;
using sms.Repository;
using sms.Repository.IRepository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace sms.Controllers
{
    [Route("api/ClassAPI")]
    [ApiController]
    public class ClassMasterController : ControllerBase
    {
        private readonly IClassMasterRepository _classMasterRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public ClassMasterController(IClassMasterRepository classMasterRepository, IMapper mapper)
        {
            _classMasterRepository = classMasterRepository;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [Route("GetClasses")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetClasses()
        {
            IEnumerable<ClassMasterDTO> classListDTO = null;

            try
            {
                classListDTO = await _classMasterRepository.GetAll();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<List<ClassMasterDTO>>(classListDTO);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }

            return _response;
        }

        [HttpGet]
        [Route("GetClass")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetClass(int classId)
        {
            if (classId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var student = await _classMasterRepository.Get(classId);

                if (student == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Class record not found" };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<ClassMasterDTO>(student);
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
        [Route("CreateClass")]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> CreateClass([FromBody] ClassMasterCUDTO classMaster)
        {
            if (classMaster == null)
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
                ClassMasterCUDTO classMasterObj1 = _mapper.Map<ClassMasterCUDTO>(classMaster);

                bool check = _classMasterRepository.CreateCheck(classMasterObj1);

                if (check == false)
                {
                    ModelState.AddModelError("ErrorMessages", "Class detail already Exists!");
                    return BadRequest(ModelState);
                }
                else
                {

                    string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                    ClassMasterCUDTO classMasterObj2 = await _classMasterRepository.Create(classMasterObj1, username);

                    ClassMasterCUDTO result = _mapper.Map<ClassMasterCUDTO>(classMasterObj2);

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
        [Route("RemoveClass")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> RemoveClass(int classId)
        {
            if (classId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                bool flag = _classMasterRepository.RemoveCheck(classId);

                if (flag == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "There is no record" };
                    return _response;
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _classMasterRepository.Remove(classId, username);

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

        //[Authorize(Roles = "1")]
        [HttpPut]
        [Route("UpdateClass")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> UpdateClass(int classId, [FromBody] ClassMasterCUDTO classMasterCUDTO)
        {
            if (classId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (classMasterCUDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "You entered empty input" };
                return BadRequest(_response);
            }

            try
            {
                string className = classMasterCUDTO.ClassName;

                bool flag1 = _classMasterRepository.UpdateCheck(classId, className);

                if (flag1 == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Class record not found or Class record already exists with the " + className};
                    return NotFound(_response);
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _classMasterRepository.Update(classId, classMasterCUDTO, username);

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
