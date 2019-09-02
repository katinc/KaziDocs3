using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaziDocs3.Data;
using KaziDocs3.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using KaziDocs3.Model;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KaziDocs3.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        private DateTime TokenExpire { get; set; }

        public AccountsController(
           UserManager<IdentityUser> userManager,
           SignInManager<IdentityUser> signInManager,
           IConfiguration configuration,
           ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterModel register)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var user = new IdentityUser { Email = register.Email, UserName = register.Email };
                var result = await _userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    var account = new Account
                    {
                        Id = KeyGen.Generate(),
                        AccounBlocked = false,
                        Address = register.Address,
                        Contact = register.Contact,
                        Email = register.Email,
                        FirstName = register.FirstName,
                        LastName = register.LastName,
                        UserAccountId = user.Id,
                        CreateTime = DateTime.UtcNow
                    };

                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    var nresult = await _signInManager.PasswordSignInAsync(register.Email, register.Password, false, false);
                    if (nresult.Succeeded)
                    {
                        var token = GenerateJwtToken(user.Email, user);
                        //var role = new object();

                        try
                        {
                            //var getrole = _context.UserRoles.SingleOrDefault(u => u.UserId == user.Id);
                            //role = _context.Roles.SingleOrDefault(u => u.Id == getrole.RoleId);

                            var checkPrevious = _context.Tokens.AsNoTracking().SingleOrDefault(u => u.AccountId == user.Id);
                            if (checkPrevious == null)
                            {
                                var newToken = new UserTokens
                                {
                                    Id = KeyGen.Generate(),
                                    AccountId = user.Id,
                                    Expiry = TokenExpire,
                                    Token = token.ToString()
                                };

                                await _context.AddAsync(newToken);
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                            }
                            else
                            {
                                checkPrevious.Token = token.ToString();
                                checkPrevious.Expiry = TokenExpire;

                                _context.Entry(checkPrevious).State = EntityState.Modified;
                                _context.Update(checkPrevious);
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return StatusCode(200, new
                            {
                                status = 500,
                                ex.Message
                            });
                        }
                        try
                        {
                            return StatusCode(200, new
                            {
                                status = 100,
                                message = "Log in successful",
                                data = new
                                {
                                    user = new
                                    {
                                        id = user.Id,
                                        username = user.UserName,
                                        email = user.Email,
                                    },
                                    //role,
                                    accesstoken = new
                                    {
                                        token,
                                        expire = TokenExpire
                                    }
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return StatusCode(200, new
                            {
                                status = 500,
                                ex.Message
                            });
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return StatusCode(200, new
                        {
                            status = 500,
                            data = result.Errors
                        });
                    }
                }
                else
                {
                    //return StatusCode(200, new AppResult
                    //{
                    //    Status = Statuses.Fail,
                    //    Data = result.Errors.ToList()
                    //});
                    transaction.Rollback();
                    return StatusCode(200, new
                    {
                        status = 500,
                        data = result.Errors
                    });

                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(200, new
                {
                    status = 500,
                    ex.Message
                });

            }
        }

        // Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            var getEmp = _context.Accounts.SingleOrDefault(u => u.Email == model.Email);
            if (getEmp != null)
            {
                if (getEmp.AccounBlocked == true)
                {
                    return StatusCode(200, new 
                    {
                        Status = 105,
                        Message = "account has been blocked, please contact admin"
                    });
                }
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                var token = GenerateJwtToken(user.Email, user);
                var role = new object();

                try
                {
                    /*var getrole = _context.UserRoles.SingleOrDefault(u => u.UserId == user.Id);
                    role = _context.Roles.SingleOrDefault(u => u.Id == getrole.RoleId);*/

                    var checkPrevious = _context.Tokens.AsNoTracking().SingleOrDefault(u => u.AccountId == user.Id);
                    if (checkPrevious == null)
                    {
                        var newToken = new UserTokens
                        {
                            Id = KeyGen.Generate(),
                            AccountId = user.Id,
                            Expiry = TokenExpire,
                            Token = token.ToString()
                        };

                        await _context.AddAsync(newToken);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        checkPrevious.Token = token.ToString();
                        checkPrevious.Expiry = TokenExpire;

                        _context.Entry(checkPrevious).State = EntityState.Modified;
                        _context.Update(checkPrevious);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(200, new 
                    {
                        Status = 500,
                        Message = ex.Message
                    });
                }
                try
                {
                    return StatusCode(200, new
                    {
                        status = 100,
                        message = "Log in successful",
                        aata = new
                        {
                            user = new
                            {
                                id = user.Id,
                                username = user.UserName,
                                email = user.Email,
                            },
                            role,
                            accesstoken = new
                            {
                                token,
                                expire = TokenExpire
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(200, new
                    {
                        status = 500,
                        message = ex.Message
                    });
                }
            }
            else
            {
                return StatusCode(200, new
                {
                    status = 500,
                    message = "email or password incorrect"
                });
            }
        }


        

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var user = _userManager.GetUserId(User);
            var checkPrevious = _context.Tokens.SingleOrDefault(u => u.AccountId == user);
            if (checkPrevious != null)
            {
                _context.Remove(checkPrevious);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            return StatusCode(200, new { status = 100, message = "Session has expired." });
        }

        [HttpPost]
        public async Task<IActionResult> CheckExpiry([FromBody]ExpiryModel model)
        {
            try
            {
                var check = await _context.Tokens.SingleOrDefaultAsync(u => u.AccountId == model.Id);
                if (check == null)
                {
                    return await Logout();
                }

                if (check.Token != model.Token)
                {
                    await _signInManager.SignOutAsync();
                    return StatusCode(200, new { status = 100, message = "Session has expired." });
                }

                if (check.Expiry > DateTime.UtcNow)
                {
                    check.Expiry.AddMinutes(10);
                }
                else
                {
                    return await Logout();
                }

                return StatusCode(200, new
                {
                    status = 100,
                    message = "user is still active"
                });
            }
            catch (Exception)
            {
                return await Logout();
            }
        }

        private object GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            TokenExpire = DateTime.Now.AddDays(1);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtIssuer"],
                audience: _configuration["JwtIssuer"],
                claims: claims,
                expires: TokenExpire,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}