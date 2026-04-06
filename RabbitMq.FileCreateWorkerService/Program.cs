using Microsoft.EntityFrameworkCore;
using RabbitMq.FileCreateWorkerService;
using RabbitMq.FileCreateWorkerService.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMq"))
});
builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddDbContext<AdventureWorksLt2022Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

var host = builder.Build();
host.Run();
