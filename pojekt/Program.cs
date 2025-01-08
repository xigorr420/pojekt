using pojekt.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WarehouseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine));

builder.Services.AddControllersWithViews();

// Configure session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WarehouseContext>();

    context.Database.EnsureCreated();

    if (!context.Users.Any())
    {
        context.Users.Add(new UserModel { Name = "Admin", Email = "admin@example.com", Password = "admin123", Role = "Admin" });
        context.Users.Add(new UserModel { Name = "User", Email = "user@example.com", Password = "user123", Role = "User" });
        context.SaveChanges();
    }
    if (!context.Products.Any())
    {
        context.Products.Add(new ProductModel { Name = "M³otek", Description = "M³otek stalowy z gumow¹ r¹czk¹", Category = "Narzêdzia", Quantity = "50", Price = 35.50m });
        context.Products.Add(new ProductModel { Name = "Wiertarka", Description = "Wiertarka elektryczna 500W", Category = "Narzêdzia", Quantity = "30", Price = 249.99m });
        context.Products.Add(new ProductModel { Name = "Farba Akrylowa", Description = "Farba akrylowa do wnêtrz, 5L", Category = "Farby", Quantity = "100", Price = 89.99m });
        context.Products.Add(new ProductModel { Name = "Klej do p³ytek", Description = "Klej do p³ytek ceramicznych 25kg", Category = "Materia³y budowlane", Quantity = "200", Price = 59.99m });
        context.Products.Add(new ProductModel { Name = "P³yta Gipsowo-Kartonowa", Description = "P³yta gipsowo-kartonowa, 120x240cm", Category = "Materia³y budowlane", Quantity = "150", Price = 35.00m });
        context.SaveChanges();
    }

    if (!context.Orders.Any())
    {
        var user = context.Users.First(u => u.Email == "user@example.com");
        var product1 = context.Products.First(p => p.Name == "M³otek");
        var product2 = context.Products.First(p => p.Name == "Wiertarka");

        context.Orders.Add(new OrderModel
        {
            UserId = user.ID,
            ProductId = product1.ProductId,
            Quantity = 2,
            TotalPrice = product1.Price * 2,
            Status = "Zrealizowane"
        });

        context.Orders.Add(new OrderModel
        {
            UserId = user.ID,
            ProductId = product2.ProductId,
            Quantity = 1,
            TotalPrice = product2.Price,
            Status = "W trakcie realizacji"
        });

        context.SaveChanges();
    }
    if (!context.UserDetails.Any())
    {
        var user = context.Users.First(u => u.Email == "user@example.com");
        context.UserDetails.Add(new UserDetailsModel
        {
            UserId = user.ID,
            PhoneNumber = "123-456-789",
            City = "Warszawa",
            Street = "Kwiatowa",
            HouseNumber = "12"
        });
        context.SaveChanges();
    }


}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
