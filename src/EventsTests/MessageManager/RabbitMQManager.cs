using EventsTests.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventsTests.MessageManager
{
    /// <summary>
    /// Responsible for establishing connection to RabbitMQ and listening
    /// for new messages arriving from the queue we are interested int.
    /// </summary>
    internal class RabbitMQManager : IDisposable
    {
        const string kQueueName = "eventsQueue.tests";

        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer messageConsumer;
        private List<EventMessageDto> receivedEvents = new List<EventMessageDto>();
        readonly object sync = new object();

        /// <summary>
        /// Constructor. Establishes connection to RabbitMQ and configures new
        /// messages event listener.
        /// </summary>
        public RabbitMQManager()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
                RequestedHeartbeat = TimeSpan.FromSeconds(30)
            };

            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();

            this.channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);

            this.messageConsumer = new EventingBasicConsumer(this.channel);

            this.channel.BasicConsume(queue: kQueueName,
                                      autoAck: true,
                                      consumer: this.messageConsumer);

            this.messageConsumer.Received += MessageReceived;
        }

        /// <summary>
        /// Consume the oldest event we've received from RabbitMQ.
        /// </summary>
        /// <returns>The oldest message received from the message queue</returns>
        public EventMessageDto? ConsumeSingleEvent()
        {
            EventMessageDto result;

            Console.Write("Waiting for mq message");
            lock (this.sync)
            {
                while (this.receivedEvents.Count == 0)
                {
                    Monitor.Wait(this.sync, 1);
                }

                result = this.receivedEvents[0];
                this.receivedEvents.RemoveAt(0);
            }

            return result;
        }

        /// <summary>
        /// Invoked by the RabbitMQ client when a new message is available
        /// on the queue we are listening to.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="ea">The event message arguments</param>
        private void MessageReceived(object? sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine("Got message from RabbitMQ in thread " + System.Threading.Thread.CurrentThread.Name);
            var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine("Found RabbitMQ message: " + messageBody);

            try
            {
                Console.WriteLine("Before deserialization");
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<EventMessageDto>(messageBody);
                Console.WriteLine("RabbitMQ mesage deserialized");

                lock (this.sync)
                {
                    this.receivedEvents.Add(evt);
                    Monitor.Pulse(this.sync);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);    
            }
        }

        public void Dispose()
        {
            /**
             * Make sure the callers consumed all messages, failing to do so
             * may lead to unexpected results down the road with subsequent
             * test suites.
             */
            Assert.That(this.receivedEvents.Count, Is.EqualTo(0));

            this.Purge();
            this.channel.Dispose();
            this.connection.Dispose();
        }

        /// <summary>
        /// Purges any outstanding events in the message queue.
        /// </summary>
        /// <param name="waitMillis">Amount of time in milleseconds to wait 
        /// for new messages to arrive before the queue is purged.</param>
        private void Purge(int waitMillis = 0)
        {
            if (waitMillis != 0)
            {
                Thread.Sleep(waitMillis);
            }

            this.channel.QueuePurge(kQueueName);
        }

        /// <summary>
        /// Purges any outstanding messages on the messages.
        /// </summary>
        /// <param name="waitMillis">Amount of time in milleseconds to wait 
        /// for new messages to arrive before the queue is purged.</param>
        static public void PurgeQueue(int waitMillis = 0)
        {
            using (var self = new RabbitMQManager())
            {
                self.Purge(waitMillis);
            }

        }
    }
}
