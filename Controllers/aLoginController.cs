using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class aLoginController : ControllerBase
    {
        private readonly LibraryAPIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public aLoginController(LibraryAPIContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {
        

        ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;
            // Microsoft.AspNetCore.Identity.SignInResult signInResult;
            if (applicationUser != null && applicationUser!.IsDeleted == false)
            {
                if (_userManager.CheckPasswordAsync(applicationUser, password).Result == true)
                {
                    var userRoles = _userManager.GetRolesAsync(applicationUser).Result;

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, applicationUser.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier,applicationUser.Id)
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var userClaims = _userManager.GetClaimsAsync(applicationUser).Result;

                    authClaims.AddRange(userClaims);

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.UtcNow.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new { tokenStr = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
                }
                //signInResult= _signInManager.PasswordSignInAsync(applicationUser, password, false, false).Result;
                //if( signInResult.Succeeded== true)
                //{
                //    return Ok();
                //   "Bu satırlar JWT olmadığında session kullanmak için"
                //}
            }
            return Unauthorized();
        }
                                              /*********** LOGOUT SESSION ile birlikte kullanılıyor *********/
        //[Authorize]
        //[HttpGet("Logout")]
        //public ActionResult Logout()
        //{
        //    _signInManager.SignOutAsync();
        //    return Ok();
        //}


        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;
            string token = _userManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage("abc@abc", applicationUser.Email, "Şifre sıfırlama", token);
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("http://smtp.domain.com");
            smtpClient.Send(mailMessage);
            return token;
        }
        [HttpPost("ResetPassword")]
        public ActionResult ResetPassword(string userName, string token, string newPassword)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;

            _userManager.ResetPasswordAsync(applicationUser, token, newPassword).Wait();

            return Ok();
        }
    }
}
