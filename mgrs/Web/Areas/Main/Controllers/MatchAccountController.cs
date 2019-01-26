using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utls;

namespace Web.Areas.Main.Controllers
{
    public class MatchAccountController : BaseController
    {
        //
        // GET: /Main/MatchAccount/

        public ActionResult Index(string optMatch, string optStatus, int? pageIndex)
        {
            var sysmatchuser = new List<sysmatchuserView>();
            try
            {
                sysmatchuser = new MatchBll().GetSysMatchUser(optMatch, optStatus, pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(14);
                ViewData["Status"] = Status;

                List<SelectListItem> Role = new MemberBll().GetDict(9);
                ViewData["Role"] = Role;

                if (optMatch != null)
                {
                    //获取赛事列表
                    List<SelectListItem> Match = new MatchBll().GetMatchsList();
                    foreach (SelectListItem r in Match)
                    {
                        if (optMatch == r.Value)
                            ViewBag.optMatch += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                        else
                            ViewBag.optMatch += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }
                else
                {

                    //获取赛事列表
                    List<SelectListItem> Match = new MatchBll().GetMatchsList();
                    foreach (SelectListItem r in Match)
                    {
                        ViewBag.optMatch += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }

                foreach (SelectListItem r in Status)
                {
                    if (optStatus == r.Value)
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

            }
            catch (Exception e)
            {

            }
            return View(sysmatchuser);
        }

        public ActionResult Create()
        {
            List<SelectListItem> Status = new MemberBll().GetDict(14);
            foreach (SelectListItem r in Status)
            {
                ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            List<SelectListItem> Role = new MemberBll().GetDict(9);
            foreach (SelectListItem r in Role)
            {
                ViewBag.optRole += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            //获取赛事列表
            List<SelectListItem> Match = new MatchBll().GetMatchsList();
            ViewBag.optMatch += "<option value=''>请选择赛事</option>";
            foreach (SelectListItem r in Match)
            {
                ViewBag.optMatch += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetLinesByMatch(string matchid)
        {
            var bll = new MatchBll();
            List<SelectListItem> lines = new MatchBll().GetlinesList(matchid);
            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            var model = new sysmatchuser();
            model.Id = Guid.NewGuid().ToString();
            model.Match_id = fc["optMatch"].ToString();
            model.Name = fc["Name"];
            model.Role = fc["optRole"].ToString();
            model.Linesid = fc["optLines"].ToString();
            model.Account = fc["Account"].ToString();
            model.Pwd = fc["Pwd"].ToString();
            model.Status = fc["optStatus"].ToString();
          
            var bll = new MatchBll();

            try
            {
                bll.AddSysMatchUser(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }


        public ActionResult Edit(string id)
        {
            var bll = new MatchBll();
            var model = bll.GetSysMatchUserById(id);
            if (model != null)
            {
                List<SelectListItem> Status = new MemberBll().GetDict(14);
                foreach (SelectListItem r in Status)
                {
                    if (model.Status.ToString() == r.Value)
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
                List<SelectListItem> Role = new MemberBll().GetDict(9);
                foreach (SelectListItem r in Role)
                {
                    if (model.Role.ToString() == r.Value)
                        ViewBag.optRole += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optRole += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (model.Match_id.ToString() == r.Value)
                        ViewBag.optMatch += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optMatch += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                List<SelectListItem> lines = new MatchBll().GetlinesList(model.Match_id.ToString());
                foreach (SelectListItem r in lines)
                {
                    if (model.Linesid.ToString() == r.Value)
                        ViewBag.optLines += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optLines += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(string id, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = bll.GetSysMatchUserById(id);
            model.Match_id = fc["optMatch"].ToString();
            model.Name = fc["Name"];
            model.Role = fc["optRole"].ToString();
            model.Linesid = fc["optLines"].ToString();
            model.Account = fc["Account"].ToString();
            model.Pwd = fc["Pwd"].ToString();
            model.Status = fc["optStatus"].ToString();
            try
            {
                bll.EditSysMatchUser(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }
    }
}
