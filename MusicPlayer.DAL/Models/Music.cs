using System;
using System.Collections.Generic;

namespace MusicPlayer.DAL.Models;

public partial class Music
{
    public int MusicId { get; set; }

    public string? MusicName { get; set; }

    public string? ArtistName { get; set; }

    public string? Link { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Status { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();

<<<<<<< Updated upstream
    public virtual ICollection<User> Users { get; set; } = new List<User>();
=======
    public virtual User? User { get; set; }
>>>>>>> Stashed changes
}
