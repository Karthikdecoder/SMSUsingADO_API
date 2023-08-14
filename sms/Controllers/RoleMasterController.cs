using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sms.Models;
using sms.Repository;
using sms.Repository.IRepository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace sms.Controllers
{
    [Route("api/RoleAPI")]
    [ApiController]
    public class RoleMasterController : ControllerBase
    {
        private readonly IRoleMasterRepository _roleMasterRepository;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public RoleMasterController(IRoleMasterRepository roleMasterRepository, IMapper mapper)
        {
            _roleMasterRepository = roleMasterRepository;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [Route("GetRoles")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetRoles()
        {
            IEnumerable<RoleMasterDTO> rolesListDTO = null;

            try
            {
                rolesListDTO = await _roleMasterRepository.GetAll();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<List<RoleMasterDTO>>(rolesListDTO);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }

            return _response;
        }

        [HttpGet]
        [Route("GetRole")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetRoles(int roleId)
        {
            if (roleId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var role = await _roleMasterRepository.Get(roleId);

                if (role == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Role record not found" };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<RoleMasterDTO>(role);
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
        [Route("CreateRole")]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> CreateRole([FromBody] RoleMasterCUDTO roleMaster)
        {
            if (roleMaster == null)
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
                RoleMasterCUDTO roleMasterObj1 = _mapper.Map<RoleMasterCUDTO>(roleMaster);

                bool check = _roleMasterRepository.CreateCheck(roleMasterObj1);

                if (check == false)
                {
                    ModelState.AddModelError("ErrorMessages", "Role detail already Exists!");
                    return BadRequest(ModelState);
                }
                else
                {
                    string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                    RoleMasterCUDTO roleMasterObj2 = await _roleMasterRepository.Create(roleMasterObj1, username);

                    RoleMasterCUDTO result = _mapper.Map<RoleMasterCUDTO>(roleMasterObj2);

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
        [Route("RemoveRole")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> RemoveRole(int roleId)
        {
            if (roleId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                bool flag = _roleMasterRepository.RemoveCheck(roleId);

                if (flag == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "There is no record" };
                    return _response;
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _roleMasterRepository.Remove(roleId, username);

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
        [Route("UpdateRole")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> UpdateRole(int roleId, [FromBody] RoleMasterCUDTO roleMasterCUDTO)
        {
            if (roleId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (roleMasterCUDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "You entered empty input" };
                return BadRequest(_response);
            }

            try
            {
                bool flag1 = _roleMasterRepository.UpdateCheck(roleId);

                if (flag1 == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Class record not found" };
                    return NotFound(_response);
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _roleMasterRepository.Update(roleId, roleMasterCUDTO, username);

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
