using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Identity.Dapper.Samples.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.Identity.Dapper.Samples.Web.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
