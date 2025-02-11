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
            return View(doctores);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Doctor doc)
        {
            this.repo.InsertDoctor(doc.IdDoctor, doc.Apellido
                , doc.Especialidad, doc.Salario, doc.IdHospital);
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


    }
}
