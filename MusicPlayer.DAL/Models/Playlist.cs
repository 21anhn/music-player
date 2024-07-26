using System;
using System.Collections.Generic;

namespace MusicPlayer.DAL.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string? PlaylistName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
