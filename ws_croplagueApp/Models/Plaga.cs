using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace ws_croplagueApp.Models
{
    public class Plaga
    {
        private static readonly Plaga _instacia = new Plaga();

        public static Plaga Instancia
        {
            get { return Plaga._instacia; }
        }
        public class PlagaModel
        {
            public int Plaga { get; set; }
            public string Nombre { get; set; }
            public string Caracteristicas { get; set; }
            public string Tratamiento { get; set; }
            public int TipoPlaga { get; set; }
        }


        public DataTable ObtenerPlaga(int Plaga)
        {
            Database db = new SqlDatabase(ConfigurationManager.ConnectionStrings["Croplague"].ToString());
            DbCommand cmd = db.GetStoredProcCommand("sp_obtenerPlaga");
            db.AddInParameter(cmd, "@prmintPlaga", DbType.Int32, Plaga);
            try
            {
                return db.ExecuteDataSet(cmd).Tables[0];
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}