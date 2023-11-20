using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password" , ErrorMessage ="Password and Confirm Password Must Match !")]
        public string ConfirmPassword { get; set; }


       public string Token { get; set; }
    }
}

