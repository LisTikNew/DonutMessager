using System;
using System.Collections.Generic;
using System.Text;

namespace DonutMessager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? AvatarPath { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PasswordHash { get; set; }
    }
}
