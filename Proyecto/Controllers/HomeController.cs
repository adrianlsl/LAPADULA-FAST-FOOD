using Proyecto.Models;
using Proyecto.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace Proyecto.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Categoria()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        public ActionResult Marca()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        public ActionResult Producto()
        {
            if (Session["Usuario"] == null)
                return RedirectToAction("Index", "Login");
            return View();
        }


        [HttpGet]
        public JsonResult ListarCategoria()
        {
            List<Categoria> oLista = new List<Categoria>();
            oLista = CategoriaDAO.Instancia.Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GuardarCategoria(Categoria objeto)
        {
            bool respuesta = false;
            respuesta = (objeto.IdCategoria == 0) ? CategoriaDAO.Instancia.Registrar(objeto) : CategoriaDAO.Instancia.Modificar(objeto);
            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EliminarCategoria(int id)
        {
            bool respuesta = false;
            respuesta = CategoriaDAO.Instancia.Eliminar(id);
            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }



      
        [HttpGet]
        public JsonResult ListarProducto()
        {
            List<Producto> oLista = new List<Producto>();
            oLista = ProductoDAO.Instancia.Listar();
            oLista = (from o in oLista
                      select new Producto()
                      {
                          IdProducto = o.IdProducto,
                          Nombre = o.Nombre,
                          Descripcion = o.Descripcion,
                          marca = o.marca,
                          categoria = o.categoria,
                          Precio = o.Precio,
                          Stock = o.Stock,
                          RutaImagen = o.RutaImagen,
                          //base64 = Utils.convertirBase64(Server.MapPath(o.RutaImagen)),
                          extension = Path.GetExtension(o.RutaImagen).Replace(".", ""),
                          Activo = o.Activo
                      }).ToList();
          
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase imagenArchivo)
        {

            Response oresponse = new Response() { resultado = true, mensaje = "" };

            try
            {
                Producto oProducto = new Producto();
                oProducto = JsonConvert.DeserializeObject<Producto>(objeto);
                string GuardarEnRuta = "~/Imagenes/Productos/";
                string physicalPath = Server.MapPath("~/Imagenes/Productos");
                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);
                if (oProducto.IdProducto == 0)
                {
                    int id = ProductoDAO.Instancia.Registrar(oProducto);
                    oProducto.IdProducto = id;
                    oresponse.resultado = oProducto.IdProducto == 0 ? false : true;

                }
                else
                {
                    oresponse.resultado = ProductoDAO.Instancia.Editar(oProducto);
                }

                if (imagenArchivo != null && oProducto.IdProducto != 0)
                {
                    string extension = Path.GetExtension(imagenArchivo.FileName);
                    GuardarEnRuta = GuardarEnRuta + oProducto.IdProducto.ToString() + extension;
                    oProducto.RutaImagen = GuardarEnRuta;
                    imagenArchivo.SaveAs(physicalPath + "/" + oProducto.IdProducto.ToString() + extension);

                    oresponse.resultado = ProductoDAO.Instancia.ActualizarRutaImagen(oProducto);
                }
            }
            catch (Exception e)
            {
                oresponse.resultado = false;
                oresponse.mensaje = e.Message;
            }

            return Json(oresponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            respuesta = ProductoDAO.Instancia.Eliminar(id);
            return Json(new { resultado = respuesta }, JsonRequestBehavior.AllowGet);
        }
    }

    public class Response
    {

        public bool resultado { get; set; }
        public string mensaje { get; set; }
    }

}
