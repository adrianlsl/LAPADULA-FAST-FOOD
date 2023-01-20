using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Data;

namespace Proyecto.DAO
{
    public class UsuarioDAO
    {
        private static UsuarioDAO _instancia = null;

        public UsuarioDAO()
        {
        }

        public static UsuarioDAO Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new UsuarioDAO();
                }
                return _instancia;
            }
        }
        public Usuario Obtener(string correo, string pass)
        {
            Usuario objeto = null;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_ObtenerUsuario", cn);
                    cmd.Parameters.AddWithValue("Correo", correo);
                    cmd.Parameters.AddWithValue("Contrasena", pass);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            objeto = new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"].ToString()),
                                Nombres = dr["Nombres"].ToString(),
                                Apellidos = dr["Apellidos"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Contrasena = dr["Contrasena"].ToString(),
                                EsAdministrador = Convert.ToBoolean(dr["EsAdministrador"].ToString())
                            };
                    }
                }
                catch (Exception ex)
                {
                    objeto = null;
                }
            }
            return objeto;
        }
        public int Registrar(Usuario usuario)
        {
            int respuesta = 0;
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_RegistrarUsuario", cn);
                    cmd.Parameters.AddWithValue("Nombres", usuario.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", usuario.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("Contrasena", usuario.Contrasena);
                    cmd.Parameters.AddWithValue("EsAdministrador", usuario.EsAdministrador);
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
    }
}
