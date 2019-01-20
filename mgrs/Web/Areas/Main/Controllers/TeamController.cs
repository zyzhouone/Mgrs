using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using Utls;
using BLL;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;

namespace Web.Areas.Main.Controllers
{
    public class TeamController : BaseController
    {
        //
        // GET: /Main/Team/

        public ActionResult Index(string matchname, string teamname,string company, string phone, string optStatus,string optType,string linename, string optiscoupon,int? pageIndex)
        {
            var teams = new List<tblteamsVew>();
            try
            {
                teams = new TeamBll().GetTeams(matchname, teamname,company, phone, optStatus, optType, linename, optiscoupon,pageIndex.GetValueOrDefault(1));
                List<SelectListItem> Status = new MemberBll().GetDict(5);
                ViewData["Status"] = Status;

                foreach (SelectListItem r in Status)
                {
                    if (optStatus == r.Value)
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optStatus += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                List<SelectListItem> TeamType = new MemberBll().GetDict(12);
                ViewData["TeamType"] = TeamType;

                foreach (SelectListItem r in TeamType)
                {
                    if (optType == r.Value)
                        ViewBag.optType += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optType += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                List<SelectListItem> Iscoupon = new MemberBll().GetDict(13);
                ViewData["Iscoupon"] = Iscoupon;
                foreach (SelectListItem r in Iscoupon)
                {
                    if (optiscoupon == r.Value)
                        ViewBag.optiscoupon += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    else
                        ViewBag.optiscoupon += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }
            }
            catch (Exception e)
            {

            }
            return View(teams);
        }

        [HttpPost]
        public string ExpToExcel(string matchname, string teamname, string status)
        {
            List<tblteamsVew> o = new TeamBll().ExportTeams(matchname, teamname, status);
            string ExportField = "Matchname,Teamname,Mobile,Company,Linename,Linesname,Status,Createtime";
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
                        case "Matchname":
                            dr[fields[j]] = o[i].matchname;
                            break;
                        case "Teamname":
                            dr[fields[j]] = o[i].Teamname;
                            break;
                        case "Mobile":
                            dr[fields[j]] = o[i].Moblie;
                            break;
                        case "Company":
                            dr[fields[j]] = o[i].Company;
                            break;
                        case "Linename":
                            dr[fields[j]] = o[i].Linename;
                            break;
                        case "Linesname":
                            dr[fields[j]] = o[i].Linesname;
                            break;
                        case "Status":
                            foreach (SelectListItem r in Status)
                            {
                                if (o[i].Status.ToString() == r.Value)
                                     dr[fields[j]] = r.Text.ToString();
                            }
                            break;
                        case "Createtime":
                            dr[fields[j]] = o[i].Createtime;
                            break;
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            dt.Columns["Matchname"].ColumnName = "赛事名称";
            dt.Columns["Teamname"].ColumnName = "队伍名称";
            dt.Columns["Mobile"].ColumnName = "联系电话";
            dt.Columns["Company"].ColumnName = "所属公司";
            dt.Columns["Linename"].ColumnName = "线路类型";
            dt.Columns["Linesname"].ColumnName = "线路名称";
            dt.Columns["Status"].ColumnName = "状态";
            dt.Columns["Createtime"].ColumnName = "创建时间";

            string file = "";
            try
            {
                file = GridToExcelByNPOI(dt, DateTime.Now.ToString("yyyyMMddHHmmss") + "_队伍统计.xls");
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


        //public ActionResult Create()
        //{
        //    var bll = new TeamBll();
        //    List<SelectListItem> line = bll.GetLine();
        //    List<SelectListItem> Status = new MemberBll().GetDict(5);

        //    foreach (SelectListItem r in line)
        //    {
        //        ViewBag.line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
        //    }

        //    foreach (SelectListItem r in Status)
        //    {
        //        ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
        //    }

        //    return View();
        //}

        public JsonResult CheckTeamName(string Teamname)
        {
            bool isValidate = true;
            var tblteam = new TeamBll().GetTeamByName(Teamname);
            if (tblteam != null)
            {
                isValidate = false;
            }
            return Json(isValidate, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditCheckTeamName(string teamid, string match_id, string Teamname)
        {
            bool isValidate = true;
            var teams = new TeamBll().GetOtherTeams(match_id,teamid);

            foreach (var m in teams)
            {
                if (m.Teamname == Teamname)
                {
                    isValidate = false;
                }
            }
            return Json(isValidate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            var model = new tblteams();
            model.teamid = Guid.NewGuid().ToString();
            model.Teamname = fc["Teamname"].ToString();
            model.Company = fc["Company"].ToString();
            model.Lineid = fc["optLine"].ToString();
            model.Createtime = DateTime.Now;
            model.Status = Int32.Parse(fc["optStatus"].ToString());
            var bll = new TeamBll();

            try
            {
                bll.AddTeam(model);
            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                return View(model);
            }

            return this.RefreshParent();
        }


        [HttpPost]
        public ActionResult GetLinesByLineid(string matchid,string lineid)
        {
            List<SelectListItem> lines = new List<SelectListItem>();
            if (lineid != "")
            {
               lines = new TeamBll().GetLinesByLineid(matchid, lineid);
            }
            else
            {
                SelectListItem sli = new SelectListItem();
                sli.Value = "";
                sli.Text = "未分配";
                lines.Add(sli);
            }

            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeamUpsts(string teamid)
        {
            var bll = new TeamBll();
            var model = bll.GetTeamById1(teamid);
            List<SelectListItem> Status = new MemberBll().GetDict(5);

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
        public ActionResult TeamUpsts(string teamid, FormCollection fc)
        {
            var bll = new TeamBll();
            try
            {
                int res = bll.UpdateTeamStatus(teamid, fc["optStatus"].ToString());
            }
            catch { }
            return this.RefreshParent();
        }

        public ActionResult Edit(string id)
        {
            var bll = new TeamBll();
            var model = bll.GetTeamById1(id);

            List<SelectListItem> line = bll.GetLine(model.match_id);
            List<SelectListItem> Status = new MemberBll().GetDict(5);

            ViewBag.line += "<option value=''selected>未分配</option>";
            ViewBag.lines += "<option value=''selected>未分配</option>";
            if (model.Lineid == null || model.Lineid == "")
            {
                foreach (SelectListItem r in line)
                {
                        ViewBag.line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

            }else
            {
                foreach (SelectListItem r in line)
                {
                    if (model.Lineid.ToString() == r.Value)
                    {
                        ViewBag.line += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                    }else
                    {
                        ViewBag.line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }

                //获取线路
                List<SelectListItem> lines = new TeamBll().GetLinesByLineid(model.match_id, model.Lineid);
                if (model.Linesid == null || model.Linesid == "")
                {
                    foreach (SelectListItem r in lines)
                    {
                        ViewBag.lines += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                    }
                }
                else
                {
                    foreach (SelectListItem r in lines)
                    {
                        if (model.Linesid.ToString() == r.Value)
                        {
                            ViewBag.lines += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                        }
                        else
                        {
                            ViewBag.lines += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                        }
                    }

                }

            }


            foreach (SelectListItem r in Status)
            {
                if (model.Status.ToString() == r.Value)
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'selected>" + r.Text.ToString() + "</option>";
                else
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
            }


            if (model.Teamtype == 0)
            {
                ViewBag.Teamtype += "<option value='0'selected>否</option>";
                ViewBag.Teamtype += "<option value='1'>是</option>";
            }
            else if ((model.Teamtype == 1))
            {
                ViewBag.Teamtype += "<option value='0'>否</option>";
                ViewBag.Teamtype += "<option value='1' selected>是</option>";

            }
            else
            {
                ViewBag.Teamtype += "<option value=''selected>不详</option>";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(string id, FormCollection fc)
        {
            var bll = new TeamBll();
            var model = bll.GetTeamById(id);
            model.Teamname = fc["Teamname"].ToString();
            model.Company = fc["Company"].ToString();
            model.Lineid = fc["optLine"].ToString();
            //model.Status = Int32.Parse(fc["optStatus"].ToString());
            model.Linesid = fc["optLines"].ToString();
            if (!string.IsNullOrEmpty(fc["optTeamtype"]))
                model.Teamtype = Int32.Parse(fc["optTeamtype"].ToString());
            else
                model.Teamtype = 0;
            model.Teamno = fc["Teamno"].ToString();

            try
            {
              int res=  bll.EditTeam(model);

              if (res == -1)
                  return Alert("队伍编号重复");

            }
            catch (ValidException ex)
            {
                this.ModelState.AddModelError(ex.Name, ex.Message);
                List<SelectListItem> company = bll.GetCompany();
                List<SelectListItem> line = bll.GetLine(model.match_id);
                List<SelectListItem> Status = new MemberBll().GetDict(5);
                List<SelectListItem> lines = new TeamBll().GetLinesByLineid(model.match_id, model.Lineid);
                ViewBag.line += "<option value=''selected>未分配</option>";
                ViewBag.lines += "<option value=''selected>未分配</option>";
                foreach (SelectListItem r in company)
                {
                    ViewBag.company += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in line)
                {
                    ViewBag.line += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in lines)
                {
                    ViewBag.lines += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                foreach (SelectListItem r in Status)
                {
                    ViewBag.Status += "<option value='" + r.Value.ToString() + "'>" + r.Text.ToString() + "</option>";
                }

                ViewBag.Teamtype += "<option value='0'selected>否</option>";
                ViewBag.Teamtype += "<option value='1'>是</option>";

                return View(model);
            }

            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult Delete(List<string> ids)
        {
            var bll = new TeamBll();

            try
            {
                //bll.DeleteTeam(ids);
                bll.cancelteam(ids);
            }
            catch (ValidException ex)
            {
                return Alert(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public ActionResult TeamUsers(string matchid,string teamid)
        {
            if (teamid!=null)
            teamid = teamid.Replace("?", "");
            var teamuser = new TeamBll().getTeamUsers(matchid, teamid);
            var team = new TeamBll().GetTeamById(teamid);
            if (team != null)
            {
                ViewBag.teamname = team.Teamname; 
            }

            if (team.Status != 0)
            {
                //读取附加信息
                var teamExtra = new TeamBll().getTeamExtra(team.teamid);
                if (teamExtra != null)
                {
                    foreach (var m in teamExtra)
                    {
                        m.info3 = "http://www.chengshidingxiang.com:9004" + m.info3;
                    }
                    ViewData["teamExtra"] = teamExtra;
                }

            }
            return View(teamuser);
        }


        public ActionResult SetPlayer(string matchid, string teamid)
        {
            if (teamid != null)
                teamid = teamid.Replace("?", "");
            var teamuser = new TeamBll().getTeamUsers(matchid, teamid);
            return View(teamuser);
        }
    }
}
