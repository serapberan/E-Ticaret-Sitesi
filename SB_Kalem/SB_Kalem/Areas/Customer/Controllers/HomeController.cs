using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SB_Kalem.Data;
using SB_Kalem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SB_Kalem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db; //VT Bağlantısı
        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Search(string q) //Anasayfa arama 
        {
            if (!String.IsNullOrEmpty(q)) //aramanın içi boş veya null değilse
            {
                var ara = _db.Products.Where(i => i.Title.Contains(q) || i.Description.Contains(q));
                return View(ara);
            }
            return View();
        }
        public IActionResult CategoryDetails(int? id)
        {
            var product = _db.Products.Where(i => i.CategoryId == id).ToList();
            ViewBag.KategoriId = id;
            return View(product);
        }
        public IActionResult Index()
        {
            var products = _db.Products.Where(p=>p.IsHome).ToList(); //Anasayfada görünmesini istediğimiz ürünleri Index sayfasında görelim
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim!=null)
            {
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).ToList().Count(); //Kullanıcının sepetindeki ürünler
                HttpContext.Session.SetInt32(Diger.sShoppingCart, count);
            }
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _db.Products.FirstOrDefault(i => i.Id == id); //sadece o ait ürün bilgisi gelecek
            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //email girişi olmadan sepete ekleme olmasın
        [Authorize]
        public IActionResult Details(ShoppingCart Scart)
        {
            Scart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                Scart.ApplicationUserId = claim.Value;
                ShoppingCart cart = _db.ShoppingCarts.FirstOrDefault
                    (U => U.ApplicationUserId == Scart.ApplicationUserId && U.ProductId == Scart.ProductId);
                if (cart == null)
                {
                    _db.ShoppingCarts.Add(Scart);
                }
                else
                {
                    cart.Count += Scart.Count;
                }
                _db.SaveChanges();
                //tüm sipariş verenlerin sayısını sessionda tutuyoruz
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserId == Scart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(Diger.sShoppingCart, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _db.Products.FirstOrDefault(i => i.Id == Scart.Id); //sadece o ait ürün bilgisi gelecek
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
            }
          
            return View(Scart);
        }
        public IActionResult Privacy()
        {
            return View();
        }

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
