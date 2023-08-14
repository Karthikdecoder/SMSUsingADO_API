using AutoMapper;
using Microsoft.AspNetCore.Identity;
using sms.Models.Dto;
using sms.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using sms.Repository.IRepository;
using System.Data.SqlClient;
using System.Data;

namespace sms.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration configuration;
        private string secretKey;
        private readonly IMapper _mapper;
        private readonly string connectionString;

        public UserRepository(IConfiguration config, IConfiguration configuration, IMapper mapper)
        {
            configuration = config;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool IsUniqueUser(string username)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM ApplicationUser WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return true;
                    }
                }

                con.Close();
            }

            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Authentication", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operation", SqlDbType.NVarChar).Value = "Login";
                command.Parameters.AddWithValue("@Username", SqlDbType.NVarChar).Value = loginRequestDTO.UserName;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                LoginRequestDTO user = new();

                string Name = string.Empty;
                string RoleId = string.Empty;
                string UserId = string.Empty;
                string Role = string.Empty;

                while (dr.Read())
                {
                    user.UserName = dr.GetValue("Username").ToString();
                    user.Password = dr.GetValue("Password").ToString();
                    Name = dr.GetValue("Name").ToString();
                    RoleId = dr.GetValue("RoleId").ToString();
                    Role = dr.GetValue("RoleName").ToString();
                    UserId = dr.GetValue("ApplicationUserId").ToString();
                }

                bool isValid = false;

                if (user.Password == loginRequestDTO.Password)
                {
                    isValid = true;
                }


                if (user == null || isValid == false)
                {
                    return new LoginResponseDTO()
                    {
                        Token = "",
                        User = null
                    };
                }


                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, RoleId),
                    new Claim(ClaimTypes.SerialNumber, UserId)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                {
                    Token = tokenHandler.WriteToken(token),
                    User = _mapper.Map<UserDTO>(user),
                };

                loginResponseDTO.User.UserId = UserId;
                loginResponseDTO.User.Name = Name;
                loginResponseDTO.User.Password = String.Empty;

                connection.Close();

                return loginResponseDTO;
            }

        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO, string userName)
        {

            UserDTO user = new UserDTO();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Authentication", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Operation", SqlDbType.NVarChar).Value = "Register";

                command.Parameters.AddWithValue("@Username", SqlDbType.NVarChar).Value = registerationRequestDTO.UserName;
                command.Parameters.AddWithValue("@Password", SqlDbType.NVarChar).Value = registerationRequestDTO.Password;
                command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = registerationRequestDTO.Name;
                command.Parameters.AddWithValue("@RoleId", SqlDbType.NVarChar).Value = registerationRequestDTO.RoleId;
                command.Parameters.AddWithValue("@CreatedBy", SqlDbType.NVarChar).Value = userName;
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();

                user.UserName = registerationRequestDTO.UserName;
                user.Name = registerationRequestDTO.Name;
                user.Password = registerationRequestDTO.Password;
                user.UserId = registerationRequestDTO.RoleId;
            }

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserTableDTO>> GetAll()
        {
            List<UserTableDTO> UsersList = new List<UserTableDTO>();
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Authentication", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetUsers";

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserTableDTO user = new UserTableDTO();

                        user.ApplicationUserId = dr.GetValue("ApplicationUserId").ToString();
                        user.Name = dr.GetValue("Name").ToString();
                        user.Username = dr.GetValue("Username").ToString();
                        user.Password = dr.GetValue("Password").ToString();
                        user.RoleId = dr.GetValue("RoleId").ToString();
                        user.CreatedBy = dr.GetValue("CreatedBy").ToString();
                        user.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        user.UpdatedBy = dr.GetValue("UpdatedBy").ToString();
                        user.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                        UsersList.Add(user);
                    }
                }
                connection.Close();
            }

            return UsersList;
        }

        public async Task<UserTableDTO> Get(int ApplicationUserId)
        {
            UserTableDTO user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Authentication", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "GetUser";
                command.Parameters.AddWithValue("@ApplicationUserId", SqlDbType.Int).Value = ApplicationUserId;

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        user = new UserTableDTO();

                        user.ApplicationUserId = dr.GetValue("ApplicationUserId").ToString();
                        user.Name = dr.GetValue("Name").ToString();
                        user.Username = dr.GetValue("Username").ToString();
                        user.Password = dr.GetValue("Password").ToString();
                        user.RoleId = dr.GetValue("RoleId").ToString();
                        user.CreatedBy = dr.GetValue("CreatedBy").ToString();
                        user.CreatedOn = dr.GetValue("CreatedOn").ToString();
                        user.UpdatedBy = dr.GetValue("UpdatedBy").ToString();
                        user.UpdatedOn = dr.GetValue("UpdatedOn").ToString();

                    }
                }
                connection.Close();
            }

            return user;
        }

        public async Task Remove(int ApplicationUserId, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Authentication", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "RemoveUser";

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ApplicationUserId", SqlDbType.Int).Value = ApplicationUserId;

                connection.Open();

                int n = command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool RemoveCheck(int ApplicationUserId)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM ApplicationUser WHERE ApplicationUserId = @ApplicationUserId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ApplicationUserId", ApplicationUserId);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }

        public async Task Update(int ApplicationUserId, UserTableUpdateDTO userUpdate, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Authentication", connection);
                command.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = "UpdateUser";
                command.Parameters.AddWithValue("@ApplicationUserId", SqlDbType.Int).Value = ApplicationUserId;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = userUpdate.Name;
                command.Parameters.AddWithValue("@Username", SqlDbType.Int).Value = userUpdate.Username;
                command.Parameters.AddWithValue("@Password", SqlDbType.NVarChar).Value = userUpdate.Password;
                command.Parameters.AddWithValue("@RoleId", SqlDbType.NVarChar).Value = userUpdate.RoleId;
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = userName;

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public bool UpdateCheck(int ApplicationUserId)
        {
            bool flag = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM ApplicationUser WHERE ApplicationUserId = @ApplicationUserId AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ApplicationUserId", ApplicationUserId);

                    int rowCount = (int)command.ExecuteScalar();

                    if (rowCount > 0)
                    {
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }
        
    }

}