using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TiendaCubos.Models;
using TiendaCubos.Repositories;

namespace TiendaCubos.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryCubos repo;

        public ManagedController(RepositoryCubos repo)
        {
            this.repo = repo;
        }
        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario user = await this.repo.LoginUserAsync(email, password);
            if (user != null)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                        new Claim(ClaimTypes.Name, user.Nombre),
                        new Claim(ClaimTypes.Email, user.Email)
                    };
                
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                string controller = TempData["controller"]?.ToString() ?? "Home";
                string action = TempData["action"]?.ToString() ?? "Index";

                if (TempData["id"] != null)
                {
                    string id = TempData["id"].ToString();
                    return RedirectToAction(action, controller, new { id = id });
                }
                else
                {
                    return RedirectToAction(action, controller);
                }
            }else
            {
                ViewData["Error"] = "Email o contraseña incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Cubos");
        }
    }
}
