using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class ServiceBusSenderService : IServiceBusSenderService
{
    private readonly IAppLogger<ServiceBusSenderService> logger;
    private readonly ServiceBusSender sbSender;

    public ServiceBusSenderService(IAppLogger<ServiceBusSenderService> appLogger, ServiceBusSender sbSender)
    {
        this.logger = appLogger;
        this.sbSender = sbSender;
    }

    public async Task SendMessageAsync(string id, object order, CancellationToken cancellationToken)
    {
        var messageBody = JsonSerializer.Serialize(order);
        var message = new ServiceBusMessage(messageBody);

        await sbSender.SendMessageAsync(message, cancellationToken);

        logger.LogInformation("Sent message to Service Bus with id: {messageId}", id);
    }
}
