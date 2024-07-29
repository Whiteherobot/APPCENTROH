using APPCENTROH.Models;
using APPCENTROM.ViewModels;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

        public bool AuthenticateUser(NetworkCredential credential, out string role, out bool isActive)
        {
            bool validUser = false;
            role = null;
            isActive = false;

            using (var connection = GetConnection())
            using (var command = new OracleCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
                        SELECT USU_ID, USU_PERMISO, USU_ACTIVO, USU_CONTRASENA 
                        FROM IDM_USUARIOS 
                        WHERE USU_USUARIO = :username";
                command.Parameters.Add(":username", OracleDbType.NVarchar2).Value = credential.UserName;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader["USU_CONTRASENA"].ToString();
                        string isActiveStatus = reader["USU_ACTIVO"].ToString();
                        role = reader["USU_PERMISO"].ToString();

                        // Check if the user is active
                        isActive = isActiveStatus == "1";

                        // Check if the password matches
                        if (credential.Password == storedPassword)
                        {
                            validUser = true;
                        }
                    }
                }
            }

            return validUser;
        }

        public void InsertService(string nombre, decimal precio, string iva)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string queryInsertService = "INSERT INTO IDM_SERVICIOS (SER_ID, SER_NOMBRE, SER_PRECIO, SER_IVA) " +
                                                    "VALUES (SER_ID.NEXTVAL, :nombre, :precio, :iva)";

                        using (var command = new OracleCommand(queryInsertService, connection))
                        {
                            command.Parameters.Add(":nombre", OracleDbType.Varchar2).Value = nombre;
                            command.Parameters.Add(":precio", OracleDbType.Decimal).Value = precio;
                            command.Parameters.Add(":iva", OracleDbType.Char).Value = iva;

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al insertar el servicio en la base de datos: " + ex.Message);
                    }
                }
            }
        }
        public int GetUserId(string username)
        {
            int userId = -1;

            using (var connection = GetConnection())
            using (var command = new OracleCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT USU_ID 
                                    FROM IDM_USUARIOS 
                                    WHERE USU_USUARIO = :username";
                command.Parameters.Add(":username", OracleDbType.NVarchar2).Value = username;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userId = Convert.ToInt32(reader["USU_ID"]);
                    }
                }
            }

            return userId;
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


        public void UpdateUserStatus(string userId, string newStatus)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new OracleCommand("UPDATE IDM_USUARIOS SET USU_ACTIVO = :newStatus WHERE USU_ID = :userId", connection))
                {
                    command.Parameters.Add(":newStatus", OracleDbType.Varchar2).Value = newStatus;
                    command.Parameters.Add(":userId", OracleDbType.Varchar2).Value = userId;

                    command.ExecuteNonQuery();
                }
            }
        }


        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"SELECT p.PER_ID, p.PER_CEDULA, p.PER_NOMBRE, p.PER_APELLIDO, p.PER_DIRECCION, p.PER_TELEFONO, p.PER_EMAIL, 
                                                e.EMP_ID, e.EMP_TIPO, u.USU_USUARIO, u.USU_CONTRASENA, u.USU_ACTIVO
                                     FROM IDM_PERSONAS p
                                     INNER JOIN IDM_EMPLEADOS e ON p.PER_ID = e.IDM_PERSONAS_PER_ID
                                     INNER JOIN IDM_USUARIOS u ON e.EMP_ID = u.IDM_EMPLEADOS_EMP_ID
                                     ORDER BY p.PER_ID", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id2 = reader.GetInt32(reader.GetOrdinal("PER_ID")).ToString(),
                            Cedula = reader.GetString(reader.GetOrdinal("PER_CEDULA")),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            LastName = reader.GetString(reader.GetOrdinal("PER_APELLIDO")),
                            Dirreccion = reader.GetString(reader.GetOrdinal("PER_DIRECCION")),
                            Telefono = reader.GetString(reader.GetOrdinal("PER_TELEFONO")),
                            Email = reader.GetString(reader.GetOrdinal("PER_EMAIL")),
                            Id = reader.GetInt32(reader.GetOrdinal("EMP_ID")).ToString(),
                            EmployeeType = reader.GetString(reader.GetOrdinal("EMP_TIPO")),
                            Username = reader.GetString(reader.GetOrdinal("USU_USUARIO")),
                            Password = reader.GetString(reader.GetOrdinal("USU_CONTRASENA")),
                            IsActive = reader.GetString(reader.GetOrdinal("USU_ACTIVO"))
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
                                     INNER JOIN IDM_PACIENTES pa ON p.PER_ID = pa.IDM_PERSONAS_PER_ID
                                     ORDER BY p.PER_ID", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id2 = reader.GetInt32(reader.GetOrdinal("PER_ID")).ToString(),
                            Cedula = reader.GetString(reader.GetOrdinal("PER_CEDULA")),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            LastName = reader.GetString(reader.GetOrdinal("PER_APELLIDO")),
                            Dirreccion = reader.GetString(reader.GetOrdinal("PER_DIRECCION")),
                            Telefono = reader.GetString(reader.GetOrdinal("PER_TELEFONO")),
                            Email = reader.GetString(reader.GetOrdinal("PER_EMAIL")),
                            Id = reader.GetInt32(reader.GetOrdinal("PAC_ID")).ToString()
                        });
                    }
                }
            }

            return users;
        }

        public List<UserModel> GetAllServices()
        {
            var services = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"SELECT SER_ID, SER_NOMBRE, SER_PRECIO, SER_IVA
                                             FROM IDM_SERVICIOS
                                             ORDER BY SER_ID", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("SER_ID")).ToString(),
                            Name = reader.GetString(reader.GetOrdinal("SER_NOMBRE")),
                            price = reader.GetString(reader.GetOrdinal("SER_PRECIO")),
                            IVA = reader.GetString(reader.GetOrdinal("SER_IVA"))
                        });
                    }
                }
            }

            return services;
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
                                                 WHERE USU_ID IN (SELECT IDM_PERSONAS_PER_ID FROM IDM_EMPLEADOS WHERE IDM_EMPLEADOS_EMP_ID = :userId )";
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

        public bool DeleteService(string serviceId)
        {
            if (int.TryParse(serviceId, out int intServiceId))
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

                            // Eliminar el registro en IDM_SERVICIOS
                            command.CommandText = "DELETE FROM IDM_SERVICIOS WHERE SER_ID = :serviceId";
                            command.Parameters.Clear();
                            command.Parameters.Add(new OracleParameter(":serviceId", OracleDbType.Int32) { Value = intServiceId });
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Error al eliminar servicio: " + ex.Message);
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error al convertir serviceId a int.");
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

        public List<UserModel> SearchUsersByName1(string name)
        {
            var users = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"
        SELECT p.PER_ID, p.PER_NOMBRE, p.PER_APELLIDO, p.PER_CEDULA, p.PER_DIRECCION, p.PER_TELEFONO, p.PER_EMAIL, 
               e.EMP_ID, e.EMP_TIPO, u.USU_USUARIO, u.USU_CONTRASENA, u.USU_PERMISO, u.USU_ACTIVO
        FROM IDM_PERSONAS p
        INNER JOIN IDM_EMPLEADOS e ON p.PER_ID = e.IDM_PERSONAS_PER_ID
        INNER JOIN IDM_USUARIOS u ON e.EMP_ID = u.IDM_EMPLEADOS_EMP_ID
        WHERE p.PER_NOMBRE LIKE :name", connection))
            {
                connection.Open();
                command.Parameters.Add(new OracleParameter(":name", OracleDbType.NVarchar2)).Value = "%" + name + "%";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id2 = reader.GetInt32(reader.GetOrdinal("PER_ID")).ToString(),
                            Cedula = reader.GetString(reader.GetOrdinal("PER_CEDULA")),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            LastName = reader.GetString(reader.GetOrdinal("PER_APELLIDO")),
                            Dirreccion = reader.GetString(reader.GetOrdinal("PER_DIRECCION")),
                            Telefono = reader.GetString(reader.GetOrdinal("PER_TELEFONO")),
                            Email = reader.GetString(reader.GetOrdinal("PER_EMAIL")),
                            Id = reader.GetInt32(reader.GetOrdinal("EMP_ID")).ToString(),
                            EmployeeType = reader.GetString(reader.GetOrdinal("EMP_TIPO")),
                            Username = reader.GetString(reader.GetOrdinal("USU_USUARIO")),
                            Password = reader.GetString(reader.GetOrdinal("USU_CONTRASENA")),
                            
                            IsActive = reader.GetString(reader.GetOrdinal("USU_ACTIVO")) == "1" ? "Activo" : "Inactivo"
                        });
                    }
                }
            }

            return users;
        }


        public List<UserModel> SearchServicesByName(string serviceName)
        {
            var services = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
                                        SELECT SER_ID, SER_NOMBRE, SER_PRECIO, SER_IVA
                                        FROM IDM_SERVICIOS
                                        WHERE UPPER(SER_NOMBRE) LIKE '%' || UPPER(:serviceName) || '%'";
                command.Parameters.Add(new OracleParameter(":serviceName", OracleDbType.Varchar2) { Value = serviceName });

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("SER_ID")).ToString(),
                            Name = reader.GetString(reader.GetOrdinal("SER_NOMBRE")),
                            price = reader.GetString(reader.GetOrdinal("SER_PRECIO")),
                            IVA = reader.GetString(reader.GetOrdinal("SER_IVA"))
                        });
                    }
                }
            }

            return services;
        }

        public bool IsCedulaExists(string cedula)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM idm_personas WHERE per_cedula = :cedula";

                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":cedula", OracleDbType.Varchar2).Value = cedula;
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Retorna true si existe, false si no existe
                }
            }
        }

        public void InsertData(RegisterViewModel registerViewModel, string username, string password, string permiso)
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
                                                         "VALUES (emp_id.NEXTVAL, :tipo, :v_per_id) " +
                                                         "RETURNING emp_id INTO :v_emp_id";

                            command.CommandText = queryInsertEmpleado;
                            command.Parameters.Clear();
                            command.Parameters.Add(":tipo", OracleDbType.Varchar2).Value = registerViewModel.tipo; // Usar emp_tipo de RegisterViewModel
                            command.Parameters.Add(":v_per_id", OracleDbType.Decimal).Value = v_per_id;
                            command.Parameters.Add(":v_emp_id", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();
                            decimal v_emp_id = ((OracleDecimal)command.Parameters[":v_emp_id"].Value).Value;

                            // Inserción en idm_usuarios
                            string queryInsertUsuario = "INSERT INTO idm_usuarios (usu_id, usu_usuario, usu_contrasena, usu_permiso, usu_activo, IDM_EMPLEADOS_EMP_ID) " +
                                                        "VALUES (usu_id.NEXTVAL, :usuario, :contrasena, :permiso, '1', :v_emp_id)";

                            command.CommandText = queryInsertUsuario;
                            command.Parameters.Clear();
                            command.Parameters.Add(":usuario", OracleDbType.Varchar2).Value = username;
                            command.Parameters.Add(":contrasena", OracleDbType.Varchar2).Value = password;
                            command.Parameters.Add(":permiso", OracleDbType.Varchar2).Value = permiso; // Usar permiso seleccionado
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
        public void InsertData1(RegisterViewModel registerViewModel)
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

                            // Inserción en idm_pacientes
                            string queryInsertPaciente = "INSERT INTO idm_pacientes (pac_id, pac_fecha, IDM_PERSONAS_PER_ID) " +
                                                         "VALUES (pac_id.NEXTVAL, :fecha, :v_per_id) " +
                                                         "RETURNING pac_id INTO :v_pac_id";

                            command.CommandText = queryInsertPaciente;
                            command.Parameters.Clear();
                            command.Parameters.Add(":fecha", OracleDbType.Date).Value = DateTime.ParseExact(registerViewModel.fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            command.Parameters.Add(":v_per_id", OracleDbType.Decimal).Value = v_per_id;
                            command.Parameters.Add(":v_pac_id", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();
                            decimal v_pac_id = ((OracleDecimal)command.Parameters[":v_pac_id"].Value).Value;

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


        public List<UserModel> GetDoctors()
        {
            var doctors = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"
        SELECT e.EMP_ID, p.PER_NOMBRE
        FROM IDM_EMPLEADOS e
        JOIN IDM_PERSONAS p ON e.EMP_ID = p.PER_ID
        WHERE e.EMP_TIPO = 'Médico'", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        doctors.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EMP_ID")).ToString(),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE")),
                            // No necesitas añadir el tipo si solo buscas médicos
                        });
                    }
                }
            }

            return doctors;
        }

        public List<UserModel> GetPatients()
        {
            var patients = new List<UserModel>();

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"
        SELECT p.PAC_ID, per.PER_NOMBRE
        FROM IDM_PACIENTES p
        JOIN IDM_PERSONAS per ON p.PAC_ID = per.PER_ID
        WHERE p.PAC_ID IS NOT NULL AND per.PER_NOMBRE IS NOT NULL", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        patients.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("PAC_ID")).ToString(),
                            Name = reader.GetString(reader.GetOrdinal("PER_NOMBRE"))
                        });
                    }
                }
            }

            return patients;
        }

        public bool IsDoctorAvailable(int doctorId, DateTime appointmentDateTime)
        {
            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"SELECT COUNT(*)
                                             FROM CITAS
                                             WHERE IDM_EMPLEADOS_EMP_ID = :doctorId AND CIT_FECHA_HORA = :appointmentDateTime", connection))
            {
                connection.Open();
                command.Parameters.Add(new OracleParameter(":doctorId", OracleDbType.Int32)).Value = doctorId;
                command.Parameters.Add(new OracleParameter(":appointmentDateTime", OracleDbType.Date)).Value = appointmentDateTime;

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count == 0;
            }
        }

        public bool AddAppointment(DateTime appointmentDateTime, string estado, int doctorId, int patientId)
        {
            if (!IsDoctorAvailable(doctorId, appointmentDateTime))
            {
                MessageBox.Show("El médico ya tiene una cita a esta hora.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            using (var connection = GetConnection())
            using (var command = new OracleCommand(@"INSERT INTO CITAS (CIT_FECHA_HORA, CIT_ESTADO, IDM_EMPLEADOS_EMP_ID, IDM_PACIENTES_PAC_ID)
                                             VALUES (:appointmentDateTime, :estado, :doctorId, :patientId)", connection))
            {
                connection.Open();
                command.Parameters.Add(new OracleParameter(":appointmentDateTime", OracleDbType.Date)).Value = appointmentDateTime;
                command.Parameters.Add(new OracleParameter(":estado", OracleDbType.Varchar2)).Value = estado;
                command.Parameters.Add(new OracleParameter(":doctorId", OracleDbType.Int32)).Value = doctorId;
                command.Parameters.Add(new OracleParameter(":patientId", OracleDbType.Int32)).Value = patientId;

                command.ExecuteNonQuery();
            }

            MessageBox.Show("Cita agregada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        public bool AuthenticateUser(NetworkCredential credential)
        {
            throw new NotImplementedException();
        }
    }
}
