using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class DailyMotionPlaylist : Playlist
{
    //JSON parsed
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int videos_total { get; set; }
    public string thumbnail_120_url { get; set; }
    public string thumbnail_360_url { get; set; }

    //ABSTRACT CLASS implemented
    public override string ID { get { return this.id; } }
    public override string Title { get { return this.name; } }
    public override string Description { get { return this.description; } }
    public override int TotalVideosCount { get { return this.videos_total; } }
    public override string SmallThumbnailURL { get { return this.thumbnail_120_url; } }
    public override string BigThumbnailURL { get { return this.thumbnail_360_url; } }

    public override IEnumerable<Video> Videos { get; set; }

    public DailyMotionPlaylist()
	{
	}

}