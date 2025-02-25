using RabbitMQ.Client;
using System;
using System.Text;

namespace CartService.Services
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher()
        {
           var factory = new ConnectionFactory() 
             {
                HostName = "localhost",
                Port = 5672,            
                UserName = "guest",    
                Password = "guest"      
            };


                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
        } 

       

        /// Объявляет очередь, если она еще не создана
      
        public void DeclareQueue(string queueName)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true, // Очередь сохраняется при перезапуске RabbitMQ
                exclusive: false, // Очередь доступна для нескольких подключений
                autoDelete: false, // Очередь не удаляется при закрытии последнего соединения
                arguments: null);
        }

      
        /// Отправляет сообщение в указанную очередь
       
       public void SendMessage(string queue, string message)
{
    try
    {
        _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: null, body: body);

        Console.WriteLine($"[!] Отправлено сообщение в очередь '{queue}': {message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка отправки в RabbitMQ: {ex.Message}");
    }
}

      
        /// Закрывает соединение и канал при завершении работы
 
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
