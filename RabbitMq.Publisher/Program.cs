using RabbitMQ.Client;
using System.Text;
using static Program;

public class Program
{
    public enum LogLevel
    {
        Info = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }
    private static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://wkupebkx:rlWjuwammdHu0xF2Tihn7HfXo9pgCqz2@chameleon.lmq.cloudamqp.com/wkupebkx");

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        //await channel.QueueDeclareAsync(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        //await channel.ExchangeDeclareAsync("logs-fanout", durable: true, type: ExchangeType.Fanout);
        await channel.ExchangeDeclareAsync("logs-direct", durable: true, type: ExchangeType.Direct);


        foreach (var logLevel in Enum.GetNames(typeof(LogLevel)))
        {
            var routeKey = $"route-{logLevel}"; // ✅ tutarlı: hep lowercase
            var queueName = $"direct-queue-{logLevel}";
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue: queueName, exchange: "logs-direct", routingKey: routeKey);
        }

        foreach (var i in Enumerable.Range(1, 50))
        {
            LogLevel loglevel = (LogLevel)new Random().Next(1, 5);

            var message = $"log-type: {loglevel}";
            var messageBody = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties();

            var routeKey = $"route-{loglevel.ToString()}"; // ✅ tutarlı: hep lowercase

            await channel.BasicPublishAsync(
                exchange: "logs-direct",
                routingKey: routeKey,
                mandatory: false,
                basicProperties: properties,
                body: messageBody);

            Console.WriteLine($"Log {i} gönderildi! {message}");
        }


        //var message = "Hello World!";
        //var messageBody = Encoding.UTF8.GetBytes(message);
        //var properties = new BasicProperties();

        //await channel.BasicPublishAsync(
        //            exchange: string.Empty,
        //            routingKey: "hello-queue",
        //            mandatory: false,
        //            basicProperties: properties, 
        //            body: messageBody);

        Console.WriteLine("Mesaj gönderildi!");
        Console.ReadLine();
    }
}