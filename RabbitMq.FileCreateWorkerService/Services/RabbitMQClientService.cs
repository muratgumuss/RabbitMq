using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.FileCreateWorkerService.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IChannel _channel;
        public static string QueueName = "queue-excel-file";
        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IChannel> Connect()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = await _connection.CreateChannelAsync();
            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");

            return _channel;
        }

        public async void Dispose()
        {
            await _channel?.CloseAsync();
            _channel?.DisposeAsync().GetAwaiter();

            await _connection?.CloseAsync();
            _connection?.DisposeAsync().GetAwaiter();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");

        }
    }
}
