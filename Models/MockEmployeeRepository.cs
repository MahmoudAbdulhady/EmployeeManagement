namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
            new Employee() { Id = 1, Name = "Mahmoud" , Email = "Mahmoud@m.com" ,Department = Department.FullStack},
            new Employee() { Id = 2, Name = "Bido" , Email = "Bido@b.com" ,      Department = Department.CyberSecurity},
            new Employee() { Id = 3, Name = "Sarah" , Email = "Sarah@s.com" ,    Department = Department.GraphicDesigner},
            };
        }
        public IEnumerable<Employee> GetAll()
        {
            return _employeeList;
        }


        public Employee GetEmployeeById(int id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == id);
        }

        public Employee AddEmployee(Employee employee)
        {
            employee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee UpdateEmployee(Employee employeeChanges)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id); 
            if(employee !=null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                _employeeList.Remove(employee);
            }
            return employee;
        }
    }
}
