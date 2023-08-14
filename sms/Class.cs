//using Microsoft.IdentityModel.Tokens;
//using sms.Models.Dto;
//using System.Data.SqlClient;
//using System.Data;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO LoginRequestDTO)
//{

//    SqlConnection sqlConnection = new SqlConnection(connectionString);
//    sqlConnection.Open();

//    SqlCommand displayCommand = new SqlCommand("LoginAndRegistration", sqlConnection);
//    displayCommand.CommandType = System.Data.CommandType.StoredProcedure;
//    displayCommand.Parameters.AddWithValue("@Request", "LOGIN");
//    displayCommand.Parameters.AddWithValue("@UserName", SqlDbType.VarChar).Value = LoginRequestDTO.UserName;

//    SqlDataReader dataReader = displayCommand.ExecuteReader();
//    LoginRequestDTO user = new();
//    var Role = "";

//    while (dataReader.Read())
//    {

//        user.UserName = dataReader.GetValue("UserName").ToString();
//        user.Password = dataReader.GetValue("Password").ToString();
//        Role = dataReader.GetValue("RollName").ToString();
//    }
//    bool isValid = false;
//    if (user.Password == LoginRequestDTO.Password)
//    {
//        isValid = true;
//    }
//    if (user == null || isValid == false)
//    {
//        return new LoginResponseDTO()
//        {
//            Token = "",
//            User = null
//        };
//    }
//    var tokenHandler = new JwtSecurityTokenHandler();
//    var key = Encoding.ASCII.GetBytes(secretkey);
//    var tokenDescriptor = new SecurityTokenDescriptor
//    {

//        Subject = new ClaimsIdentity(new[]
//      {

//           new Claim(ClaimTypes.Name, user.UserName.ToString()),
//           new Claim(ClaimTypes.Role, Role)
//      }),
//        Expires = DateTime.UtcNow.AddDays(1),
//        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//    };
//    var token = tokenHandler.CreateToken(tokenDescriptor);
//    LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
//    {
//        Token = tokenHandler.WriteToken(token),
//        User = _mapper.Map<UserDTO>(user),

//    };


//    return loginResponseDTO;
//}