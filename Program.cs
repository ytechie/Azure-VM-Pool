using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Management.Models;

namespace AzureVmPool
{
    class Program
    {
        //References:
        //http://www.bradygaster.com/post/getting-started-with-the-windows-azure-management-libraries

        //Get the subid from https://manage.windowsazure.com/publishsettings/index?client=vsserverexplorer&schemaversion=2.0
        private const string SubscriptionId = "8e95e0bb-d7cc-4454-9443-75ca862d34c1";

        private const string Location = LocationNames.NorthCentralUS;

        static void Main(string[] args)
        {
            Task.Run(() => ShowMenu()).Wait();
            Console.Read();
        }

        private static string LoadCert()
        {
            var certStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                typeof(Program), "AzureCert.txt");

            if(certStream == null)
                throw new Exception("This program requires an embedded file called AzureCert.txt in the root folder, and it must contain the base64 management certificate.");

            using (var sr = new StreamReader(certStream))
            {
                return sr.ReadToEnd();
            }
        }

        private static async void ShowMenu()
        {
            var cert = LoadCert();
            var azure = new AzureOperations(SubscriptionId, cert);
            var serviceName = Guid.NewGuid().ToString();

            char key;
            do
            {
                Console.WriteLine("Azure Operations:");
                Console.WriteLine("-----------------");
                Console.WriteLine("1. Create Cloud Service");
                Console.WriteLine("2. Populate Pool");
                Console.WriteLine("3. Get VM");
                Console.WriteLine();
                Console.WriteLine("0. Exit");

                key = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (key == '1')
                {
                    await azure.CreateCloudServiceIfNotExists(serviceName, Location);
                }
                if (key == '2')
                {
                    await azure.VerifyPool(serviceName);   
                }
                if (key == '3')
                {
                    //TODO
                }


            } while (key != '0');
        }
    }
}
