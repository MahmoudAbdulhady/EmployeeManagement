using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? City { get; set; }
        public string? Gender { get; set; }
    }
}
