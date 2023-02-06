using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web_Asg.DAL;
using Web_Asg.Models;

namespace Web_Asg.Controllers
{
    public class CompCreateController : Controller
    {
        private AreaInterestDAL areaInterestContext = new AreaInterestDAL();
        private CompetitionDAL compCreate = new CompetitionDAL();
        private CompetitionSubmissionDAL compSubContext = new CompetitionSubmissionDAL();
        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Competition> comp = compCreate.GetAllComps();
            return View(comp);
        }
        //get all area of interests
        private List<SelectListItem> GetAreaInterest()
        {
            List<AreaInterest> areaInterestList = areaInterestContext.GetAllAi();
            List<SelectListItem> AreaInterest = new List<SelectListItem>();
            foreach (AreaInterest AI in areaInterestList)
            {
                AreaInterest.Add(new SelectListItem
                {
                    Value = Convert.ToString(AI.AreaInterestID),
                    Text = AI.Name
                });
            }
            return AreaInterest;
        }

        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["AreaInterestList"] = GetAreaInterest();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Competition comp)
        {
            if (ModelState.IsValid)
            {
                comp.CompetitionID = compCreate.AddComp(comp);
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(comp);
            }

        }
        public ActionResult Edit(int id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            Competition comp = compCreate.GetDetails(id);
            //get amt of competitors
            List<CompetitionSubmission> Competitors = compSubContext.GetAllSubmissionsByComp(comp.CompetitionID);
            ViewData["competitorCount"] = Competitors.Count();
            return View(comp);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Competition comp)
        {
            compCreate.Update(comp);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            Competition comp = compCreate.GetDetails(id);
            List<CompetitionSubmission> Competitors = compSubContext.GetAllSubmissionsByComp(comp.CompetitionID);
            ViewData["competitorCount"] = Competitors.Count();
            return View(comp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Competition comp)
        {
            compCreate.Delete(comp.CompetitionID);
            return RedirectToAction("Index");
        }
    }
}
