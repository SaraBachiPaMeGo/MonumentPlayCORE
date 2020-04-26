using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteMonument.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("idUser")]
        public int IdUser { get; set; }
        [Column("NombreUs")]
        public String NombreUs { get; set; }
        [Column("Email")]
        public String Email { get; set; }
        [Column("NickName")]
        public String NickName { get; set; }
        [Column("Password")]
        public string Password { get; set; }

        [Column("Salt")]
        public String Salt { get; set; }
    }
}
