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
    public class StatisticsBll : BaseBll
    {
        /// <summary>
        /// 查询各线路队伍报名统计
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<signupView> GetSignupInfo(string matchid,string status)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT case when l.linename is null then '未分配' else  l.linename end as linesname, count(t.teamid) as count FROM tbl_teams t left join tbl_lines l on l.lines_id = t.linesid and l.match_id = t.match_id where 1=1 ");
                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND t.match_id=  '{0}'", matchid);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND t.status = '{0}'", status);
                
                sql.AppendFormat(" group by l.lines_id ");
                sql.AppendFormat(" ORDER BY l.linename ");

                return db.SqlQuery<signupView>(sql.ToString()).ToList();
            }
        }

         /// <summary>
        /// 查询各线路队伍数据
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<ExportTeamModel> GetTeamBylines(string matchid, string status)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select l.linename ,t.teamname,u.name as leader,u.mobile as phone,t.status from tbl_teams t left join tbl_lines l on l.lines_id = t.linesid left join tbl_users u on u.userid = t.userid where 1=1 ");
                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND t.match_id=  '{0}'", matchid);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND t.status = '{0}'", status);

                return db.SqlQuery<ExportTeamModel>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 查询赛事统计
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public MatchInfoStatistics GetMatchInfo(string matchid)
        {
            using (var db = new BFdbContext())
            {
                MatchInfoStatistics ms = new MatchInfoStatistics();

                StringBuilder sql = new StringBuilder(); //显示各线路已检录但未开始比赛的队伍
                sql.Append("select l.linename as linesname,count(t.teamid) as count from tbl_teams t,tbl_lines l,tbl_points p where t.linesid = l.lines_id and t.record = p.pointid and t.teamno not like '07%' and t.status=0  and p.pointtype='1'"); 
                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" and t.match_id = '{0}'", matchid);
                sql.Append(" group by l.lines_id");

                StringBuilder sql1 = new StringBuilder(); //显示各线路已开始比赛但未完成的队伍
                sql1.Append("select l.linename as linesname,count(t.teamid) as count from tbl_teams t,tbl_lines l,tbl_points p where t.linesid = l.lines_id and t.record = p.pointid and t.teamno not like '07%' and t.status=0  and p.pointtype='0'");
                if (!string.IsNullOrEmpty(matchid))
                    sql1.AppendFormat(" and t.match_id = '{0}'", matchid);
                sql1.Append(" group by l.lines_id");

                StringBuilder sql2 = new StringBuilder(); //显示各线路已完成比赛的队伍
                sql2.Append("select l.linename as linesname,count(t.teamid) as count from tbl_teams t,tbl_lines l,tbl_points p where t.linesid = l.lines_id and t.record = p.pointid and t.teamno not like '07%' and t.status=0  and p.pointtype='2'");
                if (!string.IsNullOrEmpty(matchid))
                    sql2.AppendFormat(" and t.match_id = '{0}'", matchid);
                sql2.Append(" group by l.lines_id");

                List<signupView> nocheckedteam = db.SqlQuery<signupView>(sql.ToString()).ToList();
                List<signupView> checkedteam = db.SqlQuery<signupView>(sql1.ToString()).ToList();
                List<signupView> linesfinishteam = db.SqlQuery<signupView>(sql2.ToString()).ToList();

                ms.nocheckedteam = nocheckedteam;
                ms.checkedteam = checkedteam;
                ms.linesfinishteam = linesfinishteam;

                return ms;
            }

        }

        /// <summary>
        /// 查询各线路队伍数据 比赛状态分类
        /// </summary>
        /// <param name="status">1：已检录 0：比赛中 2：已完赛</param>
        /// <returns></returns>
        public List<ExportTeamModel> GetMatchInfoBylines(string matchid, string status)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select l.linename,t.teamname,u.name as leader,u.mobile as phone from tbl_teams t,tbl_lines l,tbl_points p,tbl_users u where t.userid = u.userid and t.linesid = l.lines_id and t.record = p.pointid and t.teamno not like '07%' and t.status=0  and p.pointtype='" + status + "' ");
                
                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND t.match_id=  '{0}'", matchid);
                sql.Append(" order by l.lines_id");

                return db.SqlQuery<ExportTeamModel>(sql.ToString()).ToList();
            }
        }

         /// <summary>
        /// 查询赛事下线路
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public List<SelectListItem> GetlinesList(string matchid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select t.lines_id as value,t.linename as text from tbl_lines t ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" where t.match_id=  '{0}'", matchid);

                return db.SqlQuery<SelectListItem>(sql.ToString()).ToList();
            }
        }

         /// <summary>
        /// 查询线路排名
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public PagedList<MatchRankView> GetlinesRank(string linesid, int pageindex)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT aa.* from (SELECT b.teamname,b.teamno,c.line_no,c.linename,e.nickname,e.mobile, (UNIX_TIMESTAMP(max(a.createtime))-UNIX_TIMESTAMP(min(a.createtime)))/3600 as total,max(a.createtime) as endtime,min(a.createtime) as begintime FROM tbl_match_record a,tbl_teams b,tbl_lines c,tbl_points d,tbl_match_users e where a.teamid=b.teamid and b.linesid=c.lines_id and b.teamid=e.teamid and e.leader=1 and a.pointid=d.pointid and d.pointtype in (0,2) and b.eventid=3 and b.teamno not like '07%'  and d.pointname NOT like '%起点%'  ");

                if (!string.IsNullOrEmpty(linesid))
                    sql.AppendFormat(" and c.lines_id =  '{0}'", linesid);

                sql.Append(" GROUP BY b.teamname,b.teamno,c.line_no,c.linename,e.nickname,e.mobile) aa ORDER BY aa.total ");

                return db.SqlQuery1<MatchRankView, Decimal?>(sql.ToString(), pageindex, p => p.total);
            }
        }

        /// <summary>
        /// 查询线路排名
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public List<MatchRankView> GetlinesRank(string linesid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT aa.* from (SELECT b.teamname,b.teamno,c.line_no,c.linename,e.nickname,e.mobile, (UNIX_TIMESTAMP(max(a.createtime))-UNIX_TIMESTAMP(min(a.createtime)))/3600 as total,max(a.createtime) as endtime,min(a.createtime) as begintime FROM tbl_match_record a,tbl_teams b,tbl_lines c,tbl_points d,tbl_match_users e where a.teamid=b.teamid and b.linesid=c.lines_id and b.teamid=e.teamid and e.leader=1 and a.pointid=d.pointid and d.pointtype in (0,2) and b.eventid=3 and b.teamno not like '07%'  and d.pointname NOT like '%起点%'  ");

                if (!string.IsNullOrEmpty(linesid))
                    sql.AppendFormat(" and c.lines_id =  '{0}'", linesid);

                sql.Append(" GROUP BY b.teamname,b.teamno,c.line_no,c.linename,e.nickname,e.mobile) aa ORDER BY aa.total ");

                return db.SqlQuery<MatchRankView>(sql.ToString()).ToList();
            }
        }

        /// <summary>
        /// 查询赛事队员信息
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public tblmatchusers GetMatchUser(string matchid, string phone)
        {

            using (var db = new BFdbContext())
            {
                return db.tblmatchusers.FirstOrDefault(p => p.Match_Id == matchid && p.Mobile == phone);
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
        public tblteamsVew GetTeamView(string teamid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT  u.name as nickname,t.*,u.mobile as Moblie,c.name as Companyname,l.linename as Linename,m.match_name as Matchname,o.ordertotal as paytotal,o.paytime,o.status as paystatus FROM tbl_teams t left join tbl_company c on c.companyid = t.company left join tbl_lines l on l.lines_id = t.linesid left join tbl_users u on u.userid = t.userid  left join tbl_match m on m.match_id = t.match_id left join tbl_orders o on o.match_id = t.match_id and o.teamid = t.teamid where 1=1 ");

                if (!string.IsNullOrEmpty(teamid))
                    sql.AppendFormat(" AND t.teamid  = '{0}'", teamid);

                return db.SqlQuery<tblteamsVew>(sql.ToString()).FirstOrDefault();
            }
        }
        /// <summary>
        /// 查询赛队伍成员信息
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public List<tblmatchusers> GetTeamMembers(string matchid,string teamid)
        {
            using (var db = new BFdbContext())
            {
                return db.FindAll<tblmatchusers>(p => p.Teamid == teamid && p.Match_Id == matchid && p.Status == "1").ToList();
            }
        }

        /// <summary>
        /// 查询赛队伍成员信息
        /// </summary>
        /// <param name="matchid"></param>
        /// <returns></returns>
        public List<otherStatistic> getMatchUsersList(string matchid)
        {
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.sexy,a.age FROM tbl_match_users a,tbl_match b where a.match_id=b.match_id  AND a.status  <>  '9' and a.age > 0 ");

                if (!string.IsNullOrEmpty(matchid))
                    sql.AppendFormat(" AND b.match_id = '{0}'", matchid);

                return db.SqlQuery<otherStatistic>(sql.ToString()).ToList();
            }
        }
    }
}
