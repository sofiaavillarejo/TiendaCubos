using Microsoft.AspNetCore.Mvc;
using TiendaCubos.Models;
using TiendaCubos.Repositories;

namespace TiendaCubos.ViewComponents
{
    public class BuscadorMarcaViewComponent: ViewComponent
    {
        private RepositoryCubos repo;

        public BuscadorMarcaViewComponent(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<string> marcas = await this.repo.GetMarcasAsync();
            return View(marcas);
        }
    }
}
