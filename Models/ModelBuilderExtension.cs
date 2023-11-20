using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData
                (
                   new Employee() { Id = 1, Name = "Mahmoud", Email = "Mahmoud@m.com", Department = Department.FullStack  , PhotoPath=""},
                   new Employee() { Id = 2, Name = "Bido", Email = "Bido@b.com", Department = Department.CyberSecurity, PhotoPath = "" },
                   new Employee() { Id = 3, Name = "Sarah", Email = "Sarah@s.com", Department = Department.GraphicDesigner, PhotoPath = "" },
                   new Employee() { Id = 4, Name = "Ahmed", Email = "Ahmed@s.com", Department = Department.FullStack , PhotoPath = "" }
                );
        }
    }
}
