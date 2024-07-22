using System;
using System.Collections.Generic;

namespace MusicPlayer.DAL;

public partial class Music
{
    public int MusicId { get; set; }

    public string? MusicName { get; set; }

    public string? Link { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();
}
