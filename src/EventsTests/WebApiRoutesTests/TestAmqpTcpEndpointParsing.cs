using System;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace EventsTests.WebApiRoutesTests
{
    [TestFixture]
    public class TestAmqpTcpEndpointParsing
    {
        [Test]
        public void TestHostWithPort()
        {
            AmqpTcpEndpoint e = AmqpTcpEndpoint.Parse("localhost:5672");

            Assert.That(e.Port, Is.EqualTo(5672));
            Assert.That(e.HostName, Is.EqualTo("localhost"));
        }

        [Test]
        public void TestHostWithoutPort()
        {
            AmqpTcpEndpoint e = AmqpTcpEndpoint.Parse("localhost");

            Assert.That(e.HostName, Is.EqualTo("localhost"));
            Assert.That(e.Port, Is.EqualTo(Protocols.DefaultProtocol.DefaultPort));

        }
    }
}
