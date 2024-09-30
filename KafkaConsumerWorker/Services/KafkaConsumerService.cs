using KafkaConsumerWorker.Data;

namespace KafkaConsumerWorker.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider   _serviceProvider;
        private readonly string             _bootstrapServers;
        private readonly string             _topic;

        public KafkaConsumerService(IServiceProvider serviceProvider, string bootstrapServers, string topic)
        {
            _serviceProvider    = serviceProvider;
            _bootstrapServers   = bootstrapServers;
            _topic              = topic;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId                 = "product-consumer-group-1",
                BootstrapServers        = _bootstrapServers,
                AutoOffsetReset         = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics   = true
            };

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                consumer.Subscribe(_topic);
                Console.WriteLine("Consumer started. Press Ctrl+C to exit.");
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumeResult   = consumer.Consume(stoppingToken);
                        var productJson     = consumeResult.Message.Value;

                        var product                 = JsonConvert.DeserializeObject<Product>(productJson);
                        string productInfomation    = $"[Product Name: {product.Name} - Price: {product.Price} - Description: {product.Description}]";
                        await Console.Out.WriteLineAsync($"Consumed product: {productInfomation} [ at {consumeResult.TopicPartition} ]");

                        using var scope     = _serviceProvider.CreateScope();
                        var dbContext       = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        dbContext.Products.Add(product);
                        await dbContext.SaveChangesAsync();

                        await Console.Out.WriteLineAsync($"Added {product.Name} into database");
                    }
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }

            }
        }
    }
}
