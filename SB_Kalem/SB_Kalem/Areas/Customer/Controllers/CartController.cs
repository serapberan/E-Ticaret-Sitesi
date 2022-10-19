using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SB_Kalem.Data;
using SB_Kalem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SB_Kalem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        //Sadece üye giri yapanlar sipariş versinler istiyoruz
        private readonly IEmailSender _emailSender;
        //Sepete ürün ekleyen kullanıcılar
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCartVm ShoppingCartVm { get; set; }
        public CartController(UserManager<IdentityUser> userManager,
                               IEmailSender emailSender,
                               ApplicationDbContext db)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVm = new ShoppingCartVm()
            {
                orderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            foreach (var item in ShoppingCartVm.ListCart)
            {
                item.Price = item.Product.Price;
                ShoppingCartVm.orderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVm model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVm.ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product);
            ShoppingCartVm.orderHeader.OrderStatus = Diger.Durum_Beklemede; //Siparin durumu
            ShoppingCartVm.orderHeader.ApplicationUserId = claim.Value; // Siparişi veren kişi
            ShoppingCartVm.orderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(ShoppingCartVm.orderHeader);
            _db.SaveChanges();
            foreach (var item in ShoppingCartVm.ListCart) //Birden fazla şipariş olabilir bunu dönelim
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId=item.ProductId,
                    OrderId = ShoppingCartVm.orderHeader.Id,
                    Count = item.Count
                };
                ShoppingCartVm.orderHeader.OrderTotal += item.Count * item.Product.Price;
                model.orderHeader.OrderTotal += item.Count * item.Product.Price;
                _db.OrderDetails.Add(orderDetails);
            }
            _db.ShoppingCarts.RemoveRange(ShoppingCartVm.ListCart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.sShoppingCart, 0);  //Sipariş verdikten sonra sepeti sıfırla
            return RedirectToAction("SiparisTamam");
        }
        public IActionResult SiparisTamam()
        {
            return View();
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVm = new ShoppingCartVm()
            {
                orderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            ShoppingCartVm.orderHeader.OrderTotal = 0;
            ShoppingCartVm.orderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            foreach (var item in ShoppingCartVm.ListCart)
            {
                //sepetteki fiyatı hesapladık
                ShoppingCartVm.orderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVm);
        }
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Aktivasyon İçin Lütfen Emailinizi Doğrulayınız!");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            ModelState.AddModelError(string.Empty, "Email Doğrulama Kodu Gönder.");
            return RedirectToAction("Success");
        }
        public IActionResult Success() //uyarı sayfası 
        {
            return View();
        }
        public IActionResult Add(int cartId) //ürün arttırma  butonu
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index)); // ındex sayfasına git
        }
        public IActionResult Decrease(int cartId) //ürün azaltma butonu
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            if (cart.Count == 1)
            {
                //sepetteki ürün 1 adet ise ve siliyorsak sepetten kaldırıyoruz
                var count = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.sShoppingCart, count - 1); // birer birer azalt
            }
            else //değilse ürünü birer birer azalt
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }
           
            return RedirectToAction(nameof(Index)); // ındex sayfasına git
        }

        public IActionResult Remove(int cartId) //ürün silme butonu
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
           
                //sepetteki ürün 1 adet ise ve siliyorsak sepetten kaldırıyoruz
                var count = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.sShoppingCart, count - 1); // birer birer azalt
           
            return RedirectToAction(nameof(Index)); // ındex sayfasına git
        }
    }


}