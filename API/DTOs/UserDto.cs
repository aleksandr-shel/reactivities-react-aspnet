namespace API.DTOs
{
    //DTO-Data transfer object
    //what information passes to user when logged in
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
    }
}
