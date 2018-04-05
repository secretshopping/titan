using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public abstract class PlaylistResultPage
{
    public abstract int PageNumber { get; } //Starting from 1

    public abstract int TotalResults { get; }
    public abstract int CurentLimit { get; }

    public abstract bool HasMorePages { get; }

    public abstract IEnumerable<Playlist> Playlists { get; }
}