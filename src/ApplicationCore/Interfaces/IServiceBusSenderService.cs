using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IServiceBusSenderService
{
    Task SendMessageAsync(string id, object order, CancellationToken cancellationToken);
}
