using Microsoft.AspNetCore.Mvc;
using sms.Models.Dto;
using sms.Models;
using sms.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using sms.Repository;
using AutoMapper;

namespace sms.Controllers
{

    [Route("api/UsersAuthAPI")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _response = new();
            _mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _userRepo.Login(loginRequestDTO);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [Authorize(Roles = "1")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registrationRequestDTO)
            {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(registrationRequestDTO.UserName);

            if (ifUserNameUnique == true)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            string username = User.FindFirstValue(ClaimTypes.SerialNumber);

            var user = await _userRepo.Register(registrationRequestDTO, username);

            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = user;
            return Ok(_response);
        }

        
        [HttpGet]
        [Route("GetUsers")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            IEnumerable<UserTableDTO> UserTableDTOList = null;

            try
            {
                UserTableDTOList = await _userRepo.GetAll();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<List<UserTable>>(UserTableDTOList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }

            return _response;
        }

        [HttpGet]
        [Route("GetUser")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetUser(int ApplicationUserId)
        {
            if (ApplicationUserId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var UserTable = await _userRepo.Get(ApplicationUserId);

                if (UserTable == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Student record not found" };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = _mapper.Map<UserTable>(UserTable);
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
        [Route("RemoveUser")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> RemoveUser(int ApplicationUserId)
        {
            if (ApplicationUserId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                bool flag = _userRepo.RemoveCheck(ApplicationUserId);

                if (flag == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "There is no record" };
                    return _response;
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _userRepo.Remove(ApplicationUserId, username);

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
        [Route("UpdateUser")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<APIResponse>> UpdateUser(int ApplicationUserId, [FromBody] UserTableUpdateDTO userTableUpdateDTO)
        {
            if (ApplicationUserId == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (userTableUpdateDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "You entered empty input" };
                return BadRequest(_response);
            }

            try
            {
                bool flag1 = _userRepo.UpdateCheck(ApplicationUserId);

                if (flag1 == true)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Student record not found" };
                    return NotFound(_response);
                }

                string username = User.FindFirstValue(ClaimTypes.SerialNumber);

                await _userRepo.Update(ApplicationUserId, userTableUpdateDTO, username);

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
