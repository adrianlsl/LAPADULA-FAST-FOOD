using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using Proyecto.DAO;
using System.IO;
using System.Web.Security;

namespace Proyecto.Controllers
{
    public class TiendaController : Controller
    {
        private static Usuario usuario;
        //  Lapadula fast - food
        public ActionResult Index()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            else
                usuario = (Usuario)Session["Usuario"];

            return View();
        }
        public ActionResult Producto(int idproducto = 0)
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            else
                usuario = (Usuario)Session["Usuario"];
            Producto producto = new Producto();
            List<Producto> lista = new List<Producto>();

            lista = ProductoDAO.Instancia.Listar();
            producto = (from p in lista
                        where p.IdProducto == idproducto
                        select new Producto()
                        {
                            IdProducto = p.IdProducto,
                            Nombre = p.Nombre,
                            Descripcion = p.Descripcion,
                            marca = p.marca,
                            categoria = p.categoria,
                            Precio = p.Precio,
                            Stock = p.Stock,
                            RutaImagen = p.RutaImagen,
                            base64 = Utils.convertirBase64(Server.MapPath(p.RutaImagen)),
                            extension = Path.GetExtension(p.RutaImagen).Replace(".", ""),
                            Activo = p.Activo
                        }).FirstOrDefault();

            return View(producto);
        }
        public ActionResult Carrito()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            else
                usuario = (Usuario)Session["Usuario"];

            return View();
        }
        public ActionResult Compras()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            else
                usuario = (Usuario)Session["Usuario"];

            return View();
        }
        [HttpPost]
        public JsonResult ListarProducto(int idcategoria = 0)
        {
            List<Producto> lista = new List<Producto>();
            lista = ProductoDAO.Instancia.Listar();
            lista = (from p in lista
                     select new Producto()
                     {
                         IdProducto = p.IdProducto,
                         Nombre = p.Nombre,
                         Descripcion = p.Descripcion,
                         marca = p.marca,
                         categoria = p.categoria,
                         Precio = p.Precio,
                         Stock = p.Stock,
                         RutaImagen = p.RutaImagen,
                         base64 = Utils.convertirBase64(Server.MapPath(p.RutaImagen)),
                         extension = Path.GetExtension(p.RutaImagen).Replace(".", ""),
                         Activo = p.Activo
                     }).ToList();
            if (idcategoria != 0)
            {
                lista = lista.Where(x => x.categoria.IdCategoria == idcategoria).ToList();
            }

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarCategoria()
        {
            List<Categoria> lista = new List<Categoria>();
            lista = CategoriaDAO.Instancia.Listar();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertarCarrito(Carrito carrito)
        {
            carrito.usuario = new Usuario() { IdUsuario = usuario.IdUsuario };
            int _respuesta = 0;
            _respuesta = CarritoDAO.Instancia.Registrar(carrito);
            return Json(new { respuesta = _respuesta }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CantidadCarrito()
        {
            int _respuesta = 0;
            _respuesta = CarritoDAO.Instancia.Cantidad(usuario.IdUsuario);
            return Json(new { respuesta = _respuesta }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ObtenerCarrito()
        {
            List<Carrito> _lista = new List<Carrito>();
            _lista = CarritoDAO.Instancia.Obtener(usuario.IdUsuario);

            if (_lista.Count != 0)
            {
                _lista = (from d in _lista
                         select new Carrito()
                         {
                             IdCarrito = d.IdCarrito,
                             producto = new Producto()
                             {
                                 IdProducto = d.producto.IdProducto,
                                 Nombre = d.producto.Nombre,
                                 marca = new Marca() { Descripcion = d.producto.marca.Descripcion},
                                 Precio = d.producto.Precio,
                                 RutaImagen = d.producto.RutaImagen,
                                 base64 = Utils.convertirBase64(Server.MapPath(d.producto.RutaImagen)),
                                 extension = Path.GetExtension(d.producto.RutaImagen).Replace(".", ""),
                             }
                         }).ToList();
            }
            return Json(new { lista = _lista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EliminarCarrito(string IdCarrito, string IdProducto)
        {
            bool respuesta = false;
            respuesta = CarritoDAO.Instancia.Eliminar(IdCarrito, IdProducto);
            return Json(new { respuesta = respuesta }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session["Usuario"] = null;
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public JsonResult ObtenerDepartamento()
        {
            List<Departamento> lista = new List<Departamento>();
            lista = UbicacionDAO.Instancia.ObtenerDepartamento();
            return Json(new { lista = lista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObtenerProvincia(string _IdDepartamento)
        {
            List<Provincia> lista = new List<Provincia>();
            lista = UbicacionDAO.Instancia.ObtenerProvincia(_IdDepartamento);
            return Json(new { lista = lista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RegistrarCompra(Compra compra)
        {
            bool respuesta = false;
            compra.IdUsuario = usuario.IdUsuario;
            respuesta = CompraDAO.Instancia.Registrar(compra);
            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ObtenerCompra()
        {
            List<Compra> _lista = new List<Compra>();
            _lista = CarritoDAO.Instancia.ObtenerCompra(usuario.IdUsuario);
            _lista = (from c in _lista
                      select new Compra()
                     {
                         Total = c.Total,
                         FechaTexto = c.FechaTexto,
                         detallecompra = (from dc in c.detallecompra
                                          select new DetalleCompra() { 
                                              producto = new Producto() { 
                                                  marca = new Marca() { Descripcion = dc.producto.marca.Descripcion },
                                                  Nombre = dc.producto.Nombre,
                                                  RutaImagen = dc.producto.RutaImagen,
                                                  base64 = Utils.convertirBase64(Server.MapPath(dc.producto.RutaImagen)),
                                                  extension = Path.GetExtension(dc.producto.RutaImagen).Replace(".", ""),
                                              },
                                              Total = dc.Total,
                                              Cantidad = dc.Cantidad
                                          }).ToList()
                     }).ToList();
            return Json(new { lista = _lista }, JsonRequestBehavior.AllowGet);
        }
    }
}