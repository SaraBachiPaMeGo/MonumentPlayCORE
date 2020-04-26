using NuggetMonument.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMonumentPlay.Data
{
    public class ContextoUser : DbContext
    {
        //conexionMonument
        public ContextoUser(DbContextOptions options) : base(options)
        { }

        public DbSet<Usuario> Usuario { get; set; } 
    }
}
