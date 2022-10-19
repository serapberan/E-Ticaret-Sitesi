using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } //Kullanıcı ile siparişleri birbirine bağladık
        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Adress { get; set; }
        [Required]
        public string Semt { get; set; }
        [Required]
        public string Sehir { get; set; }
        [Required]
        public string PostaKodu { get; set; }


    }
}
