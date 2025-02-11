using NetCoreLinqToSqlInjection.Models;
using System.Collections.Generic;

namespace NetCoreLinqToSqlInjection.Repositories
{
    public interface IRepositoryDoctores
    {
        List<Doctor> GetDoctores();

        void InsertDoctor(int idDoctor, string apellido, string especialidad, int salario, int idHospital);

        void DeleteDoctor(int idDoctor);

        void UpdateDoctor(int idDoctor, string apellido, string especialidad, int salario, int idHospital);

        List<Doctor> GetDoctoresByEspecialidad(string especialidad);
        List<string> GetEspecialidades();

    }
}
