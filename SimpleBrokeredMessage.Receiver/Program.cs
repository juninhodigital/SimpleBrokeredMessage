using System;
using System.Diagnostics;

using Microsoft.ServiceBus.Messaging;

namespace SimpleBrokeredMessage.Receiver
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
            ReceiveMessages();
        }

        #endregion

        #region| Methods |

        /// <summary>
        /// Send messages to the Azure Service Bus (Brokered message/Queue)
        /// </summary>
        static void ReceiveMessages()
        {
            var client = QueueClient.CreateFromConnectionString(Connection, ReceiveMode.PeekLock);

            // Create a message pump to receive messages
            client.OnMessage(message=>
            {
                try
                {
                    ProcessMessage(message);

                    message.Complete();
                }
                catch (Exception e)
                {
                    // Handle any message processing specific exceptions here
                    message.Abandon();
                    
                    if(Debugger.IsAttached)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }, new OnMessageOptions() { AutoComplete = false });

            Console.WriteLine("Press enter to exit");

            Console.ReadLine();

            client.Close();
            client = null;
        }

        /// <summary>
        /// Process the incoming message
        /// </summary>
        /// <param name="message"></param>
        static void ProcessMessage(BrokeredMessage message)
        {
            var text = message.GetBody<string>();

            Console.WriteLine($"Message: {text}");
        }

        #endregion
    }
}
