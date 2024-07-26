using System;
using System.Collections.Generic;

namespace MusicPlayer.DAL.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Music> Musics { get; set; } = new List<Music>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
