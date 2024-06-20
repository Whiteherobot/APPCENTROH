using APPVETERINARIA.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APPVETERINARIA.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {

        public void Add(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;
            using (var connection = GetConnection())
            using (var command = new OracleCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM BV_USUARIO_SISTEMA WHERE USU_USUARIO = :username AND USU_CONTRASEÑA = :password";   // Asegúrate de usar comillas dobles para los nombres de tabla y columna
                command.Parameters.Add(":username", OracleDbType.NVarchar2).Value = credential.UserName;
                command.Parameters.Add(":password", OracleDbType.NVarchar2).Value = credential.Password;
                validUser = command.ExecuteScalar() == null ? false : true;
            }
            return validUser;
        }
        public UserModel GetByUsername(string username)
        {
            UserModel user = null;
            using (var connection = GetConnection())
            using (var command = new OracleCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
                                        SELECT
                                            U.bv_empleado_emp_id,
                                            U.bv_cliente_cli_cedula,
                                            COALESCE(E.EMP_NOMBRES, C.CLI_NOMBRES) AS Nombres,
                                            COALESCE(E.EMP_APELLIDOS, C.CLI_APELLIDOS) AS Apellidos
                                        FROM
                                            BV_USUARIO_SISTEMA U
                                        LEFT JOIN
                                            BV_EMPLEADO E ON U.bv_empleado_emp_id = E.EMP_ID
                                        LEFT JOIN
                                            BV_CLIENTE C ON U.bv_cliente_cli_cedula = C.CLI_CEDULA
                                        WHERE
                                            U.USU_USUARIO = :username";

                command.Parameters.Add(":username", OracleDbType.NVarchar2).Value = username;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader["bv_empleado_emp_id"].ToString(),
                            Username = username,
                            Password = string.Empty,
                            Name = reader["Nombres"].ToString(),
                            LastName = reader["Apellidos"].ToString(),
                        };
                    }
                }
            }
            return user;
        }
    }
}
