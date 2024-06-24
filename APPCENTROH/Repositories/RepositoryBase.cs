using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace APPCENTROH.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString = "Data Source=localhost:1521/XE; User ID=CentroMedico; Password=12345E;";
        }

        protected OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }

    public class DBoracle
    {
        string conexionstring = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;


        public void Conectar()
        {
            OracleConnection conexion = new OracleConnection(conexionstring);

        }

    }
}
