using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        [Required(ErrorMessage = "Role Name is Required")]
        public string RoleName { get; set; }



        public List<string> Users { get; set; }
    }
}
