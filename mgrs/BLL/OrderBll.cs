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
    public class OrderBll : BaseBll
    {
        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="matchname"></param>
        /// <param name="teamname"></param>
        /// <param name="status"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public PagedList<tblordersView> GetOrders(string matchname,string teamid,string teamname, string moblie, string status, string teamtype, string orderbypaytime, int pageindex)
        {
            PagedList<tblordersView> orders = null;
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(@"select  o.orderid,match_name as Matchname,t.teamname,mobile,t.teamtype,ordertotal,orderstatus,l.name as linename,ls.linename as linesname
                                ,IFNULL(o.paytime,p.paytime) as paytime,p.paytype
                                from (select r.*,r.status as orderstatus,m.match_name,u.mobile 
                                from tbl_orders r,tbl_users u,tbl_match m 
                                where  u.userid = r.userid and m.match_id = r.match_id) o
                                left join tbl_pay p on p.orderid=o.orderid
                                inner join tbl_teams t on t.teamid = o.teamid
                                left join tbl_line l on l.lineid = t.lineid
                                left join tbl_lines ls on ls.lines_id = t.linesid 
                                 where 1=1 ");

                if (!string.IsNullOrEmpty(matchname))
                    sql.AppendFormat(" AND match_name like '%{0}%'", matchname);

                if (!string.IsNullOrEmpty(teamid))
                    sql.AppendFormat(" AND t.teamid = '{0}'", teamid);

                if (!string.IsNullOrEmpty(moblie))
                    sql.AppendFormat(" AND o.mobile like '%{0}%'", moblie);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND teamname like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND orderstatus  = '{0}'", status);

                if (!string.IsNullOrEmpty(teamtype))
                    sql.AppendFormat(" AND teamtype  = '{0}'", teamtype);

                if (!string.IsNullOrEmpty(orderbypaytime))
                {
                    if (orderbypaytime.Equals("1"))
                    {
                        orders = db.SqlQuery<tblordersView, DateTime?>(sql.ToString(), pageindex, p => p.Paytime);
                    }
                    else
                    {
                        orders = db.SqlQuery1<tblordersView, DateTime?>(sql.ToString(), pageindex, p => p.Paytime);
                    }

                }
                else
                {
                    orders = db.SqlQuery<tblordersView, string>(sql.ToString(), pageindex, p => p.Id);
                }
                return orders;
            }
        }

        /// <summary>
        /// 导出订单信息
        /// </summary>
        /// <param name="teamname"></param>
        /// <param name="company"></param>
        /// <param name="status"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public List<tblordersView> ExportOrders(string teamname, string moblie, string status, string orderBy)
        {
            List<tblordersView> orders = null;
            using (var db = new BFdbContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(@"select  orderid,match_name as Matchname,t.teamname,mobile,t.teamtype,paytime,ordertotal,orderstatus,l.name as linename,ls.linename as linesname  from (select r.*,r.status as orderstatus,m.match_name,u.mobile from tbl_orders r,tbl_users u,tbl_match m where  u.userid = r.userid and m.match_id = r.match_id) o
                            inner join tbl_teams t on t.teamid = o.teamid
                            left join tbl_line l on l.lineid = t.lineid
                            left join tbl_lines ls on ls.lines_id = t.linesid  where 1=1 ");

                if (!string.IsNullOrEmpty(moblie))
                    sql.AppendFormat(" AND u.mobile like '%{0}%'", moblie);

                if (!string.IsNullOrEmpty(teamname))
                    sql.AppendFormat(" AND t.teamname like '%{0}%'", teamname);

                if (!string.IsNullOrEmpty(status))
                    sql.AppendFormat(" AND o.status  = '{0}'", status);
                if (!string.IsNullOrEmpty(orderBy))
                {
                    if (orderBy.Equals("1"))
                    {
                        orders = db.SqlQuery<tblordersView>(sql.ToString()).OrderByDescending(p => p.Paytime).ToList();
                    }
                    else
                    {
                        orders = db.SqlQuery<tblordersView>(sql.ToString()).OrderBy(p => p.Paytime).ToList();
                    }

                }
                else
                {
                    orders = db.SqlQuery<tblordersView>(sql.ToString()).OrderBy(p => p.Paytime).ToList();
                }
                return orders;

            }
        }

      

    }
}
