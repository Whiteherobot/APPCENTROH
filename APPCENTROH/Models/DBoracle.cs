using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Windows;

namespace WPF_LoginForm.Models
{
    public class DBoracle
    {
        string conexionstring = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;


        public void Conectar()
        {
            string mensaje = "Conectado a Oracle";
            OracleConnection conexion = new OracleConnection(conexionstring);
            conexion.Open();
            MessageBox.Show(mensaje);
        }

    }
}
