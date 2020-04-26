using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMonumentPlay.Token
{
    public class HelperToken
    {
        public String issuer { get; set; }
        public String audience { get; set; }
        public String secretKey { get; set; }

        //Recuperamos nuestra configuración
        public HelperToken(IConfiguration configuration)
        {
            //Y los valores
            this.issuer = configuration["ApiAuth:Issuer"];
            this.audience = configuration["ApiAuth:Audience"];
            this.secretKey = configuration["ApiAuth:SecretKey"];
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(this.secretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtOptions()
        {
            Action<JwtBearerOptions> jwtoptions =
                    new Action<JwtBearerOptions>(options =>
                    {
                        options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateActor = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = this.issuer,
                            ValidAudience = this.audience,
                            IssuerSigningKey = this.GetKeyToken()
                        };
                    });
            return jwtoptions;
        }

        public Action<AuthenticationOptions> GetAuthOptions()
        {
            Action<AuthenticationOptions> authOptions =
                new Action<AuthenticationOptions>(
                    options =>
                    {
                        options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                    });
            return authOptions;
        }
    }
}
