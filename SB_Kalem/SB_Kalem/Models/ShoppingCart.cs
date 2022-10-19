using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1; //  sipariş adet 
        }
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } //Kullanıcı ile siparişleri birbirine bağladık
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get; set; }
        [NotMapped] // Sipari,iş verdiğimizde ilk 1 adet görükecek
        public double Price { get; set; } 
    }
}
