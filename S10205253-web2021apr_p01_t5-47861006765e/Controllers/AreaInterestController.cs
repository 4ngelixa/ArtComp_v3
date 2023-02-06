using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{
    public class AreaInterestController : Controller
    {
        private AreaInterestDAL areaInterestContext = new AreaInterestDAL();
        private AreaInterestDAL CompAmt = new AreaInterestDAL();
        private CompetitionDAL compContext = new CompetitionDAL();
        //Get List of area of interests and pass to view
        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<AreaInterest> areaInterestiList = areaInterestContext.GetAllAi();
            return View(areaInterestiList);
        }


        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Create Area Interest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AreaInterest AI)
        {
            AI.AreaInterestID = areaInterestContext.Add(AI);
            return RedirectToAction("Index");
        }



        // GET: AreaInterestController/Delete/5
        public ActionResult Delete(int id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            AreaInterest areaInterest = areaInterestContext.GetDetails(id);
            AreaInterestViewModel areaInterestVM = MapToAreaInterestVM(areaInterest);
            return View(areaInterestVM);
        }

        //make the view model 
        public AreaInterestViewModel MapToAreaInterestVM(AreaInterest areaInterest)
        {
            int id = 0;
            List<Competition> compList = compContext.GetAllComps();
            foreach (Competition c in compList)
            {
                if (areaInterest.AreaInterestID == c.AreaInterestID)
                {
                    id += 1;
                }
            }
            AreaInterestViewModel areaInterestVM = new AreaInterestViewModel
            {
                AreaInterestID = areaInterest.AreaInterestID,
                Name = areaInterest.Name,
                CompNo = id
            };
            return areaInterestVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AreaInterestViewModel areaInterestVM)
        {
            if (areaInterestVM.CompNo == 0)
            {
                areaInterestContext.Delete(areaInterestVM.AreaInterestID);
            }
            //areaInterestContext.Delete(areaInterestVM.AreaInterestID);
            return RedirectToAction("Index");
        }
    }
}
