using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Handlers;

public class OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IEmailSender emailSender, IServiceBusSenderService serviceBusSender) : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Order #{orderId} placed: ", domainEvent.Order.Id);

        await emailSender.SendEmailAsync("to@test.com",
                                         "Order Created",
                                         $"Order with id {domainEvent.Order.Id} was created.");

        using var httpClient = new HttpClient();
        var order = new
        {
            Id = domainEvent.Order.Id,
            Address = domainEvent.Order.ShipToAddress,
            ListOfItems = domainEvent.Order.OrderItems,
            TotalPrice = domainEvent.Order.Total()
        };
        await serviceBusSender.SendMessageAsync(domainEvent.Order.Id.ToString(), order, cancellationToken);
        logger.LogInformation("Notified external system about order #{orderId}", domainEvent.Order.Id);

    }
}
