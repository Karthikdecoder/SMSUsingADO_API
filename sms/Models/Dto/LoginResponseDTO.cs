namespace sms.Models.Dto
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; } // UserDTO contains -----=> Name, Username, Password, UserId 
        //public string Role { get; set; }
        public string Token { get; set; }
    }
}

/*
 * And that way user will have all the details of the log in user and token will be used to authenticate or rather validate the identity of that user.
 */
