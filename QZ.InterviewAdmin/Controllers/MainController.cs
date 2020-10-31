using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Controllers
{
    public class MainController : BaseController
    {
        public IActionResult Index()
        {
            if (!base.ValidUser(out QZ_Model_In_AdminInfo adminInfo))
            {
                return Redirect("/Login/Index");
            }
            else
            {
                return View(adminInfo);
            }
        }
    }
}