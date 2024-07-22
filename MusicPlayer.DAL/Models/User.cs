using System;
using System.Collections.Generic;

namespace MusicPlayer.DAL;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }

    public string? Password { get; set; }
}
