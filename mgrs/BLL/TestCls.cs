using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DAL;
using Model;
using Utls;

namespace BLL
{
    public class TestCls
    {
        public int gongyifenzu()
        {
            using (var db = new BFdbContext())
            {
                // 公益名额队伍，已支付
                List<tblteams> gongyiTeam = db.FindAll<tblteams>(p => p.Status.Value == 0 && p.Linesid == "119eeb28-935e-479c-b157-1dd17e6427f6" && p.match_id == "6a61b95b-2d5d-4373-abaf-bf4e4c438800").ToList();
                
                // 需要分配到的线路
                string strSql = "SELECT lines_id,match_id,line_id,linename,line_no FROM dx.tbl_lines where match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and linename in ";
                strSql += "('都市白领','运动健身','全新时代','创客空间','活力徐汇','CBD TV', ";
                strSql += "'大城小爱','红色巅峰','圆梦普陀','时空穿越','环保家园','美轮美奂', ";
                strSql += "'爱满申城','灵动城市','东方网','饮食男女','璀璨徐汇','文明普陀')";
                List<linesfenzu> gongyilines = db.SqlQuery<linesfenzu>(strSql, new object[0]).ToList();

                if (gongyiTeam.Count != gongyilines.Count)
                    return 1;
                List<int> lstInt=new List<int>();
                Random rnd = new Random();
                int tmp;
                while(true)
                {
                    tmp = rnd.Next(0, gongyiTeam.Count);
                    if (!lstInt.Contains(tmp))
                    {
                        lstInt.Add(tmp);
                    }
                    if (lstInt.Count >= 18)
                        break;
                }
                int x=0;
                foreach(tblteams tm in gongyiTeam)
                {
                    tm.Teamno = gongyilines[lstInt[x]].line_no + "001";
                    tm.Lineid = gongyilines[lstInt[x]].line_id;
                    tm.Linesid = gongyilines[lstInt[x]].lines_id;
                    db.TUpdate<tblteams>(tm);
                    x++;
                }
                db.SaveChanges();
                return 0;
            }
        }

        public int suijifenzu()
        {
            using (var db = new BFdbContext())
            {
                // 随机线路队伍，已支付
                List<tblteams> suijiTeam = db.FindAll<tblteams>(p => p.Status.Value == 0 && p.Linesid == "207bfe24-bef3-4e1d-95dc-22228e0e04e8" && p.match_id == "6a61b95b-2d5d-4373-abaf-bf4e4c438800").ToList();

                // 要分配队伍的线路
                string strSql = "SELECT lines_id,match_id,line_id,linename,line_no,pointscount FROM dx.tbl_lines where match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and linename in ";
                strSql += "('都市白领')";
                List<linesfenzu> daifenpeilines = db.SqlQuery<linesfenzu>(strSql, new object[0]).ToList();

                if (daifenpeilines == null || daifenpeilines.Count == 0)
                    return 1;

                strSql = "SELECT teamid,teamno FROM dx.tbl_teams where linesid='" + daifenpeilines[0].lines_id + "' and match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and status=0 ";
                strSql += "and teamno like '" + daifenpeilines[0].line_no + "%' order by teamno desc ";
                // 要分配线路中已编组的队伍
                List<teamfenzu> yicunzaiTeam = db.SqlQuery<teamfenzu>(strSql, new object[0]).ToList();
                // 还需要分配的队伍数量
                int tcount = daifenpeilines[0].pointscount.Value - yicunzaiTeam.Count;

                // 队伍编号起始值
                int no = 1;
                if (yicunzaiTeam != null && yicunzaiTeam.Count > 0)
                {
                    no = int.Parse(yicunzaiTeam[0].teamno.Substring(2)) + 1;
                }

                int max = tcount;
                if (suijiTeam.Count < tcount)
                    max = suijiTeam.Count;
                // 随机取队伍索引号
                List<int> lstInt = new List<int>();
                Random rnd = new Random();
                int tmp;
                while (true)
                {
                    tmp = rnd.Next(0, suijiTeam.Count);
                    if (!lstInt.Contains(tmp))
                    {
                        lstInt.Add(tmp);
                    }
                    if (lstInt.Count >= max)
                        break;
                }

                for (int x = 0; x < max; x++)
                {
                    suijiTeam[lstInt[x]].Teamno = daifenpeilines[0].line_no + no.ToString().PadLeft(3, '0');
                    suijiTeam[lstInt[x]].Lineid = daifenpeilines[0].line_id;
                    suijiTeam[lstInt[x]].Linesid = daifenpeilines[0].lines_id;

                    db.TUpdate<tblteams>(suijiTeam[lstInt[x]]);
                    no++;
                }
                db.SaveChanges();

                return 0;
            }
        }

        public int benxianluteamfenzu()
        {
            using (var db = new BFdbContext())
            {
                // 要分配队伍的线路
                string strSql = "SELECT lines_id,match_id,line_id,linename,line_no,pointscount FROM dx.tbl_lines where match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and linename in ";
                strSql += "('爱SHOW')";
                List<linesfenzu> daifenpeilines = db.SqlQuery<linesfenzu>(strSql, new object[0]).ToList();

                if (daifenpeilines == null || daifenpeilines.Count == 0)
                    return 1;

                strSql = "SELECT teamid,teamno FROM dx.tbl_teams where linesid='" + daifenpeilines[0].lines_id + "' and match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and status=0 ";
                strSql += "and teamno like '" + daifenpeilines[0].line_no + "%' order by teamno desc ";
                // 要分配线路中已编组的队伍
                List<teamfenzu> yibianzuTeam = db.SqlQuery<teamfenzu>(strSql, new object[0]).ToList();
                // 还需要分配的队伍数量
                int tcount = daifenpeilines[0].pointscount.Value - yibianzuTeam.Count;

                // 队伍编号起始值
                int no = 1;
                if (yibianzuTeam != null && yibianzuTeam.Count > 0)
                {
                    no = int.Parse(yibianzuTeam[0].teamno.Substring(2)) + 1;
                }

                strSql = "SELECT teamid,teamno FROM dx.tbl_teams where linesid='" + daifenpeilines[0].lines_id + "' and match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and status=0 ";
                strSql += "and teamno not like '" + daifenpeilines[0].line_no + "%' order by teamno desc ";
                // 要分配线路中没有编组的队伍
                List<teamfenzu> weibianzuTeam = db.SqlQuery<teamfenzu>(strSql, new object[0]).ToList();

                if (weibianzuTeam == null || weibianzuTeam.Count == 0)
                    return 1;

                int max = tcount;
                if (weibianzuTeam.Count < tcount)
                    max = weibianzuTeam.Count;
                // 随机取队伍索引号
                List<int> lstInt = new List<int>();
                Random rnd = new Random();
                int tmp;
                while (true)
                {
                    tmp = rnd.Next(0, weibianzuTeam.Count);
                    if (!lstInt.Contains(tmp))
                    {
                        lstInt.Add(tmp);
                    }
                    if (lstInt.Count >= max)
                        break;
                }

                for (int x = 0; x < max; x++)
                {
                    string s = weibianzuTeam[lstInt[x]].teamid;
                    tblteams tm = db.tblteams.FirstOrDefault(p => p.teamid == s);
                    if(tm!=null)
                    {
                        tm.Teamno = daifenpeilines[0].line_no + no.ToString().PadLeft(3, '0');
                        tm.Lineid = daifenpeilines[0].line_id;
                        tm.Linesid = daifenpeilines[0].lines_id;
                        db.TUpdate<tblteams>(tm);
                        no++;
                    }
                }
                db.SaveChanges();

                return 0;
            }
        }

        public int zhidingxianluteamfenzu(string srcLine,string desLine)
        {
            using (var db = new BFdbContext())
            {
                // 要分配队伍的线路
                string strSql = "SELECT lines_id,match_id,line_id,linename,line_no,pointscount FROM dx.tbl_lines where match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and linename in ";
                strSql += "('" + desLine + "')";
                List<linesfenzu> daifenpeilines = db.SqlQuery<linesfenzu>(strSql, new object[0]).ToList();

                if (daifenpeilines == null || daifenpeilines.Count == 0)
                    return 1;

                // 取队伍的线路
                strSql = "SELECT lines_id,match_id,line_id,linename,line_no,pointscount FROM dx.tbl_lines where match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and linename in ";
                strSql += "('" + srcLine + "')";
                List<linesfenzu> quduiwulines = db.SqlQuery<linesfenzu>(strSql, new object[0]).ToList();

                if (quduiwulines == null || quduiwulines.Count == 0)
                    return 1;

                strSql = "SELECT teamid,teamno FROM dx.tbl_teams where linesid='" + daifenpeilines[0].lines_id + "' and match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and status=0 ";
                strSql += "and teamno like '" + daifenpeilines[0].line_no + "%' order by teamno desc ";
                // 要分配线路中已编组的队伍
                List<teamfenzu> yibianzuTeam = db.SqlQuery<teamfenzu>(strSql, new object[0]).ToList();
                // 还需要分配的队伍数量
                int tcount = daifenpeilines[0].pointscount.Value - yibianzuTeam.Count;

                // 队伍编号起始值
                int no = 1;
                if (yibianzuTeam != null && yibianzuTeam.Count > 0)
                {
                    no = int.Parse(yibianzuTeam[0].teamno.Substring(2)) + 1;
                }

                strSql = "SELECT teamid,teamno FROM dx.tbl_teams where linesid='" + quduiwulines[0].lines_id + "' and match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and status=0 ";
                strSql += "and teamno not like '" + quduiwulines[0].line_no + "%' order by teamno desc ";
                // 取队伍线路中没有编组的队伍
                List<teamfenzu> weibianzuTeam = db.SqlQuery<teamfenzu>(strSql, new object[0]).ToList();

                if (weibianzuTeam == null || weibianzuTeam.Count == 0)
                    return 1;

                int max = tcount;
                if (weibianzuTeam.Count < tcount)
                    max = weibianzuTeam.Count;
                // 随机取队伍索引号
                List<int> lstInt = new List<int>();
                Random rnd = new Random();
                int tmp;
                while (true)
                {
                    tmp = rnd.Next(0, weibianzuTeam.Count);
                    if (!lstInt.Contains(tmp))
                    {
                        lstInt.Add(tmp);
                    }
                    if (lstInt.Count >= max)
                        break;
                }

                for (int x = 0; x < max; x++)
                {
                    string s = weibianzuTeam[lstInt[x]].teamid;
                    tblteams tm = db.tblteams.FirstOrDefault(p => p.teamid == s);
                    if (tm != null)
                    {
                        tm.Teamno = daifenpeilines[0].line_no + no.ToString().PadLeft(3, '0');
                        tm.Lineid = daifenpeilines[0].line_id;
                        tm.Linesid = daifenpeilines[0].lines_id;
                        db.TUpdate<tblteams>(tm);
                        no++;
                    }
                }
                db.SaveChanges();

                return 0;
            }
        }

        public int chongzhiteamfenzu(string srcLine)
        {
            using (var db = new BFdbContext())
            {
                // 要分配队伍的线路
                string strSql = "SELECT lines_id,match_id,line_id,linename,line_no,pointscount FROM dx.tbl_lines where match_id='6a61b95b-2d5d-4373-abaf-bf4e4c438800' and linename in ";
                strSql += "('" + srcLine + "')";
                List<linesfenzu> daifenpeilines = db.SqlQuery<linesfenzu>(strSql, new object[0]).ToList();

                if (daifenpeilines == null || daifenpeilines.Count == 0)
                    return 1;

                strSql = "update tbl_teams set teamno='0' where linesid='" + daifenpeilines[0].lines_id + "'";

                int ret = db.ExecuteSqlCommand(strSql, new object[0]);

                return 0;
            }
        }
    }
}
