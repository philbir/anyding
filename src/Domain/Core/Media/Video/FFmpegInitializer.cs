using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Anyding.Media;

public class FFmpegInitializer : IFFmpegInitializer
{
    private readonly IOptions<FFmpegOption> _options;
    private readonly ILogger<FFmpegInitializer> _logger;

    public FFmpegInitializer(IOptions<FFmpegOption> options, ILogger<FFmpegInitializer> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task Initialize()
    {
        var location = GetDirectory();
        _logger.LogInformation("Initialize FFmpeg with location: {Location}", location);

        if (_options.Value.AutoDownload)
        {
            _logger.LogInformation("FFmpeg GetLatestVersion");
            await FFmpegDownloader.GetLatestVersion(
                FFmpegVersion.Official,
                location);
        }

        FFmpeg.SetExecutablesPath(location);
    }

    private string GetDirectory()
    {
        if (_options.Value.Location == null)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        }
        else
        {
            return _options.Value.Location;
        }
    }
}

public class FFmpegOption
{
    public string? Location { get; set; }

    public bool AutoDownload { get; set; } = true;
}

public interface IFFmpegInitializer
{
    Task Initialize();
}
