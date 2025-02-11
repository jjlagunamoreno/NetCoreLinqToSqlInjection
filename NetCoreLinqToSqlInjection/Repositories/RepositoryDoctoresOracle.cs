using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using NetCoreLinqToSqlInjection.Models;
using Oracle.ManagedDataAccess.Client;
using static Azure.Core.HttpHeader;

#region procedure delete Oracle
//create or replace procedure sp_delete_doctor
//(p_iddoctor DOCTOR.DOCTOR_NO%TYPE)
//as
//begin
//  delete from DOCTOR where DOCTOR_NO=p_iddoctor;
//commit;
//end;
#endregion

#region procedure update y select Doctor Oracle
//CREATE OR REPLACE PROCEDURE sp_update_doctor(
//    p_idDoctor IN NUMBER,
//    p_apellido IN VARCHAR2,
//    p_especialidad IN VARCHAR2,
//    p_salario IN NUMBER,
//    p_idHospital IN NUMBER
//)
//AS
//BEGIN
//    UPDATE DOCTOR
//    SET APELLIDO = p_apellido,
//        ESPECIALIDAD = p_especialidad,
//        SALARIO = p_salario,
//        HOSPITAL_COD = p_idHospital
//    WHERE DOCTOR_NO = p_idDoctor;
//END;

//CREATE OR REPLACE PROCEDURE sp_get_by_especialidad(
//    p_especialidad IN VARCHAR2,
//    cur OUT SYS_REFCURSOR
//)
//AS
//BEGIN
//    OPEN cur FOR
//    SELECT DOCTOR_NO, APELLIDO, ESPECIALIDAD, SALARIO, HOSPITAL_COD 
//    FROM DOCTOR 
//    WHERE ESPECIALIDAD = p_especialidad;
//END;
#endregion

namespace NetCoreLinqToSqlInjection.Repositories
{
    public class RepositoryDoctoresOracle : IRepositoryDoctores

    {
        private DataTable tablaDoctores;
        private OracleConnection cn;
        private OracleCommand com;
        public RepositoryDoctoresOracle()
        {
            string connectionString =
            "Data Source=LOCALHOST:1521/XE;Persist Security Info=True; User Id=SYSTEM; Password=oracle;";
            this.tablaDoctores = new DataTable();
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand();
            this.com.Connection = this.cn;
            OracleDataAdapter ad = new OracleDataAdapter
                ("select * from DOCTOR", connectionString);
            ad.Fill(this.tablaDoctores);
        }
        public void DeleteDoctor(int idDoctor)
        {
            string sql = "sp_delete_doctor";
            OracleParameter pamId =
                new OracleParameter(":p_iddoctor", idDoctor);
            this.com.Parameters.Add(pamId);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
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
        public void InsertDoctor(int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "INSERT INTO DOCTOR (DOCTOR_NO, APELLIDO, ESPECIALIDAD, SALARIO, HOSPITAL_COD) " +
                         "VALUES (:iddoctor, :apellido, :especialidad, :salario, :idhospital)";

            this.com.Parameters.Clear(); // Limpiar parámetros antes de añadir nuevos

            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;

            // Añadiendo parámetros con tipo de dato explícito
            this.com.Parameters.Add(new OracleParameter(":iddoctor", OracleDbType.Int32) { Value = idDoctor });
            this.com.Parameters.Add(new OracleParameter(":apellido", OracleDbType.Varchar2) { Value = apellido });
            this.com.Parameters.Add(new OracleParameter(":especialidad", OracleDbType.Varchar2) { Value = especialidad });
            this.com.Parameters.Add(new OracleParameter(":salario", OracleDbType.Int32) { Value = salario });
            this.com.Parameters.Add(new OracleParameter(":idhospital", OracleDbType.Int32) { Value = idHospital });

            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
        }
        public void UpdateDoctor(int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "sp_update_doctor";
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            this.com.Parameters.Clear();
            this.com.Parameters.Add(new OracleParameter("p_idDoctor", idDoctor));
            this.com.Parameters.Add(new OracleParameter("p_apellido", apellido));
            this.com.Parameters.Add(new OracleParameter("p_especialidad", especialidad));
            this.com.Parameters.Add(new OracleParameter("p_salario", salario));
            this.com.Parameters.Add(new OracleParameter("p_idHospital", idHospital));

            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
        }
        public List<Doctor> GetDoctoresByEspecialidad(string especialidad)
        {
            List<Doctor> doctores = new List<Doctor>();

            if (especialidad == "Todas" || string.IsNullOrEmpty(especialidad))
            {
                return GetDoctores(); // Si selecciona "Todas", devolvemos todos los doctores
            }

            string sql = "sp_get_by_especialidad";
            this.com.Parameters.Clear();
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.StoredProcedure;

            this.com.Parameters.Add(new OracleParameter("p_especialidad", OracleDbType.Varchar2) { Value = especialidad });

            OracleParameter outputCursor = new OracleParameter("cur", OracleDbType.RefCursor);
            outputCursor.Direction = ParameterDirection.Output;
            this.com.Parameters.Add(outputCursor);

            this.cn.Open();
            OracleDataReader reader = this.com.ExecuteReader();

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

        public List<string> GetEspecialidades()
        {
            List<string> especialidades = new List<string> { "Todas" }; // Agregamos "Todas" como primera opción
            string sql = "SELECT DISTINCT ESPECIALIDAD FROM DOCTOR";

            this.com.Parameters.Clear();
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;

            this.cn.Open();
            OracleDataReader reader = this.com.ExecuteReader();

            while (reader.Read())
            {
                especialidades.Add(reader.GetString(0));
            }

            reader.Close();
            this.cn.Close();
            return especialidades;
        }
    }
}
