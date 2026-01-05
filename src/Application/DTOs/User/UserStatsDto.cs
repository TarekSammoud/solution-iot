using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.User
{
    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
    }

}
