using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiMonumentPlay.Repositories;
using ApiMonumentPlay.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuggetMonument.Models;

namespace ApiMonumentPlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        RepositoryUsuario repo;

        public UsuarioController(RepositoryUsuario repo)
        {
            this.repo = repo;

        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<List<Usuario>> GetUsuarios()
        {
            return repo.GetUsuarios();
        }
      
        [HttpGet]
        [Route("{id}")]
        public ActionResult<Usuario> BuscarUsuario(int id)
        {
            return this.repo.BuscarUsuario(id);
        } //PERFIL   

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<Usuario> PerfilUsuario()
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();

            String json = claims.SingleOrDefault(x => x.Type == "UserData").Value;

            Usuario user = JsonConvert.DeserializeObject<Usuario>(json);
            return user;
        }

        [HttpPost]
        public void InsertarUsuario(Usuario user)
        {
            this.repo.InsertarUsuario(user.NombreUs,user.Email,
            user.NickName, user.Password); 
        }

        [Authorize]
        [HttpPut]
        public void ActualizarUsuario(Usuario user)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();

            String json = claims.SingleOrDefault(x => x.Type == "UserData").Value;

            user = JsonConvert.DeserializeObject<Usuario>(json);

            this.repo.ActualizarUsuario(user.NombreUs, user.Email,
            user.NickName, user.Password);
        }
    }
}