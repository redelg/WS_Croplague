using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ws_croplagueApp.Models;
using static ws_croplagueApp.Models.Plaga;

namespace ws_croplagueApp.Controllers
{
    public class ReconocimientoController : ApiController
    {
        [HttpPost]
        [ActionName("ReconocerPlaga")]
        public HttpResponseMessage ReconocerPlaga([FromBody] Imagen imagen)
        {
            guardarImagen(imagen);

            Resultado res = new Resultado();
            char[] delimiters = {'k'};
            
            string resultado = "";
            ProcessStartInfo startInfo;
            Process process;
            string directory = @"C:\Users\Administrador\Desktop\TensorFlow";
            string script = @"C:\Users\Administrador\Desktop\TensorFlow\test.py";

            startInfo = new ProcessStartInfo(@"C:\Users\Administrador\AppData\Local\Programs\Python\Python36\python.exe");
            startInfo.WorkingDirectory = directory;
            startInfo.Arguments = script;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string s;
            while ((s = process.StandardOutput.ReadLine()) != null)
            {
                resultado += s;
            }
            string[] splitString = resultado.Split(delimiters);
            res.nombrePlaga = splitString[1];
            res.precision = splitString[2];
            double precision = double.Parse(res.precision.Replace(".",","));
            if (precision >= 50)
            {
                guardarEnRetrain(Convert.ToInt32(splitString[0]), imagen);
            }
            else
            {
                guardarParaAnalizar(Convert.ToInt32(splitString[0]), imagen);
            }
            DataTable Plaga = Instancia.ObtenerPlaga(Convert.ToInt32(splitString[0]));
            var Plagas = from c in Plaga.AsEnumerable()
            select new
            {
                Plaga = Convert.ToInt32(c["Plaga"]),
                Nombre = c["Nombre"].ToString(),
                Precision = res.precision,
                Caracteristicas = c["Caracteristicas"].ToString(),
                Tratamiento = c["Tratamiento"].ToString(),
                TipoPlaga = Convert.ToInt32(c["TipoPlaga"])
     
            };

            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, Plagas);

            return httpResponseMessage;
        }

        private void guardarImagen(Imagen imagen) {

            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Users\Administrador\Desktop\TensorFlow\test_images");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            if (imagen.imagen.Length > 0)
            {
                byte[] imgbinaryarray = imagen.imagen;
                String str = Convert.ToBase64String(imgbinaryarray);
                string strdocPath;
                strdocPath = @"C:\Users\Administrador\Desktop\TensorFlow\test_images\" + imagen.nombre;
                FileStream objfilestream = new FileStream(strdocPath, FileMode.Create, FileAccess.ReadWrite);
                objfilestream.Write(imgbinaryarray, 0, imgbinaryarray.Length);
                objfilestream.Close();
            }
        }

        private void guardarEnRetrain (int codigo, Imagen imagen)
        {
            string ruta = "";
            switch (codigo)
            {
                case 1: ruta = @"C:\Users\Administrador\Desktop\TensorFlow\training_images\1kHeliothis\";
                    break;
                case 2: ruta = @"C:\Users\Administrador\Desktop\TensorFlow\training_images\2kArgyrotaenia\";
                    break;
                case 3: ruta = @"C:\Users\Administrador\Desktop\TensorFlow\training_images\3kMoscaBlanca\";
                    break;
            }
                        
            if (imagen.imagen.Length > 0)
            {
                byte[] imgbinaryarray = imagen.imagen;
                String str = Convert.ToBase64String(imgbinaryarray);
                string strdocPath;
                strdocPath = ruta + imagen.nombre;
                FileStream objfilestream = new FileStream(strdocPath, FileMode.Create, FileAccess.ReadWrite);
                objfilestream.Write(imgbinaryarray, 0, imgbinaryarray.Length);
                objfilestream.Close();
            }

        }

        private void guardarParaAnalizar(int codigo, Imagen imagen)
        {
            string ruta = @"C:\Users\Administrador\Desktop\TensorFlow\analizar_images\";

            if (imagen.imagen.Length > 0)
            {
                byte[] imgbinaryarray = imagen.imagen;
                String str = Convert.ToBase64String(imgbinaryarray);
                string strdocPath;
                strdocPath = ruta + imagen.nombre;
                FileStream objfilestream = new FileStream(strdocPath, FileMode.Create, FileAccess.ReadWrite);
                objfilestream.Write(imgbinaryarray, 0, imgbinaryarray.Length);
                objfilestream.Close();
            }

        }



    }
}
