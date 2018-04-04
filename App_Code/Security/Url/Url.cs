using System;
using System.Linq;

public class Url
{
    protected string url;
    public Url(Uri uri)
    {
        this.url = uri.AbsoluteUri;
    }
    public Url(string url)
    {
        this.url = url;
    }
    public virtual string ReplaceUrl()
    {
        return url;
    }
    protected void RemoveParameters()
    {
        if (url.Contains('&'))
            url = url.Remove(url.IndexOf('&'));

        if (url.Contains('?'))
            url = url.Remove(url.IndexOf('?'));
    }
    protected virtual void AddAutoplay()
    {
        url += "?autoplay=1";
    }

    protected virtual void Embed() { }
}