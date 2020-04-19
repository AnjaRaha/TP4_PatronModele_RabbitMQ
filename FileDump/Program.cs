using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileDump
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

                List<string> theList= new List<string>();
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
                    string log = "[" + DateTime.Now.ToString() + "]" + "[" + routingKey.ToString() + "]" + ":" + message;
                    theList.Add(log);

                    //Ecriture des logs dans un fichier .log
                    string fileName = @".\MessageFile.log";
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                       foreach(string element in theList) { writer.WriteLine(element); }                                                  
                        
                    }
                    

                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();



            }
        }
    }
}
