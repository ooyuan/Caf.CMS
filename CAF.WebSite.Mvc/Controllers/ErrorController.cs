﻿
using CAF.WebSite.Mvc.Infrastructure.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 

namespace CAF.WebSite.Mvc.Controllers
{
	// Keep this very simple: no dependencies at all!
	public class ErrorController : Controller
	{

		[MapLegacyRoutes]
		public ActionResult NotFound()
		{
			return HttpNotFound();
		}

		public ActionResult Index()
		{
			this.Response.StatusCode = 500;
			this.Response.TrySkipIisCustomErrors = true;

			return View("Error");
		}

		//public ActionResult DoThrow()
		//{
		//	throw Error.Application("This error was thrown on purpose for testing reasons.");
		//}

	}
}