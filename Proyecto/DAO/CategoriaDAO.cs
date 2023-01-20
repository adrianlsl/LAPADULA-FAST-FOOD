using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Data;

namespace Proyecto.DAO
{
    public class CategoriaDAO 
    { 
        private static CategoriaDAO _instancia = null;

        public CategoriaDAO()
        {
        }

        public static CategoriaDAO Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CategoriaDAO();
                }
                return _instancia;
            }
        }
        public List<Categoria> Listar()
        {
            List<Categoria> listacategoria = new List<Categoria>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SP_ObtenerCategoria", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        listacategoria.Add(new Categoria()
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"].ToString()),
                            Descripcion = dr["Descripcion"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"].ToString())
                        });
                    }
                    dr.Close();
                    return listacategoria;
                }
                catch (Exception ex)
                {
                    listacategoria = null;
                    return listacategoria;
                }
            }
        }
        public bool Registrar(Categoria categoria)
        {
            bool respuesta = true;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_RegistrarCategoria", cn);
                    cmd.Parameters.AddWithValue("Descripcion", categoria.Descripcion);
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
        public bool Modificar(Categoria categoria)
        {
            bool respuesta = true;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_EditarCategoria", cn);
                    cmd.Parameters.AddWithValue("IdCategoria", categoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Descripcion", categoria.Descripcion);
                    cmd.Parameters.AddWithValue("Activo", categoria.Activo);
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
        public bool Eliminar(int id)
        {
            bool respuesta = true;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("delete from TB_CATEGORIA where idcategoria = @id", cn);
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
