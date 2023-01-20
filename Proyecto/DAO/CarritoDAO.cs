using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace Proyecto.DAO
{
    public class CarritoDAO
    {
        private static CarritoDAO _instancia = null;

        public CarritoDAO()
        {

        }
        public static CarritoDAO Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CarritoDAO();
                }
                return _instancia;
            }
        }
        public int Registrar(Carrito carrito)
        {
            int respuesta = 0;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                   //Console.Write("Se cae");

                    SqlCommand cmd = new SqlCommand("SP_InsertarCarrito", cn);
                    cmd.Parameters.AddWithValue("IdUsuario", carrito.usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("IdProducto", carrito.producto.IdProducto);
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
        public int Cantidad(int idusuario)
        {
            int respuesta = 0;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("select count(*) from tb_carrito where idusuario = @idusuario", cn);
                    cmd.Parameters.AddWithValue("@idusuario", idusuario);
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    respuesta = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                }
                catch (Exception ex)
                {
                    respuesta = 0;
                }
            }
            return respuesta;
        }
        public List<Carrito> Obtener(int _idusuario)
        {
            List<Carrito> listar = new List<Carrito>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_ObtenerCarrito", cn);
                    cmd.Parameters.AddWithValue("IdUsuario", _idusuario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listar.Add(new Carrito()
                            {

                                IdCarrito = Convert.ToInt32(dr["IdCarrito"].ToString()),
                                producto = new Producto()
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"].ToString()),
                                    Nombre = dr["Nombre"].ToString(),
                                    marca = new Marca() { Descripcion = dr["Descripcion"].ToString() },
                                    Precio = Convert.ToDecimal(dr["Precio"].ToString(), new CultureInfo("es-PE")),
                                    RutaImagen = dr["RutaImagen"].ToString()
                                },

                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    listar = new List<Carrito>();
                }
                return listar;
            }
        }
        public bool Eliminar(string IdCarrito, string IdProducto)
        {
            bool respuesta = true;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("delete from tb_carrito where idcarrito = @idcarrito");
                    query.AppendLine("update TB_PRODUCTO set Stock = Stock + 1 where IdProducto = @idproducto");

                    SqlCommand cmd = new SqlCommand(query.ToString(), cn);
                    cmd.Parameters.AddWithValue("@idcarrito", IdCarrito);
                    cmd.Parameters.AddWithValue("@idproducto", IdProducto);
                    cmd.CommandType = CommandType.Text;

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
        public List<Compra> ObtenerCompra(int IdUsuario)
        {
            List<Compra> detallecompra = new List<Compra>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SP_ObtenerCompra", cn);
                cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cn.Open();
                    using (XmlReader dr = cmd.ExecuteXmlReader())
                    {
                        while (dr.Read())
                        {
                            XDocument doc = XDocument.Load(dr);
                            if (doc.Element("DATA") != null)
                            {
                                detallecompra = (from c in doc.Element("DATA").Elements("TB_COMPRA")
                                                 select new Compra()
                                                 {
                                                     Total = Convert.ToDecimal(c.Element("Total").Value, new CultureInfo("es-PE")),
                                                     FechaTexto = c.Element("Fecha").Value,
                                                     detallecompra = (from d in c.Element("TB_DETALLE_PRODUCTO").Elements("TB_PRODUCTO")
                                                                      select new DetalleCompra()
                                                                      {
                                                                          producto = new Producto()
                                                                          {
                                                                              marca = new Marca() { Descripcion = d.Element("Descripcion").Value },
                                                                              Nombre = d.Element("Nombre").Value,
                                                                              RutaImagen = d.Element("RutaImagen").Value
                                                                          },
                                                                          Total = Convert.ToDecimal(d.Element("Total").Value, new CultureInfo("es-PE")),
                                                                          Cantidad = Convert.ToInt32(d.Element("Cantidad").Value)
                                                                      }).ToList()
                                                 }).ToList();
                            }
                            else
                            {
                                detallecompra = new List<Compra>();
                            }
                        }
                        dr.Close();
                    }
                    return detallecompra;
                }
                catch (Exception ex)
                {
                    detallecompra = null;
                    return detallecompra;
                }
            }
        }
    }
}