using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Handlers;

public class OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IEmailSender emailSender) : INotificationHandler<OrderCreatedEvent>
{
    private readonly string functionUrl = "https://eshop-functions-abevcza5ahhxedf4.centralindia-01.azurewebsites.net/api/OrderItemsReserver";
    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Order #{orderId} placed: ", domainEvent.Order.Id);

        await emailSender.SendEmailAsync("to@test.com",
                                         "Order Created",
                                         $"Order with id {domainEvent.Order.Id} was created.");

        using var httpClient = new HttpClient();
        var order = new
        {
            Address = domainEvent.Order.ShipToAddress,
            ListOfItems = domainEvent.Order.OrderItems,
            TotalPrice = domainEvent.Order.Total()
        };
        var response = await httpClient.PostAsJsonAsync(functionUrl, order, cancellationToken);
        logger.LogInformation("Notified external system about order #{orderId}, response status: {statusCode}", domainEvent.Order.Id, response.StatusCode);

    }
}
