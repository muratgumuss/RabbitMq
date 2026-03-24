using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://wkupebkx:rlWjuwammdHu0xF2Tihn7HfXo9pgCqz2@chameleon.lmq.cloudamqp.com/wkupebkx");

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        //await channel.ExchangeDeclareAsync("logs-topic", durable: true, type: ExchangeType.Topic);
        // Kuyruğu garantiye almak için declare etmek her zaman iyidir (Idempotent)
        //await channel.QueueDeclareAsync(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        //await channel.ExchangeDeclareAsync("logs-fanout", durable: true, type: ExchangeType.Fanout);
        //var queueDeclareResult = await channel.QueueDeclareAsync();
        //var randomQueueName = queueDeclareResult.QueueName;
        //await channel.QueueBindAsync(queue: randomQueueName, exchange: "logs-fanout", routingKey: "");

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false); // Her seferinde sadece 1 mesaj alır

        var consumer = new AsyncEventingBasicConsumer(channel);
        var queueName = "direct-queue-Error";
        //var routeKey = "*.Error.*";
        //await channel.QueueBindAsync(queue: queueName, exchange: "logs-topic", routingKey: routeKey);

        // Mesaj geldiğinde ne yapılacağını tanımlıyoruz
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var messageBody = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(messageBody);
            Console.WriteLine($" [x] Gelen mesaj: {message}");

            // Mesajın işlendiğini onaylamak (Eğer autoAck: false ise)
            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            //Thread.Sleep(1000); // Mesaj işlenirken biraz bekleyelim (simülasyon)
            await Task.Delay(1000);
        };

        // Bu satır RabbitMQ'ya "Abone oldum, gönder gelsin" der.
        //await channel.BasicConsumeAsync(queue: "hello-queue", autoAck: false, consumer: consumer);
        //var directQueueName = $"direct-queue-Critical";
        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        Console.WriteLine(" [*] Loglar dinleniyor. Çıkmak için [Enter] tuşuna basın.");
        Console.ReadLine();
        await Task.Delay(-1);
    }
}