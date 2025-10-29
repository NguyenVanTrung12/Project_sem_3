using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("AdminCookie")
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/Admin/Logon/Index";
        options.AccessDeniedPath = "/Admin/Logon/AccessDenied";
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<online_aptitude_testsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

var app = builder.Build();
// 🧩 Seed tài khoản Super Manager mặc định
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<online_aptitude_testsContext>();

    // Tạo role nếu chưa có
    var superRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Role_Supper_Managers");
    if (superRole == null)
    {
        superRole = new Role { RoleName = "Role_Supper_Managers" };
        context.Roles.Add(superRole);
        await context.SaveChangesAsync();
    }

    // Tạo tài khoản Super Manager mặc định nếu chưa có
    var superManager = await context.Managers.FirstOrDefaultAsync(m => m.Username == "canhnt");
    var password = "Canh19012005@"; // mật khẩu chuẩn
    var hashed = Project_sem_3.Areas.Admin.Helpers.PasswordHelper.HashPassword(password);

    if (superManager == null)
    {
        superManager = new Manager
        {
            Username = "canhnt",
            Fullname = "Nguyễn Tuyển Cảnh",
            Email = "canh19012k5@gmail.com",
            Phone = "0379255680",
            PasswordHash = hashed,
            RoleId = superRole.Id,
            Status = 1,
            CreatedAt = DateTime.Now
        };

        context.Managers.Add(superManager);
    }
    else
    {
        // Update lại password hash chuẩn
        superManager.PasswordHash = hashed;
        context.Managers.Update(superManager);
    }

    await context.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // bắt buộc
app.Use(async (context, next) =>
{
    if (!context.Session.IsAvailable)
    {
        await context.Session.LoadAsync();
    }
    await next();
});
app.UseAuthentication(); // nếu dùng
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin_root",
        pattern: "Admin",
        defaults: new { area = "Admin", controller = "Logon", action = "Index" });

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}); ;


app.Run();
