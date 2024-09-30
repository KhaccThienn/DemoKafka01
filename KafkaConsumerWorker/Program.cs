using KafkaConsumerWorker;
using KafkaConsumerWorker.Data;
using KafkaConsumerWorker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("MssqlDb")));

builder.Services.AddHostedService<KafkaConsumerService>(sp =>
                    new KafkaConsumerService(sp, "localhost:9092", "products"));

var host = builder.Build();
host.Run();
