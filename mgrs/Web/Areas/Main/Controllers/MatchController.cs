using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using Utls;
using BLL;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Web.Areas.Main.Controllers
{
    public class MatchController : BaseController
    {
        //
        // GET: /Main/Match/

        public ActionResult Index(string matchname, string area2, int? pageIndex)
        {
            var teams = new List<tblmatch>();
            try
            {
                teams = new MatchBll().GetMatchs(matchname, area2, pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(4);
                ViewData["Status"] = Status;
            }
            catch (Exception e)
            {

            }
            return View(teams);
        }

        public ActionResult Create()
        {

            List<SelectListItem> Status = new MemberBll().GetDict(4);
            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            List<SelectListItem> Ispic = new MemberBll().GetDict(10);
            foreach (SelectListItem r in Ispic)
            {
                ViewBag.Ispic += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection fc)
        {
            var model = new tblmatch();
            model.Match_id = Guid.NewGuid().ToString();
            model.Match_name = fc["Match_name"].ToString();
            model.Content = fc["Content"].ToString();
            model.Notice = fc["Notice"].ToString();
            model.Area1 = fc["Area1"].ToString();
            model.Area2 = fc["Area2"].ToString();
            if (fc["Date1"] != "")
            {
                model.Date1 = DateTime.Parse(fc["Date1"].ToString());
            }
            if (fc["Date2"] != "")
            {
                model.Date2 = DateTime.Parse(fc["Date2"].ToString());
            }
            if (fc["Date3"] != "")
            {
                model.Date3 = DateTime.Parse(fc["Date3"].ToString());
            }
            if (fc["Date4"] != "")
            {
                model.Date4 = DateTime.Parse(fc["Date4"].ToString());
            }
            string filename = "";

            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["Pic1"];
            if (file != null)
            {
                filename = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && file.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Big_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename;
                    file.SaveAs(path + filename);
                    model.Pic1 = filename;
                }
            }

            HttpPostedFileBase file2 = files["Pic2"];
            if (file2 != null)
            {
                string filename2 = file2.FileName.Substring(file2.FileName.LastIndexOf(".") + 1);
                if ((filename2.ToUpper() == "PNG" || filename2.ToUpper() == "JPG") && file2.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Small_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename2;
                    file2.SaveAs(path + filename);
                    model.Pic2 = filename;
                }
            }

            HttpPostedFileBase Tasklogo = files["Tasklogo"];
            if (Tasklogo != null)
            {
                filename = Tasklogo.FileName.Substring(Tasklogo.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && Tasklogo.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Tasklogo_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename;
                    Tasklogo.SaveAs(path + filename);
                    model.Tasklogo = filename;
                }
            }

            HttpPostedFileBase Logopic = files["Logopic"];
            if (Logopic != null)
            {
                string filename2 = Logopic.FileName.Substring(Logopic.FileName.LastIndexOf(".") + 1);
                if ((filename2.ToUpper() == "PNG" || filename2.ToUpper() == "JPG") && Logopic.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Logopic_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename2;
                    Logopic.SaveAs(path + filename);
                    model.Logopic = filename;
                }
            }

            //string[] otherPics = null;
            //if(Request.Files.Count > 2){
            //    otherPics = new string[Request.Files.Count - 2];
            //    for (int i = 0; i < Request.Files.Count - 2; i++)
            //    {
            //        HttpPostedFileBase otherpic = files[i + 2];
            //        if (otherpic != null)
            //        {
            //            string filename3 = otherpic.FileName.Substring(otherpic.FileName.LastIndexOf(".") + 1);
            //            if ((filename3.ToUpper() == "PNG" || filename3.ToUpper() == "JPG") && otherpic.ContentLength / 1024 < 2000)
            //            {
            //                string path = Server.MapPath("~/UploadFiles/");
            //                filename = "Other_" + i.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename3;
            //                otherpic.SaveAs(path + filename);
            //                otherPics[i] = filename;
            //            }
            //        }
            //    }
            //}


            model.Ispic = fc["optIspic"].ToString();
            model.Status = fc["optStatus"].ToString();

            var bll = new MatchBll();

            try
            {
                bll.AddMatch(model, null);
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
            var model = bll.GetMatchById(id);
           // var otherPics = bll.GetOtherPics(id);
            //ViewData["otherPics"] = otherPics;
            List<SelectListItem> Status = new MemberBll().GetDict(4);
            foreach (SelectListItem r in Status)
            {
                if (model.Status.ToString() == r.Value)
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            List<SelectListItem> Ispic = new MemberBll().GetDict(10);

            foreach (SelectListItem r in Ispic)
            {
                if (model.Ispic.ToString() == r.Value)
                    ViewBag.Ispic += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.Ispic += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            return View(model);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string id, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = bll.GetMatchById(id);

            model.Match_name = fc["Match_name"].ToString();
            model.Content = fc["Content"].ToString();
            model.Notice = fc["Notice"].ToString();
            model.Area1 = fc["Area1"].ToString();
            model.Area2 = fc["Area2"].ToString();
            if (fc["Date1"] != "")
            {
                model.Date1 = DateTime.Parse(fc["Date1"].ToString());
            }
            if (fc["Date2"] != "")
            {
                model.Date2 = DateTime.Parse(fc["Date2"].ToString());
            }
            if (fc["Date3"] != "")
            {
                model.Date3 = DateTime.Parse(fc["Date3"].ToString());
            }
            if (fc["Date4"] != "")
            {
                model.Date4 = DateTime.Parse(fc["Date4"].ToString());
            }

            string filename = "";
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["Pic1"];
            if (file != null)
            {
                filename = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && file.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Big_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename;
                    file.SaveAs(path + filename);
                    model.Pic1 = filename;
                }
            }

            HttpPostedFileBase file2 = files["Pic2"];
            if (file2 != null)
            {
                string filename2 = file2.FileName.Substring(file2.FileName.LastIndexOf(".") + 1);
                if ((filename2.ToUpper() == "PNG" || filename2.ToUpper() == "JPG") && file2.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Small_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename2;
                    file2.SaveAs(path + filename);
                    model.Pic2 = filename;
                }
            }

            HttpPostedFileBase Tasklogo = files["Tasklogo"];
            if (Tasklogo != null)
            {
                filename = Tasklogo.FileName.Substring(Tasklogo.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && Tasklogo.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Tasklogo_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename;
                    Tasklogo.SaveAs(path + filename);
                    model.Tasklogo = filename;
                }
            }

            HttpPostedFileBase Logopic = files["Logopic"];
            if (Logopic != null)
            {
                string filename2 = Logopic.FileName.Substring(Logopic.FileName.LastIndexOf(".") + 1);
                if ((filename2.ToUpper() == "PNG" || filename2.ToUpper() == "JPG") && Logopic.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Logopic_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename2;
                    Logopic.SaveAs(path + filename);
                    model.Logopic = filename;
                }
            }
            //string[] otherPics = null;
            //if (Request.Files.Count > 2)
            //{
            //    otherPics = new string[Request.Files.Count - 2];
            //    for (int i = 0; i < Request.Files.Count - 2; i++)
            //    {
            //        HttpPostedFileBase otherpic = files[i + 2];
            //        if (otherpic != null && otherpic.ContentLength > 0)
            //        {
            //            string filename3 = otherpic.FileName.Substring(otherpic.FileName.LastIndexOf(".") + 1);
            //            if ((filename3.ToUpper() == "PNG" || filename3.ToUpper() == "JPG") && otherpic.ContentLength / 1024 < 2000)
            //            {
            //                string path = Server.MapPath("~/UploadFiles/");
            //                filename = "Other_" + i.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename3;
            //                otherpic.SaveAs(path + filename);
            //                otherPics[i] = filename;
            //            }
            //        }
            //    }
            //}

            model.Ispic = fc["optIspic"].ToString();
            model.Status = fc["optStatus"].ToString();
            try
            {
                bll.EditMatch(model, null);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                List<SelectListItem> Status = new MemberBll().GetDict(3);
                foreach (SelectListItem r in Status)
                {
                    if (model.Status.ToString() == r.Value)
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
                List<SelectListItem> Ispic = new MemberBll().GetDict(10);

                foreach (SelectListItem r in Ispic)
                {
                    if (model.Ispic.ToString() == r.Value)
                        ViewBag.Ispic += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Ispic += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
                return View(model);
            }

            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult Delete(List<string> ids)
        {
            var bll = new MatchBll();

            try
            {
                bll.DeleteMatch(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public ActionResult lineType(string matchname, string name, int? pageIndex)
        {
            var line = new List<tbllineView>();
            try
            {
                line = new MatchBll().GetLine(matchname, name, pageIndex.GetValueOrDefault(1));

                List<SelectListItem> Status = new MemberBll().GetDict(3);
                ViewData["Status"] = Status;

            }
            catch (Exception e)
            {

            }
            return View(line);
        }

        public ActionResult LineCreate()
        {
            List<SelectListItem> Status = new MemberBll().GetDict(3);
            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            //获取赛事列表
            List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
            foreach (SelectListItem r in Matchs)
            {
                ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }


            return View();
        }

        [HttpPost]
        public ActionResult LineCreate(FormCollection fc)
        {
            var model = new tblline();
            var bll = new MatchBll();
            model.Match_id = fc["optMatch"].ToString();
            model.Lineid = Guid.NewGuid().ToString();
            model.Name = fc["Name"].ToString().Trim();
            model.Content = fc["Content"].ToString().Trim();
            //model.personprice = Decimal.Parse(fc["personprice"]);
            //model.teamprice = Decimal.Parse(fc["teamprice"]);
            model.Players = Int32.Parse(fc["Players"].ToString().Trim());
            model.Count = Int32.Parse(fc["Count"].ToString().Trim());
            model.Conditions = "{players:\"" + model.Players.ToString() + "\",count:\"" + model.Count.ToString() + "\"}";
            model.Createtime = DateTime.Now;
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            try
            {
                bll.AddLine(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        public ActionResult LineEdit(string id)
        {
            var bll = new MatchBll();
            var model = bll.GetLineById(id);
            if (model != null)
            {
                List<SelectListItem> Status = new MemberBll().GetDict(3);
                foreach (SelectListItem r in Status)
                {
                    if (model.Status.ToString() == r.Value)
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (model.Match_id.ToString() == r.Value)
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult LineEdit(string id,FormCollection fc)
        {
            var bll = new MatchBll();
            var model = bll.GetLineById(id);
            model.Match_id = fc["optMatch"].ToString();
            model.Name = fc["Name"].ToString().Trim();
            model.Content = fc["Content"].ToString().Trim();
            //model.personprice = Decimal.Parse(fc["personprice"]);
            //model.teamprice = Decimal.Parse(fc["teamprice"]);
            model.Players = Int32.Parse(fc["Players"].ToString().Trim());
            model.Count = Int32.Parse(fc["Count"].ToString().Trim());
            model.Conditions = "{players:\"" + model.Players.ToString() + "\",count:\"" + model.Count.ToString() + "\"}";
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            try
            {
                bll.EditLine(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult LineDelete(List<string> ids)
        {
            var bll = new MatchBll();

            try
            {
                bll.DeleteLine(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("line");
        }

        public ActionResult lines(string optMatch, string optLine, int? pageIndex)
        {
            var lines = new List<tbllinesView>();
            try
            {
                lines = new MatchBll().GetLines(optMatch, optLine, pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(3);
                ViewData["Status"] = Status;

                if (optMatch != "")
                {
                    //获取赛事列表
                    List<SelectListItem> Match = new MatchBll().GetMatchsList();
                    foreach (SelectListItem r in Match)
                    {
                        if (optMatch == r.Value)
                            ViewBag.Match += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                        else
                            ViewBag.Match += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }
                else
                {

                    //获取赛事列表
                    List<SelectListItem> Match = new MatchBll().GetMatchsList();
                    foreach (SelectListItem r in Match)
                    {
                        ViewBag.Match += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }

                if (optLine != "")
                {
                    List<SelectListItem> line = new MatchBll().GetlineList(optMatch);
                    foreach (SelectListItem r in line)
                    {
                        if (optLine == r.Value)
                            ViewBag.Line += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                        else
                            ViewBag.Line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }

            }
            catch (Exception e)
            {

            }
            return View(lines);
        }

        [HttpPost]
        public ActionResult GetLinesByLine(string lineid)
        {
            var bll = new MatchBll();
            List<SelectListItem> line = new MatchBll().GetlinesListByline(lineid);
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLineByMatch(string matchid)
        {
            var bll = new MatchBll();
            List<SelectListItem> line = new MatchBll().GetlineList(matchid);
            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LinesEdit(string id)
        {
            var bll = new MatchBll();
            var model = bll.GetLinesById(id);
            if (model != null)
            {
                List<SelectListItem> Status = new MemberBll().GetDict(3);
                foreach (SelectListItem r in Status)
                {
                    if (model.Status.ToString() == r.Value)
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (model.Matchid.ToString() == r.Value)
                        ViewBag.Match += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Match += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                List<SelectListItem> line = new MatchBll().GetlineList(model.Matchid);
                foreach (SelectListItem r in line)
                {
                    if (model.Lineid == r.Value)
                        ViewBag.Line += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                    else
                        if (r.Value != null && r.Text != null)
                        {
                            ViewBag.Line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                        }
                        
                }
            }


            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LinesEdit(string id, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = bll.GetLinesById(id);
            model.Matchid = fc["optMatch"].ToString();
            if (fc["optLine"] != "")
            {
                model.Lineid = fc["optLine"].ToString();
            }
            model.Linename = fc["Linename"].ToString().Trim();
            model.Content = fc["Content"].ToString().Trim();
            model.Notice = fc["Notice"].ToString().Trim();
            model.Price = fc["Price"].ToString().Trim();
            if (fc["Paycount"] != "")
            {
                model.Paycount = Int32.Parse(fc["Paycount"].ToString().Trim());
            }
            
            if (fc["Lineno"] != "")
            {
                model.Lineno = fc["Lineno"].ToString().Trim();
            }
            model.Playercount = Int32.Parse(fc["Playercount"].ToString().Trim());
            model.Pointscount = Int32.Parse(fc["Pointscount"].ToString().Trim());
           // model.Condition_Sex = Int32.Parse(fc["Condition_Sex"].ToString().Trim());
            //model.Condition_Age = Int32.Parse(fc["Condition_Age"].ToString().Trim());
            //model.Condition_Subline = Int32.Parse(fc["Condition_Subline"].ToString().Trim());
            model.Url = fc["Url"].ToString();
            model.Summary = fc["Summary"].ToString();
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            try
            {
                bll.EditLines(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        public ActionResult LinesCreate()
        {
            List<SelectListItem> Status = new MemberBll().GetDict(3);
            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            //获取赛事列表
            List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
            foreach (SelectListItem r in Matchs)
            {
                ViewBag.Match += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }


            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LinesCreate(string id, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = new tbllines();
            model.Linesid = Guid.NewGuid().ToString();
            model.Matchid = fc["optMatch"].ToString();
            if (fc["optLine"] != "")
            {
                model.Lineid = fc["optLine"].ToString();
            }
            model.Linename = fc["Linename"].ToString().Trim();
            model.Content = fc["Content"].ToString().Trim();
            model.Notice = fc["Notice"].ToString().Trim();
            if (fc["Paycount"] != "")
            {
                model.Paycount = Int32.Parse(fc["Paycount"].ToString().Trim());
            }
            if (fc["Lineno"] != "")
            {
                model.Lineno = fc["Lineno"].ToString().Trim();
            }
            if (fc["Playercount"].ToString() != "")
            {
                model.Playercount = Int32.Parse(fc["Playercount"].ToString().Trim());
            }
            if (fc["Pointscount"].ToString() != "")
            {
                model.Pointscount = Int32.Parse(fc["Pointscount"].ToString().Trim());
            }
            
            model.Price = fc["Price"].ToString().Trim();
            // model.Condition_Sex = Int32.Parse(fc["Condition_Sex"].ToString().Trim());
            //model.Condition_Age = Int32.Parse(fc["Condition_Age"].ToString().Trim());
            //model.Condition_Subline = Int32.Parse(fc["Condition_Subline"].ToString().Trim());
            model.Url = fc["Url"].ToString();
            model.Summary = fc["Summary"].ToString();
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            model.Createtime = DateTime.Now;
            try
            {
                bll.AddLines(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        public ActionResult points(string id,int? pageIndex)
        {
            var points = new List<tblpointsView>();
            try
            {
                points = new MatchBll().GetPoints(id, pageIndex.GetValueOrDefault(1));
                if(points!=null)
                ViewBag.linesid = points[0].Lineguid;
            }
            catch (Exception e)
            {

            }
            return View(points);
        }


        public ActionResult PointsEdit(string id)
        {
            var bll = new MatchBll();
            var model = bll.GetPointById(id);
            List<SelectListItem> Status = new MemberBll().GetDict(3);
            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PointsEdit(string id, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = bll.GetPointById(id);
            model.Pointname = fc["Pointname"].ToString();
            model.Content = fc["Content"].ToString().Trim();
            model.Sort = Int32.Parse(fc["Sort"].ToString().Trim());
            model.Pointtype = Int32.Parse(fc["Pointtype"].ToString().Trim());
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            try
            {
                bll.EditPoints(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        [HttpPost]
         public ActionResult ckeditorUpload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string filename = "";
            string path = Server.MapPath("~/UploadFiles/");
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["upload"];
            if (file != null)
            {
                filename = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && file.ContentLength / 1024 < 2000)
                {
                   
                    filename = Guid.NewGuid().ToString() + "." + filename;
                    file.SaveAs(path + filename);
                }
            }

            var imageUrl = Url.Content("~/UploadFiles/" + filename);
            var vMessage = string.Empty;

            var result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
 


            return Content(result);


        }
        

        public ActionResult PointsCreate(string id)
        {
            var bll = new MatchBll();
            ViewBag.linesid = id;
            var lines = bll.GetLinesById(id);
            ViewBag.linesname = lines.Linename;
            List<SelectListItem> Status = new MemberBll().GetDict(3);
            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PointsCreate(string id, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = new tblpoints();
            model.Pointid = Guid.NewGuid().ToString();
            model.Lineguid = fc["linesid"].ToString();
            model.Pointname = fc["Pointname"].ToString();
            model.Content = fc["Content"].ToString().Trim();
            model.Sort = Int32.Parse(fc["Sort"].ToString().Trim());
            model.Pointtype = Int32.Parse(fc["Pointtype"].ToString().Trim());
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            try
            {
                bll.AddPoints(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }


        [HttpPost]
        public ActionResult PointDelete(List<string> ids, string linesid)
        {
            var bll = new MatchBll();

            try
            {
                bll.DeletePoint(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("points", new { id = linesid });
        }

        [HttpPost]
        public ActionResult LinesDelete(List<string> ids)
        {
            var bll = new MatchBll();

            try
            {
                bll.DeleteLines(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("lines");
        }


        public ActionResult pics(string matchname, int? pageIndex)
        {
            var matchpics = new List<tblmatchpicsView>();
            try
            {
                matchpics = new MatchBll().GetMatchPics(matchname,pageIndex.GetValueOrDefault(1));
            }
            catch (Exception e)
            {

            }
            return View(matchpics);
        }

        [HttpPost]
        public ActionResult PicDel(List<string> ids)
        {
            var bll = new MatchBll();

            try
            {
                bll.DeletePics(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("pics");
        }

        public ActionResult CreatePic()
        {
            //获取赛事列表
            List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
            foreach (SelectListItem r in Matchs)
            {
                ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreatePic(FormCollection fc)
        {
            var model = new tblmatchpics();
            model.Id = Guid.NewGuid().ToString();
            model.Match_id = fc["optMatch"].ToString();
           
            string filename = "";

            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["Picture"];
            if (file != null)
            {
                filename = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && file.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "Big_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename;
                    file.SaveAs(path + filename);
                    model.Picture = filename;
                }
            }
            model.Createtime = DateTime.Now;
            var bll = new MatchBll();

            try
            {
                bll.AddMatchPics(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }


        public ActionResult Coupon(string matchname, string teamname,string company, string mobile,string couponchar, string optType,string optStatus, int? pageIndex)
        {
            var coupon = new List<tblcouponView>();
            try
            {
                coupon = new MatchBll().GetCoupons(matchname, teamname, company, mobile,couponchar, optType, optStatus, pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(11);
                ViewData["Status"] = Status;
                foreach (SelectListItem r in Status)
                {
                    if (optStatus == r.Value)
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
                List<SelectListItem> Types = new MemberBll().GetDict(17);
                ViewData["TTTT"] = Types;
                foreach (SelectListItem r in Types)
                {
                    if (optType == r.Value)
                        ViewBag.Type += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Type += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }
            catch (Exception e)
            {

            }
            return View(coupon);
        }


        [HttpPost]
        public string ExpCoupon(string matchname, string teamname, string company, string mobile, string couponchar, string optType, string optStatus)
        {
            List<tblcouponView> o = new MatchBll().GetCoupons(matchname, teamname, company, mobile, couponchar, optType, optStatus);
          
            string ExportField = "Teamname,Nickname,Company,Mobile,Couponchar,Lines_name,Usedtime,Type,Status";
            string[] fields = ExportField.Split(',');
            DataTable dt = new DataTable();
            List<SelectListItem> Status = new MemberBll().GetDict(11);
            List<SelectListItem> Types = new MemberBll().GetDict(17);
            for (int j = 0; j <= fields.Length - 1; j++)
            {
                dt.Columns.Add(fields[j]);
            }

            for (int i = 0; i < o.Count; i++)
            {

                DataRow dr = dt.NewRow();
                for (int j = 0; j <= fields.Length - 1; j++)
                {
                    switch (fields[j])
                    {
                        case "Teamname":
                            dr[fields[j]] = o[i].Teamname;
                            break;
                        case "Nickname":
                            dr[fields[j]] = o[i].Nickname;
                            break;
                        case "Company":
                            dr[fields[j]] = o[i].Company;
                            break;
                        case "Mobile":
                            dr[fields[j]] = o[i].Mobile;
                            break;
                        case "Couponchar":
                            dr[fields[j]] = o[i].Couponchar;
                            break;
                        case "Lines_name":
                            dr[fields[j]] = o[i].Lines_name;
                            break;
                        case "Status":
                            foreach (SelectListItem r in Status)
                            {
                                if (o[i].Status.ToString() == r.Value)
                                    dr[fields[j]] = r.Text.ToString();
                            }
                            break;
                        case "Type":
                            foreach (SelectListItem r in Types)
                            {
                                if (o[i].Type.ToString() == r.Value)
                                    dr[fields[j]] = r.Text.ToString();
                            }
                            break;
                        case "Usedtime":
                            dr[fields[j]] = o[i].Usedtime;
                            break;
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            //队伍名称、队员、单位、电话、邀请码、线路名称、使用时间、类型、状态
            dt.Columns["Teamname"].ColumnName = "队伍名称";
            dt.Columns["Nickname"].ColumnName = "队员";
            dt.Columns["Company"].ColumnName = "单位";
            dt.Columns["Mobile"].ColumnName = "电话";
            dt.Columns["Couponchar"].ColumnName = "邀请码";
            dt.Columns["Lines_name"].ColumnName = "线路名称";
            dt.Columns["Usedtime"].ColumnName = "使用时间";
            dt.Columns["Type"].ColumnName = "类型";
            dt.Columns["Status"].ColumnName = "状态";

            string file = "";
            try
            {
                file = GridToExcelByNPOI(dt, DateTime.Now.ToString("yyyyMMddHHmmss") + "_邀请码.xls");
            }
            catch
            {
                file = "";
            }

            return file;
        }


        public static string GridToExcelByNPOI(DataTable dt, string strExcelFileName)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                HeadercellStyle.SetFont(headerfont);


                //用column name 作为列名
                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                foreach (DataColumn item in dt.Columns)
                {
                    ICell cell = headerRow.CreateCell(icolIndex);
                    cell.SetCellValue(item.ColumnName);
                    cell.CellStyle = HeadercellStyle;
                    icolIndex++;
                }

                ICellStyle cellStyle = workbook.CreateCellStyle();

                //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                cellfont.Boldweight = (short)FontBoldWeight.Normal;
                cellStyle.SetFont(cellfont);

                //建立内容行
                int iRowIndex = 1;
                int iCellIndex = 0;
                foreach (DataRow Rowitem in dt.Rows)
                {
                    IRow DataRow = sheet.CreateRow(iRowIndex);
                    foreach (DataColumn Colitem in dt.Columns)
                    {

                        ICell cell = DataRow.CreateCell(iCellIndex);
                        cell.SetCellValue(Rowitem[Colitem].ToString());
                        cell.CellStyle = cellStyle;
                        iCellIndex++;
                    }
                    iCellIndex = 0;
                    iRowIndex++;
                }

                //自适应列宽度
                for (int i = 0; i < icolIndex; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                //写Excel
                string filepath = System.Web.HttpContext.Current.Server.MapPath("~") + "\\Data\\" + strExcelFileName;
                FileStream file = new FileStream(filepath, FileMode.OpenOrCreate);
                workbook.Write(file);
                file.Flush();
                file.Close();
                return strExcelFileName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public ActionResult CreateCoupon()
        {
            string strchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] char_ = strchar.Split(',');
            ViewBag.Identifying = "";
            for (int i = 0; i < char_.Length; i++)
            {
                ViewBag.Identifying += "<option value='" + char_[i].ToString() + "'>" + char_[i].ToString() + "</option>";
            }
            List<SelectListItem> Status = new MemberBll().GetDict(11);
            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            List<SelectListItem> Type = new MemberBll().GetDict(17);
            foreach (SelectListItem r in Type)
            {
                ViewBag.Type += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
            foreach (SelectListItem r in Matchs)
            {
                ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateCoupon(FormCollection fc)
        {
            string matchid = fc["optMatch"].ToString();
            string lineid = fc["optLine"].ToString();
            string linesid = fc["optLines"].ToString();
            string count = fc["Count"].ToString();
            string identifying = fc["optIdentifying"].ToString();
            string company = fc["Company"].ToString();
            string mobile = fc["Mobile"].ToString();
            int count_ = 0;
            if (count != "" && count != null)
            {
                count_ = Int32.Parse(count);
            }

            DateTime Createtime = DateTime.Now;
            string status = fc["optStatus"].ToString();
            string type = fc["optType"].ToString();

            if (count_ != 0)
            {
                List<tblcoupon> coupons = new List<tblcoupon>();
                for (int i = 0; i < count_; i++)
                {
                    tblcoupon coupon = new tblcoupon();
                    coupon.Couponid = Guid.NewGuid().ToString();
                    coupon.Couponchar = identifying + GetRadomCode(9);
                    coupon.Matchid = matchid;
                    coupon.Lineid = lineid;
                    coupon.Company = company;
                    coupon.Mobile = mobile;
                    coupon.Linesid = linesid;
                    coupon.Createtime = Createtime;
                    coupon.Status = status;
                    coupon.Type = type;
                    coupons.Add(coupon);
                }
                try
                {
                    var bll = new MatchBll();
                    bll.AddCoupon(coupons);
                }
                catch (ValidException ex)
                {
                    this.ModelState.AddModelError(ex.Name, ex.Message);
                    return View();
                }

            }
            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult DeleteCoupon(List<string> ids)
        {
            var bll = new MatchBll();

            try
            {
                bll.DeleteCoupon(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("Coupon");
        }

        public string GetRadomCode(int length)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random randrom = new Random((int)DateTime.Now.Ticks);
            string code = "";
            for (int j = 0; j < 10000; j++)
            {
                string str = "";
                for (int i = 0; i < 9; i++)
                {
                    str += chars[randrom.Next(chars.Length)];//randrom.Next(int i)返回一个小于所指定最大值的非负随机数
                }
                code = Md5(str, 32);//MD5加密
            }
            return code;
        }

        public static string Md5(string str, int code)
        {
            string strEncrypt = string.Empty;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.GetEncoding("GB2312").GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            for (int i = 0; i < targetData.Length; i++)
            {
                strEncrypt += targetData[i].ToString("X2");
            }
            if (code == 16)
            {
                strEncrypt = strEncrypt.Substring(8, 16);
            }
            return strEncrypt;
        }

    }
}
