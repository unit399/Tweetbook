using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Installers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();

using(var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var adminRole = new IdentityRole("Admin");
        await roleManager.CreateAsync(adminRole);
    }
    
    if (!await roleManager.RoleExistsAsync("Poster"))
    {
        var posterRole = new IdentityRole("Poster");
        await roleManager.CreateAsync(posterRole);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


await app.RunAsync();
public partial class Program { }

//https://www.youtube.com/watch?v=APLjIrZgxyo&list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU&index=12
//https://github.com/Alvin-Leung/tweetbook-api