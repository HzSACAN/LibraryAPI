using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly LibraryAPIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public MembersController(LibraryAPIContext context,UserManager<ApplicationUser>userManager,SignInManager<ApplicationUser>signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // GET: api/Members
        [Authorize(Roles ="Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMember()
        {
          if (_context.Member == null)
          {
              return NotFound();
          }
            var members = await _context.Member.Include(m => m.ApplicationUser).ToListAsync();
            return members;
        }

        // GET: api/Members/5
        [Authorize(Roles ="Member")]
        [HttpGet("Self")]
        public async Task<ActionResult<Member>> GetSelfMember()
        {
          if (_context.Member == null)
          {
              return NotFound();
          }
            var selfId= User.FindFirstValue(ClaimTypes.NameIdentifier);

            var member = await _context.Member.Include(m=>m.ApplicationUser).FirstOrDefaultAsync(m=> m.Id==selfId);

            return member!;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Member")]
        [HttpPut]
        public async Task<IActionResult> PutMember( Member member,string? currentPassword=null)
        {
            string selfId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = _userManager.FindByIdAsync(selfId).Result;

            if (applicationUser.IsDeleted)
            {
                return BadRequest();
            }

            applicationUser.IdNumber = member.ApplicationUser!.IdNumber;
            applicationUser.Name = member.ApplicationUser!.Name;
            applicationUser.MiddleName = member.ApplicationUser!.MiddleName;
            applicationUser.FamilyName = member.ApplicationUser!.FamilyName;
            applicationUser.Gender = member.ApplicationUser!.Gender;
            applicationUser.Address = member.ApplicationUser!.Address;
            applicationUser.BirthDate = member.ApplicationUser!.BirthDate;
            applicationUser.Email = member.ApplicationUser!.Email;
            applicationUser.RegisterDate = member.ApplicationUser!.RegisterDate;
            applicationUser.Status = member.ApplicationUser!.Status;
            
            _userManager.UpdateAsync(applicationUser).Wait();

            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.PassWord).Wait();
            }
            member.ApplicationUser = null;

            _context.Entry(member).State = EntityState.Modified;

             try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(selfId))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
          if (_context.Member == null)
          {
              return Problem("Entity set 'LibraryAPIContext.Member'  is null.");
          }
            _userManager.CreateAsync(member.ApplicationUser!, member.ApplicationUser!.PassWord).Wait();
            _userManager.AddToRoleAsync(member.ApplicationUser, "Member").Wait();
            member.Id = member.ApplicationUser!.Id;
            member.ApplicationUser = null;
            
            _context.Member.Add(member);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [Authorize(Roles = "Employee")]
        [HttpDelete("UserName")]
        public async Task<IActionResult> DeleteMember(string userName)
        {
            if (_context.Member == null)
            {
                return NotFound();
            }
            var member = await _userManager.FindByNameAsync(userName);

            if (member == null)
            {
                return NotFound();
            }

            member.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool MemberExists(string id)
        {
            return (_context.Member?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
