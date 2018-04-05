using System;

public sealed class YouTubeUrl : Url
{
    public YouTubeUrl(Uri uri) : base(uri)
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
        if (!url.Contains("embed/"))
            url = url.Replace("watch?v=", "embed/");
    }
}