﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForSup.Controllers
{
    [AllowAnonymous]
    public class NoRightController : Controller
    {
        // GET: NoRight
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}