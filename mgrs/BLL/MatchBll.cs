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
    public class MatchBll : BaseBll
    {
        /// <summary>
        /// 查询赛事信息
        /// </summary>
        /// <param name="playerid"></param>
        /// <param name="tel"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tblmatch> GetMatchs(string matchname,string area2, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.* FROM tbl_match a WHERE 1=1");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND a.match_name  like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(area2))
                    sql.AppendFormat(" AND a.area2 like '%{0}%'", area2);


                return db.SqlQuery<tblmatch, DateTime?>(sql.ToString(), pageindex, p => p.Date3);
            }
        }

        /// <summary>
        /// 根据matchid查询赛事
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public tblmatch GetMatchById(string id)
        {
            using (var db = new BFdbContext())
            {
                return db.tblmatch.FirstOrDefault(p => p.Match_id == id);
            }
        }

        /// <summary>
        /// 根据matchid查询赛事其他图片
        /// </summary>
        /// <param name="Teamname"></param>
        /// <returns></returns>
        public List<tblmatchpics> GetOtherPics(string matchid)
        {
            using (var db = new BFdbContext())
            {
                 return db.FindAll<tblmatchpics>(p=>p.Match_id==matchid).ToList();
            }
        }

        /// <summary>
        /// 根据matchuserid查询赛事成员
        /// </summary>
        /// <param name="Teamname"></param>
        /// <returns></returns>
        public tblmatchusers getMatchUserByID(string matchuserid)
        {
            using (var db = new BFdbContext())
            {
                return db.tblmatchusers.FirstOrDefault(p => p.Matchuserid == matchuserid);
            }
        }
        
        /// <summary>
        /// 根据tel查询会员
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public tblusers GetMemberByTel(string tel)
        {
            using (var db = new BFdbContext())
            {
                return db.tblusers.FirstOrDefault(p => p.Mobile == tel);
            }
        }


        /// <summary>
        /// 新增赛事
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddMatch(tblmatch ent,string[] otherpics)
        {
            using (var db = new BFdbContext())
            {
                try
                {
                    db.TInsert<tblmatch>(ent);
                    if (otherpics != null && otherpics.Length > 0)
                    {
                        for (int i = 0; i < otherpics.Length; i++)
                        {
                            if (otherpics[i] != null)
                            {
                                var pic = new tblmatchpics();
                                pic.Id = Guid.NewGuid().ToString();
                                pic.Match_id = ent.Match_id;
                                pic.Picture = otherpics[i].ToString();
                                pic.Createtime = DateTime.Now;
                                db.TInsert<tblmatchpics>(pic);

                            }

                        }
                    }

                    db.SaveChanges();
                    return 0;

                }catch
                {
                    return 99;
                }
               
            }
        }

        /// <summary>
        /// 更新赛事
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditMatch(tblmatch ent,string[] otherpics)
        {
            using (var db = new BFdbContext())
            {
                try
                {
                    db.TUpdate<tblmatch>(ent);

                    if (otherpics != null && otherpics.Length > 0)
                    {
                        for (int i = 0; i < otherpics.Length; i++)
                        {
                            if (otherpics[i] != null)
                            {
                                var pic = new tblmatchpics();
                                pic.Id = Guid.NewGuid().ToString();
                                pic.Match_id = ent.Match_id;
                                pic.Picture = otherpics[i].ToString();
                                pic.Createtime = DateTime.Now;
                                db.TInsert<tblmatchpics>(pic);
                            }

                        }
                    }

                    db.SaveChanges();

                    return 0;
                }catch(Exception ex)
                {
                    return 99;
                }
            }
        }

        /// <summary>
        /// 更新赛事用户
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditPlayer(tblmatchusers ent)
        {
            using (var db = new BFdbContext())
            {
                try
                {
                    return db.Update<tblmatchusers>(ent);
                }
                catch
                {
                    return 99;
                }
            }
        }

        /// <summary>
        /// 删除赛事
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeleteMatch(List<string> ids)
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
                            tblmatch ent = db.tblmatch.FirstOrDefault(p => p.Match_id == id);
                            ent.Status = "2";
                            db.TUpdate<tblmatch>(ent);
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

        /// <summary>
        /// 赛事置顶
        /// zzy 2019-01-24
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public int TopMatch(string matchid)
        {
            using (var db = new BFdbContext())
            {
                string sql="update tbl_match set sort=DATE_FORMAT(date4,'%Y%m%d') where left(sort,4)='9999'";
                db.ExecuteSqlCommand(sql);
                sql=string.Format(@"update tbl_match set sort=CONCAT('9999',DATE_FORMAT(date4,'%m%d')) where match_id='{0}'",matchid);

           
                return db.ExecuteSqlCommand(sql);
            }
        }

        /// <summary>
        /// 查询线路类型信息
        /// </summary>
        /// <param name="matchname"></param>
        /// <param name="linename"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tbllineView> GetLine(string matchname,string name, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,m.match_name as Matchname FROM tbl_line a left join tbl_match m on m.match_id = a.match_id WHERE 1=1");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND m.match_name  like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(name))
                    sql.AppendFormat(" AND a.name  like '%{0}%'", name);

                return db.SqlQuery<tbllineView, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
            }
        }

        
        /// <summary>
        /// 获取赛事列表
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public List<SelectListItem> GetMatchsList()
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT  a.match_id as value,a.match_name as text FROM tbl_match a order by a.date4 desc limit 20 ");
                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }


        /// <summary>
        /// zzy 2019-03-24
        /// 组合下拉
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetValidMatchsList()
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT  a.match_id as value,a.match_name as text FROM tbl_match a  where `status`<>'5' ");
                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }


        /// <summary>
        /// 获取线路类型列表
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public List<SelectListItem> GetlineList(string matchid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.lineid as value,a.name as text FROM tbl_line a  where 1=1 ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND a.match_id = '{0}'", matchid);

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 获取线路列表
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public List<SelectListItem> GetlinesListByline(string lineid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.lines_id as value,a.linename as text FROM tbl_lines a  where 1=1 ");

                if (!string.IsNullOrEmpty(lineid))
                    sql.AppendFormat(" AND a.line_id = '{0}'", lineid);

                sql.Append(" order by a.line_no ");

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 获取线路列表
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public List<SelectListItem> GetlinesList(string matchid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.lines_id as value,a.linename as text FROM tbl_lines a  where 1=1 ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND a.match_id = '{0}'", matchid);

                sql.Append(" order by a.line_no ");

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }
        


        /// <summary>
        /// 新增线路类型
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddLine(tblline ent)
        {
            using (var db = new BFdbContext())
            {
                return db.Insert<tblline>(ent);
            }
        }

        /// <summary>
        /// 新增线路
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddLines(tbllines ent)
        {
            using (var db = new BFdbContext())
            {
                return db.Insert<tbllines>(ent);
            }
        }

        /// <summary>
        /// 根据lineid查询线路类型
        /// </summary>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public tblline GetLineById(string id)
        {
            using (var db = new BFdbContext())
            {
                return db.tblline.FirstOrDefault(p => p.Lineid == id);
            }
        }

        /// <summary>
        /// 根据lineid查询线路
        /// </summary>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public tbllines GetLinesById(string id)
        {
            using (var db = new BFdbContext())
            {
                return db.tbllines.FirstOrDefault(p => p.Linesid == id);
            }
        }

        /// <summary>
        /// zzy 2019-03-02
        /// 获取已支付数量
        /// </summary>
        /// <param name="linesid"></param>
        /// <returns></returns>
        public String GetPayCount(string linesid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select cast(count(*) as char(10)) as ispaycount from tbl_orders a,tbl_teams b where a.teamid=b.teamid and a.status =2 ");

                if (!string.IsNullOrEmpty(linesid))
                    sql.AppendFormat("and b.linesid='{0}'", linesid);

                return db.SqlQuery<String>(sql.ToString()).First().ToString();
                
                
            }
        

        }

        /// <summary>
        /// 更新线路类型
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditLine(tblline ent)
        {
            using (var db = new BFdbContext())
            {
                tblline line = db.tblline.FirstOrDefault(p => p.Lineid == ent.Lineid);
                line.Name = ent.Name;
                line.Match_id = ent.Match_id;
                line.Content = ent.Content;
                line.Players = ent.Players;
                line.teamprice = ent.teamprice;
                line.personprice = ent.personprice;
                line.Count = ent.Count;
                line.Conditions = ent.Conditions;
                line.Status = ent.Status;
                return db.Update<tblline>(line);
            }
        }

        /// <summary>
        /// 更新线路
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditLines(tbllines ent)
        {
            using (var db = new BFdbContext())
            {
                return db.Update<tbllines>(ent);
            }
        }

        /// <summary>
        /// 删除线路类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeleteLine(List<string> ids)
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
                            tblline ent = db.tblline.FirstOrDefault(p => p.Lineid == id);
                            db.TDelete(ent);
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

        /// <summary>
        /// 删除线路
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeleteLines(List<string> ids)
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
                            tbllines ent = db.tbllines.FirstOrDefault(p => p.Linesid == id);
                            db.TDelete<tbllines>(ent);
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

        /// <summary>
        /// 查询线路信息
        /// </summary>
        /// <param name="matchid"></param>
        /// <param name="lineid"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tbllinesView> GetLines(string matchid, string lineid, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,m.match_name as Matchname,l.name as Line_name FROM tbl_lines a left join tbl_match m on m.match_id = a.match_id left join tbl_line l on l.lineid = a.line_id  WHERE 1=1");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND m.match_id  = '{0}'", matchid);

                if (!string.IsNullOrEmpty(lineid))
                    sql.AppendFormat(" AND a.line_id = '{0}' ", lineid);

                return db.SqlQuery<tbllinesView, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
            }
        }

        /// <summary>
        /// 查询标识点
        /// </summary>
        /// <param name="pointid"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tblpointsView> GetPoints(string linesid, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,l.linename as Linename FROM tbl_points a left join tbl_lines l on l.lines_id = a.lineguid  WHERE 1=1 ");

                if (!string.IsNullOrEmpty(linesid))
                    sql.AppendFormat(" AND a.lineguid = '{0}'", linesid);


                return db.SqlQuery<tblpointsView, int?>(sql.ToString(), pageindex, p => p.Sort);
            }
        }

        /// <summary>
        /// 根据pointid查询标识
        /// </summary>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public tblpointsView GetPointById(string id)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,l.linename as Linename FROM tbl_points a left join tbl_lines l on l.lines_id = a.lineguid  WHERE 1=1 ");

                if (!string.IsNullOrEmpty(id))
                    sql.AppendFormat(" AND a.pointid = '{0}'", id);


                return db.SqlQuery<tblpointsView>(sql.ToString()).FirstOrDefault();
            }
        }

        /// <summary>
        /// 更新线路标识
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditPoints(tblpointsView ent)
        {
            using (var db = new BFdbContext())
            {
                tblpoints point = db.tblpoints.FirstOrDefault(p => p.Pointid == ent.Pointid);
                point.Pointname = ent.Pointname;
                point.Content = ent.Content;
                point.Sort = ent.Sort;
                point.Pointtype = ent.Pointtype;
                point.Status = ent.Status;
                return db.Update<tblpoints>(point);
            }
        }

        /// <summary>
        /// 新增标识
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddPoints(tblpoints ent)
        {
            using (var db = new BFdbContext())
            {
                return db.Insert<tblpoints>(ent);
            }
        }

        /// <summary>
        /// 删除标识
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeletePoint(List<string> ids)
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
                            tblpoints ent = db.tblpoints.FirstOrDefault(p => p.Pointid == id);
                            db.TDelete<tblpoints>(ent);
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

        /// <summary>
        /// 查询match后台账户
        /// </summary>
        /// <param name="pointid"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<sysmatchuserView> GetSysMatchUser(string matchid, string status, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT m.match_name,l.linename as Linesname,t.* FROM sys_match_user t left join tbl_lines l on l.lines_id = t.linesid  left join tbl_match m on m.match_id = t.match_id WHERE 1=1 ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND t.match_id = '{0}'", matchid);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND t.status = '{0}'", status);

                sql.Append(" order by l.line_no ");

                return db.SqlQuery<sysmatchuserView, string>(sql.ToString(), pageindex, p => p.Match_id);
            }
        }

        /// <summary>
        /// 新增赛事定线员
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddSysMatchUser(sysmatchuser ent)
        {
            using (var db = new BFdbContext())
            {
                try
                {
                    db.Insert<sysmatchuser>(ent);
                    return 0;

                }
                catch
                {
                    return 99;
                }

            }
        }

        /// <summary>
        /// 根据id查询定线员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public sysmatchuser GetSysMatchUserById(string id)
        {
            using (var db = new BFdbContext())
            {
                return db.sysmatchuser.FirstOrDefault(p => p.Id == id);
            }
        }

        /// <summary>
        /// 更新定线员
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int EditSysMatchUser(sysmatchuser ent)
        {
            using (var db = new BFdbContext())
            {
                return db.Update<sysmatchuser>(ent);
            }
        }

        /// <summary>
        /// 查询赛事图片信息
        /// </summary>
        /// <param name="matchname"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tblmatchpicsView> GetMatchPics(string matchname,int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,m.match_name,m.area1,m.area2 FROM tbl_match_pics a  left join tbl_match m on m.match_id = a.match_id where 1=1 ");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND m.match_name  like '%{0}%'", matchname);

                return db.SqlQuery<tblmatchpicsView, DateTime?>(sql.ToString(), pageindex, p => p.createtime);
            }
        }


        /// <summary>
        /// 新增赛事图片
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddMatchPics(tblmatchpics ent)
        {
            using (var db = new BFdbContext())
            {
                return db.Insert<tblmatchpics>(ent);
            }
        }

        /// <summary>
        /// 删除赛事图片
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeletePics(List<string> ids)
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
                            tblmatchpics ent = db.tblmatchpics.FirstOrDefault(p => p.Id == id);
                            db.TDelete<tblmatchpics>(ent);
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

        /// <summary>
        /// 查询邀请码列表
        /// </summary>
        /// <param name="teamname"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tblcouponView> GetCoupons(string matchname,string teamname,string company, string mobile,string couponchar, string optType,string optStatus, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,m.match_name,mu.name as nickname,tm.teamname,l.name as line_name,ls.linename as lines_name  FROM tbl_coupon a left join  tbl_match m on m.match_id = a.matchid left join tbl_users mu on mu.userid = a.userid  left join tbl_teams tm on tm.teamid = a.teamid left join tbl_line l on l.lineid = a.lineid left join tbl_lines ls on ls.lines_id = a.linesid WHERE 1=1");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND m.match_name  like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND tm.teamname  like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(company))
                    sql.AppendFormat(" AND a.company  like '%{0}%'", company);

                if (!string.IsNullOrEmpty(mobile))
                    sql.AppendFormat(" AND a.mobile  like '%{0}%'", mobile);

                if (!string.IsNullOrEmpty(couponchar))
                    sql.AppendFormat(" AND a.couponchar  like '%{0}%'", couponchar);

                if (!string.IsNullOrEmpty(optStatus))
                    sql.AppendFormat(" AND a.status = '{0}'", optStatus);

                if (!string.IsNullOrEmpty(optType))
                    sql.AppendFormat(" AND a.type = '{0}'", optType);

                return db.SqlQuery<tblcouponView, DateTime?>(sql.ToString(), pageindex, p => p.Createtime);
            }
        }

        /// <summary>
        /// 查询邀请码列表
        /// </summary>
        /// <param name="teamname"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public List<tblcouponView> GetCoupons(string matchname, string teamname, string company, string mobile, string couponchar, string optType, string optStatus)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,m.match_name,mu.name as nickname,tm.teamname,l.name as line_name,ls.linename as lines_name  FROM tbl_coupon a left join  tbl_match m on m.match_id = a.matchid left join tbl_users mu on mu.userid = a.userid  left join tbl_teams tm on tm.teamid = a.teamid left join tbl_line l on l.lineid = a.lineid left join tbl_lines ls on ls.lines_id = a.linesid WHERE 1=1");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND m.match_name  like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND tm.teamname  like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(company))
                    sql.AppendFormat(" AND a.company  like '%{0}%'", company);

                if (!string.IsNullOrEmpty(mobile))
                    sql.AppendFormat(" AND a.mobile  like '%{0}%'", mobile);

                if (!string.IsNullOrEmpty(couponchar))
                    sql.AppendFormat(" AND a.couponchar  like '%{0}%'", couponchar);

                if (!string.IsNullOrEmpty(optStatus))
                    sql.AppendFormat(" AND a.status = '{0}'", optStatus);

                if (!string.IsNullOrEmpty(optType))
                    sql.AppendFormat(" AND a.type = '{0}'", optType);

                return db.SqlQuery<tblcouponView>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 新增邀请码
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public int AddCoupon(List<tblcoupon> ent)
        {
            using (var db = new BFdbContext())
            {
                try
                {
                    if (ent != null && ent.Count > 0)
                    {
                        for (int i = 0; i < ent.Count; i++)
                        {
                            if (ent[i] != null)
                            {
                                db.TInsert<tblcoupon>(ent[i]);

                            }

                        }
                    }

                    db.SaveChanges();
                    return 0;

                }
                catch
                {
                    return 99;
                }

            }
        }

        /// <summary>
        /// 删除赛事
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeleteCoupon(List<string> ids)
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
                            tblcoupon ent = db.tblcoupon.FirstOrDefault(p => p.Couponid == id);
                            db.TDelete<tblcoupon>(ent);
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

        /// <summary>
        /// zzy 2019-03-26
        /// 获取线路
        /// </summary>
        /// <param name="lineid"></param>
        /// <returns></returns>
        public List<SelectListItem> GetlinesList2(string lineid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.lines_id as value,a.linename as text FROM tbl_lines a  where 1=1 and playercount=5 ");

                if (!string.IsNullOrEmpty(lineid))
                    sql.AppendFormat(" AND a.line_id = '{0}'", lineid);

                sql.Append(" order by a.line_no ");

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

    }
}
