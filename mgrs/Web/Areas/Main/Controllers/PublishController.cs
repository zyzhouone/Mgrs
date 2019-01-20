using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Utls;
using BLL;

namespace Web.Areas.Main.Controllers
{
    public class PublishController : BaseController
    {
        //
        // GET: /Main/Publish/

        public ActionResult Sms(string tel, string type, int? pageIndex)
        {
            var sms = new List<tblinformation>();
            try
            {
                sms = new PublishBll().GetSms(tel, type, pageIndex.GetValueOrDefault(1));

                
                if (string.IsNullOrEmpty(type))
                {
                    ViewBag.smsType += "<option value='' selected>全部</option>";
                    ViewBag.smsType += "<option value='1'>短信</option>";
                    ViewBag.smsType += "<option value='2'>APP</option>";
                    ViewBag.smsType += "<option value='3'>其他</option>";
                }
                else if (type == "1")
                {
                    ViewBag.smsType += "<option value=''>全部</option>";
                    ViewBag.smsType += "<option value='1' selected>短信</option>";
                    ViewBag.smsType += "<option value='2'>APP</option>";
                    ViewBag.smsType += "<option value='3'>其他</option>";
                }
                else if (type == "2")
                {
                    ViewBag.smsType += "<option value=''>全部</option>";
                    ViewBag.smsType += "<option value='1'>短信</option>";
                    ViewBag.smsType += "<option value='2' selected>APP</option>";
                    ViewBag.smsType += "<option value='3'>其他</option>";

                }else if(type == "3"){
                    ViewBag.smsType += "<option value=''>全部</option>";
                    ViewBag.smsType += "<option value='1'>短信</option>";
                    ViewBag.smsType += "<option value='2'>APP</option>";
                    ViewBag.smsType += "<option value='3' selected>其他</option>";
                }

            }
            catch (Exception e)
            {

            }
            return View(sms);
        }

    }
}
