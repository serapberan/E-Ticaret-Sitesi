using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Models
{
    public class Category  
    {
        [Key]  //Primarykey
        public int Id { get; set; }
        [Required]  //Boş geçilmez
        public string  Name { get; set; }

        //DBContexte tanıt 2.işlem
    }
}
