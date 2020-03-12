using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        #region Campos
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        #endregion

        #region Propiedades
        public IConfiguration Configuration { get; }
        #endregion

        #region Constructor
        public UsuarioController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Configuration = configuration;
        }
        #endregion

        // POST: api/Usuario
        [HttpPost]
        public async Task<TokenResponse> Post([FromBody] Usuario usuario)
        {
            var oldUserASP = await _userManager.FindByEmailAsync(usuario.Email);
            if (oldUserASP != null)
            {
                var token = BuildToken(oldUserASP);
                token.ErrorDescription = "Usted ya esta registrado";
                return token;
            }
            else 
            { 
                var userASP = new IdentityUser
                {
                    Email = usuario.Email,
                    UserName = usuario.Email,
                };

                var result = await _userManager.CreateAsync(userASP, usuario.Contrasena);

                if (result.Succeeded)
                {
                    var newUserASP = await _userManager.FindByEmailAsync(usuario.Email);
                    var signInResult = await _signInManager.PasswordSignInAsync(newUserASP.Email, usuario.Contrasena, isPersistent: false, lockoutOnFailure: false);

                    if (signInResult.Succeeded)
                    {
                        var token = BuildToken(userASP);
                        return token; 
                    }
                    else
                    {
                        return new TokenResponse()
                        {
                            ErrorDescription = "Invalido inicio de sección",
                            UserName = usuario.Email,
                        };
                    }
                }
                else
                {
                    return new TokenResponse()
                    {
                        ErrorDescription = "No se pudo crear usuario",
                        UserName = usuario.Email,
                    };
                }
            }
        }

        #region MetodosPrivados
        private TokenResponse BuildToken(IdentityUser userASP)
        {
            var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.UniqueName, userASP.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(365);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: Configuration["Dominio:validIssuer"],
               audience: Configuration["Dominio:validAudience"],
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            var a = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration
            };

            TokenResponse tokenResponse = new TokenResponse()
            {
                AccessToken = a.token,
                ErrorDescription = string.Empty,
                Expires = a.expiration,
                TokenType = Configuration["JWT:tokenType"],
                UserName = userASP.UserName
            };
            return tokenResponse;
        }

        #endregion
    }
}
