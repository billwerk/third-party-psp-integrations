﻿using System;
using Core.Interfaces;
using Persistence.Interfaces;

namespace Persistence.Models
{
    public class User : DbObject, ISoftDelete, IUser
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
        
        
        public DateTime? DeletedAt { get; }
    }
}