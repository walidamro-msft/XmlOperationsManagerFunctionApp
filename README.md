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

## Code Usage Warning:
This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment. THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.

This sample script is not supported under any Microsoft standard support program or service. The sample script is provided AS IS without warranty of any kind. Microsoft further disclaims all implied warranties including, without limitation, any implied warranties of merchantability or of fitness for a particular purpose. The entire risk arising out of the use or performance of the sample scripts and documentation remains with you. In no event shall Microsoft, its authors, or anyone else involved in the creation, production, or delivery of the scripts be liable for any damages whatsoever (including, without limitation, damages for loss of business profits, business 
interruption, loss of business information, or other pecuniary loss) arising out of the use of or inability to use the sample scripts or documentation, even if Microsoft has been advised of the possibility of such damages.
