﻿
namespace CustomerAPI.Models
{
    public class TblUser
    {
        public int Id { get; set; }
        public string Userid { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool? IsActive { get; set; }

    }
}
