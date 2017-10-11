using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Android.App;
using Newtonsoft.Json;

namespace GatewayXNative.Services
{
    public class AWSService
    {
        AmazonSQSClient sqsClient { get; set; }
        string queueUrl { get; set; }

        public static AWSService instance = null;

        public static AWSService SharedInstance()
        {
            if (instance == null)
            {
                instance = new AWSService();
            }
            return instance;
        }

        public AWSService()
        {
            string accessKey = Application.Context.Resources.GetString(Resource.String.aws_access_key);
            string secretKey = Application.Context.Resources.GetString(Resource.String.aws_secret_key);
            queueUrl = Application.Context.Resources.GetString(Resource.String.aws_queue_endpoint);
            sqsClient = new AmazonSQSClient(accessKey, secretKey, RegionEndpoint.USEast1);
        }

        public async Task<bool> SendMessageAsync(object item)
        {
            SendMessageRequest request = new SendMessageRequest();
            request.MessageBody = JsonConvert.SerializeObject(item);
            request.QueueUrl = queueUrl;

            var response = await sqsClient.SendMessageAsync(request);
            return true;
        }

        public async Task<bool> SendBatchMessagesAsync(List<object> items)
        {
            //only 10 messages can be sent in a batch
            for (int i = 0; i < items.Count; i += 10)
            {
                List<SendMessageBatchRequestEntry> requests = new List<SendMessageBatchRequestEntry>(10);
                List<object> ten = items.Skip(i).Take(10).ToList();
                foreach (object item in ten)
                {
                    requests.Add(new SendMessageBatchRequestEntry(queueUrl, JsonConvert.SerializeObject(item)));
                }
                SendMessageBatchRequest request = new SendMessageBatchRequest();
                request.Entries = requests;

                var response = await sqsClient.SendMessageBatchAsync(request);
            }
            return true;
        }
    }
}
