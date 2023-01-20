using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace Proyecto.DAO
{
    public class ProductoDAO
    {
        private static ProductoDAO _instancia = null;
        public ProductoDAO()
        {
        }
        public static ProductoDAO Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new ProductoDAO();
                }
                return _instancia;
            }
        }
        public List<Producto> Listar()
        {
            List<Producto> listarproductos = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SP_ObtenerProducto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        listarproductos.Add(new Producto()
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"].ToString()),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            marca = new Marca() { IdMarca = Convert.ToInt32(dr["IdMarca"].ToString()), Descripcion = dr["DescripcionMarca"].ToString() },
                            categoria = new Categoria() { IdCategoria = Convert.ToInt32(dr["IdCategoria"].ToString()), Descripcion = dr["DescripcionCategoria"].ToString() },
                            Precio = Convert.ToDecimal(dr["Precio"].ToString(), new CultureInfo("es-PE")),
                            Stock = Convert.ToInt32(dr["Stock"].ToString()),
                            RutaImagen = dr["RutaImagen"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"].ToString())
                        });
                    }
                    dr.Close();
                    return listarproductos;
                }
                catch (Exception ex)
                {
                    listarproductos = null;
                    return listarproductos;
                }
            }
        }
        public int Registrar(Producto producto)
        {
            int respuesta = 0;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_RegistrarProducto", cn);
                    cmd.Parameters.AddWithValue("Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("IdMarca", producto.marca.IdMarca);
                    cmd.Parameters.AddWithValue("IdCategoria", producto.categoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Precio", producto.Precio);
                    cmd.Parameters.AddWithValue("Stock", producto.Stock);
                    cmd.Parameters.AddWithValue("RutaImagen", producto.RutaImagen);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    respuesta = Convert.ToInt32(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = 0;
                }
            }
            return respuesta;
        }
        public bool Editar(Producto producto)
        {
            bool respuesta = false;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_EditarProducto", cn);
                    cmd.Parameters.AddWithValue("Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("IdMarca", producto.marca.IdMarca);
                    cmd.Parameters.AddWithValue("IdCategoria", producto.categoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Precio", producto.Precio);
                    cmd.Parameters.AddWithValue("Stock", producto.Stock);
                    cmd.Parameters.AddWithValue("RutaImagen", producto.RutaImagen);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
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
        public bool ActualizarRutaImagen(Producto producto)
        {
            bool respuesta = true;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_ActualizarRutaImagen", cn);
                    cmd.Parameters.AddWithValue("IdProducto", producto.IdProducto);
                    cmd.Parameters.AddWithValue("RutaImagen", producto.RutaImagen);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }
        public bool Eliminar(int id)
        {
            bool respuesta = true;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("delete from TB_PRODUCTO where idproducto = @id", cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    respuesta = true;
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