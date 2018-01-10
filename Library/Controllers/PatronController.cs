using Library.Models.Patron;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IPatron _patron;
        public PatronController(IPatron patron)
        {
            _patron = patron;
        }

        public IActionResult Index()
        {
            var allPatron = _patron.GetAll();

            var patronModel = allPatron
                .Select(p => new PatronDetailModel
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    LibraryCardId = p.LibraryCard.Id,
                    OverdueFees = p.LibraryCard.Fees,
                    HomeBranchLibrary = p.HomeLibraryBranch.Name,
                })
            .ToList();

            var model = new PatronIndexModel
            {
                Patron = patronModel
            };

            return View(model);
        }

        //public IActionResult Detail(int id)
        //{
        //    var patron = _patron.Get(id);

        //    var

        //    return View();
        //}

    }
}
