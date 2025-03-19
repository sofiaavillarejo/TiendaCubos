using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TiendaCubos.Models
{
    [Table("COMPRA")]
    public class Compra
    {
        [Key]
        [Column("id_compra")]
        public int IdCompra { get; set; }

        [Column("nombre_cubo")]
        public string Nombre { get; set; }

        [Column("precio")]
        public int Precio { get; set; }

        [Column("fechapedido")]
        public DateTime FechaPedido { get; set; }

        [NotMapped]
        public int Cantidad { get; set; }

        //public int IdUser { get; set; }
    }
}
