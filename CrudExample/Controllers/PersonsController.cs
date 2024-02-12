using Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CrudExample.Controllers
{
    public class PersonsController : Controller
    {
        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
