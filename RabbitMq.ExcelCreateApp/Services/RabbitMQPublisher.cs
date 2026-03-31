using RabbitMq.Shared;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMq.ExcelCreateApp.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public async void Publish(CreateExcelMessage productImageCreatedEvent)
        {
            var channel = await _rabbitMQClientService.Connect();

            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
            exchange: RabbitMQClientService.ExchangeName,
            routingKey: RabbitMQClientService.RoutingExcel,
            mandatory: false,
            basicProperties: properties,
            body: bodyByte);

            //channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingWatermark, basicProperties: properties, body: bodyByte);
        }
    }
}

