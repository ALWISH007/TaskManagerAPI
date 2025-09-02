using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data;

public class AppDbContext : DbContext
{
	// This DbSet represents the "TaskItems" table in the database
	public DbSet<TaskItem> TaskItems => Set<TaskItem>();

	// Constructor that accepts DbContextOptions (required for configuration)
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	// Optional: You can add initial data here
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Seed the database with some initial tasks
		modelBuilder.Entity<TaskItem>().HasData(
			new TaskItem { Id = 1, Title = "Learn C#", Description = "Complete Phase 1", IsCompleted = true },
			new TaskItem { Id = 2, Title = "Learn HTML/CSS/JS", Description = "Complete Phase 2", IsCompleted = false }
		);
	}
}