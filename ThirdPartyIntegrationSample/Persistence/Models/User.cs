using System;
using Persistence.Interfaces;

namespace Persistence.Models
{
    public class User : DbObject, ISoftDelete
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
        
        
        public DateTime? DeletedAt { get; }
    }
}