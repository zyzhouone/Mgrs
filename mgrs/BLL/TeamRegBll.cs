using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using DAL;
using Model;
using Utls;

namespace BLL
{
    public class TeamRegBll : BaseBll
    {
        /// <summary>
        /// 团队注册1步
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public int Step1(string mobile)
        {
            using (var db = new BFdbContext())
            {
                tblusers usr = db.tblusers.FirstOrDefault(p => p.Mobile == mobile && p.Status == 0);
                int res = 0;
                if (usr == null)
                {
                    usr = new tblusers();
                    usr.Mobile = mobile;
                    usr.mono = VerifyCode.Get6SzCode();
                    usr.Status = 6;

                    res = db.Insert<tblusers>(usr);
                }
                else
                {
                    usr.mono = VerifyCode.Get6SzCode();
                    res = db.Update<tblusers>(usr);
                }
                if (res > 0)
                    SMSHepler.SendRegSms(mobile, usr.mono);

                return res;
            }
        }

        /// <summary>
        /// 检查验证码是否正确
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="vercode"></param>
        /// <returns></returns>
        public string CheckSms(string mobile, string vercode, string matchid)
        {
            using (var db = new BFdbContext())
            {
                var usr = db.tblusers.FirstOrDefault(p => p.Mobile == mobile && p.mono == vercode && (p.Status == 6 || p.Status == 0));
                if (usr == null)
                    return "-1";
                else
                {
                    if (db.tblmatchusers.Any(p => p.Match_Id == matchid && p.Mobile == mobile && p.Leader == 1))
                        return "-2";

                    return usr.userid;
                }
            }
        }

        /// <summary>
        /// 检查队伍名称是否重复
        /// </summary>
        /// <param name="matchid"></param>
        /// <param name="tname"></param>
        /// <returns></returns>
        public int CheckTname(string matchid, string tname, string mobile)
        {
            if (mobile.Contains("000000000")||mobile.StartsWith("2"))
                return 0;

            using (var db = new BFdbContext())
            {
                tblusers tmu = db.tblusers.FirstOrDefault(p => p.Mobile == mobile);
                if (tmu != null)
                {
                    tblteams tma = db.tblteams.FirstOrDefault(p => p.match_id == matchid && p.Userid == tmu.userid);
                    if (tma != null && tma.Status == 0)
                        return -1;

                    tblmatchusers tus = db.tblmatchusers.FirstOrDefault(p => p.Match_Id == matchid && p.Userid == tmu.userid && "1,8".Contains(p.Status));
                    if (tus != null)
                        return -4;
                }

                tblteams tm = db.tblteams.FirstOrDefault(p => p.match_id == matchid && p.Teamname == tname);
                if (tm == null)
                    return 0;

                tblusers usr = db.tblusers.FirstOrDefault(p => p.userid == tm.Userid);
                if (usr.Mobile != mobile)
                    return -2;

                if (tm.Status == 0)
                    return -2;

                return 0;
            }
        }

        /// <summary>
        /// 注册队伍
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tid"></param>
        /// <param name="tname"></param>
        /// <param name="tcom"></param>
        /// <returns></returns>
        public string RegTname(string uid, string tid, string tname, string tcom, string pwd, ref string teamid)
        {
            using (var db = new BFdbContext())
            {
                if (db.tblteams.Any(p => p.match_id == tid && p.Teamname == tname))
                    return "-1";
                var tms = db.tblteams.FirstOrDefault(p => p.match_id == tid && p.Userid == uid);
                if (tms != null)
                {
                    teamid = tms.teamid;
                    return "-3";
                }

                //var usr = db.tblusers.FirstOrDefault(p => p.userid == uid);
                //if (usr == null || usr.Isupt == "0")
                //    return "-2";

                tblteams tm = new tblteams();
                tm.Company = string.IsNullOrEmpty(tcom) ? "个人" : tcom;
                tm.Createtime = DateTime.Now;
                tm.Eventid = 1;
                tm.Lineid = "";
                tm.match_id = tid;
                tm.Status = 6;
                tm.Teamname = tname;
                tm.Teamno = "0";
                tm.Userid = uid;
                tm.teamid = Guid.NewGuid().ToString();

                db.TInsert<tblteams>(tm);

                //tblmatchusers m = new tblmatchusers();
                //m.Cardno = usr.cardno;
                //m.Cardtype = usr.cardtype;
                //m.Createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //m.Leader = 1;
                //m.Match_Id = tid;
                //m.Matchuserid = Guid.NewGuid().ToString();
                //m.Mobile = usr.Mobile;
                ////m.Nickname = usr.Name;
                //m.Pay = 0;
                ////m.birthday = usr.birthday;

                //SetYearOld(m);

                //int sx = 1;
                //if (int.TryParse(usr.sexy, out sx))
                //    m.Sexy = sx;
                //else
                //    m.Sexy = 1;
                ////m.Status = "1";

                //m.Teamid = tm.teamid;
                //m.Teamname = tm.Teamname;
                ////m.Userid = usr.userid;
                //db.TInsert<tblmatchusers>(m);

                db.SaveChanges();

                return tm.teamid;
            }
        }

        public List<tblline> GetLines(string mid)
        {
            using (var db = new BFdbContext())
            {
                string sql = string.Format("select a.* from tbl_line a,tbl_teams b where a.match_id=b.match_id and b.teamid='{0}'", mid);
                return db.SqlQuery<tblline>(sql).ToList();
            }
        }

        public tblmatchentity GetMatchByTeamid(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.SqlQuery<tblmatchentity>("select a.*,b.teamname,b.userid,b.status as Paystatus from tbl_match a,tbl_teams b where a.match_id=b.match_id and b.teamid='" + tid + "'").FirstOrDefault();
            }
        }

        public List<tblmatchentity> GetMatchUsersByTeamid(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.SqlQuery<tblmatchentity>("SELECT a.*,c.status as mstatus,c.match_name,c.date3,c.date4,b.status as Teamstatus FROM tbl_match_users a,tbl_teams b,tbl_match c where a.match_id=c.match_id and a.teamid=b.teamid and a.teamid='" + tid + "'").ToList();
            }
        }

        //public int GetReplayerCnt(string tid)
        //{
        //    using (var db = new BFdbContext())
        //    {
        //        return db.tblreplace.Count(p => p.Teamid == tid);
        //    }
        //}

        //public int GetFOrders(string tid)
        //{
        //    using (var db = new BFdbContext())
        //    {
        //        return db.tblorders.Count(p => p.Match_Id == tid && p.Status == 2);
        //    }
        //}

        public tblteams GetTeamById(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.tblteams.FirstOrDefault(p => p.teamid == tid);
            }
        }

        public List<tblmatch> GetMatch(string status)
        {
            using (var db = new BFdbContext())
            {
                if (string.IsNullOrEmpty(status))
                    return db.tblmatch.ToList();
                else
                    //return db.tblmatch.Where(p => p.Status != "4" && p.Status != "5").ToList();
                    return db.tblmatch.Where(p => p.Status != "5").ToList();
            }
        }

        public tblmatch GetMatchById(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.tblmatch.FirstOrDefault(p => p.Match_id == tid);
            }
        }

        public tblteammatchusers GetTeamByum(string usrid, string matchid)
        {
            using (var db = new BFdbContext())
            {
                string sql = string.Format("select a.*,b.lineid from tbl_match_users a,tbl_teams b where a.teamid=b.teamid and (a.status='1' or a.leader=1) and a.userid='{0}' and a.match_id='{1}'", usrid, matchid);
                return db.SqlQuery<tblteammatchusers>(sql).FirstOrDefault();
            }
        }

        public tblline GetLineById(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.SqlQuery<tblline>("select a.* from tbl_line a,tbl_teams b where a.lineid=b.lineid and b.teamid='" + tid + "'").FirstOrDefault();
            }
        }


        public int CheckTeamMobile(string mid, string mobile)
        {
            using (var db = new BFdbContext())
            {
                return db.SqlQuery<autoid>(string.Format("select count(1) Id from tbl_teams a,tbl_users b where a.userid=b.userid and a.status = 0 and a.match_id='{0}' and b.mobile='{0}'", mid, mobile)).FirstOrDefault().Id;
            }
        }

        public List<tblline> GetLineByMId(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.tblline.Where(p => p.Match_id == tid).ToList();
            }
        }

        public List<tblmatchusers> GetMatchuserByTeamId(string tid)
        {
            using (var db = new BFdbContext())
            {
                return db.tblmatchusers.Where(p => p.Teamid == tid && p.Leader == 0).ToList();
            }
        }

        public PagedList<tblSignVew> GetSignList(string matchname,string linename,string teamname,string nickname,int pageindex)
        {
            using (var db = new BFdbContext())
            {
                string sql = @"SELECT b.match_name,f.linename,c.teamname,d.name,d.mobile,a.dtime
                                     FROM tbl_signedon a,tbl_match b,tbl_teams c,tbl_users d,tbl_lines f
                                    where a.teamid=c.teamid and c.match_id=b.match_id and a.userid=d.userid and c.linesid=f.lines_id";

                if (!string.IsNullOrEmpty(matchname))
                    sql += " and b.match_name like '%"+matchname.Trim()+"%'";
                if (!string.IsNullOrEmpty(linename))
                    sql += " and f.linename like '%" + linename.Trim() + "%'";
                if (!string.IsNullOrEmpty(teamname))
                    sql += " and c.teamname like '%" + teamname.Trim() + "%'";
                if (!string.IsNullOrEmpty(nickname))
                    sql += " and d.name like '%"+nickname.Trim()+"%'";

                return db.SqlQuery<tblSignVew, DateTime?>(sql, pageindex, p => p.dtime);
            }
        }

        public List<tblSignVew> GetSignList(string matchname, string linename, string teamname, string nickname)
        {
            using (var db = new BFdbContext())
            {
                string sql = @"SELECT b.match_name,f.linename,c.teamname,d.name,d.mobile,a.dtime
                                     FROM tbl_signedon a,tbl_match b,tbl_teams c,tbl_users d,tbl_lines f
                                    where a.teamid=c.teamid and c.match_id=b.match_id and a.userid=d.userid and c.linesid=f.lines_id";

                if (!string.IsNullOrEmpty(matchname))
                    sql += " and b.match_name like '%" + matchname.Trim() + "%'";
                if (!string.IsNullOrEmpty(linename))
                    sql += " and f.linename like '%" + linename.Trim() + "%'";
                if (!string.IsNullOrEmpty(teamname))
                    sql += " and c.teamname like '%" + teamname.Trim() + "%'";
                if (!string.IsNullOrEmpty(nickname))
                    sql += " and d.name like '%" + nickname.Trim() + "%'";

                return db.SqlQuery<tblSignVew>(sql).ToList();
            }
        }

        public tbllines GetLinesByNo(string mid,string linename)
        {
            using (var db = new BFdbContext())
            {
                return db.tbllines.FirstOrDefault(p => p.Matchid == mid && p.Linename== linename);
            }
        }

        //public tblmatchusers GetMatchuserById(string tid, string uid)
        //{
        //    using (var db = new BFdbContext())
        //    {
        //        return db.tblmatchusers.FirstOrDefault(p => p.Teamid == tid && p.Userid == uid);
        //    }
        //}

        public int DelMatchuser(string uid)
        {
            using (var db = new BFdbContext())
            {
                tblmatchusers tbl = new tblmatchusers();
                tbl.Matchuserid = uid;

                return db.Delete<tblmatchusers>(tbl);
            }
        }

        public int cancelteam(string teamid)
        {
            using (var db = new BFdbContext())
            {
                var team = db.tblteams.FirstOrDefault(p => p.teamid == teamid);
                if (team == null)
                    return -1;
                
                using (var tran = db.BeginTransaction())
                {
                    string sql1 = string.Format("insert into tbl_match_users_del select *,now() from tbl_match_users where teamid='{0}'", teamid);
                    string sql2 = string.Format("insert into tbl_teams_del select *,now() from tbl_teams where teamid='{0}'", teamid);
                    string sql3 = string.Format("delete from tbl_match_users where teamid='{0}'", teamid);
                    string sql4 = string.Format("delete from tbl_teams where teamid='{0}'", teamid);

                    db.ExecuteSqlCommand(sql1);
                    db.ExecuteSqlCommand(sql2);
                    db.ExecuteSqlCommand(sql3);
                    db.ExecuteSqlCommand(sql4);

                    tran.Commit();
                }

                return 0;
            }
        }

        public int UpdatelMatchuser(tblmatchusers user)
        {
            using (var db = new BFdbContext())
            {
                if (db.tblusers.Any(p => p.userid != user.Userid && p.cardno == user.Cardno))
                    return -2;

                var ur = db.tblmatchusers.FirstOrDefault(p => p.Userid == user.Userid&&p.Teamid==user.Teamid&&p.Leader==1);

                ur.Mobile = user.Mobile;
                ur.Cardno = user.Cardno;
                ur.Cardtype = user.Cardtype;
                ur.Nickname = user.Nickname;
                ur.Sexy = user.Sexy;
                ur.Leader = user.Leader;

                DateTime dt;
                if (DateTime.TryParse(user.Pnov, out dt))
                    ur.birthday = dt;
                else
                    ur.birthday = null;

                SetYearOld(ur);

                db.TUpdate<tblmatchusers>(ur);

                var bur = db.tblusers.FirstOrDefault(p => p.userid == user.Userid);
                //bur.Name = user.Nickname;
                bur.birthday = ur.birthday;
                bur.cardno = user.Cardno;
                bur.cardtype = user.Cardtype;
                bur.Mobile = user.Mobile;
                bur.sexy = user.Sexy.ToString();
                db.TUpdate<tblusers>(bur);

                return db.SaveChanges();
            }
        }

        public int AddMatchuser(string tid, string mobile)
        {
            using (var db = new BFdbContext())
            {
                var usr = db.tblusers.FirstOrDefault(p => p.Mobile == mobile && p.Status == 0);
                if (usr == null)
                    return -1;

                if (db.tblmatchusers.Any(p => p.Userid == usr.userid && p.Teamid == tid))
                    return -2;

                var team = db.tblteams.FirstOrDefault(p => p.teamid == tid);
                if (team.Userid == usr.userid)
                    return -3;

                var match = db.tblmatch.FirstOrDefault(p => p.Match_id == team.match_id);
                var dusr = db.tblusers.FirstOrDefault(p => p.userid == team.Userid);

                tblmatchusers m = new tblmatchusers();
                m.Cardno = usr.cardno;
                m.Cardtype = usr.cardtype;
                m.birthday = usr.birthday;

                SetYearOld(m);

                m.Createtime = DateTime.Now;//.ToString("yyyy-MM-dd HH:mm:ss");
                m.Leader = 0;
                m.Match_Id = team.match_id;
                m.Matchuserid = Guid.NewGuid().ToString();
                m.Mobile = usr.Mobile;
                //m.Nickname = usr.Name;
                m.Pay = 0;

                int sx = 1;
                if (int.TryParse(usr.sexy, out sx))
                    m.Sexy = sx;
                else
                    m.Sexy = 1;
                m.Status = "2";
                m.Createtime = DateTime.Now;//.ToString("yyyy-MM-dd HH:mm:ss");
                m.Teamid = team.teamid;
                m.Teamname = team.Teamname;
                m.Userid = usr.userid;
                db.TInsert<tblmatchusers>(m);

                tblinfomation info = new tblinfomation();
                info.Context = string.Format("用户[{0}]邀请你加入[{1}]队伍,参加[{2}],赶快去看看并接受邀请吧.", dusr.Mobile, team.Teamname, match.Match_name);
                info.createtime = DateTime.Now;
                info.Infoid = Guid.NewGuid().ToString();
                info.Mobile = m.Mobile;
                info.Status = "0";
                info.Type = "3";
                info.Userid = m.Userid;
                info.Field1 = m.Matchuserid;
                info.Field2 = "0";
                db.TInsert<tblinfomation>(info);

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 选择路线
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lid"></param>
        /// <returns></returns>
        public int SelectLine(string id, string lid)
        {
            using (var db = new BFdbContext())
            {
                var team = db.tblteams.FirstOrDefault(p => p.teamid == id);
                if (team == null)
                    return -1;

                team.Lineid = lid;//int.Parse(lid);
                return db.Update<tblteams>(team);
            }
        }

        private void SetYearOld(tblmatchusers item)
        {
            try
            {
                //if (item.Cardtype == "1")
                {
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
                }
            }
            catch(Exception ex)
            {
                item.Age = 0;
            }
        }

        private void SetYearOld_Card(tblmatchusers item)
        {
            if (item.Cardtype == "1")
            {
                if (string.IsNullOrEmpty(item.Cardno))
                    item.Age = 0;
                else if (item.Cardno.Length == 18)
                {
                    string dy = item.Cardno.Substring(6, 8);
                    string nw = DateTime.Now.ToString("yyyyMMdd");
                    string m = (int.Parse(nw) - int.Parse(dy)).ToString();

                    if (m.Length > 4)
                        item.Age = int.Parse(m.Substring(0, m.Length - 4));
                    else
                        item.Age = 0;
                }
                else if (item.Cardno.Length == 15)
                {
                    string dy = item.Cardno.Substring(6, 6);
                    if (dy.StartsWith("0") || dy.StartsWith("1") || dy.StartsWith("2"))
                        dy = "20" + dy;
                    else
                        dy = "19" + dy;

                    string nw = DateTime.Now.ToString("yyyyMMdd");
                    string m = (int.Parse(nw) - int.Parse(dy)).ToString();

                    if (m.Length > 4)
                        item.Age = int.Parse(m.Substring(0, m.Length - 4));
                    else
                        item.Age = 0;
                }
                else
                    item.Age = 0;
            }
        }

        public string InputMb(List<tblmatchusers> mus, string tid)
        {
            using (var db = new BFdbContext())
            {
                var team = db.tblteams.FirstOrDefault(p => p.teamid == tid);
                if (team == null)
                    return "-1";

                var tblmusers = db.tblmatchusers.Where(p => p.Teamid == team.teamid);

                if (!tblmusers.Any(p => p.Sexy == 2 && p.Teamid == team.teamid))
                    return "-2";

                if (tblmusers.Any(p => p.Age < 16 && p.Teamid == team.teamid))
                    return "-3";

                if (tblmusers.Any(p => p.Age > 60 && p.Teamid == team.teamid))
                    return "-4";

                if (tblmusers.Any(p => p.Nickname == null && p.Teamid == team.teamid))
                    return "-5";

                if (tblmusers.Any(p => p.Cardno == null && p.Teamid == team.teamid))
                    return "-6";

                if (tblmusers.Any(p => p.Age == null))
                    return "-7";

                //var user = db.tblusers.FirstOrDefault(p => p.userid == team.Userid);
                var match = db.tblmatch.FirstOrDefault(p => p.Match_id == team.match_id);
                var line = db.tblline.FirstOrDefault(p => p.Lineid == team.Lineid);

                team.Status = 1;
                db.TUpdate<tblteams>(team);

                ////添加订单
                //tblorders order = new tblorders();
                //order.Createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //order.Id = Guid.NewGuid().ToString();
                //order.Lineid = team.Lineid;
                //order.Match_Id = team.match_id;
                //order.Orderid = IDGenerator.GetIdF();
                //order.Ordertotal = line.PersonPrice.ToString();
                //order.Status = 0;
                //order.Teamid = team.teamid;
                //order.Title = string.Format("[{0}]报名费用", match.Match_name);
                //order.Userid = team.Userid;

                //db.TInsert<tblorders>(order);

                db.SaveChanges();
                return "";
            }
        }

        /// <summary>
        /// 团队导入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int ImpTeams(List<tblmatchentity> mus, ref int count)
        {
            using (var db = new BFdbContext())
            {
                try
                {
                    StringBuilder sbt = new StringBuilder();

                    string lno = mus[0].Match_Id;

                    var sns = mus.Distinct(new Comparint());
                    count = sns.Count();

                    foreach (var sn in sns)
                    {
                        var teams = mus.Where(p => p.Pnov == sn.Pnov);
                        string tmid = Guid.NewGuid().ToString();


                        sbt.AppendLine("tm:" + tmid);

                        if (!teams.Any(p => p.Leader == 1))
                        {
                            var t = teams.FirstOrDefault();
                            t.Leader = 1;
                        }

                        foreach (var team in teams)
                        {
                            tblmatchusers musr = new tblmatchusers();
                            musr.Leader = 0;
                            musr.Matchuserid = Guid.NewGuid().ToString();

                            sbt.AppendLine("muis:" + musr.Matchuserid);

                            musr.Age = team.Age;
                            musr.Cardno = team.Cardno;
                            musr.Cardtype = team.Cardtype;
                            musr.Createtime = DateTime.Now;//.ToString("yyyy-MM-dd HH:mm:ss");
                            musr.Match_Id = team.Match_Id;
                            musr.Mobile = team.Mobile;
                            musr.Nickname = team.Nickname;
                            musr.Pay = 0;
                            musr.Pnov = team.Pnov;
                            musr.Sexy = team.Sexy;
                            musr.Teamname = team.Teamname;
                            //musr.Teamno = team.Teamno;
                            if (team.LeaderM != "0")
                                musr.Teamno = int.Parse(team.LeaderM);

                            musr.Teamid = tmid;
                            musr.Status = "1";

                            tblusers usr = null;

                            if (!team.Mobile.Contains("0000000000"))
                                usr = db.tblusers.FirstOrDefault(p => p.Mobile == team.Mobile && p.Status == 0);

                            if (usr == null)
                            {
                                usr = new tblusers();
                                usr.cardno = team.Cardno;
                                usr.cardtype = team.Cardtype;
                                usr.Mobile = team.Mobile;
                                usr.Name = team.Nickname;
                                usr.Passwd = team.Password;

                                if (team.Mobile.Contains("0000000000"))
                                    usr.Passwd = "ABCDEFG";

                                usr.Playerid = 0;
                                usr.sexy = team.Sexy.ToString();
                                usr.Status = 0;
                                usr.Type = "2";

                                usr.userid = Guid.NewGuid().ToString();

                                sbt.AppendLine("uid:" + usr.userid);

                                if (team.Cardtype == "1")
                                {
                                    string y = team.Cardno.Substring(6, 8);
                                    DateTime date;
                                    if (DateTime.TryParseExact(y, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out date))
                                        usr.birthday = date;
                                }
                                else
                                    usr.birthday = DateTime.Now.AddYears(-1 * team.Age.Value);

                                db.TInsert<tblusers>(usr);
                            }
                            if (team.Leader == 1)
                            {
                                tblteams ctm = db.tblteams.FirstOrDefault(p => p.match_id == team.Match_Id && p.Userid == usr.userid);
                                if (ctm != null)
                                {
                                    string sql1 = string.Format("delete from tbl_match_users where teamid='{0}'", ctm.teamid);
                                    string sql2 = string.Format("delete from tbl_orders where teamid='{0}'", ctm.teamid);
                                    string sql3 = string.Format("delete from tbl_teams where teamid='{0}'", ctm.teamid);

                                    db.ExecuteSqlCommand(sql1);
                                    db.ExecuteSqlCommand(sql2);
                                    db.ExecuteSqlCommand(sql3);
                                }

                                var line = db.tbllines.FirstOrDefault(p => p.Linesid == team.Lineno);

                                tblteams tm = new tblteams();
                                tm.Company = team.Passwd;
                                tm.Createtime = DateTime.Now;
                                tm.Eventid = 1;
                                tm.Lineid = line.Lineid;
                                tm.Linesid = team.Lineno;

                                tm.match_id = team.Match_Id;
                                if (team.Area2 == "0")
                                {
                                    tm.Status = 1;
                                    tm.Teamtype = 1;
                                }
                                else
                                    tm.Status = 0;
                                tm.teamid = tmid;
                                tm.Teamname = team.Teamname;
                                tm.Teamno = team.Teamno.ToString().PadLeft(5, '0');
                                tm.Userid = usr.userid;
                                tm.Type = "1";

                                db.TInsert<tblteams>(tm);

                                tblorders ord = new tblorders();
                                ord.Createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                ord.Id = Guid.NewGuid().ToString();

                                sbt.AppendLine("oid:" + ord.Id);

                                ord.Lineid = line.Lineid;
                                ord.Match_Id = team.Match_Id;
                                //ord.Orderid = IDGenerator.GetIdG();
                                ord.Ordertotal = team.Pic2;

                                if (team.Area2 == "0")
                                {
                                    ord.Orderid = IDGenerator.GetIdG();
                                    ord.Status = 0;
                                }
                                else
                                {
                                    ord.Orderid = IDGenerator.GetIdW();
                                    ord.Status = 2;
                                }
                                ord.Teamid = tmid;
                                ord.Title = string.Format("[{0}]报名费用", team.Match_name);
                                ord.Userid = usr.userid;

                                db.TInsert<tblorders>(ord);

                                musr.Leader = 1;
                            }
                            musr.birthday = usr.birthday;
                            musr.Userid = usr.userid;
                            db.TInsert<tblmatchusers>(musr);
                        }

                    }
                    return db.SaveChanges();

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        public PagedList<tblteamsVew> GetImpTeams(string matchname, string teamname, string company, string datepicker1, string status, string optOrderBy,int pageindex)
        {
            using (var db = new BFdbContext())
            {
                PagedList<tblteamsVew> teams = null;

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT u.name,t.*,u.mobile as Moblie,l.name as Linename,m.match_name as matchname FROM tbl_teams t  left join tbl_line l on l.lineid = t.lineid left join tbl_users u on u.userid = t.userid  left join tbl_match m on m.match_id = t.match_id  where t.status!=2 and t.type_= '1' ");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND m.match_name like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND t.teamname like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(company))
                    sql.AppendFormat(" AND t.company like '%{0}%'", company);

                if (!string.IsNullOrEmpty(datepicker1))
                    sql.AppendFormat(" AND  DATE_FORMAT(t.createtime,'%Y-%m-%d')  = '{0}'", datepicker1);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND t.status  = '{0}'", status);

                if (!string.IsNullOrEmpty(optOrderBy))
                {
                    if (optOrderBy.Equals("1"))
                    {
                        teams = db.SqlQuery<tblteamsVew, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
                    }
                    else
                    {
                        teams = db.SqlQuery1<tblteamsVew, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
                    }

                }
                else
                {
                    teams = db.SqlQuery<tblteamsVew, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
                }

                return teams;
            }
        }
    }
}
