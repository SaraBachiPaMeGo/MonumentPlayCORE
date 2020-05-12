using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiMonumentPlay.Repositories;
using ApiMonumentPlay.Token;
using ApiMonumentPlay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuggetMonument.Models;

namespace ApiMonumentPlay.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        HelperToken helper;
        RepositoryUsuario repo;
        public AuthController(RepositoryUsuario repo, IConfiguration configuration)
        {
            this.helper = new HelperToken(configuration);
            this.repo = repo;
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Login(LogIn model)
        {
            Usuario user = this.repo.ValidarUsuario(model.NickName, model.Password);

            if (user != null)
            {
                Claim[] claims = new[]
                {
                    new Claim("UserData",
                    JsonConvert.SerializeObject(user))
                };

                JwtSecurityToken token = new JwtSecurityToken
                    (
                        issuer: helper.issuer,
                        audience: helper.audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        notBefore: DateTime.UtcNow,
                        signingCredentials: new SigningCredentials
                            (this.helper.GetKeyToken(),
                            SecurityAlgorithms.HmacSha256
                            )
                    );                
                return Ok(
                    new
                    {
                        response = new JwtSecurityTokenHandler().WriteToken(token)
                    }
                );
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}