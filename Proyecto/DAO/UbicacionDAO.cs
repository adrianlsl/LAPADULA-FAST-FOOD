using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Proyecto.Models;
using System.Data;

namespace Proyecto.DAO
{
    public class UbicacionDAO
    {
        private static UbicacionDAO _instancia = null;

        public UbicacionDAO()
        {
        }

        public static UbicacionDAO Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new UbicacionDAO();
                }
                return _instancia;
            }
        }
        public List<Departamento> ObtenerDepartamento()
        {
            List<Departamento> listar = new List<Departamento>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tb_departamento", cn);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listar.Add(new Departamento()
                            {
                                IdDepartamento = dr["IdDepartamento"].ToString(),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    listar = new List<Departamento>();
                }
            }
            return listar;
        }
        public List<Provincia> ObtenerProvincia(string _iddepartamento)
        {
            List<Provincia> listar = new List<Provincia>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tb_provincia where IdDepartamento @ iddepartamento", cn);
                    cmd.Parameters.AddWithValue("@iddepartamento", _iddepartamento);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listar.Add(new Provincia()
                            {
                                IdProvincia = dr["IdProvincia"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                IdDepartamento = dr["IdDepartamento"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    listar = new List<Provincia>();
                }
            }
            return listar;
        }
        public List<Distrito> ObtenerDistrito(string _idprovincia, string _iddepartamento)
        {
            List<Distrito> listar = new List<Distrito>();
            using (SqlConnection cn = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from tb_distrito where IdProvincia = @idprovincia and IdDepartamento = @iddepartamento", cn);
                    cmd.Parameters.AddWithValue("@idprovincia", _idprovincia);
                    cmd.Parameters.AddWithValue("@iddepartamento", _iddepartamento);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listar.Add(new Distrito()
                            {
                                IdDistrito = dr["IdDistrito"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                IdProvincia = dr["IdProvincia"].ToString(),
                                IdDepartamento = dr["IdDepartamento"].ToString()
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    listar = new List<Distrito>();
                }
            }
            return listar;
        }
    }
}