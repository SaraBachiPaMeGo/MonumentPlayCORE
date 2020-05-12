using ClienteMonument.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClienteMonument.Repositories
{
    public class RepositoryMonument
    {
        private String url;
        private MediaTypeWithQualityHeaderValue header;

        public RepositoryMonument()
        {
            //https://tools.wmflabs.org/heritage/api/api.php?action=search&srcountry=es&srlang=es
            //AL FINAL --> &format=json
            //EJ.: api.php?action=search&format=dynamickml
            // TODOS LOS PAISES
            //https://tools.wmflabs.org/heritage/api/api.php?action=search&format=json

            //MONUMENTO POR PAIS -->
            //https://tools.wmflabs.org/heritage/api/api.php?action=search&srcountry=fr
            this.url = "https://apimonumentplay.azurewebsites.net";
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<String> GetToken(String nickname, String password) // VALIDAR
        {
            //String url2 = "https://apimonumentplay.azurewebsites.net";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);

                LogIn user = new LogIn();
                user.NickName = nickname;
                user.Password = password;

                String json = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(json, Encoding.UTF8,
                    "application/json");
                String request = "/api/auth/login";
                HttpResponseMessage response =
                    await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    String data = await response.Content.ReadAsStringAsync(); // ""

                    JObject objeto = JObject.Parse(data); //

                    String token = objeto.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApi<T>(String request, String token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                client.DefaultRequestHeaders.Add("Authorization",
                    "bearer " + token);

                HttpResponseMessage response =
                    await client.GetAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();

                    return (T)Convert.ChangeType(data, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task<T> CallApiNoTOKEN<T>(String request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);                

                HttpResponseMessage response =
                    await client.GetAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();

                    return (T)Convert.ChangeType(data, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task CallApiPut<T>(String request, String token, T user)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                client.DefaultRequestHeaders.Add("Authorization",
                    "bearer " + token);

                String json = JsonConvert.SerializeObject(user);

                StringContent content =
                    new StringContent(json, Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response =
                    await client.PutAsync(request, content);

            }
        }

        private async Task CallApiPost<T>(String request, T user)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);

                String json = JsonConvert.SerializeObject(user);

                StringContent content =
                    new StringContent(json, Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(request, content);

            }
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                String request = "https://restcountries.eu/rest/v2/all";
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0");//Chrome/51.0.2704.106
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    String data = await response.Content.ReadAsStringAsync();
                    List<Country> lista =
                        JsonConvert.DeserializeObject<List<Country>>(data);
                    //List<Country> paises = lista.Paises.
                    //    OrderByDescending(x=> x.NombrePais).ToList();
                    return lista;//.OrderByDescending(x => x.NombrePais).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<Country> FindCountriesAsync(String siglas)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                String request = "https://restcountries.eu/rest/v2/alpha/"+ siglas;
                // "https://restcountries.eu/rest/v2/all";
                //"https://api.printful.com/countries";
                // CONTINENTE --> 
                // https://restcountries.eu/rest/v2/region/{region}
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/4.0");//Chrome/51.0.2704.106
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    //String data = await response.Content.ReadAsStringAsync();
                    //CountryList lista =
                    //    JsonConvert.DeserializeObject<CountryList>(data);
                    //List<Country> paises = lista.Paises.OrderBy
                    //    (x => x.NombrePais).ToList();
                    Country paises =
                        await response.Content.ReadAsAsync<Country>();
                    return paises;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Monument>> GetMonumentosAsync(String siglas)
        {
            using (HttpClient client = new HttpClient())
            {                
                client.DefaultRequestHeaders.Accept.Clear();
                String request = "https://tools.wmflabs.org/heritage/api/api.php?action=search&format=json&srcountry="
                    + siglas.ToLower();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0");
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    String data = await response.Content.ReadAsStringAsync();
                    if (data !="[]")
                    {
                        MonumentosList lista =
                           JsonConvert.DeserializeObject<MonumentosList>(data);
                        List<Monument> monuments = lista.Monumentos;

                        return monuments;
                    }
                    else {
                        return null;
                    }
                   
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Country>> GetCountriesRegion(String region)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                String request = "https://restcountries.eu/rest/v2/region/"
                    + region.ToLower();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0");
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    String data = await response.Content.ReadAsStringAsync();
                    List<Country> paises =
                        JsonConvert.DeserializeObject<List<Country>> (data);
                    //List<Country> paises = lista.Monumentos;

                    return paises;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<Usuario> PerfilUsuario(String token)
        {
            String request = "/api/Usuario/perfilusuario";
            Usuario user = await this.CallApi<Usuario>(request, token);

            return user;            
        }
        public async Task ActualizarUsuario(int id, String nombre, String email,
            String nickname, String token)
        {
            String request = "/api/Usuario/";
            Usuario user = await this.PerfilUsuario(token);

            user.NombreUs = nombre;
            user.Email = email;
            user.NickName = nickname;

            await this.CallApiPut<Usuario>(request, token, user);
        }
        public async Task InsertarUsuario(String nombre, String email,
            String nickname, String password)//, String token
        {
            Usuario user = new Usuario()
                { 
                    NombreUs= nombre,
                    NickName = nickname,
                    Email = email,
                    Password = password
                };
            String request = "/api/Usuario/";
            await this.CallApiPost<Usuario>(request, user);// token,
        }            
    }
}
