using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;

namespace AzureVmPool
{
    public class AzureOperations
    {
        private readonly ComputeManagementClient _client;
        private const int MaxPoolSize = 2; //Keeping it small for speed
        private readonly List<string> _pooledMachineNames = new List<string>(); 

        public AzureOperations(string subscriptionId, string subscriptionCert)
        {
            var creds = GetCredentials(subscriptionId, subscriptionCert);
            _client = CloudContext.Clients.CreateComputeManagementClient(creds);
        }

        public SubscriptionCloudCredentials GetCredentials(string subscriptionId, string subscriptionCert)
        {
            var cert = new X509Certificate2(Convert.FromBase64String(subscriptionCert));
            return new CertificateCloudCredentials(subscriptionId, cert);
        }

        public async Task CreateCloudServiceIfNotExists(string serviceName, string location)
        {
            var hostedServices = await _client.HostedServices.ListAsync();

            if (hostedServices.SingleOrDefault(x => x.ServiceName == serviceName) == null)
            {
                //var locations = await client.VirtualMachines.g
                var config = new HostedServiceCreateParameters
                {
                    ServiceName = serviceName,
                    Location = location
                };
                Console.WriteLine("Creating cloud service...");
                var response = await _client.HostedServices.CreateAsync(config);
                if (response.StatusCode != HttpStatusCode.Created)
                    throw new Exception("Service creation failed with status " + response.StatusCode);
                Console.WriteLine("Cloud service created");
            }
            else
            {
                Console.WriteLine("Cloud service already exists.");
            }
        }

        public async Task VerifyPool(string serviceName)
        {
            for(var i = _pooledMachineNames.Count; i < MaxPoolSize; i++)
            {
                await CreateVm(serviceName, "PoolVM" + i + 1);
            }
        }

        private async Task CreateVm(string serviceName, string computerName)
        {
            var mediaLocation = new Uri("https://jyvhdstorage.blob.core.windows.net/vhds/disk3vhd");
            const string deploymentName = "deployment";

            DeploymentGetResponse deploymentResponse;
            try
            {
                deploymentResponse = await _client.Deployments.GetBySlotAsync(serviceName, DeploymentSlot.Production);
            }
            catch (CloudException ex)
            {
                deploymentResponse = null;
            }

            var vmCreated = false;
            if (deploymentResponse == null)
            {
                var createParams = new VirtualMachineCreateDeploymentParameters
                {
                    DeploymentSlot = DeploymentSlot.Production,
                    Name = deploymentName,
                    Label = "testlabel",


                    Roles = new List<Role>
                    {
                        new Role
                        {
                            RoleSize = "Small",
                            RoleName = "role-" + computerName,
                            ProvisionGuestAgent = true,


                            ConfigurationSets = new List<ConfigurationSet>
                            {
                                new ConfigurationSet
                                {
                                    ComputerName = "cs-" + computerName,
                                    AdminUserName = "admin123",
                                    AdminPassword = "#Fa3aadsfa#R2", //Just some random creds
                                    ConfigurationSetType = "WindowsProvisioningConfiguration"
                                }
                            },
                            RoleType = "PersistentVMRole",
                            OSVirtualHardDisk = new OSVirtualHardDisk
                            {

                                SourceImageName =
                                    "a699494373c04fc0bc8f2bb1389d6106__Windows-Server-2012-R2-201409.01-en.us-127GB.vhd",
                                MediaLink = mediaLocation
                            }
                        }
                    }
                };
                Console.WriteLine("Creating deployment. This may take some time...");
                await _client.VirtualMachines.CreateDeploymentAsync(serviceName, createParams);
                Console.WriteLine("Deployment Created.");
                vmCreated = true;
            }

            if (!vmCreated)
            {
                var vmParameters = new VirtualMachineCreateParameters
                {
                    RoleSize = "Small",
                    RoleName = "role-" + computerName,
                    ProvisionGuestAgent = true,


                    ConfigurationSets = new List<ConfigurationSet>
                    {
                        new ConfigurationSet
                        {
                            ComputerName = "cs-" + computerName,
                            AdminUserName = "admin123",
                            AdminPassword = "#Fa3aadsfa#R2",
                            ConfigurationSetType = "WindowsProvisioningConfiguration"
                        }
                    },
                    OSVirtualHardDisk = new OSVirtualHardDisk
                    {

                        SourceImageName =
                            "a699494373c04fc0bc8f2bb1389d6106__Windows-Server-2012-R2-201409.01-en.us-127GB.vhd",
                        MediaLink = mediaLocation
                    }
                };
                Console.WriteLine("Creating VM " + computerName);
                await _client.VirtualMachines.CreateAsync(serviceName, deploymentName, vmParameters);
            }


            //var images = await client.VirtualMachineVMImages.ListAsync();
        }
    }
}
