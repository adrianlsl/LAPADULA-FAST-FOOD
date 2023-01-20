using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using Proyecto.DAO;
using System.Web.Security;

namespace Proyecto.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string correo, string pass)
        {
            Usuario usuario = new Usuario();

            usuario = UsuarioDAO.Instancia.Obtener(correo, pass);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o Contraseña incorrecta";
                return View();
            }

            FormsAuthentication.SetAuthCookie(usuario.Correo, false);
            Session["Usuario"] = usuario;

            if (usuario.EsAdministrador == true)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Tienda");
            }

        }
        public ActionResult Registrarse()
        {
            return View(new Usuario() { Nombres = "", Apellidos = "", Correo = "", Contrasena = "", ConfirmarContrasena = "" });
        }
        [HttpPost]
        public ActionResult Registrarse(string nombres, string apellidos, string correo, string contrasena, string confirmarcontrasena)
        {
            Usuario usuario = new Usuario()
            {
                Nombres = nombres,
                Apellidos = apellidos,
                Correo = correo,
                Contrasena = contrasena,
                ConfirmarContrasena = confirmarcontrasena,
                EsAdministrador = false
            };

            if (contrasena != confirmarcontrasena)
            {
                ViewBag.Error = "Las contrasenas no coinciden";
                return View(usuario);
            }
            else
            {
                int idusuario = UsuarioDAO.Instancia.Registrar(usuario);

                if (idusuario == 0)
                {
                    ViewBag.Error = "Error al registrar";
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
        }
    }
}