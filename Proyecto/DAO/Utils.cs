using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Proyecto.DAO
{
    public class Utils
    {
        //Convermos la imagen a base 64
        public static string convertirBase64(string ruta)
        {
            byte[] bytes = File.ReadAllBytes(ruta);
            string file = Convert.ToBase64String(bytes);
            return file;
        }
    }
}