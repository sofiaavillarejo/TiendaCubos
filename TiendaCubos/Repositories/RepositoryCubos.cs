using Humanizer;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using TiendaCubos.Data;
using TiendaCubos.Models;
using Microsoft.Data.SqlClient;

namespace TiendaCubos.Repositories
{

    #region VISTA
    /*CREATE OR ALTER VIEW VISTACOMPRAS AS
        SELECT
            c.id_pedido AS IDVISTACOMPRA,
            c.id_usuario AS IdUsuario,
            u.nombre AS NombreUsuario,
            c.id_cubo AS IdCubo,
            cubo.nombre AS NombreCubo,
            cubo.precio AS PrecioCubo,
            cubo.imagen AS ImagenCubo,
            c.cantidad AS Cantidad,
            c.fecha AS FechaCompra,
            (c.cantidad* cubo.precio) AS PrecioFinal
        FROM
    COMPRA c
        JOIN
            CUBOS cubo ON c.id_cubo = cubo.id_cubo
        JOIN
            USUARIOS u ON c.id_usuario = u.id_user;


    ----PAGINACION------------

    alter view V_GRUPO_CUBOS
    as
        select cast( ROW_NUMBER() over (order by nombre) as int) AS POSICION, ID_CUBO, nombre, marca, modelo, imagen, precio from CUBOS
    go
    ------
    alter procedure SP_GRUPO_CUBOS (@posicion int)
    as
        select ID_CUBO, nombre, marca, modelo, imagen, precio from V_GRUPO_CUBOS where POSICION	>= @posicion AND POSICION  < (@posicion + 2) --ahora paginamos de 3 en 3
    go

    exec SP_GRUPO_CUBOS 2
    */
    #endregion

    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }
        #region CUBOS
        public async Task<List<Cubo>> GetCubosAsync()
        {
            var consulta = from datos in this.context.Cubo select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Cubo> FindCuboAsync(int idCubo)
        {
            return await this.context.Cubo.FirstOrDefaultAsync(c => c.IdCubo == idCubo);
        }

        public async Task<List<Cubo>> GetCuboByMarcaAsync(string marca)
        {
            return await this.context.Cubo.Where(c => c.Marca == marca).ToListAsync();
        }

        public async Task<List<string>> GetMarcasAsync()
        {
            return await this.context.Cubo.Select(c => c.Marca).Distinct().ToListAsync();
        }

        #endregion

        #region USUARIOS
        public async Task<Usuario> LoginUserAsync(string email, string password)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        //public async Task<Usuario> GetUserAsync(int idUser)
        //{
        //    return await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUser == idUser);
        //}
        #endregion

        #region CARRITO
        public async Task<List<Cubo>> GetCubosCarritoAsync(List<int> carrito)
        {
            return await this.context.Cubo.Where(c => carrito.Contains(c.IdCubo)).ToListAsync();
        }

        public async Task<int> GetMaxIdCompraAsync()
        {
            if (this.context.Compras.Count() == 0) return 1;
            else return await this.context.Compras
                    .MaxAsync(p => p.IdCompra) + 1;
        }

        public async Task FinalizarCompraAsync(List<int> carrito, int idUser)
        {
            int idCompra = await GetMaxIdCompraAsync();
            DateTime fecha = DateTime.Now;
            foreach (int idCubo in carrito.Distinct())
            {
                var cubo = await this.context.Cubo.Where(c => c.IdCubo == idCubo).FirstOrDefaultAsync();
                int idpedido = await GetMaxIdCompraAsync();
                await this.context.Compras.AddAsync
                    (new Compra
                    {
                        IdCompra = idpedido,
                        Nombre = cubo.Nombre,
                        Precio = cubo.Precio,
                        FechaPedido = fecha,
                        Cantidad = carrito.Count(id => id == idCubo),
                    });

                await this.context.SaveChangesAsync();
            }
        }

        //public async Task<List<VistaPedido>> GetPedidosUsuarioAsync
        //    (int idusuario)
        //{
        //    return await this.context.VistaPedidos.Where(vp => vp.IdUsuario == idusuario).ToListAsync();
        //}
        #endregion

        public async Task<int> GetNumeroRegistrosVistaCubosAsync()
        {
            return await this.context.Cubo.CountAsync();
        }
        public async Task<List<Cubo>> GetGrupoCubosASync(int posicion)
        {
            string sql = "SP_GRUPO_CUBOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Cubo.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }
    }
}
