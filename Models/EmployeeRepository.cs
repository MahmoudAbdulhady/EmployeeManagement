namespace EmployeeManagement.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDbContext _employeeDbContext;

        public EmployeeRepository(EmployeeDbContext employeeDbContext)
        {
            _employeeDbContext = employeeDbContext;
        }

        public Employee AddEmployee(Employee employee)
        {
            _employeeDbContext.Employees.Add(employee);
            _employeeDbContext.SaveChanges();
            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
            Employee employee = _employeeDbContext.Employees.Find(id);
            if (employee != null)
            {
                _employeeDbContext.Employees.Remove(employee);
                _employeeDbContext.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employeeDbContext.Employees;
        }

        public Employee GetEmployeeById(int id)
        {
            return _employeeDbContext.Employees.Find(id);
        }

        public Employee UpdateEmployee(Employee employeeChanges)
        {
            Employee employee = new Employee()
            { Name = employeeChanges.Name, Department = employeeChanges.Department, Email = employeeChanges.Email , PhotoPath = employeeChanges.PhotoPath};

            _employeeDbContext.Update(employee);
            return employee;
        }
    }
}
