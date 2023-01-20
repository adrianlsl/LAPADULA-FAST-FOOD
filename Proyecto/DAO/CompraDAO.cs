using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Text;
using System.Data;

namespace Proyecto.DAO
{
    public class CompraDAO
    {
        private static CompraDAO _instancia = null;

        public CompraDAO()
        {
        }
        public static CompraDAO Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CompraDAO();
                }
                return _instancia;
            }
        }
        public bool Registrar (Compra compra)
        {
            bool respuesta = false;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    foreach (DetalleCompra dc in compra.detallecompra)
                    {
                        query.AppendLine("insert into tb_detalle_compra(IdCompra,IdProducto,Cantidad,Total) values (¡idcompra!," + dc.IdProducto + "," + dc.Cantidad + "," + dc.Total + ")");
                    }
                    SqlCommand cmd = new SqlCommand("SP_RegistrarCompra", cn);
                    cmd.Parameters.AddWithValue("IdUsuario", compra.IdUsuario);
                    cmd.Parameters.AddWithValue("TotalProducto", compra.TotalProducto);
                    cmd.Parameters.AddWithValue("Total", compra.Total);
                    cmd.Parameters.AddWithValue("Contacto", compra.Contacto);
                    cmd.Parameters.AddWithValue("Telefono", compra.Telefono);
                    cmd.Parameters.AddWithValue("Direccion", compra.Direccion);
                    cmd.Parameters.AddWithValue("IdDistrito", compra.IdDistrito);
                    cmd.Parameters.AddWithValue("QueryDetalleCompra", query.ToString());
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
                catch (Exception ex)
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }
    }
}