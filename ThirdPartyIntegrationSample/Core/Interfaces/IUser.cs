namespace Core.Interfaces
{
    public interface IUser
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
    }
}