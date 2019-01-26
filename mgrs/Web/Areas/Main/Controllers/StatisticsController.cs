using BLL;
using Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Main.Controllers
{
    public class StatisticsController : BaseController
    {
        //
        // GET: /Main/Statistics/

        public ActionResult SignUp(string optMatch)
        {
            if (!string.IsNullOrEmpty(optMatch))
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (r.Value == optMatch)
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                    }
                    else
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }                   
                }
            }
            else
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }


            if (!string.IsNullOrEmpty(optMatch))
            {
                //获取各个线路已组队但未完成预报名的队伍数统计表
                var signup1 = new StatisticsBll().GetSignupInfo(optMatch, "6");
                int signup1_count = signup1.ToList().Sum(p => p.count);

                //各个线路已完成预报名的队伍数统计表
                var signup2 = new StatisticsBll().GetSignupInfo(optMatch, "1");
                int signup2_count = signup2.ToList().Sum(p => p.count);

                //各个线路已完成正式报名的队伍数统计表；
                var signup3 = new StatisticsBll().GetSignupInfo(optMatch, "0");
                int signup3_count = signup3.ToList().Sum(p => p.count);

                ViewData["signup1"] = signup1;
                ViewData["signup2"] = signup2;
                ViewData["signup3"] = signup3;

                ViewBag.signup1_count = signup1_count;
                ViewBag.signup2_count = signup2_count;
                ViewBag.signup3_count = signup3_count;
                ViewBag.totalcount = signup1_count + signup2_count + signup3_count;

                ViewBag.show = "1";
            }

            return View();
        }

        public ActionResult PlayerInfo(string optMatch,string phone)
        {
            playerInfo pi =  new playerInfo();

            if (!string.IsNullOrEmpty(optMatch))
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (r.Value == optMatch)
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                    }
                    else
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }
            }
            else
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }

            if (optMatch != null && phone != null)
            {
                tblmatchusers matchuser = new StatisticsBll().GetMatchUser(optMatch,phone);
                if (matchuser != null)
                {
                    pi.matchusers = matchuser;
                    //获取队伍信息
                    var team = new StatisticsBll().GetTeamView(matchuser.Teamid);
                    pi.teams = team;

                    //查询改队伍所有成员信息
                    var teammember = new StatisticsBll().GetTeamMembers(optMatch,matchuser.Teamid);
                    pi.teammember = teammember;
                }
            }
            else
            {
                pi = null;

            }


            return View(pi);

        }
        

        public ActionResult MatchInfo(string optMatch)
        {
            if (!string.IsNullOrEmpty(optMatch))
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (r.Value == optMatch)
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                    }
                    else
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }

                }
            }
            else
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }


            if (!string.IsNullOrEmpty(optMatch))
            {
                //显示各线路已检录但未开始比赛的队伍
                var matchinfo = new StatisticsBll().GetMatchInfo(optMatch);


                ViewData["info1"] = matchinfo.nocheckedteam;
                ViewData["info2"] = matchinfo.checkedteam;
                ViewData["info3"] = matchinfo.linesfinishteam;

                int info1_count = matchinfo.nocheckedteam.ToList().Sum(p => p.count);
                int info2_count = matchinfo.checkedteam.ToList().Sum(p => p.count);
                int info3_count = matchinfo.linesfinishteam.ToList().Sum(p => p.count);

                ViewBag.info1_count = info1_count;
                ViewBag.info2_count = info2_count;
                ViewBag.info3_count = info3_count;
                ViewBag.totalcount = info1_count + info2_count + info3_count;

                ViewBag.show = "1";
            }

            return View();
        }


        [HttpPost]
        public ActionResult GetLinesByMatch(string matchid)
        {
            List<SelectListItem> lines = new StatisticsBll().GetlinesList(matchid);
            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MatchRank(string optMatch, string optLines, int? pageIndex)
        {
            var matchrank = new List<MatchRankView>();

            if (!string.IsNullOrEmpty(optMatch))
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                ViewBag.Matchs += "<option value=''>--请选择一个赛事--</option>";
                foreach (SelectListItem r in Matchs)
                {
                    if (r.Value == optMatch)
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                    }
                    else
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }

                }
                if (!string.IsNullOrEmpty(optLines))
                {
                    List<SelectListItem> lines = new MatchBll().GetlinesList(optMatch);
                    foreach (SelectListItem r in lines)
                    {
                        if (r.Value == optMatch)
                        {
                            ViewBag.Lines += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                        }
                        else
                        {
                            ViewBag.Lines += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                        }

                    }

                }

            }
            else
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                ViewBag.Matchs += "<option value=''>--请选择一个赛事--</option>";
                foreach (SelectListItem r in Matchs)
                {
                    ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }

           


            if (!string.IsNullOrEmpty(optMatch) && !string.IsNullOrEmpty(optLines))
            {
                //显示各线路已检录但未开始比赛的队伍
                matchrank = new StatisticsBll().GetlinesRank(optLines, pageIndex.GetValueOrDefault(1));
            }
            else
            {
                matchrank = null;
            }


            return View(matchrank);
        }
        

        [HttpPost]
        public string ExportSignup(string matchid,string status)
        {
            List<ExportTeamModel> o = new StatisticsBll().GetTeamBylines(matchid,status);
            string ExportField = "linename,teamname,leader,phone,Status";
            string[] fields = ExportField.Split(',');
            DataTable dt = new DataTable();
            List<SelectListItem> Status = new MemberBll().GetDict(5);

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
                        case "linename":
                            dr[fields[j]] = o[i].linename;
                            break;
                        case "teamname":
                            dr[fields[j]] = o[i].teamname;
                            break;
                        case "leader":
                            dr[fields[j]] = o[i].leader;
                            break;
                        case "phone":
                            dr[fields[j]] = o[i].phone;
                            break;
                        case "Status":
                            foreach (SelectListItem r in Status)
                            {
                                if (o[i].status.ToString() == r.Value)
                                    dr[fields[j]] = r.Text.ToString();
                            }
                            break;
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            dt.Columns["linename"].ColumnName = "线路名称";
            dt.Columns["teamname"].ColumnName = "队伍名称";
            dt.Columns["leader"].ColumnName = "队长";
            dt.Columns["phone"].ColumnName = "电话";
            dt.Columns["Status"].ColumnName = "状态";

            string file = "";
            try
            {
                string name_ = "";
                if (status == "0")
                {
                    name_ = "完成正式报名队伍";
                }
                else if (status == "1")
                {
                    name_ = "完成预报名队伍";
                }
                else if (status == "6")
                {
                    name_ = "未完成预报名队伍";
                }
                file = GridToExcelByNPOI(dt, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + name_ + ".xls");
            }
            catch
            {
                file = "";
            }

            return file;
        }

        [HttpPost]
        public string ExportMatchInfo(string matchid, string status)
        {
            List<ExportTeamModel> o = new StatisticsBll().GetMatchInfoBylines(matchid, status);
            string ExportField = "linename,teamname,leader,phone";
            string[] fields = ExportField.Split(',');
            DataTable dt = new DataTable();
            List<SelectListItem> Status = new MemberBll().GetDict(5);

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
                        case "linename":
                            dr[fields[j]] = o[i].linename;
                            break;
                        case "teamname":
                            dr[fields[j]] = o[i].teamname;
                            break;
                        case "leader":
                            dr[fields[j]] = o[i].leader;
                            break;
                        case "phone":
                            dr[fields[j]] = o[i].phone;
                            break;
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            dt.Columns["linename"].ColumnName = "线路名称";
            dt.Columns["teamname"].ColumnName = "队伍名称";
            dt.Columns["leader"].ColumnName = "队长";
            dt.Columns["phone"].ColumnName = "电话";

            string file = "";
            try
            {
                string name_ = "";
                if (status == "1")
                {
                    name_ = "已检录队伍";
                }
                else if (status == "0")
                {
                    name_ = "已开始比赛队伍";
                }
                else if (status == "2")
                {
                    name_ = "已完成比赛队伍";
                }
                file = GridToExcelByNPOI(dt, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + name_ + ".xls");
            }
            catch
            {
                file = "";
            }

            return file;
        }

        [HttpPost]
        public string ExportMatchRank(string matchid, string linesid)
        {
            List<MatchRankView> o = new StatisticsBll().GetlinesRank(linesid);
            string ExportField = "linename,teamname,teamno,line_no,nickname,mobile,total,begintime,endtime";
            string[] fields = ExportField.Split(',');
            DataTable dt = new DataTable();
            string filename = "";
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
                        case "linename":
                            dr[fields[j]] = o[i].linename;
                            break;
                        case "teamname":
                            dr[fields[j]] = o[i].teamname;
                            break;
                        case "teamno":
                            dr[fields[j]] = o[i].teamno;
                            break;
                        case "line_no":
                            dr[fields[j]] = o[i].line_no;
                            break;
                        case "nickname":
                            dr[fields[j]] = o[i].nickname;
                            break;
                        case "mobile":
                            dr[fields[j]] = o[i].mobile;
                            break;
                        case "total":
                            dr[fields[j]] = o[i].total;
                            break;
                        case "begintime":
                            dr[fields[j]] = o[i].begintime;
                            break;
                        case "endtime":
                            dr[fields[j]] = o[i].endtime;
                            break;
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            dt.Columns["linename"].ColumnName = "线路名称";
            dt.Columns["teamname"].ColumnName = "队伍名称";
            dt.Columns["teamno"].ColumnName = "队伍编号";
            dt.Columns["line_no"].ColumnName = "线路编号";
            dt.Columns["nickname"].ColumnName = "姓名";
            dt.Columns["mobile"].ColumnName = "电话";
            dt.Columns["total"].ColumnName = "成绩";
            dt.Columns["begintime"].ColumnName = "开始时间";
            dt.Columns["endtime"].ColumnName = "结束时间";

            if (dt.Rows.Count > 0)
                filename = dt.Rows[0][0].ToString();

            string file = "";
            try
            {
                file = GridToExcelByNPOI(dt, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + filename + "_排名.xls");
            }
            catch
            {
                file = "";
            }

            return file;
        }

        private static string GridToExcelByNPOI(DataTable dt, string strExcelFileName)
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



        //其他统计 包括性别，年龄段，区域统计
        public ActionResult Other(string optMatch)
        {
            if (!string.IsNullOrEmpty(optMatch))
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    if (r.Value == optMatch)
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "' selected>" + r.Text.ToString() + "</option>";
                    }
                    else
                    {
                        ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }
            }
            else
            {
                //获取赛事列表
                List<SelectListItem> Matchs = new MatchBll().GetMatchsList();
                foreach (SelectListItem r in Matchs)
                {
                    ViewBag.Matchs += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }

            if (!string.IsNullOrEmpty(optMatch))
            {
                ViewBag.male = 0;
                ViewBag.female = 0;
                //读取赛事所有队员信息
                var userList = new StatisticsBll().getMatchUsersList(optMatch);
                if(userList!=null)
                {
                    //性别统计
                    var query = from s in userList.AsEnumerable()
                        group s by new { s.sexy } into g
                        select new
                        {
                             g.Key.sexy,
                            count = g.Count()
                        };

                    foreach (var m in query)
                    {
                        if (m.sexy == 1)
                            ViewBag.male = m.count;
                        else
                            ViewBag.female = m.count;
                    }
                    //年龄段统计
                    var q1 = from num in userList.AsEnumerable()
                               where num.age <= 18
                               select num;
                    var q2 = from num in userList.AsEnumerable()
                             where num.age <= 30 && num.age > 18
                             select num;
                    var q3 = from num in userList.AsEnumerable()
                             where num.age <= 48 && num.age > 30
                             select num;
                    var q4 = from num in userList.AsEnumerable()
                             where num.age > 48
                             select num;

                    ViewBag.q1 = q1.Count();
                    ViewBag.q2 = q2.Count();
                    ViewBag.q3 = q3.Count();
                    ViewBag.q4 = q4.Count();

                }
            }
           
            return View();
        }
    }
}
