using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SB_Kalem.Data;
using SB_Kalem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Diger.Role_Admin)]
    public class UserController : Controller
    {
        public readonly ApplicationDbContext _context;   //Kullanıcıları ve rolleri Index sayfasına getirmek için oluşturduk
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.ApplicationUsers.ToList(); //farklı tablolardan verileri çektik
            var role = _context.Roles.ToList();
            var userRol = _context.UserRoles.ToList();
            foreach (var item in users)
            {
                var roleId = userRol.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return View(users);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
