using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tweetbook.Contracts.HealthCheck;
using Tweetbook.Data;
using Tweetbook.Installers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.InstallServicesInAssembly(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

using(var serviceScope = app.Services.CreateScope())
{
    /*var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
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
    }*/
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

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new HealthCheck
            {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description,
            }),
            Duration = report.TotalDuration
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
});

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

//https://www.youtube.com/watch?v=BGASedyh16I&list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU&index=23
//https://github.com/Alvin-Leung/tweetbook-api