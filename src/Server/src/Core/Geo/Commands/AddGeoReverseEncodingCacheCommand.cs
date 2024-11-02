using Anyding.Data;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Anyding.Geo.Commands;

public record AddGeoReverseEncodingCacheCommand(GeoReverseEncodingCache Item) : IRequest;

public class AddGeoReverseEncodingCacheCommandHandler(
    IAnydingDbContext dbContext,
    ILogger<AddGeoReverseEncodingCacheCommandHandler> logger)
    : IRequestHandler<AddGeoReverseEncodingCacheCommand>
{
    public async Task Handle(
        AddGeoReverseEncodingCacheCommand request,
        CancellationToken cancellationToken)
    {
        await dbContext.GeoReverseEncodings.AddAsync(request.Item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


