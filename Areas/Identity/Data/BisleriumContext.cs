﻿using Bislerium.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bislerium.Models;

namespace Bislerium.Data;

public class BisleriumContext : IdentityDbContext<BisleriumUser>
{
    public BisleriumContext(DbContextOptions<BisleriumContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

public DbSet<Bislerium.Models.Blogs> Blogs { get; set; } = default!;

public DbSet<Bislerium.Models.Comment> Comment { get; set; } = default!;

public DbSet<Bislerium.Models.Reaction> Reaction { get; set; } = default!;

public DbSet<Bislerium.Models.Notification> Notification { get; set; } = default!;
}
