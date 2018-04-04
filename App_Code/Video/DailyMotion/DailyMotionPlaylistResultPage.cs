using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DailyMotionPlaylistResultPage : PlaylistResultPage
{
    //JSON parsed
    public int total { get; set; }
    public int page { get; set; }
    public int limit { get; set; }
    public bool has_more { get; set; }
    public List<DailyMotionPlaylist> list { get; set; }

    //ABSTRACT CLASS implemented
    public override int PageNumber { get { return this.page; } }
    public override int CurentLimit { get { return this.limit; } }
    public override int TotalResults { get { return this.total; } }
    public override bool HasMorePages { get { return this.has_more; } }
    public override IEnumerable<Playlist> Playlists { get { return this.list; } }

    public DailyMotionPlaylistResultPage()
	{
	}
}