using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;

namespace LogEmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Message msg = new Message();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())

            {
                String saisie = "";

                do
                {
                    channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");
                    var message = msg.GenerateMsg();
                    var severity = msg.GenerateLevel();


                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_logs",
                                     routingKey: severity,
                                     basicProperties: null,
                                     body: body);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
                    Console.WriteLine("Tapez Entree pour continuer et 0 pour arreter");
                    

                    saisie = Console.ReadLine();

                } while (saisie != "0");

                

                
            }
           
        }
    }
    public class Message
    {

        public Message()
        {
        }

        public String GenerateLevel()
        {
            var severity = new List<string> { "Info", "Warning", "Error", "Critical" };
            Random randNum = new Random();
            int RandomPosition = randNum.Next(severity.Count);
            String severityRandom = severity[RandomPosition];
            return severityRandom;
        }

        public String GenerateMsg()
        {

            var message = new List<string> { "Confinement", "Amour", "Paix", "Prochain", "Sagesse", "Humilité" };
            Random randNum = new Random();
            int RandomPosition = randNum.Next(message.Count);
            String messageRandom = message[RandomPosition];
            return messageRandom;
        }
    }

}
