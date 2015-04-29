using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Services;

namespace FillDBFromGmail
{
    class GmailHelper
    {
        static void Main(string[] args)
        {
            try
            {
                Task.Run(async () =>
                    {
                        GmailService servise = await Authorize();
                        List<Message> messages = ListMessages(servise, "me", "before:2013/04/02");
                        foreach (var m in messages)
                        {
                            var message = GetMessage(servise, "me", m.Id);
                            PutToDb(message);
                            Console.WriteLine("putted to db");
                        }
                    }).Wait();

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

        }

        private static void PutToDb(Message input)
        {
            try
            {
                using (var db = new MessageContext())
                {
                    var message = new EMessage { MessageId = input.Id, Size = (int)input.SizeEstimate, Snippet = input.Snippet };
                    db.Messages.Add(message);
                    db.SaveChanges();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }


        public static async Task<GmailService> Authorize()
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(Secrets,
                new[] { GmailService.Scope.GmailReadonly },
                "user", CancellationToken.None);
           
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail Test",
            });
            return service;
        }

        /// <summary>
        /// List all Messages of the user's mailbox matching the query.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="query">String used to filter Messages returned.</param>
        public static List<Message> ListMessages(GmailService service, String userId, String query)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);
            request.Q = query;

            do
            {
                try
                {
                    ListMessagesResponse response = request.Execute();
                    result.AddRange(response.Messages);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }

        /// <summary>
        /// Retrieve a Message by ID.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="messageId">ID of Message to retrieve.</param>
        public static Message GetMessage(GmailService service, String userId, String messageId)
        {
            try
            {
                return service.Users.Messages.Get(userId, messageId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return null;
        }

        public static ClientSecrets Secrets = new ClientSecrets()
        {
            ClientId = "Your client ID",
            ClientSecret = "Your client secret"
        };
    }
}
