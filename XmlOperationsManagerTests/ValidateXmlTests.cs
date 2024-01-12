using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace XmlOperationsManagerTests
{
    [TestClass]
    public class ValidateXmlTests
    {

        [TestMethod]
        public async Task ValidateXml_1000_Requests_OnAazure_Success()
        {

            // Arrange
            string azureFunctionUrl = "https://<function-app-name>.azurewebsites.net/api/ValidateXml"; 
            string xmlBody = "https://<your-storage-account>.blob.core.windows.net/<container-name>/test.xml";
            string xsdSchemaUrl = "https://<your-storage-account>.blob.core.windows.net/<container-name>/rfcInvoiceImageRfc.xsd";

            List<HttpRequestMessage> messages = new List<HttpRequestMessage>();

            HttpRequestMessage request;

            // Create 100 requests
            for (int i = 0; i < 1000; i++)
            {
                // Prep the HTTP POST request with a custom hearder value [x-xsd-schema] for the XSD schema URL and the cmlBody in the request body
                request = new HttpRequestMessage(HttpMethod.Post, azureFunctionUrl);
                request.Headers.Add("x-xsd-schema", xsdSchemaUrl);
                request.Content = new StringContent(xmlBody, System.Text.Encoding.UTF8, "text/plain");
                messages.Add(request);
            }


            // Act
            var responses = await Task.WhenAll(messages.ConvertAll(async m => await new HttpClient().SendAsync(m)));
            

            // Assert
            // Print Debug message for number for Ok responses and number of Bad responses
            int okResponses = 0;
            int badResponses = 0;
            foreach (var response in responses)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    okResponses++;
                }
                else
                {
                    badResponses++;
                }
            }
            Console.WriteLine($"OK Responses: {okResponses}");
            Console.WriteLine($"Bad Responses: {badResponses}");

            // Print Debug message for the response body of the bad responses
            foreach (var response in responses)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Response Body: {await response.Content.ReadAsStringAsync()}");
                }
            }

            // Check that all responses are OK
            foreach (var response in responses)
            {
                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
            
        }
    }
}
