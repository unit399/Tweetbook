using Tweetbook.Installers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();

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

app.Run();
//https://www.youtube.com/watch?v=APLjIrZgxyo&list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU&index=12