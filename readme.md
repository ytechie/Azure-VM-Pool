# Azure VM Pool

This is a proof of concept for an application that pre-provisions virtual machines in Azure using the .NET SDK. It's meant as a sample, and nothing more. There are hard-coded constants all over the place.

There are a couple of things to note:

* There is obviously a cost to keeping extra VMs around
* It may be hard to predict when new machines will be needed
* To change the cloud service, location, or virtual network of a VM, it will need to be deallocated, cloned, and recreated which will defeat the purpose unfortunately.

### Using

Before this app will work, you need to open `Program.cs` and enter your subscription ID. You'll also need to create a file called `AzureCert.txt` in the root folder of the application. Both of these can be found in your [publish profile XML](https://manage.windowsazure.com/publishsettings/index?client=vsserverexplorer&schemaversion=2.0).

You'll also want to look through the code and change various hard-coded paramters.

### Other thoughts:

* Any value to creating a simplified fluent .NET interface to Azure that uses the SDK and looks more like the PowerShell signatures?

# License

Microsoft Developer & Platform Evangelism

Copyright (c) Microsoft Corporation. All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

The example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious. No association with any real company, organization, product, domain name, email address, logo, person, places, or events is intended or should be inferred.
