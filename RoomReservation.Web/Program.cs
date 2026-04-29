using RoomReservation.Data.Database;
using RoomReservation.Data.Repositories;

namespace RoomReservation.Web
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
				?? "Data Source=room_reservation.db";

			builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
			builder.Services.AddScoped<DatabaseInitializer>();
			builder.Services.AddScoped<RoomRepository>();
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
				await databaseInitializer.InitializeAsync();
			}

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapControllers();

			app.Run();

		}
	}
}
