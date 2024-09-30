namespace KafkaProductAPI.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;

        public KafkaProducerService(string bootstrapServers, string topic)
        {
            _bootstrapServers   = bootstrapServers;
            _topic              = topic;
        }

        public async Task ProduceAsync(Product product)
        {
            var config = new ProducerConfig 
            { 
                BootstrapServers = _bootstrapServers 
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var productJson = JsonConvert.SerializeObject(product);

                await producer.ProduceAsync(_topic, new Message<Null, string> 
                { 
                    Value = productJson 
                });
                Console.WriteLine($"Produced message: {productJson}");
            }
        }
    }
}
