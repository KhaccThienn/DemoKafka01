namespace KafkaProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly KafkaProducerService _kafkaProducer;

        public ProductController(KafkaProducerService kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            // Produce message to Kafka
            await _kafkaProducer.ProduceAsync(product);
            return Ok(new { message = "Product added and produced to Kafka." });
        }
    }
}
