using RabbitMQ.Client;
using System.Text;

public class Program
{
    private static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://wkupebkx:rlWjuwammdHu0xF2Tihn7HfXo9pgCqz2@chameleon.lmq.cloudamqp.com/wkupebkx");

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var message = "Hello World!";
        var messageBody = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties();

        await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "hello-queue",
                    mandatory: false,
                    basicProperties: properties, 
                    body: messageBody);

        Console.WriteLine("Mesaj gönderildi!");
        Console.ReadLine();
    }
}