using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly LibraryAPIContext _context;
        private readonly UserManager<ApplicationUser>_userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public EmployeesController(LibraryAPIContext context,UserManager<ApplicationUser> userManager ,SignInManager<ApplicationUser> signInManager,IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: api/Employees
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
          if (_context.Employee == null)
          {
              return NotFound();
          }
            var employee = await _context.Employee.Include(x => x.ApplicationUser).ToListAsync();
            return employee;
        }

        // GET: api/Employees/5
        [Authorize(Roles ="Employee")]
        [HttpGet("self")]
        public async Task<ActionResult<Employee>> GetSelfEmployee()
        {
          if (_context.Employee == null)
          {
              return NotFound();
          }
            var selfId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _context.Employee.Include(m=> m.ApplicationUser).FirstOrDefaultAsync(c=> c.Id==selfId);

            return employee!;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee, string? currentPassword=null)
        {
            ApplicationUser applicationUser= _userManager.FindByIdAsync(id).Result;

            if (id != employee.Id || applicationUser.IsDeleted)
            {
                return BadRequest();
            }

            applicationUser.IdNumber = employee.ApplicationUser!.IdNumber;
            applicationUser.Name = employee.ApplicationUser!.Name;
            applicationUser.MiddleName = employee.ApplicationUser!.MiddleName;
            applicationUser.FamilyName = employee.ApplicationUser!.FamilyName;
            applicationUser.Gender = employee.ApplicationUser!.Gender;
            applicationUser.Address = employee.ApplicationUser!.Address;
            applicationUser.BirthDate= employee.ApplicationUser!.BirthDate;
            applicationUser.Email = employee.ApplicationUser!.Email;
            applicationUser.RegisterDate = employee.ApplicationUser!.RegisterDate;
            applicationUser.Status = employee.ApplicationUser!.Status;

            _userManager.UpdateAsync(applicationUser).Wait();
            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser,currentPassword, applicationUser.PassWord).Wait();
            }
            employee.ApplicationUser = null;

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
          if (_context.Employee == null)
          {
              return Problem("Entity set 'LibraryAPIContext.Employee'  is null.");
          }
            _userManager.CreateAsync(employee.ApplicationUser!,employee.ApplicationUser!.PassWord).Wait();
            _userManager.AddToRoleAsync(employee.ApplicationUser,"Employee").Wait();
            employee.Id = employee.ApplicationUser!.Id;
            employee.ApplicationUser=null;
            _context.Employee.Add(employee);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [Authorize(Roles ="Admin")]
        [HttpDelete("UserName")]
        public async Task<IActionResult> DeleteEmployee(string userName)
        {

            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _userManager.FindByNameAsync(userName);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool EmployeeExists(string id)
        {
            return (_context.Employee?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
