using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCubos.Extensions;
using TiendaCubos.Filters;
using TiendaCubos.Models;
using TiendaCubos.Repositories;

namespace TiendaCubos.Controllers
{
    public class CubosController : Controller
    {
        private RepositoryCubos repo;

        public CubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        [AuthorizeUsers]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Cubos(string? marca)
        {
            List<Cubo> cubos;
            if (marca != null)
            {
                cubos = await this.repo.GetCuboByMarcaAsync(marca);
            }
            else
            {
                cubos = await this.repo.GetCubosAsync();
            }
            return View(cubos);
        }

        public async Task<IActionResult> Details(int idCubo)
        {
            Cubo cubo = await this.repo.FindCuboAsync(idCubo);
            return View(cubo);
        }

        public IActionResult _PerfilUser()
        {
            return PartialView("_PerfilUser");
        }

        public IActionResult ComprarCubo(int? idCubo)
        {
            if (idCubo != null)
            {
                List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO") ?? new List<int>();

                carrito.Add(idCubo.Value);

                HttpContext.Session.SetObject("CARRITO", carrito);
                ViewData["totalProd"] = "Total de productos en el carrito: " + carrito.Count;
            }
            return RedirectToAction("Carrito");
        }

        public async Task<IActionResult> Carrito()
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (carrito != null)
            {
                List<Cubo> cubos = await this.repo.GetCubosCarritoAsync(carrito);
                return View(cubos);
            }
            return View();
        }

        public IActionResult QuitarCubo(int idCubo)
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO") ?? new List<int>();

            carrito.Remove(idCubo);

            HttpContext.Session.SetObject("CARRITO", carrito);

            return RedirectToAction("Carrito");
        }

        [AuthorizeUsers]
        public async Task<IActionResult> FinalizarCompra()
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await this.repo.FinalizarCompraAsync(carrito, idusuario);
            HttpContext.Session.Remove("CARRITO");
            return RedirectToAction("PedidosUsuario");
        }

        //public async Task<IActionResult> PedidosUsuario()
        //{
        //    int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    List<VistaPedido> vistaPedidos =
        //        await this.repo.GetPedidosUsuarioAsync(idusuario);
        //    return View(vistaPedidos);
        //}
    }
}
