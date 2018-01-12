using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using LibraryData;
using LibraryServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        public BranchController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}