namespace Instant.Server.Domain.OperationOptions
{
    public class AuthorizeUserOptions
    {
        public string Login { get; set; }
        
        public string Password { get; set; }

        public AuthorizeUserOptions()
        { }
        
        public AuthorizeUserOptions(
            string login,
            string password)
        {
            this.Login = login;
            this.Password = password;
        }
    }
}