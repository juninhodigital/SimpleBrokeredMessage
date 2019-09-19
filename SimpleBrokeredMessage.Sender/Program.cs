using System;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Messaging;

namespace SimpleBrokeredMessage.Sender
{
    class Program
    {
        #region| Properties |

        private const string Connection = "Endpoint=sb://juninhodev-esb-demo.servicebus.windows.net/;SharedAccessKeyName=demoqueuepolicy;SharedAccessKey=5e39kKZXTLWX1IxNo2Szsdj/bL7I22h2kR48+ri3Lq4=;EntityPath=demoqueue";
        private const string QueueName = "demoqueue";

        #endregion

        #region| Constructor |

        static void Main(string[] args)
        {
            SendMessages().ConfigureAwait(false);

            Console.ReadLine();
        }

        #endregion

        #region| Methods |

        /// <summary>
        /// Send messages to the Azure Service Bus (Brokered message/Queue)
        /// </summary>
        static async Task SendMessages()
        {
            var client = QueueClient.CreateFromConnectionString(Connection);

            for (int i = 0; i < 10; i++)
            {
                var message = new BrokeredMessage($"Message: {i}");

                await client.SendAsync(message);

                Console.WriteLine($"Message id: {i} sent");
            }

            await client.CloseAsync();

            client = null;
        }

        #endregion
    }
}