using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace LogAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] severityList = { "Warning ", "Info", "Critical", "Error" };
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");
                var queueName = channel.QueueDeclare().QueueName;

                foreach (var severity in severityList)
                {
                    channel.QueueBind(queue: queueName,
                                      exchange: "direct_logs",
                                      routingKey: severity);
                }

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);

                List<Log> listeLog = new List<Log>();
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var routingKey = ea.RoutingKey;
                    //Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
                    Log log = new Log(routingKey, message);
                    listeLog.Add(log);
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromSeconds(30);

                var timer = new System.Threading.Timer((e) =>
                {

                    //Affichage
                    Console.WriteLine("Info:" + listeLog.Where(elem => elem.severity == "Info").Count() + " log");
                    Console.WriteLine("Warning:" + listeLog.Where(elem => elem.severity == "Warning").Count() + " log");
                    Console.WriteLine("Error:" + listeLog.Where(elem => elem.severity == "Error").Count() + " log");
                    Console.WriteLine("Critical:" + listeLog.Where(elem => elem.severity == "Critical").Count() + " log");
                    Console.WriteLine("------");
                }, null, startTimeSpan, periodTimeSpan);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();



            }
        }
    }
}
