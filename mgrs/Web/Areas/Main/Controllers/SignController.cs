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
    public class SignController : Controller
    {
        //
        // GET: /Main/Sign/

        public ActionResult signlist(string matchname, string linename, string nickname, string teamname, int? pageIndex)
        {
            var teams = new List<tblSignVew>();
            try
            {
                teams = new TeamRegBll().GetSignList(matchname,linename, teamname, nickname, pageIndex.GetValueOrDefault(1));               
            }
            catch (Exception e)
            {

            }
            return View(teams);
        }

        [HttpPost]
        public string ExpToExcel(string matchname, string linename, string nickname, string teamname)
        {
            List<tblSignVew> o = new TeamRegBll().GetSignList(matchname, linename, teamname, nickname);
            string ExportField = "match_name,linename,teamname,name,mobile,dtime";
            string[] fields = ExportField.Split(',');
            DataTable dt = new DataTable();
           
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
                        case "match_name":
                            dr[fields[j]] = o[i].match_name;
                            break;
                        case "linename":
                            dr[fields[j]] = o[i].Linename;
                            break;
                        case "teamname":
                            dr[fields[j]] = o[i].Teamname;
                            break;
                        case "name":
                            dr[fields[j]] = o[i].Name;
                            break;
                        case "mobile":
                            dr[fields[j]] = o[i].Mobile;
                            break;
                        case "dtime":
                            dr[fields[j]] = o[i].dtime;
                            break;
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }

            dt.Columns["match_name"].ColumnName = "比赛名称";
            dt.Columns["linename"].ColumnName = "路线名称";
            dt.Columns["teamname"].ColumnName = "队伍名称";
            dt.Columns["name"].ColumnName = "签到队员";
            dt.Columns["mobile"].ColumnName = "手机号";
            dt.Columns["dtime"].ColumnName = "签到时间";


            string file = "";
            try
            {
                file = GridToExcelByNPOI(dt, DateTime.Now.ToString("yyyyMMddHHmmss") + "_签到统计.xls");
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
    }
}
