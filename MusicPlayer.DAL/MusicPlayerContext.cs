using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.DAL;

public partial class MusicPlayerContext : DbContext
{
    public MusicPlayerContext()
    {
    }

    public MusicPlayerContext(DbContextOptions<MusicPlayerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Music> Musics { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistMusic> PlaylistMusics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];

        return strConn;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Music>(entity =>
        {
            entity.HasKey(e => e.MusicId).HasName("PK__Music__AB12F87E47703982");

            entity.ToTable("Music");

            entity.Property(e => e.MusicId).HasColumnName("musicId");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Link)
                .IsUnicode(false)
                .HasColumnName("link");
            entity.Property(e => e.MusicName).HasColumnName("musicName");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PK__Playlist__D52A1126EB3EE2B7");

            entity.ToTable("Playlist");

            entity.Property(e => e.PlaylistId).HasColumnName("playlistId");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.PlaylistName).HasColumnName("playlistName");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<PlaylistMusic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Playlist__3213E83FA7E9B817");

            entity.ToTable("PlaylistMusic");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MusicId).HasColumnName("musicId");
            entity.Property(e => e.PlaylistId).HasColumnName("playlistId");

            entity.HasOne(d => d.Music).WithMany(p => p.PlaylistMusics)
                .HasForeignKey(d => d.MusicId)
                .HasConstraintName("FK__PlaylistM__music__3C69FB99");

            entity.HasOne(d => d.Playlist).WithMany(p => p.PlaylistMusics)
                .HasForeignKey(d => d.PlaylistId)
                .HasConstraintName("FK__PlaylistM__playl__3D5E1FD2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__CB9A1CFF24C1DB21");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.FullName).HasColumnName("fullName");
            entity.Property(e => e.Password)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
