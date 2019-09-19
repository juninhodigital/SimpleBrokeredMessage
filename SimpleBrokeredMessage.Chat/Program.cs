﻿using System;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SimpleBrokeredMessage.Chat
{
    class Program
    {
        #region| Properties |

        private static string Username;
        
        private const string Connection = "Endpoint=sb://juninhodev-esb-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=WbPfzOPHjydsjlffOsRp6sslXqNCJTm7NDeEnIMOZKA=";
        private const string Topic      = "demochat";

        #endregion

        #region| Constructor |

        static void Main(string[] args)
        {
            Login();

            CreateTopic();
            CreateSubscription();
            Subscribe();
        }

        #endregion

        #region| Methods |

        /// <summary>
        /// Get the chat user
        /// </summary>
        private static void Login()
        {
            Console.WriteLine("Username:");

            Username = Console.ReadLine();

            Console.Clear();
        }

        /// <summary>
        /// Create a namespace manager to manage artifacts
        /// </summary>
        private static NamespaceManager GetNamespace() => NamespaceManager.CreateFromConnectionString(Connection);

        /// <summary>
        /// Create a topic
        /// </summary>
        private static void CreateTopic()
        {
            var manager = GetNamespace();

            if (!manager.TopicExists(Topic))
            {
                manager.CreateTopic(Topic);
            }
        }

        /// <summary>
        /// Create the subscription
        /// </summary>
        private static void CreateSubscription()
        {
            var manager = GetNamespace();

            if (!manager.SubscriptionExists(Topic, Username))
            {
                // Create a subscription for the user
                var description = new SubscriptionDescription(Topic, Username) { AutoDeleteOnIdle = TimeSpan.FromMinutes(5) };

                manager.CreateSubscription(description);

            }
        }

        /// <summary>
        /// Subscribe to the topic
        /// </summary>
        private static void Subscribe()
        {
            // Create clients
            var factory            = MessagingFactory.CreateFromConnectionString(Connection);
            var topicClient        = factory.CreateTopicClient(Topic);
            var subscriptionClient = factory.CreateSubscriptionClient(Topic, Username);

            subscriptionClient.OnMessage(msg => ProcessMessage(msg));

            SendMessage(topicClient);
        }

        /// <summary>
        /// Process the incoming message
        /// </summary>
        /// <param name="msg"></param>
        private static void ProcessMessage(BrokeredMessage msg)
        {
            var text = msg.GetBody<string>();

            Console.WriteLine($"Message text: {text}");
        }

        /// <summary>
        /// Send message to the topic
        /// </summary>
        /// <param name="topicClient"></param>
        private static void SendMessage(TopicClient topicClient)
        {
            var message = new BrokeredMessage($" {Username} has entered the room...") { Label = Username };

            topicClient.Send(message);

            while (true)
            {
                var text = Console.ReadLine();

                if (text.Equals("exit"))
                {
                    break;
                }
                else
                {
                    message = new BrokeredMessage(text) { Label = Username };

                    topicClient.Send(message);
                }
            }
        }

        #endregion
    }
}
