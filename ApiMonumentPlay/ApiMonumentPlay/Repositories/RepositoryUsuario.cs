using ApiMonumentPlay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuggetMonument.Models;
using ApiMonumentPlay.Helper;

namespace ApiMonumentPlay.Repositories
{
    public class RepositoryUsuario
    {
        ContextoUser context;

        public RepositoryUsuario(ContextoUser context)
        {
            this.context = context;
        }

        public List<Usuario> GetUsuarios()
        {
            return this.context.Usuario.ToList(); ;
        }

        public Usuario BuscarUsuario(int idUser) //PERFIL
        {
            return this.context.Usuario.SingleOrDefault(x=> x.IdUser == idUser);
        }

        public Usuario ValidarUsuario(String nickname, String password) //Existe
        {
            //var consulta = from datos in context.Usuario
            //               where datos.NickName == nickname
            //               select datos;

            Usuario user = this.context.Usuario.SingleOrDefault(z => z.NickName == nickname);

            if (user != null)
            {
                String salt = user.Salt;
                string cifrado = HelperCifrado.CifrarPassword(password, salt);
                bool resultado = HelperCifrado.CompararBytes(cifrado, user.Password);

                if (resultado == true)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public void InsertarUsuario(String nombre, String email,
            String nickname, String password) //REGISTRO
        {
            Usuario user = new Usuario();

            String salt = HelperCifrado.GenerarSalt();

            string pass = HelperCifrado.CifrarPassword(password, salt);
            int? count = (from datos in context.Usuario
                          select datos.IdUser).Count();

            if (count == 0)
            {
                user.IdUser = 1;
            }
            else
            {
                user.IdUser = this.context.Usuario.Max(z => z.IdUser) + 1;
            }

            user.Salt = salt;
            user.NombreUs = nombre;
            user.Email = email;
            user.NickName = nickname;
            user.Password = pass;

            this.context.Usuario.Add(user);
            this.context.SaveChanges();
        }

        public void ActualizarUsuario(String nombre, String email,
            String nickname, String password)
        {
            Usuario user = ValidarUsuario(nickname, password);

            user.NombreUs = nombre;
            user.Email = email;
            user.NickName = nickname;
            user.Password = password;

            this.context.SaveChanges();
        }
    }
}
