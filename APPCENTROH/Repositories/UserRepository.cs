using APPCENTROH.Models;
using APPCENTROM.ViewModels;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Net;
using System.Windows;

namespace APPCENTROH.Repositories
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
                command.CommandText = "SELECT * FROM IDM_USUARIOS WHERE USU_USUARIO = :username AND USU_CONTRASENA = :password";   // Asegúrate de usar comillas dobles para los nombres de tabla y columna
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
                                            U.IDM_EMPLEADOS_EMP_ID,
                                            P.per_cedula,
                                            P.per_nombre AS Nombres,
                                            P.per_apellido AS Apellidos
                                        FROM
                                            idm_usuarios U
                                        LEFT JOIN
                                            idm_empleados E ON U.IDM_EMPLEADOS_EMP_ID = E.emp_id
                                        LEFT JOIN
                                            idm_personas P ON E.IDM_PERSONAS_PER_ID = P.per_id
                                        WHERE
                                            U.USU_USUARIO = :username";

                command.Parameters.Add(":username", OracleDbType.NVarchar2).Value = username;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader["IDM_EMPLEADOS_EMP_ID"].ToString(),
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

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand("SELECT PER_ID, PER_CEDULA, PER_NOMBRE, PER_APELLIDO, PER_DIRECCION, PER_TELEFONO, PER_EMAIL FROM IDM_PERSONAS ORDER BY PER_ID", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            // Corrige los índices y los tipos de datos según la consulta SQL
                            Id = reader.GetInt32(reader.GetOrdinal("PER_ID")).ToString(),
                            Cedula= reader.GetString(reader.GetOrdinal("PER_CEDULA")),// Cambia el índice de columna y el tipo de datos según corresponda
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            LastName = reader.GetString(reader.GetOrdinal("PER_APELLIDO")),
                            Dirreccion = reader.GetString(reader.GetOrdinal("PER_DIRECCION")),
                            Telefono = reader.GetString(reader.GetOrdinal("PER_TELEFONO")),
                            Email = reader.GetString(reader.GetOrdinal("PER_EMAIL"))
                            // Si `Permiso` no es una columna en la consulta, elimínalo del modelo o ajusta la consulta para incluirlo
                        });
                    }
                }
            }

            return users;
        }

        public List<UserModel> GetAllUsers1()
        {
            var users = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"SELECT p.PER_ID, p.PER_CEDULA, p.PER_NOMBRE, p.PER_APELLIDO, p.PER_DIRECCION, p.PER_TELEFONO, p.PER_EMAIL, pa.PAC_ID
                                             FROM IDM_PERSONAS p
                                             LEFT JOIN IDM_PACIENTES pa ON p.PER_ID = pa.IDM_PERSONAS_PER_ID
                                             ORDER BY p.PER_ID", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("PER_ID")).ToString(),
                            Cedula = reader.GetString(reader.GetOrdinal("PER_CEDULA")),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            LastName = reader.GetString(reader.GetOrdinal("PER_APELLIDO")),
                            Dirreccion = reader.GetString(reader.GetOrdinal("PER_DIRECCION")),
                            Telefono = reader.GetString(reader.GetOrdinal("PER_TELEFONO")),
                            Email = reader.GetString(reader.GetOrdinal("PER_EMAIL")),
                            Id2 = reader.IsDBNull(reader.GetOrdinal("PAC_ID")) ? null : reader.GetInt32(reader.GetOrdinal("PAC_ID")).ToString()
                        });
                    }
                }
            }

            return users;
        }

        public bool DeleteUserAndRelatedRecords(string userId)
        {
            if (int.TryParse(userId, out int intUserId))
            {
                using (var connection = GetConnection())
                using (var command = new OracleCommand())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;

                            // Eliminar registros en IDM_USUARIOS
                            command.CommandText = @"
                                                 DELETE FROM IDM_USUARIOS 
                                                 WHERE USU_ID IN (SELECT IDM_EMPLEADOS_EMP_ID FROM IDM_EMPLEADOS WHERE IDM_EMPLEADOS_EMP_ID = :userId )";
                            command.Parameters.Clear();
                            command.Parameters.Add(new OracleParameter(":userId", OracleDbType.Int32) { Value = intUserId });
                            command.ExecuteNonQuery();

                            // Eliminar registros en IDM_EMPLEADOS
                            command.CommandText = "DELETE FROM IDM_EMPLEADOS WHERE IDM_PERSONAS_PER_ID = :userId";
                            command.Parameters.Clear();
                            command.Parameters.Add(new OracleParameter(":userId", OracleDbType.Int32) { Value = intUserId });
                            command.ExecuteNonQuery();

                            // Eliminar el registro en IDM_PERSONAS
                            command.CommandText = "DELETE FROM IDM_PERSONAS WHERE PER_ID = :userId";
                            command.Parameters.Clear();
                            command.Parameters.Add(new OracleParameter(":userId", OracleDbType.Int32) { Value = intUserId });
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Error al eliminar usuario y registros relacionados: " + ex.Message);
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error al convertir userId a int.");
                return false;
            }
        }

        public bool DeleteUserAndRelatedRecords1(string userId)
        {
            if (int.TryParse(userId, out int intUserId))
            {
                using (var connection = GetConnection())
                using (var command = new OracleCommand())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;

                            // Eliminar registros en IDM_PACIENTES
                            command.CommandText = "DELETE FROM IDM_PACIENTES WHERE IDM_PERSONAS_PER_ID = :userId";
                            command.Parameters.Clear();
                            command.Parameters.Add(new OracleParameter(":userId", OracleDbType.Int32) { Value = intUserId });
                            command.ExecuteNonQuery();

                            // Eliminar el registro en IDM_PERSONAS
                            command.CommandText = "DELETE FROM IDM_PERSONAS WHERE PER_ID = :userId";
                            command.Parameters.Clear();
                            command.Parameters.Add(new OracleParameter(":userId", OracleDbType.Int32) { Value = intUserId });
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Error al eliminar usuario y registros relacionados: " + ex.Message);
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error al convertir userId a int.");
                return false;
            }
        }

        public List<UserModel> SearchUsersByName(string name)
        {
            var users = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand("SELECT PER_ID, PER_NOMBRE, PER_APELLIDO, PER_CEDULA, PER_DIRECCION, PER_TELEFONO, PER_EMAIL FROM IDM_PERSONAS WHERE PER_NOMBRE LIKE :name", connection))
            {
                connection.Open();
                command.Parameters.Add(new OracleParameter(":name", OracleDbType.NVarchar2)).Value = "%" + name + "%";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("PER_ID")).ToString(),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            LastName = reader.GetString(reader.GetOrdinal("PER_APELLIDO")),
                            Cedula = reader.GetString(reader.GetOrdinal("PER_CEDULA")),
                            Dirreccion = reader.GetString(reader.GetOrdinal("PER_DIRECCION")),
                            Telefono = reader.GetString(reader.GetOrdinal("PER_TELEFONO")),
                            Email = reader.GetString(reader.GetOrdinal("PER_EMAIL"))
                        });
                    }
                }
            }

            return users;
        }

        public void InsertData(RegisterViewModel registerViewModel, string username, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Inserción en idm_personas
                        string queryInsertPersona = "INSERT INTO idm_personas (per_id, per_cedula, per_nombre, per_apellido, per_direccion, per_telefono, per_email, per_activo) " +
                                                    "VALUES (per_id.NEXTVAL, :cedula, :nombre, :apellido, :direccion, :telefono, :email, '1') " +
                                                    "RETURNING per_id INTO :v_per_id";

                        using (var command = new OracleCommand(queryInsertPersona, connection))
                        {
                            command.Parameters.Add(":cedula", OracleDbType.Varchar2).Value = registerViewModel.cedula;
                            command.Parameters.Add(":nombre", OracleDbType.Varchar2).Value = registerViewModel.nombre;
                            command.Parameters.Add(":apellido", OracleDbType.Varchar2).Value = registerViewModel.apellido;
                            command.Parameters.Add(":direccion", OracleDbType.Varchar2).Value = registerViewModel.direccion;
                            command.Parameters.Add(":telefono", OracleDbType.Varchar2).Value = registerViewModel.telefono;
                            command.Parameters.Add(":email", OracleDbType.Varchar2).Value = registerViewModel.Email;
                            command.Parameters.Add(":v_per_id", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();
                            decimal v_per_id = ((OracleDecimal)command.Parameters[":v_per_id"].Value).Value;

                            // Inserción en idm_empleados
                            string queryInsertEmpleado = "INSERT INTO idm_empleados (emp_id, emp_tipo, IDM_PERSONAS_PER_ID) " +
                                                         "VALUES (emp_id.NEXTVAL, 'Atencion', :v_per_id) " +
                                                         "RETURNING emp_id INTO :v_emp_id";

                            command.CommandText = queryInsertEmpleado;
                            command.Parameters.Clear();
                            command.Parameters.Add(":v_per_id", OracleDbType.Decimal).Value = v_per_id;
                            command.Parameters.Add(":v_emp_id", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();
                            decimal v_emp_id = ((OracleDecimal)command.Parameters[":v_emp_id"].Value).Value;

                            // Inserción en idm_usuarios
                            string queryInsertUsuario = "INSERT INTO idm_usuarios (usu_id, usu_usuario, usu_contrasena, usu_permiso, usu_activo, IDM_EMPLEADOS_EMP_ID) " +
                                                        "VALUES (usu_id.NEXTVAL, :usuario, :contrasena, 'Administrador', '1', :v_emp_id)";

                            command.CommandText = queryInsertUsuario;
                            command.Parameters.Clear();
                            command.Parameters.Add(":usuario", OracleDbType.Varchar2).Value = username;
                            command.Parameters.Add(":contrasena", OracleDbType.Varchar2).Value = password;
                            command.Parameters.Add(":v_emp_id", OracleDbType.Decimal).Value = v_emp_id;

                            command.ExecuteNonQuery();

                            // Commit de la transacción
                            transaction.Commit();
                            MessageBox.Show("Registro exitoso");
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error al registrar datos: " + ex.Message);
                    }
                }
            }
        }
    }
}
