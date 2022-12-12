using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApiJWT.Repositories;



namespace ApiJWT.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public DateTimeOffset CreatedAt { get; set; }



    }
}