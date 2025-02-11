using Microsoft.AspNetCore.Mvc;
using NetCoreLinqToSqlInjection.Models;
using NetCoreLinqToSqlInjection.Repositories;

namespace NetCoreLinqToSqlInjection.Controllers
{
    public class DoctoresController : Controller
    {
        IRepositoryDoctores repo;

        public DoctoresController(IRepositoryDoctores repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Doctor> doctores = this.repo.GetDoctores();
            ViewData["ESPECIALIDAD"] = this.repo.GetEspecialidades(); // Cargar especialidades desde el inicio
            return View(doctores);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Doctor doc)
        {
            this.repo.InsertDoctor(doc.IdDoctor, doc.Apellido, doc.Especialidad, doc.Salario, doc.IdHospital);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int idDoctor)
        {
            this.repo.DeleteDoctor(idDoctor);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int idDoctor)
        {
            Doctor doctor = this.repo.GetDoctores().FirstOrDefault(d => d.IdDoctor == idDoctor);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        [HttpPost]
        public IActionResult Edit(Doctor doc)
        {
            this.repo.UpdateDoctor(doc.IdDoctor, doc.Apellido, doc.Especialidad, doc.Salario, doc.IdHospital);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Buscar(string especialidad)
        {
            List<Doctor> doctores;

            if (especialidad == "Todas" || string.IsNullOrEmpty(especialidad))
            {
                doctores = this.repo.GetDoctores(); // Si selecciona "Todas", devolvemos todos los doctores
            }
            else
            {
                doctores = this.repo.GetDoctoresByEspecialidad(especialidad);
            }

            ViewData["ESPECIALIDAD"] = this.repo.GetEspecialidades();
            return View("Index", doctores);
        }
    }
}
