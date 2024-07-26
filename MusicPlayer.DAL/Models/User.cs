using System;
using System.Collections.Generic;

namespace MusicPlayer.DAL.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }

    public string? Password { get; set; }

    public int? PlaylistId { get; set; }

    public int? MusicId { get; set; }

    public virtual Music? Music { get; set; }

    public virtual Playlist? Playlist { get; set; }
}
