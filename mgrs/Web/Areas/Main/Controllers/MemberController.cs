using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using Utls;
using BLL;
using System.Text;
using System.Security.Cryptography;
using Web.Areas.Main.Models;

namespace Web.Areas.Main.Controllers
{
    public class MemberController : BaseController
    {
        //
        // GET: /Main/Member/

        public ActionResult Index(string id, string tel, string nickName, string optStatus, int? pageIndex)
        {
            var members = new List<tblusersView>();
            try
            {
                List<SelectListItem> Status = new MemberBll().GetDict(6);

                foreach (SelectListItem r in Status)
                {
                    if (optStatus == r.Value)
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                members = new MemberBll().GetMembers(id, tel, nickName, optStatus, pageIndex.GetValueOrDefault(1));
                
                ViewData["Status"] = Status;

            }
            catch
            {

            }
            return View(members);
        }

    

        public ActionResult Create()
        {
            var bll = new MemberBll();
            List<SelectListItem> Sexy = bll.GetDict(1);
            List<SelectListItem> CardType = bll.GetDict(2);
            List<SelectListItem> Status = bll.GetDict(6);

            foreach (SelectListItem r in Sexy)
            {
                ViewBag.Sexy += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            foreach (SelectListItem r in CardType)
            {
                ViewBag.CardType += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            foreach (SelectListItem r in Status)
            {
                ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            var model = new tblusers();
            model.Name = fc["Name"].ToString();
            model.userid = Guid.NewGuid().ToString();
            model.Mobile = fc["Mobile"].ToString();
            model.Passwd = MD5Encrypt(fc["Passwd"].ToString());
            model.sexy = fc["optSexy"].ToString();
            model.cardtype = fc["optCardType"].ToString();
            model.cardno = fc["cardno"].ToString();
            model.mono = "-";
            if (fc["birthday"] != "")
            {
                model.birthday = DateTime.Parse(fc["birthday"].ToString());
            }

            model.Status = Int32.Parse(fc["optStatus"].ToString());
            var bll = new MemberBll();

            try
            {
                bll.AddMember(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        public JsonResult CheckTel(string Mobile)
        {
            bool isValidate = true;
            var tbluesr = new MemberBll().GetMemberByTel(Mobile);
            if (tbluesr != null)
            {
                isValidate = false;
            }
            return Json(isValidate, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Edit(string id)
        {
            var bll = new MemberBll();
            var model = bll.GetMember(id);

            List<SelectListItem> Sexy = bll.GetDict(1);
            List<SelectListItem> CardType = bll.GetDict(2);
            List<SelectListItem> Status = bll.GetDict(6);

            foreach (SelectListItem r in Sexy)
            {
                if (model.sexy == r.Value)
                    ViewBag.Sexy += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.Sexy += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            foreach (SelectListItem r in CardType)
            {
                if (model.cardtype == r.Value)
                    ViewBag.CardType += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.CardType += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }

            foreach (SelectListItem r in Status)
            {
                if (model.Status.ToString() == r.Value)
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(string id, FormCollection fc)
        {
            var bll = new MemberBll();
            var model = bll.GetMember(id);
            model.Name = fc["Name"].ToString();
            string pas = fc["Passwd"].ToString().Trim();
            if (pas != "******")
            {
                model.Passwd = MD5Encrypt(pas);
            }
            model.sexy = fc["optSexy"].ToString();
            model.cardtype = fc["optCardType"].ToString();
            model.cardno = fc["cardno"].ToString();
            if (fc["birthday"] != null)
            {
                model.birthday = DateTime.Parse(fc["birthday"].ToString());
            }
            model.Status = Int32.Parse(fc["optStatus"].ToString());

            try
            {
                bll.EditMember(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                List<SelectListItem> Sexy = bll.GetDict(1);
                List<SelectListItem> CardType = bll.GetDict(2);
                List<SelectListItem> Status = bll.GetDict(6);

                foreach (SelectListItem r in Sexy)
                {
                    if (model.sexy == r.Value)
                        ViewBag.Sexy += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Sexy += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in CardType)
                {
                    if (model.cardtype == r.Value)
                        ViewBag.CardType += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.CardType += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in Status)
                {
                    if (model.Status.ToString() == r.Value)
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
                return View(model);
            }

            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult Delete(List<string> ids)
        {
            var bll = new MemberBll();

            try
            {
                bll.DeleteMember(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("Index");
        }
        public String MD5Encrypt(string code)
        {

            byte[] result = Encoding.Default.GetBytes(code.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }

    }
}
