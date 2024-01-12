# XML Operations Manager in Azure Function App
This is a Function App Visual Studio solution in .NET 4.8 to validate XML against an XSD nested files. This is a workaround for XML Validate built-in functionality in Logic Apps Standard as it currently has a bug and does nto work with nested XSDs as the root cause issue in the .NET Core Xml library has a bug which does not exist in .NET Framework 4.x.

Currenlty this XML validation issue is affecting 2 products:
- Logic Apps Standard XML Validator
- Azure Integration Account Premium Schema Upload (It will fail to upload a nested XSD file to Schemas)

## Pre-reqs:
- Windows-based App Service Plan to host the Windows-based .NET 4.8 Azure Function App
- Azure Storage Account to store the nested XSDs in a Blob Contaner.

You can store the included XSDs (if they do not have any includes in them, which means it's a leaf XSD) in Azure Integration Account (Premium).

I recommend to have a multi-core App Service Plan or Elastic Premim Plan to host this Function App as if you hit this Function App alot then it might fail as it loads the XML and XSD files in memory and you need a large memory to run this Azure Function App effectivly.

I ran a load test for this function using Azure Load Testing service and ran around 25K request for 1 minute on EP3 instance and it all the requests successfully returned HTTP 200 OK.

