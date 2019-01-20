using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DAL;
using Model;
using Utls;
using System.Web.Mvc;

namespace BLL
{
    public class TeamBll : BaseBll
    {
        /// <summary>
        /// 查询队伍信息
        /// </summary>
        /// <param name="teamname"></param>
        /// <param name="company"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tblteamsVew> GetTeams(string matchname, string teamname, string company,string phone, string status, string opttype, string linename,string optiscoupon, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(@"select * from (SELECT case when isNULL(tc.team_combine_id)=1 then t.company else '【合并组队】' end as 'CompanyText',
                                u.name as nickname,
                                t.*,
                                u.mobile as Moblie,
                                l.name as Linename,
                                m.match_name as matchname,
                                ls.linename as linesname,
                                case when (select c.couponid from tbl_coupon c where c.teamid = t.teamid and c.matchid = t.match_id limit 1) is not null then 0 else 1  end iscoupon
                                FROM tbl_teams t  
                                left join tbl_line l on l.lineid = t.lineid
                                left join tbl_users u on u.userid = t.userid  
                                left join tbl_match m on m.match_id = t.match_id 
                                left join tbl_lines ls on ls.lines_id = t.linesid
                                left join tbl_teams_combine tc on tc.team_id=t.teamid
                                ) a
                                 where 1=1 ");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND a.matchname like '%{0}%'", matchname.Trim());

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND a.teamname like '%{0}%'", teamname.Trim());

                if (!string.IsNullOrEmpty(company))
                    sql.AppendFormat(" AND a.company like '%{0}%'", company.Trim());

                if (!string.IsNullOrEmpty(phone))
                    sql.AppendFormat(" AND a.Moblie like '%{0}%'", phone.Trim());

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND a.status  = '{0}'", status);

                if (!string.IsNullOrEmpty(opttype))
                    sql.AppendFormat(" AND a.type_  = '{0}'", opttype);

                if (!string.IsNullOrEmpty(optiscoupon))
                    sql.AppendFormat(" AND a.iscoupon  = '{0}'", optiscoupon);

                if (!string.IsNullOrEmpty(linename))
                    sql.AppendFormat(" AND a.linename  like '%{0}%'", linename.Trim());

                return db.SqlQuery<tblteamsVew, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
            }
        }

        /// <summary>
        /// 导出队伍信息
        /// </summary>
        /// <param name="teamname"></param>
        /// <param name="company"></param>
        /// <param name="status"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public List<tblteamsVew> ExportTeams(string matchname, string teamname, string status)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT t.*,u.mobile as Moblie,c.name as Companyname,l.name as Linename,ls.linename as linesname, m.match_name as Matchname FROM tbl_teams t left join tbl_company c on c.companyid = t.company left join tbl_line l on l.lineid = t.lineid left join tbl_users u on u.userid = t.userid  left join tbl_match m on m.match_id = t.match_id left join tbl_lines ls on ls.lines_id = t.linesid and ls.line_id = t.lineid where 1=1 ");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND m.match_name like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND t.teamname like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND t.status  = '{0}'", status);


                return db.SqlQuery<tblteamsVew>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 队伍队员信息
        /// </summary>
        /// <param name="teamname"></param>
        /// <param name="company"></param>
        /// <param name="status"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public List<tblmatchusers> getTeamUsers(string matchid, string teamid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM tbl_match_users  where status = '1' ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND match_id = '{0}'", matchid);

                if (!string.IsNullOrEmpty(teamid))
                    sql.AppendFormat(" AND teamid = '{0}'", teamid);

                return db.SqlQuery<tblmatchusers>(sql.ToString()).ToList();
            }
        }

        public PagedList<tblteamsVew> getMatchUsersList(string matchname, string teamname, string playername, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.matchuserid,a.nickname,a.mobile as Moblie,a.teamid,a.teamname,d.name as Linename,b.match_name as matchname,c.status,a.Createtime,a.leader FROM tbl_match_users a,tbl_match b,tbl_teams c,tbl_line d  where a.match_id=b.match_id and a.teamid=c.teamid and b.match_id=d.match_id  and c.lineid = d.lineid");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND b.match_name like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND a.teamname like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(playername))
                    sql.AppendFormat(" AND a.nickname  like '%{0}%'", playername);

                sql.AppendFormat(" AND a.status  <>  '{0}'", "9");

               return db.SqlQuery<tblteamsVew, string>(sql.ToString(), pageindex, p => p.teamid);
            }
        }

        /// <summary>
        /// 查询公司信息
        /// </summary>
        public List<SelectListItem> GetCompany()
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT  a.companyid  as value,a.name as text FROM tbl_company a order by a.name");

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 查询线路信息
        /// </summary>
        public List<SelectListItem> GetLine(string matchid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.lineid as value,a.name as text FROM tbl_line a  where a.match_id='" + matchid + "' order by a.name");

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 根据teamname查询队伍
        /// </summary>
        /// <param name="Teamname"></param>
        /// <returns></returns>
        public tblteams GetTeamByName(string Teamname)
        {
            using (var db = new BFdbContext())
            {
                return db.tblteams.FirstOrDefault(p => p.Teamname == Teamname);
            }
        }

        /// <summary>
        /// 根据teamid查询其他队伍
        /// </summary>
        /// <param name="Teamname"></param>
        /// <returns></returns>
        public List<tblteamsEntity> GetOtherTeams(string match_id,string teamid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM tbl_teams where 1=1 ");
                if (!string.IsNullOrEmpty(teamid))
                    sql.AppendFormat(" and teamid not in ('{0}')", teamid);

                if (!string.IsNullOrEmpty(match_id))
                    sql.AppendFormat(" and match_id = '{0}'", match_id);

                return db.SqlQuery<tblteamsEntity>(sql.ToString()).ToList();
            }
        }
        

        /// <summary>
        /// 根据id查询队伍
        /// </summary>
        /// <param name="Teamname"></param>
        /// <returns></returns>
        public tblteams GetTeamById(string teamid)
        {
            using (var db = new BFdbContext())
            {
                return db.tblteams.FirstOrDefault(p => p.teamid == teamid);
            }
        }

        public tblteamsEntity GetTeamById1(string teamid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM tbl_teams ");
                if (!string.IsNullOrEmpty(teamid))
                    sql.AppendFormat(" where teamid  = '{0}'", teamid);

                return db.SqlQuery<tblteamsEntity>(sql.ToString()).FirstOrDefault();
            }
        }

        /// <summary>
        /// 新增队伍
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddTeam(tblteams ent)
        {
            using (var db = new BFdbContext())
            {
                if (db.tblteams.Any(u => u.Teamname == ent.Teamname))
                    throw new ValidException("Mobile", "此队伍名称已经存在！");
                return db.Insert<tblteams>(ent);
            }
        }

        /// <summary>
        /// 更新队伍
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditTeam(tblteams ent)
        {
            using (var db = new BFdbContext())
            {
                int res = 0;

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        string tno = ent.Teamno.Replace("0", "");
                        if (!string.IsNullOrEmpty(tno))
                        {
                            if (db.tblteams.Any(p => p.Teamno == ent.Teamno && p.Linesid == ent.Linesid && p.teamid != ent.teamid))
                                return -1;
                        }
                        db.TUpdate<tblteams>(ent);
                        //同步更新matchuser表的teamname
                        string sql = string.Format("update tbl_match_users t set t.teamname = '{0}'  where t.teamid = '{1}'",ent.Teamname,ent.teamid);
                        db.ExecuteSqlCommand(sql);                            
                        db.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw ex;
                    }
                }

                return res;
            }
        }

        public int UpdateTeamStatus(string teamid, string status)
        {
            using (var db = new BFdbContext())
            {
                var tm = db.tblteams.FirstOrDefault(p => p.teamid == teamid);
                if (tm.Status.ToString() == status)
                    return 0;

                string sql = string.Format("insert into tbl_team_stsupd(id,teamid,dtime,org_status,des_status) values('{0}','{1}',now(),'{2}','{3}')",
                        Guid.NewGuid().ToString(), tm.teamid, tm.Status, status);

                var od = db.tblorders.FirstOrDefault(p => p.Teamid == teamid);
                if (status == "6")
                {
                    db.ExecuteSqlCommand(sql);

                    if (od != null)
                    {
                        db.TDelete<tblorders>(od);
                    }
                    tm.Status = 6;
                    db.TUpdate<tblteams>(tm);
                    return db.SaveChanges();
                }
                else if (status == "1")
                {
                    db.ExecuteSqlCommand(sql);

                    if (od != null)
                    {
                        od.Status = 0;
                        db.TUpdate<tblorders>(od);
                    }
                    else
                    {
                        tblorders odr = new tblorders();
                        odr.Createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        odr.Id = Guid.NewGuid().ToString();
                        odr.Lineid = tm.Linesid;
                        odr.Match_Id = tm.match_id;
                        odr.Orderid = Utls.IDGenerator.GetIdC();

                        var ln = db.tbllines.FirstOrDefault(p => p.Linesid == tm.Linesid);
                        odr.Ordertotal = ln.Price;
                        odr.Status = 0;
                        odr.Teamid = tm.teamid;

                        var mc = db.tblmatch.FirstOrDefault(p => p.Match_id == tm.match_id);
                        odr.Title = string.Format("[{0}]报名费用", mc.Match_name);
                        odr.Userid = tm.Userid;

                        db.TInsert<tblorders>(odr);
                    }
                    tm.Status = 1;
                    db.TUpdate<tblteams>(tm);
                    return db.SaveChanges();
                }
                else if (status == "0")
                {
                    db.ExecuteSqlCommand(sql);

                    if (od != null)
                    {
                        od.Status = 2;
                        db.TUpdate<tblorders>(od);
                    }
                    else
                    {
                        tblorders odr = new tblorders();
                        odr.Createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        odr.Id = Guid.NewGuid().ToString();
                        odr.Lineid = tm.Linesid;
                        odr.Match_Id = tm.match_id;
                        odr.Orderid = Utls.IDGenerator.GetIdC();

                        var ln = db.tbllines.FirstOrDefault(p => p.Linesid == tm.Linesid);
                        odr.Ordertotal = ln.Price;
                        odr.Status = 2;
                        odr.Teamid = tm.teamid;

                        var mc = db.tblmatch.FirstOrDefault(p => p.Match_id == tm.match_id);
                        odr.Title = string.Format("[{0}]报名费用", mc.Match_name);
                        odr.Userid = tm.Userid;
                        odr.Paytime = DateTime.Now;
                        db.TInsert<tblorders>(odr);
                    }
                    tm.Status = 0;
                    db.TUpdate<tblteams>(tm);
                    return db.SaveChanges();
                }
                return -1;
            }
        }


        /// <summary>
        /// 删除队伍
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeleteTeam(List<string> ids)
        {
            using (var db = new BFdbContext())
            {
                int res = 0;

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        foreach (string id in ids)
                        {
                            tblteams ent = db.tblteams.FirstOrDefault(p => p.teamid == id);
                            ent.Status = 2;
                            db.TUpdate<tblteams>(ent);
                        }
                        db.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw ex;
                    }
                }

                return res;
            }
        }


        public int cancelteam(List<string> ids)
        {
            using (var db = new BFdbContext())
            {
                using (var tran = db.BeginTransaction())
                {
                    foreach (string tid in ids)
                    {
                        string sql1 = string.Format("insert into tbl_match_users_del select *,now() from tbl_match_users where teamid='{0}'", tid);
                        string sql2 = string.Format("insert into tbl_teams_del select *,now() from tbl_teams where teamid='{0}'", tid);
                        string sql3 = string.Format("delete from tbl_match_users where teamid='{0}'", tid);
                        string sql4 = string.Format("delete from tbl_teams where teamid='{0}'", tid);

                        db.ExecuteSqlCommand(sql1);
                        db.ExecuteSqlCommand(sql2);
                        db.ExecuteSqlCommand(sql3);
                        db.ExecuteSqlCommand(sql4);
                    }

                    tran.Commit();
                }

                return 0;
            }
        }

        public int Replayer(string mobile, string mid)
        {
            using (var db = new BFdbContext())
            {
                var usr = db.tblusers.FirstOrDefault(p => p.Mobile == mobile && p.Status == 0);
                if (usr == null || usr.Isupt=="0")
                    return -4;

                var musr = db.tblmatchusers.FirstOrDefault(p => p.Matchuserid == mid);
                if (musr == null || (musr.Status != "1" && musr.Status != "8"))
                    return -3;

                if (db.tblmatchusers.Any(p => p.Match_Id == musr.Match_Id && p.Userid == usr.userid && p.Status == "1"))
                    return -1;

                tblmatchusers tm = new tblmatchusers();
                tm.birthday = usr.birthday;
                tm.Cardno = usr.cardno;
                tm.Cardtype = usr.cardtype;
                tm.Createtime = DateTime.Now;
                tm.Leader = 0;
                tm.Match_Id = musr.Match_Id;
                tm.Matchuserid = Guid.NewGuid().ToString();
                tm.Mobile = usr.Mobile;
                tm.Nickname = usr.Name;
                tm.Pay = 0;
                tm.Sexy = int.Parse(usr.sexy);
                tm.Status = "1";
                tm.Teamid = musr.Teamid;
                tm.Teamname = musr.Teamname;
                tm.Userid = usr.userid;
                SetYearOld(tm);

                if (tm.Age < 16 || tm.Age > 60)
                    return -9;

                db.TInsert<tblmatchusers>(tm);
                db.TDelete<tblmatchusers>(musr);

                tblreplace tbl = new tblreplace();
                tbl.Createtime = DateTime.Now;
                tbl.D_Age = musr.Age;
                tbl.D_Birthday = musr.birthday;
                tbl.D_Cardno = musr.Cardno;
                tbl.D_Cardtype = musr.Cardtype;
                tbl.D_Flag = "1";
                tbl.D_Agreetime = DateTime.Now;
                tbl.D_Matchuserid = musr.Matchuserid;
                tbl.D_Mobile = musr.Mobile;
                tbl.D_Nickname = musr.Nickname;
                tbl.D_Sexy = musr.Sexy;
                tbl.D_Userid = musr.Userid;
                tbl.Id = Guid.NewGuid().ToString();
                tbl.Match_Id = musr.Match_Id;
                tbl.S_Flag = "1";
                tbl.S_Userid = usr.userid;
                tbl.S_Matchuserid = tm.Matchuserid;
                tbl.S_Agreetime = DateTime.Now;
                tbl.Teamid = musr.Teamid;

                db.TInsert<tblreplace>(tbl);
                
                return db.SaveChanges();
            }
        }

        private void SetYearOld(tblmatchusers item)
        {
            try
            {
                //if (item.Cardtype == "1")
                //{
                if (item.birthday.HasValue)
                {
                    string dy = item.birthday.Value.ToString("yyyyMMdd");
                    string nw = DateTime.Now.ToString("yyyyMMdd");
                    string m = (int.Parse(nw) - int.Parse(dy) + 1).ToString();

                    if (m.Length > 4)
                        item.Age = int.Parse(m.Substring(0, m.Length - 4));
                    else
                        item.Age = 0;
                }
                else
                    item.Age = 0;
                //}
            }
            catch (Exception ex)
            {
                item.Age = 0;
            }
        }

        public int ReLeader(string mid)
        {
            using (var db = new BFdbContext())
            {
                var player = db.tblmatchusers.FirstOrDefault(p => p.Matchuserid == mid);

                var leader = db.tblmatchusers.FirstOrDefault(p => p.Leader == 1 && p.Teamid == player.Teamid);
                if (leader.Matchuserid == player.Matchuserid)
                    return -1;

                var team = db.tblteams.FirstOrDefault(p => p.teamid == leader.Teamid);

                leader.Leader = 0;
                leader.Mono = DateTime.Now.ToString("yyMMddHHmmss");

                player.Leader = 1;
                team.Userid = player.Userid;

                db.TUpdate<tblmatchusers>(leader);
                db.TUpdate<tblmatchusers>(player);
                db.TUpdate<tblteams>(team);

                return db.SaveChanges();
            }
        }


        public List<tblteamextra> getTeamExtra(string teamid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * from tbl_match_extra where 1=1 ");

                if (!string.IsNullOrEmpty(teamid))
                    sql.AppendFormat(" AND teamid = '{0}'", teamid);
                return db.SqlQuery<tblteamextra>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 查询线路类型下线路
        /// </summary>
        /// <param name="matchid"></param>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public List<SelectListItem> GetLinesByLineid(string matchid,string lineid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select t.lines_id as value,t.linename as text from tbl_lines t where 1=1 ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" and t.match_id=  '{0}'", matchid);
                if (!string.IsNullOrEmpty(lineid))
                    sql.AppendFormat(" and t.line_id=  '{0}'", lineid);

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

        
    }
}
