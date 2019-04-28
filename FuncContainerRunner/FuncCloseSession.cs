using System;
using System.Threading.Tasks;
using Automation.Container;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;


namespace FuncAppContainerRunner
{
    public static class FuncCloseSession
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            RunSample(Guid.NewGuid().ToString()).GetAwaiter().GetResult();

        }

        private static async Task RunSample(string Guid)
        {
            var azureSubscriptionConfiguration = ConfigurationFactory.CreateConfigWithActiveDirectoryAppAuth(
                azureSubscriptionId: "aef64d66-5785-4f41-83d6-d598c720f9dc",
                resourceGroup: "gab2019vrrg001",
                clientId: "c152b5a0-2efb-40e2-8104-8dcd9cd89c82",
                clientSecret: "P2JHKFe72WjUMpyu2w1n7bSdmMhn7cxjeDo7QtZW9IQ=",
                tenantId: "6065539e-1467-456e-929c-c87c64b86dd3",
                imageRegistryServer: "gab2019vracr001.azurecr.io",
                imageRegistryUsername: "gab2019vracr001",
                imageRegistryPassword: "GYR4=j56W4Td1HRJuXfDFPWTeJislwFc"
            );

            var containerCreationConfiguration = new ContainerCreationConfiguration()
            {
                //For some reason, container name can't be in Pascal case. Kebab case works.
                ContainerName = Guid,
                CpuCore = 2,
                MemoryInGB = 4,
                ImageName = "gab2019vracr001.azurecr.io/demo.vr.2019.gab.test1:latest",
                Port = 12345,
                Location = "west europe",
                OS = ContainerCreationConfiguration.OsType.Linux
            };

            try
            {
                var resourceAccess = new AciResourceAccess(azureSubscriptionConfiguration);
                var container = resourceAccess.CreateContainer(containerCreationConfiguration).GetAwaiter().GetResult();
                Console.WriteLine("Container created successfully on ip "
                                  + container.properties.ipAddress.ip);

                //Get container object
                await Task.Delay(TimeSpan.FromSeconds(10));//Wait for image to finish pulling

                //container = await resourceAccess.GetContainer("my-container");
                //Console.WriteLine("Container status now is: " + container.properties.containers.First().properties.instanceView.currentState.state); //'Waiting' because it's still pulling image

                //Alternatively, we can use 'GetContainerGroupStatus' to get status
                var containerStatus = await resourceAccess.GetContainerGroupStatus(Guid);
                Console.WriteLine("Container status now is: " + containerStatus);

                bool IsRunning = true;
                Console.WriteLine("Container goes into Cycle with this status: " + containerStatus);
                while (IsRunning)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));//Wait for image to finish pulling
                    containerStatus = await resourceAccess.GetContainerGroupStatus(Guid);
                    Console.WriteLine("Container status now is: " + containerStatus);//ContainerStatus.RUNNING

                    IsRunning = !containerStatus.Equals((ContainerStatus.TERMINATED));
                }
                //Delete container
                await resourceAccess.DeleteContainer(Guid);
                containerStatus = await resourceAccess.GetContainerGroupStatus(Guid);
                Console.WriteLine("Container status now is: " + containerStatus);//ContainerStatus.DELETED

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e}");
            }
        }



    }
}
