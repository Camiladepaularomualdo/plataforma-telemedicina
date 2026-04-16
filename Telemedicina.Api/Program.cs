using Microsoft.EntityFrameworkCore;
using Telemedicina.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Telemedicina.Infrastructure")));

// Register Repositories
builder.Services.AddScoped(typeof(Telemedicina.Interfaces.IGenericRepository<>), typeof(Telemedicina.Infrastructure.Repositories.GenericRepository<>));
builder.Services.AddScoped<Telemedicina.Interfaces.IDoctorRepository, Telemedicina.Infrastructure.Repositories.DoctorRepository>();
builder.Services.AddScoped<Telemedicina.Interfaces.IPatientRepository, Telemedicina.Infrastructure.Repositories.PatientRepository>();
builder.Services.AddScoped<Telemedicina.Interfaces.IAppointmentRepository, Telemedicina.Infrastructure.Repositories.AppointmentRepository>();

// Register Services
builder.Services.AddScoped<Telemedicina.Interfaces.IDoctorService, Telemedicina.Services.DoctorService>();
builder.Services.AddScoped<Telemedicina.Interfaces.IPatientService, Telemedicina.Services.PatientService>();
builder.Services.AddScoped<Telemedicina.Interfaces.IAppointmentService, Telemedicina.Services.AppointmentService>();
builder.Services.AddScoped<Telemedicina.Interfaces.IEmailService, Telemedicina.Services.EmailService>();

// Register Background Services
builder.Services.AddHostedService<Telemedicina.Api.Services.CreditRenewalBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://3.217.32.12", "https://3.217.32.12")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
//app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
