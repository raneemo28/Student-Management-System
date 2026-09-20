using App.LoggingMicroservice.Consumers;
using App.LoggingMicroservice.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LogsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogsDbConnection")));

builder.Services.AddHostedService<LogConsumer>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<LogsDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();