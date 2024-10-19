namespace Anyding.Media;

public interface IVideoProcessingService
{
    Task<ExtractVideoDataResult> ExtractVideoDataAsync(
        string filename,
        CancellationToken cancellationToken);

    Task<string> GeneratePreviewGifAsync(
        string filename,
        string? outfile,
        CancellationToken cancellationToken);

    Task<string> ConvertToWebMAsync(
        string filename,
        string? outfile,
        CancellationToken cancellationToken);

    Task<string> ConvertTo720Async(
        string filename,
        string? outfile,
        CancellationToken cancellationToken);
}
