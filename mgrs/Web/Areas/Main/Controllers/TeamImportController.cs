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
    public class TeamImportController : BaseController
    {
        //
        // GET: /Main/TeamImport/
        public ActionResult Index(string matchname, string teamname, string company, string datepicker1, string optStatus, string optOrderBy, int? pageIndex)
        {
            var teams = new List<tblteamsVew>();
            try
            {
                teams = new TeamRegBll().GetImpTeams(matchname, teamname, company, datepicker1,optStatus,optOrderBy, pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(5);
                ViewData["Status"] = Status;

                foreach (SelectListItem r in Status)
                {
                    if (optStatus == r.Value)
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                if (!string.IsNullOrEmpty(datepicker1))
                {
                    ViewBag.datepicker1 = datepicker1;
                }

                if (!string.IsNullOrEmpty(optOrderBy))
                {
                    if (optOrderBy == "0")
                    {
                        ViewBag.optOrderBy += "<option value='0' selected>升序</option>";
                        ViewBag.optOrderBy += "<option value='1'>降序</option>";
                    }
                    else if (optOrderBy == "1")
                    {
                        ViewBag.optOrderBy += "<option value='0'>升序</option>";
                        ViewBag.optOrderBy += "<option value='1' selected>降序</option>";

                    }
                }
                else
                {
                    ViewBag.optOrderBy += "<option value='0'>升序</option>";
                    ViewBag.optOrderBy += "<option value='1'>降序</option>";
                }
            }
            catch (Exception e)
            {

            }
            return View(teams);
        }

        public ActionResult Edit(string id)
        {
            var bll = new TeamBll();
            var model = bll.GetTeamById(id);
            List<SelectListItem> line = bll.GetLine(model.match_id);
            List<SelectListItem> Status = new MemberBll().GetDict(5);

            foreach (SelectListItem r in line)
            {
                if (model.Lineid.ToString() == r.Value)
                    ViewBag.line += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
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
            var bll = new TeamBll();
            var model = bll.GetTeamById(id);
            model.Company = fc["Company"].ToString();
            model.Lineid = fc["optLine"].ToString();
            model.Status = Int32.Parse(fc["optStatus"].ToString());

            try
            {
                bll.EditTeam(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                List<SelectListItem> company = bll.GetCompany();
                List<SelectListItem> line = bll.GetLine(model.match_id);
                List<SelectListItem> Status = new MemberBll().GetDict(5);

                foreach (SelectListItem r in company)
                {
                    ViewBag.company += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in line)
                {
                    ViewBag.line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in Status)
                {
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                return View(model);
            }

            return this.RefreshParent();
        }


        public ActionResult import()
        {
            TeamRegBll bll = new TeamRegBll();
            var list = bll.GetMatch("1");

            StringBuilder sbt = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    sbt.AppendFormat("<option value='{0}' selected>{1}</option>", list[i].Match_id, list[i].Match_name);
                else
                    sbt.AppendFormat("<option value='{0}'>{1}</option>", list[i].Match_id, list[i].Match_name);
            }
            ViewBag.match = sbt.ToString();
            ViewBag.flag = "0";

            return View();
        }

        [HttpPost]
        public ActionResult Delete(List<string> ids)
        {
            //var bll = new TeamRegBll();

            //try
            //{
            //    foreach (string id in ids)
            //    {
            //        if (string.IsNullOrEmpty(id))
            //            continue;

            //        bll.cancelteam(id);
            //    }
            //}
            //catch (ValidException ex)
            //{
            //    return Alert(ex.Message);
            //}

            var bll = new TeamBll();

            try
            {
                bll.DeleteTeam(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public FileStreamResult DownFile()
        {
            string file = Server.MapPath("~/UploadFiles/xls/templates/group_import.xlsx");
            return File(new FileStream(file, FileMode.Open), "application/octet-stream", Server.UrlEncode("定向赛团体报名统计表.xlsx"));
        }

        [HttpPost]
        public ActionResult import(FormCollection fc, HttpPostedFileBase file)
        {
            if (file == null)
            {
                TeamRegBll bll = new TeamRegBll();
                var list = bll.GetMatch("1");

                StringBuilder sbt = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Match_id == fc["match"])
                        sbt.AppendFormat("<option value='{0}' selected>{1}</option>", list[i].Match_id, list[i].Match_name);
                    else
                        sbt.AppendFormat("<option value='{0}'>{1}</option>", list[i].Match_id, list[i].Match_name);
                }
                ViewBag.match = sbt.ToString();
                ViewBag.flag = "-1";

                return View();
            }
            else
            {
                string filename = string.Format("{0}{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(file.FileName));
                file.SaveAs(System.IO.Path.Combine(Server.MapPath("~/UploadFiles/xls/upload"), filename));
                return RedirectToAction("confirm", new { matchid = fc["match"], fid = filename });
            }
        }

        /// <summary>
        /// 导入确认
        /// </summary>
        /// <param name="matchid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>、
        public string seqno;
        public ActionResult confirm(string matchid, string fid)
        {
           
            try
            {
                ViewBag.error = "0";
                ViewBag.matchid = matchid;
                ViewBag.fid = fid;

                List<tblmatchentity> lstMatchusers = new List<tblmatchentity>();

                DataTable data = NpoiHelper.XlSToDataTable(System.IO.Path.Combine(Server.MapPath("~/UploadFiles/xls/upload"), fid), "TTBM", 0);

                if (data == null || data.Rows.Count < 1)
                    return View(lstMatchusers);


                Dictionary<string, string> tm = new Dictionary<string, string>();
                Dictionary<string, string> no = new Dictionary<string, string>();
                Dictionary<string, string> mb = new Dictionary<string, string>();

                TeamRegBll bll = new TeamRegBll();
                tblmatch match = bll.GetMatchById(matchid);

                ViewBag.matchname = match.Match_name;//HttpUtility.HtmlEncode(match.Match_name);

                //List<tblline> ln = bll.GetLineByMId(matchid);
                //string line_id = "";
                //if (ln != null && ln.Count > 0)
                //    line_id = ln[0].Lineid;


                StringBuilder sbtError = new StringBuilder();

                int sn = 0;
                string lineid = "";
                string teamno = "";
                string teamname = "";
                string company = "";
                int year = 0;

                foreach (DataRow row in data.Rows)
                {
                    sbtError.Clear();
                    year = 0;

                    if (string.IsNullOrEmpty(row["队员姓名"].ToString().Trim()))
                        continue;

                    //记录序号，以标记团队
                    if (!string.IsNullOrEmpty(row["序号"].ToString().Trim()))
                    {
                        sn = int.Parse(row["序号"].ToString().Trim());
                        company = "个人";
                        lineid = "";
                    }
                    if (!string.IsNullOrEmpty(row["线路名称"].ToString().Trim()))
                    {
                        var d = bll.GetLinesByNo(matchid, row["线路名称"].ToString().Trim());
                        if (d != null)
                            lineid = d.Linesid;
                        else
                            sbtError.AppendFormat("[线路名称:{0}]不存在;", row["线路名称"].ToString().Trim());
                    }

                    if (!string.IsNullOrEmpty(row["队列号"].ToString().Trim()))
                        teamno = row["队列号"].ToString().Trim();

                    
                    string mmb=row["手机号码"].ToString().Trim();
                    if (!mmb.Contains("000000000")&&!mmb.StartsWith("2"))
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(mmb, @"^[1]+[0-9]+\d{9}"))
                            sbtError.AppendFormat("[手机号码:{0}]格式错误;", row["手机号码"]);
                        else
                        {
                            if (mb.ContainsKey(mmb))
                                sbtError.Append("xls中存在重复手机号;");
                            else
                                mb.Add(mmb, mmb);
                        }
                    }

                    if (!string.IsNullOrEmpty(row["队名(6个字符以内)"].ToString().Trim()))
                    {
                        teamname = row["队名(6个字符以内)"].ToString().Trim();

                        if (tm.ContainsKey(teamname))
                            sbtError.Append("队名存在重复;");
                        else
                            tm.Add(teamname, teamname);
                    }

                    if (!string.IsNullOrEmpty(teamname))
                    {
                        int kv = bll.CheckTname(matchid, teamname, row["手机号码"].ToString().Trim());
                        switch (kv)
                        {
                            case 0:
                                break;
                            case -1:
                                sbtError.Append("手机号已经注册队伍;");
                                break;
                            case -2:
                                sbtError.Append("队名存在重复;");
                                break;
                            case -3:
                                sbtError.Append("队名存在重复;");
                                break;
                            case -4:
                                sbtError.Append("手机号已经参加了比赛;");
                                break;
                            default:
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(row["单位名称"].ToString().Trim()))
                        company = row["单位名称"].ToString().Trim();


                    if (string.IsNullOrEmpty(row["性别"].ToString().Trim()))
                        sbtError.Append("[性别]不能为空;");

                    if (string.IsNullOrEmpty(row["身份证/护照"].ToString().Trim()))
                        sbtError.Append("[身份证/护照]不能为空;");
                    else
                    {
                        if (row["是否护照"].ToString().Trim() != "是")
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(row["身份证/护照"].ToString().Trim(), @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$"))
                                sbtError.Append("[身份证/护照]格式错误;");
                        }

                        if (no.ContainsKey(row["身份证/护照"].ToString().Trim()))
                            sbtError.Append("[身份证/护照]存在重复;");
                        else
                            no.Add(row["身份证/护照"].ToString().Trim(), row["身份证/护照"].ToString().Trim());
                    }

                    if (!int.TryParse(row["年龄"].ToString().Trim(), out year))
                    {
                        sbtError.AppendFormat("[年龄:{0}]是否输入及正确;", row["年龄"]);
                    }

                    if (row["队员编号"].ToString().Trim() == "队长")
                    {
                        int cnt = bll.CheckTeamMobile(matchid, row["手机号码"].ToString().Trim());
                        if (cnt >= 1)
                        {
                            sbtError.Append("已经存在相同手机号的队伍;");
                        }
                    }


                    if (sbtError.Length > 0)
                        ViewBag.error = "-1";

                    tblmatchentity muser = new tblmatchentity();
                    muser.Pnov = sn.ToString();
                    muser.Teamname = teamname;
                    muser.Cardno = row["身份证/护照"].ToString().Trim();
                    muser.Cardtype = "1";
                    muser.Createtime = DateTime.Now;//.ToString("yyyy-MM-dd");
                    muser.Leader = row["队员编号"].ToString().Trim() == "队长" ? 1 : 0;
                    muser.Match_Id = "";
                    muser.Matchuserid = Guid.NewGuid().ToString();
                    muser.Mobile = row["手机号码"].ToString().Trim();
                    muser.Nickname = row["队员姓名"].ToString().Trim();
                    muser.Lineno = lineid;
                    muser.LeaderM = row["队员编号"].ToString().Trim() == "队长" ? "是" : "";                   
                    muser.Sexy = row["性别"].ToString().Trim() == "男" ? 1 : 0;
                    muser.Age = year;
                    muser.Mono = row["是否健康"].ToString().Trim();
                    muser.Content = HttpUtility.HtmlEncode(sbtError.ToString());
                    muser.Area2 = "0";
                    if (data.Columns.Contains("状态") && row["状态"].ToString() == "1")
                        muser.Area2 = "1";

                    lstMatchusers.Add(muser);
                    //edit by pang
                    seqno = row["手机号码"].ToString().Trim();
                }

                return View(lstMatchusers);
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(this.GetType());
                log.Error(ex);

                ViewBag.error = "-2";
                return View();
            }
        }

        /// <summary>
        /// 开始导入数据
        /// </summary>
        /// <param name="matchid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult confirm(string matchid, string fid, string fc)
        {
            try
            {
                List<tblmatchentity> lstMatchusers = new List<tblmatchentity>();

                DataTable data = NpoiHelper.XlSToDataTable(System.IO.Path.Combine(Server.MapPath("~/UploadFiles/xls/upload"), fid), "TTBM", 0);

                TeamRegBll bll = new TeamRegBll();
                tblmatch match = bll.GetMatchById(matchid);
                //List<tblline> ln = bll.GetLineByMId(matchid);
                //string line_id = "";
                //if (ln != null && ln.Count > 0)
                //    line_id = ln[0].Lineid;

                int sn = 0;
                string lineid = "";
                string teamno = "";
                string teamname = "";
                string company = "";
                int year = 0;
                int dm = 0;
                int playerno = 0;
                string price = "300";

                foreach (DataRow row in data.Rows)
                {
                    year = 0;

                    if (string.IsNullOrEmpty(row["队员姓名"].ToString().Trim()))
                        continue;

                    //记录序号，以标记团队
                    if (!string.IsNullOrEmpty(row["序号"].ToString().Trim()))
                    {
                        sn = int.Parse(row["序号"].ToString().Trim());
                        company = "个人";
                        lineid = "";
                        price = "300";
                    }
                    if (!string.IsNullOrEmpty(row["线路名称"].ToString().Trim()))
                    {
                        var d = bll.GetLinesByNo(matchid, row["线路名称"].ToString().Trim());
                        if (d != null)
                        {
                            lineid = d.Linesid;
                            price = d.Price;
                        }
                    }

                    if (!string.IsNullOrEmpty(row["队列号"].ToString().Trim()))
                    {
                        teamno = row["队列号"].ToString().Trim();
                        int.TryParse(teamno, out dm);
                    }
                    if (!string.IsNullOrEmpty(row["队名(6个字符以内)"].ToString().Trim()))
                        teamname = row["队名(6个字符以内)"].ToString().Trim();

                    if (!string.IsNullOrEmpty(row["单位名称"].ToString().Trim()))
                        company = row["单位名称"].ToString().Trim();

                    tblmatchentity muser = new tblmatchentity();
                    muser.Match_name = match.Match_name;
                    muser.Pnov = sn.ToString();
                    muser.Teamname = teamname;
                    muser.Teamno = dm;
                    muser.Cardno = row["身份证/护照"].ToString().Trim();
                    if (muser.Cardno.Length < 15)
                        muser.Cardtype = "2";
                    else
                        muser.Cardtype = "1";
                    muser.Leader = row["队员编号"].ToString().Trim() == "队长" ? 1 : 0;

                    playerno = 0;
                    if (int.TryParse(row["队员编号"].ToString().Trim(), out playerno))
                        muser.LeaderM = playerno.ToString();
                    else
                        muser.LeaderM = "0";

                    muser.Match_Id = matchid;
                    muser.Mobile = row["手机号码"].ToString().Trim();
                    muser.Nickname = row["队员姓名"].ToString().Trim();
                    muser.Lineno = lineid;
                    muser.Pic2 = price;
                    muser.Sexy = row["性别"].ToString().Trim() == "男" ? 1 : 2;
                    muser.Passwd = company;
                    int.TryParse(row["年龄"].ToString().Trim(), out year);
                    muser.Age = year;
                    muser.Mono = row["是否健康"].ToString().Trim();
                    muser.Password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(muser.Cardno.Substring(muser.Cardno.Length - 6), "MD5");
                    muser.Area2 = "0";
                    if (data.Columns.Contains("状态") && row["状态"].ToString() == "1")
                        muser.Area2 = "1";

                    lstMatchusers.Add(muser);
                  
                }

                int count = 0;
                TeamRegBll tbll = new TeamRegBll();
                int res = tbll.ImpTeams(lstMatchusers, ref count);
                return this.RefreshParent();
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(this.GetType());
                log.Error(ex);
                return new EmptyResult();
            }
        }

        public class Comparint : IEqualityComparer<tblmatchentity>
        {
            public bool Equals(tblmatchentity x, tblmatchentity y)
            {
                return x.Pnov == y.Pnov;
            }

            public int GetHashCode(tblmatchentity obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
    }
}