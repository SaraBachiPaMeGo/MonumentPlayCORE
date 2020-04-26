using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClienteMonument.Filter;
using ClienteMonument.Models;
using ClienteMonument.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClienteMonument.Controllers
{
    //[EnableCors("AllowAnyOrigin")]
    public class MonumentController : Controller
    {
        RepositoryMonument repo;

        public MonumentController(RepositoryMonument repo)
        {
            this.repo = repo;
        }
        [Seguridad]
        public async Task<IActionResult> Index()
        {
            //   ViewData["RESPONSE"] = this.repo.GetCountriesAsync();
            ViewData["PAIS"] =await this.repo.GetCountriesAsync() ;
            ViewData["valor"] = null;
            ViewData["MONUMENTS"] = null;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(String siglas)
        {
            //ViewData["RESPONSE"] = this.repo.GetCountriesAsync();
            //ViewData["MONUMENT"] = ;
            ViewData["valor"] = await this.repo.FindCountriesAsync(siglas);
            ViewData["PAIS"] = await this.repo.GetCountriesAsync();

            //ViewBag.ListaMonu = JsonConvert.SerializeObject(listaMonu);
            //List<Monument> listaMonu =
            //    await this.repo.GetMonumentosAsync(siglas);
            //ViewData["MONUMENTS"] = JsonConvert.SerializeObject(listaMonu);
            ViewData["MONUMENTS"] = await this.repo.GetMonumentosAsync(siglas);

            return View(await this.repo.GetMonumentosAsync(siglas));
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(String nickname, String password)
        {
            String token =
                await this.repo.GetToken(nickname, password);
            if (token == null)
            {
                ViewData["Mensaje"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                Usuario user = await this.repo.PerfilUsuario(token);

                //Habilitamos la seguridad de MVC CORE con claims
                //Para que haya un usuario guardado en sesion (Http.current...)
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role
                    );

                //Almacenamos el numero de usuario
                identity.AddClaim(new Claim
                    (
                    ClaimTypes.NameIdentifier, user.IdUser.ToString()
                    ));

                //Claim con el nickname
                identity.AddClaim(new Claim
                    (
                    ClaimTypes.Name, user.NickName
                    ));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.Now.AddMinutes(40)
                    }
                    );
                //Debemos almacenar el token una vez que el usuario 
            //ya existe
            HttpContext.Session.SetString("TOKEN", token);
            return RedirectToAction("Index");      
                }
                 
        }
        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registro(String NombreUs, String email,
            String nickname, String password)
        {
            //await this.repo.GetToken(nickname,password);
            //El token es el nickname + password
            //String token = HttpContext.Session.GetString("TOKEN");

            await this.repo.InsertarUsuario(NombreUs, email,
            nickname, password);
            return RedirectToAction("Index");
        }
        [Seguridad]
        public IActionResult ActualizarUsuario()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarUsuario(String NombreUs, String email,
            String nickname)
        {
            String token = HttpContext.Session.GetString("TOKEN");
            int id = int.Parse(HttpContext.Session.Id);

            await this.repo.ActualizarUsuario(id, NombreUs, email, nickname, token);
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                );

            if (HttpContext.Session.GetString("TOKEN") != null)
            {
                HttpContext.Session.SetString("TOKEN", String.Empty);
            }

            return RedirectToAction("Login");
        }

    }
}