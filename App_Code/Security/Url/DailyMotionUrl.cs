using System;

public sealed class DailyMotionUrl : Url
{
    public DailyMotionUrl(Uri uri) : base(uri)
    {
    }

    public override string ReplaceUrl()
    {
        Embed();
        RemoveParameters();
        AddAutoplay();

        return url;
    }

    protected override void Embed()
    {
        if(!url.Contains("/embed/video/"))
            url = url.Replace("/video/", "/embed/video/");
    }
}