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
    /// <summary>
    /// zzy 2019-01-20
    /// </summary>
    public class SystemController : BaseController
    {
        //
        // GET: /Main/System/

        public ActionResult BannerManager(string bannerType, int? pageIndex)
        {
            List<SelectListItem> BannerType = new MemberBll().GetDict(18);
            foreach (SelectListItem r in BannerType)
            {
                if (bannerType == r.Value.ToString())
                {
                    ViewBag.Banner_Type += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                }
                else
                {
                    ViewBag.Banner_Type += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }

            ViewData["BannerType"] = BannerType;
            var bannerpics = new List<tblbannerview>();
            try
            {
                bannerpics = new PublishBll().GetBannerPics(bannerType, pageIndex.GetValueOrDefault(1));
            }
            catch (Exception e)
            {

            }
            return View(bannerpics);
        }

        [HttpPost]
        public ActionResult BannerDel(List<string> ids)
        {
            var bll = new PublishBll();

            try
            {
                bll.DeleteBannerPics(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("BannerManager");
        }

        public ActionResult CreateBinner()
        {
            //获取赛事列表
            List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
            foreach (SelectListItem r in Matchs)
            {
                ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            List<SelectListItem> Status = new MemberBll().GetDict(18);
            foreach (SelectListItem r in Status)
            {
                ViewBag.BannerType += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }
            tblbanner model = new tblbanner();
            model.StartDateTime = DateTime.Now;
            model.EndDateTime = DateTime.Now.AddDays(10);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateBinner(FormCollection fc)
        {
            var model = new tblbanner();
            model.Banner_Id = Guid.NewGuid().ToString();

            string filename = "";

            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files["PicName"];
            if (file != null)
            {
                filename = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                if ((filename.ToUpper() == "PNG" || filename.ToUpper() == "JPG") && file.ContentLength / 1024 < 2000)
                {
                    string path = Server.MapPath("~/UploadFiles/");
                    filename = "ban_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filename;
                    file.SaveAs(path + filename);
                    model.PicName = filename;
                }
            }
           
                model.Match_Id = fc["optMatch"].ToString();
                model.Banner_Type = fc["optBannerType"].ToString();
                model.StartDateTime = Convert.ToDateTime(fc["startDateTime"]);
                model.EndDateTime = Convert.ToDateTime(fc["EndDateTime"].ToString());
                model.Sort = Convert.ToInt32(fc["Sort"].ToString());
                model.Modifydatetime = DateTime.Now;
                var bll = new PublishBll();

                try
                {
                    if (model.PicName == null)
                    {
                        this.ModelState.AddModelError("PicName", "图片不能为空");
                        //获取赛事列表
                        List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                        foreach (SelectListItem r in Matchs)
                        {
                            if (r.Value == model.Match_Id)
                            {
                                ViewBag.Matchs += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                            }
                            else
                            {
                                ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                            }
                        }
                        List<SelectListItem> Status = new MemberBll().GetDict(18);
                        foreach (SelectListItem r in Status)
                        {
                            if (r.Value == model.Banner_Type)
                            {
                                ViewBag.BannerType += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                            }
                            else
                            {
                                ViewBag.BannerType += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                            }
                        }
                        return View(model);
                    }
                    else
                    {
                        bll.AddBannerPics(model);
                    }
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
