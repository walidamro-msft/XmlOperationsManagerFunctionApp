using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XmlOperationsManager
{
    public class ValidateXml
    {
        private readonly ILogger _logger;
        public static bool hasError = false;

        /// <summary>
        /// Class constructor. It will inject the ILoggerFactory to be used for logging.
        /// </summary>
        /// <param name="loggerFactory"></param>
        public ValidateXml(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ValidateXml>();
        }

        /// <summary>
        /// This is the main method that will be called by the Azure Function. You need to host this method in a windows-based Azure Function App.
        /// It will validate the XML against its XSD schema.
        /// The XML body is passed in the request body and the XSD schema URL is passed in the request header of x-xsd-schema
        /// In the HTTP Request Body, you can pass the XML body directly or you can pass a URL to the XML body.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("ValidateXml")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Validating XML against its XSD...");

            // get the XML body from the request body
            string reqBody = await new StreamReader(req.Body).ReadToEndAsync();


            if (string.IsNullOrEmpty(reqBody))
            {
                _logger.LogError("XML body is empty");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var xmlBody = reqBody.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? await LoadFromUrl(reqBody)
                : reqBody;

            if (string.IsNullOrEmpty(xmlBody))
            {
                _logger.LogError("XML body is empty");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            // This a huge XML file, so we don't want to log it... Please uncomment if you want to log it
            //_logger.LogInformation($"XML Body:\n{xmlBody}");

            // get the XSD schema from the request header of x-xsd-schema
            string xsdSchemaUrl = req.Headers.GetValues("x-xsd-schema").FirstOrDefault();


            if (string.IsNullOrEmpty(xsdSchemaUrl))
            {
                _logger.LogError("Header [x-xsd-schema] for XSD Schema URL is empty");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            // Uncomment when debugging
            //_logger.LogInformation($"XSD Schema URL: {xsdSchemaUrl}");


            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, xsdSchemaUrl);
            settings.ValidationType = ValidationType.Schema;

            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            bool hasError = false;
            HttpResponseData response = null;

            // Convert xmlContent to Stream
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlBody)))


                try
                {
                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        try
                        {
                            while (reader.Read()) { }
                            if (!hasError)
                            {
                                _logger.LogInformation("XML is valid");

                                response = req.CreateResponse(HttpStatusCode.OK);
                                await response.WriteStringAsync($"XML is valid"); // Comment if you do not need to return a response body

                                return response;
                            }
                        }
                        catch (XmlException ex)
                        {
                            _logger.LogError($"XML is not well-formed! Error: {ex.Message}");
                            response = req.CreateResponse(HttpStatusCode.BadRequest);
                            await response.WriteStringAsync($"XML is not weel-formed! Error Message: {ex.Message}");

                            return response;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Unexpected Error: {ex.Message}");
                            response = req.CreateResponse(HttpStatusCode.BadRequest);
                            await response.WriteStringAsync($"Unexpected Error: {ex.Message}");

                            return response;
                        }
                    }

                    _logger.LogError("XML is not valid");

                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteStringAsync($"XML does not conform with XSD!");

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected Error: {ex.Message}");

                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteStringAsync($"Unexpected Error: {ex.Message}");

                    return response;
                }


        }

        /// <summary>
        /// This is a callback method that will be called when the XML is not valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.WriteLine($"\tWarning: {args.Message}");
            else if (args.Severity == XmlSeverityType.Error)
            {
                Console.WriteLine($"\tError: {args.Message}");
                hasError = true;
            }
        }

        /// <summary>
        /// Returns the content of a file from a URL. It will download the file from the URL and return the content as a string.
        /// </summary>
        /// <param name="url">Valid URL to download the file from</param>
        /// <returns>String</returns>
        private static async Task<string> LoadFromUrl(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetStringAsync(url);
            }

        }


        /// <summary>
        /// Returns the content of a file from a file path. It will read the file from the file path and return the content as a string.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static async Task<string> LoadFromFile(string filePath)
        {
            return await Task.Run(() => File.ReadAllText(filePath));
        }

    }
}
