using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using Model;
using Utls;
using BLL;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using log4net;

namespace Web.Areas.Main.Controllers
{
    public class PlayerController : BaseController
    {
        //
        // GET: /Main/Player/

        public ActionResult List(string matchname, string teamname, string playername, int? pageIndex)
        {
            try
            {
                var teams = new TeamBll().getMatchUsersList(matchname, teamname, playername, pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(5);
                ViewData["Status"] = Status;

                //foreach (SelectListItem r in Status)
                //{
                //    if (optStatus == r.Value)
                //        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                //    else
                //        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                //}
                return View(teams);
            }
            catch (Exception e)
            {
                return View(new List<tblteamsVew>());
            }
        }

        public ActionResult setplayer(string matchuid)
        {
            ViewBag.mid = matchuid;
            return View();
        }


        [HttpPost]
        public ActionResult EditPlayer(string Matchuserid, FormCollection fc)
        {
            var bll = new MatchBll();
            var model = bll.getMatchUserByID(Matchuserid);
            model.Nickname = fc["Nickname"].ToString();
            model.Mobile = fc["Mobile"].ToString();

            try
            {
                bll.EditPlayer(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }

        public ActionResult EditPlayer(string matchuid)
        {
            var matchuser = new MatchBll().getMatchUserByID(matchuid);
            return View(matchuser);
        }
        

        [HttpPost]
        public ActionResult setplayer(string mobile, string mid)
        {
            TeamBll bll = new TeamBll();
            int res = bll.Replayer(mobile, mid);

            if (res > 0)
                return this.RefreshParent();
            else if (res == -1)
                ViewBag.err = "更换队员已经参加了比赛";
            else if (res == -2)
                ViewBag.err = "你不是队长，没有权限更换";
            else if (res == -3)
                ViewBag.err = "替换的队员不存在";
            else if (res == -4)
                ViewBag.err = "输入的手机号没有注册或者信息不完善";
            else if (res == -9)
                ViewBag.err = "年龄不在16~60之间";

            ViewBag.mid = mid;
            return View();
        }

        [HttpPost]
        public string setteamer(string matchuid)
        {
            var script = "";
            TeamBll bll = new TeamBll();
            int res = bll.ReLeader(matchuid);

            if (res == -1)
                script = "{\"msg\":\"已经是队长，不能设置\",\"code\":-1}";
            else
                script = "{\"msg\":\"\",\"code\":0}";

            return script;
        }
    }
}
