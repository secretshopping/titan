using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class DailyMotionVideo : Video
{
    //JSON parsed
    public string id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public int duration { get; set; }
    public int views_total { get; set; }
    public bool ads { get; set; }
    public string thumbnail_120_url { get; set; }
    public string thumbnail_360_url { get; set; }

    //ABSTRACT CLASS implemented
    public override string ID { get { return this.id; } }
    public override string Title { get { return this.title; } }
    public override string Description { get { return this.description; } }
    public override int DurationInSeconds { get { return this.duration; } }
    public override int Views { get { return this.views_total; } }
    public override bool HasAds { get { return this.ads; } }
    public override string SmallThumbnailURL { get { return this.thumbnail_120_url; } }
    public override string BigThumbnailURL { get { return this.thumbnail_360_url; } }

    public override VideoPlayer Player { get { return new DailyMotionVideoPlayer(ID, Title, Description, base.ViewsToString()); } }


	public DailyMotionVideo()
	{
	}

}