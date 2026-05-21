using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project1.Models.API
{
    // Root object
    public class LiveScoreLeagueResponse
    {
        public List<Stage> Stages { get; set; }
    }

    public class Stage
    {
        public string Sid { get; set; }
        public string Snm { get; set; }
        public string Scd { get; set; }
        public string badgeUrl { get; set; }
        public string firstColor { get; set; }
        public string Cid { get; set; }
        public string Cnm { get; set; }
        public string CnmT { get; set; }
        public string Csnm { get; set; }
        public string Ccd { get; set; }
        public string CompId { get; set; }
        public string CompN { get; set; }
        public string CompD { get; set; }
        public string CompUrlName { get; set; }
        public string CompST { get; set; }
        public int Scu { get; set; }
        public string Sds { get; set; }
        public int Chi { get; set; }
        public int Shi { get; set; }
        public string Ccdiso { get; set; }
        public string Sdn { get; set; }

        public Feed Feed { get; set; }
        public LeagueTable LeagueTable { get; set; }
    }

    public class Feed
    {
        public string Id { get; set; }
        public List<string> Items { get; set; }
    }

    public class LeagueTable
    {
        public List<L> L { get; set; }
    }

    public class L
    {
        public List<Table> Tables { get; set; }

        [JsonProperty("Tables-f")]
        public List<TablesF> Tablesf { get; set; }
    }

    public class Table
    {
        public int LTT { get; set; }
        public List<LeagueTeam> team { get; set; }
        public List<PhrX> phrX { get; set; }
    }

    public class TablesF
    {
        public int LTT { get; set; }
        public List<LeagueTeam> team { get; set; }
    }

    public class PhrX
    {
        public int V { get; set; }
        public int D { get; set; }
    }

    public class LeagueTeam
    {
        public int rnk { get; set; }
        public string Tid { get; set; }
        public int win { get; set; }
        public string winn { get; set; }
        public int wreg { get; set; }
        public int wap { get; set; }
        public int pf { get; set; }
        public int pa { get; set; }
        public int wot { get; set; }
        public string Tnm { get; set; }
        public int lst { get; set; }
        public string lstn { get; set; }
        public int lreg { get; set; }
        public int lot { get; set; }
        public int lap { get; set; }
        public int drw { get; set; }
        public string drwn { get; set; }
        public int gf { get; set; }
        public int ga { get; set; }
        public int gd { get; set; }
        public string ptsn { get; set; }
        public List<int> phr { get; set; }
        public int Ipr { get; set; }
        public string Img { get; set; }
        public string kitImg { get; set; }
        public int pts { get; set; }
        public int pld { get; set; }
        public List<Form> Form { get; set; }
    }

    public class Form
    {
        public int r { get; set; }
        public string Eid { get; set; }
    }
}
