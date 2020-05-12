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

        public ActionResult PaginaPpal()
        {
            return View();
        }
        //[Seguridad]
        public async Task<IActionResult> Index(String continente)
        {
            List<Country> paises =
               await this.repo.GetCountriesRegion(continente);
            ViewData["PAIS"] = paises;
            ViewData["valor"] = null;
            ViewData["MONUMENTS"] = null;
            //ViewData["CONTINENTE"] = continente;
            HttpContext.Session.SetString("CONTINENTE", continente);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(String siglas, String continente)
        {
            continente = HttpContext.Session.GetString("CONTINENTE");

            ViewData["valor"] = await this.repo.FindCountriesAsync(siglas);
            ViewData["PAIS"] = await this.repo.GetCountriesRegion(continente);

            if (await this.repo.GetMonumentosAsync(siglas) == null)
            {
                ViewData["MenError"] = "No se encontraron coincidencias para esta localización";
            }
            else { 
                ViewData["MONUMENTS"] = await this.repo.GetMonumentosAsync(siglas);
            }

            return View(await this.repo.GetMonumentosAsync(siglas));
        }

        [Seguridad]
        public IActionResult MapaMundi()
        {
            List<Data> datos = new List<Data>
            {
                new Data
                {
                    Id=0,
                    Name="Americas",
                     Description= "#fdfbfb → #ebedee",
                    Css= "linear-gradient(135deg, #E3FDF5 0%, #FFE6FA 100%)",
                    Foto= "https://www.pngfind.com/pngs/b/70-704131_free-png-download-north-and-south-america-silhouette.png",

                    Height= "200"
                },
                new Data
                {
                    Id=1,
                    Name="Europe",
                     Description= "#a8edea → #fed6e3",
                    Css= "linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)",
                    Foto= "https://cdn.pixabay.com/photo/2014/03/25/15/20/europe-296545_640.png",

                    Height= "200"
                },
                new Data
                {
                    Id= 2,
                    Name="Asia",
                    Description="#f5f7fa → #c3cfe2",
                    Css ="linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)",
                    Foto="https://cdn.pixabay.com/photo/2013/07/12/17/00/asia-151642_960_720.png",
                    Height= "200"
                },
                //new Data
                //{
                //    Id= 3,
                //    Name="NADA",
                //    Description="#e0c3fc → #8ec5fc",
                //    Css ="linear-gradient(135deg, #e0c3fc 0%, #8ec5fc 100%)",
                //    Foto="https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/9c64cfe3-bb3b-4ae8-b5a6-d2f39d21ff87/d3jme6i-8c702ad4-4b7a-4763-9901-99f8b4f038b0.png/v1/fill/w_600,h_400,strp/fondo_transparente_png_by_imsnowbieber_d3jme6i-fullview.png?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOiIsImlzcyI6InVybjphcHA6Iiwib2JqIjpbW3siaGVpZ2h0IjoiPD00MDAiLCJwYXRoIjoiXC9mXC85YzY0Y2ZlMy1iYjNiLTRhZTgtYjVhNi1kMmYzOWQyMWZmODdcL2Qzam1lNmktOGM3MDJhZDQtNGI3YS00NzYzLTk5MDEtOTlmOGI0ZjAzOGIwLnBuZyIsIndpZHRoIjoiPD02MDAifV1dLCJhdWQiOlsidXJuOnNlcnZpY2U6aW1hZ2Uub3BlcmF0aW9ucyJdfQ.2JrgW8AXhfVCeL3tOxOnIhsWLlL5MTuntOq0HfeK6CA",
                //    Height= "200"

                //},
                new Data
                {
                    Id= 4,
                    Name="Africa",
                    Description="#e0c3fc → #8ec5fc",
                    Css ="linear-gradient(135deg, #e0c3fc 0%, #8ec5fc 100%)",
                    Foto="https://sites.psu.edu/global/files/2016/12/africa-u1qukv.png",
                    Height= "200"

                },
                new Data
                {
                    Id= 5,
                    Name="Oceania",
                    Description="#f093fb → #f5576c",
                    Css ="linear-gradient(135deg, #f093fb 0%, #f5576c 100%)",
                    Foto="https://cdn.pixabay.com/photo/2013/07/12/17/00/continent-151644_1280.png",
                    Height= "200"

                },
            };
            ViewData["Contient"] = datos;
            return View();
        }

        [HttpPost]
        //[Route("continente/{continente}")]
        public IActionResult MapaMundi(String continente)
        {
            return RedirectToAction("Index", continente);
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

                String usuarioSesion = JsonConvert.SerializeObject(user);

                HttpContext.Session.SetString("USUARIO", usuarioSesion);

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
            return RedirectToAction("MapaMundi");      
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
            return RedirectToAction("MapaMundi");
        }
        [Seguridad]
        public async Task<IActionResult> ActualizarUsuario()
        {
            String token = HttpContext.Session.GetString("TOKEN");
            Usuario user = await this.repo.PerfilUsuario(token);

            ViewData["USER"] = user;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarUsuario(Usuario user)
        {
            String token = HttpContext.Session.GetString("TOKEN");
            //int id = int.Parse(HttpContext.Session.Id);

            await this.repo.ActualizarUsuario(user.IdUser, user.NombreUs, user.Email, user.NickName, token);
            return RedirectToAction("MiPerfil");
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

        public async Task<IActionResult> MiPerfil()
        {
            String token = HttpContext.Session.GetString("TOKEN");
            Usuario user = await this.repo.PerfilUsuario(token);

            ViewData["USER"] = user;

            return View();
        }

    }
}