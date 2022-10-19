using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Models
{
    public static class Diger //Rollerv şiparişler ve sectionlar için oluşturduğumuz sınıf
    {
        public const string Role_Birey = "Birey"; //facebook-google giriş yapanlar
        public const string Role_Admin = "Admin";
        public const string Role_User  = "User";
        public const string sShoppingCart = "Shopping Cart Session"; //Sipariş detayları için
        //siparişin durumu  için
        public const string Durum_Onaylandi = "Onaylandı";
        public const string Durum_Beklemede = "Beklemede";
        public const string Durum_Kargoda = "Kargoya Verildi";
    }
}
