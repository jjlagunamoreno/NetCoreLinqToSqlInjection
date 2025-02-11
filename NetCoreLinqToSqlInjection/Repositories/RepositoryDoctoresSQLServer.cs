using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using NetCoreLinqToSqlInjection.Models;

#region PROCEDURE PARA UPDATE Y BUSQUEDA POR SELECT DOCTOR EN SQL SERVER
//CREATE PROCEDURE sp_update_doctor
//    @idDoctor INT,
//    @apellido NVARCHAR(50),
//    @especialidad NVARCHAR(50),
//    @salario INT,
//    @idHospital INT
//AS
//BEGIN
//    UPDATE DOCTOR
//    SET APELLIDO = @apellido,
//        ESPECIALIDAD = @especialidad,
//        SALARIO = @salario,
//        HOSPITAL_COD = @idHospital
//    WHERE DOCTOR_NO = @idDoctor;
//END;

//CREATE PROCEDURE sp_get_by_especialidad
//    @especialidad NVARCHAR(50)
//AS
//BEGIN
//    SELECT DOCTOR_NO, APELLIDO, ESPECIALIDAD, SALARIO, HOSPITAL_COD 
//    FROM DOCTOR 
//    WHERE ESPECIALIDAD = @especialidad;
//END;


#endregion

namespace NetCoreLinqToSqlInjection.Repositories
{
    public class RepositoryDoctoresSQLServer: IRepositoryDoctores
    {
        private DataTable tableDoctores;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryDoctoresSQLServer()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.tableDoctores = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter
                ("select * from DOCTOR", connectionString);
            ad.Fill(this.tableDoctores);
        }

        public void DeleteDoctor(int idDoctor)
        {
            string sql = "sp_delete_doctor";
            this.com.Parameters.AddWithValue("@iddoctor", idDoctor);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tableDoctores.AsEnumerable()
                           select datos;
            List<Doctor> doctores = new List<Doctor>();
            foreach (var row in consulta)
            {
                Doctor doc = new Doctor();
                doc.IdDoctor = row.Field<int>("DOCTOR_NO");
                doc.Apellido = row.Field<string>("APELLIDO");
                doc.Especialidad = row.Field<string>("ESPECIALIDAD");
                doc.Salario = row.Field<int>("SALARIO");
                doc.IdHospital = row.Field<int>("HOSPITAL_COD");
                doctores.Add(doc);
            }
            return doctores;
        }

        public void InsertDoctor
            (int idDoctor, string apellido, string especialidad
            , int salario, int idHospital)
        {
            string sql = "insert into DOCTOR values (@idhospital, @iddoctor "
                + ", @apellido, @especialidad, @salario)";
            this.com.Parameters.AddWithValue("@iddoctor", idDoctor);
            this.com.Parameters.AddWithValue("@apellido", apellido);
            this.com.Parameters.AddWithValue("@especialidad", especialidad);
            this.com.Parameters.AddWithValue("@salario", salario);
            this.com.Parameters.AddWithValue("@idhospital", idHospital);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
        public void UpdateDoctor(int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "sp_update_doctor";
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.com.Parameters.Clear();
            this.com.Parameters.AddWithValue("@idDoctor", idDoctor);
            this.com.Parameters.AddWithValue("@apellido", apellido);
            this.com.Parameters.AddWithValue("@especialidad", especialidad);
            this.com.Parameters.AddWithValue("@salario", salario);
            this.com.Parameters.AddWithValue("@idHospital", idHospital);

            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
        }

        public List<string> GetEspecialidades()
        {
            List<string> especialidades = new List<string> { "Todas" }; // Agregamos "Todas" como primera opción
            string sql = "SELECT DISTINCT ESPECIALIDAD FROM DOCTOR";

            this.com.Parameters.Clear();
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;

            this.cn.Open();
            SqlDataReader reader = this.com.ExecuteReader();

            while (reader.Read())
            {
                especialidades.Add(reader.GetString(0));
            }

            reader.Close();
            this.cn.Close();
            return especialidades;
        }

        public List<Doctor> GetDoctoresByEspecialidad(string especialidad)
        {
            List<Doctor> doctores = new List<Doctor>();

            string sql = "sp_get_by_especialidad";
            this.com.Parameters.Clear();
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.Parameters.AddWithValue("@especialidad", especialidad);

            this.cn.Open();
            SqlDataReader reader = this.com.ExecuteReader();

            while (reader.Read())
            {
                Doctor doc = new Doctor
                {
                    IdDoctor = reader.GetInt32(0),
                    Apellido = reader.GetString(1),
                    Especialidad = reader.GetString(2),
                    Salario = reader.GetInt32(3),
                    IdHospital = reader.GetInt32(4)
                };
                doctores.Add(doc);
            }

            reader.Close();
            this.cn.Close();
            return doctores;
        }
    }
}
