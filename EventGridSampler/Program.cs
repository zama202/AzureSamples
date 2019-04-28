using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGridSampler
{
    class Program
    {
        private static string topicEndpoint = "https://gab2019vregt001.westeurope-1.eventgrid.azure.net/api/events";
        private static string topicHostName = new Uri(topicEndpoint).Host;
        private static string topicKey = "k+WNMgRC7xCOS+TIa2AI88mMEzvv7/F/F/e9LK4AS0I=";


        public static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            var poco = new Poco { Id = "100", Name = "Tsuyoshi" };
            var pocoString = JsonConvert.SerializeObject(poco);
            Console.WriteLine($"The Function1 called: {pocoString}");

            while (true)
            {
                Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                try
                {
                    sendEventGridMessageWithEventGridClientAsync(topicHostName, "some/func1", poco).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                    throw;
                }
            }

            
            

        }
        public class Poco
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private static async Task sendEventGridMessageWithEventGridClientAsync(string topic, string subject, object data)
        {
            TopicCredentials topicCredentials = new TopicCredentials(topicKey);
            EventGridClient client = new EventGridClient(topicCredentials);

            
            var eventGridEvent = new EventGridEvent
            {
                Subject = subject,
                EventType = "func-event",
                EventTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Data = data,
                DataVersion = "1.0.0",
            };
            var events = new List<EventGridEvent>
            {
                eventGridEvent
            };

            AzureOperationResponse x = await client.PublishEventsWithHttpMessagesAsync(topic, events);
            Console.WriteLine(x.Response.Content);
        }

        private static IList<EventGridEvent> GetEventsList()
        {
            throw new NotImplementedException();
        }
    }
}
