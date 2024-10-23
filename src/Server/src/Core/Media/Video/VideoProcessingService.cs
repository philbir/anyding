using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using MetadataEx = MetadataExtractor;

namespace Anyding.Media;

public class VideoProcessingService(ILogger<VideoProcessingService> logger) : IVideoProcessingService
{
    private const string QuickTimeDateFormat = "ddd MMM dd HH:mm:ss yyyy";

    public async Task<ExtractVideoDataResult> ExtractVideoDataAsync(
        string filename,
        CancellationToken cancellationToken)
    {
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);
        IVideoStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault();

        var result = new ExtractVideoDataResult { Metadata = new VideoMetadata() };

        if (videoStream != null)
        {
            result.Metadata.Bitrate = videoStream.Bitrate;
            result.Metadata.PixelFormat = videoStream.PixelFormat;
            result.Metadata.FrameRate = Math.Round(videoStream.Framerate);
            result.Metadata.Codec = videoStream.Codec;
            result.Metadata.Duration = videoStream.Duration;
        }

        result.Metadata.Dimension = new MediaDimension(videoStream.Height, videoStream.Width);
        var tmpFile = Path.Combine(new FileInfo(filename).Directory.FullName, $"VideoImage.png");
        videoStream.SetCodec(VideoCodec.png);

        try
        {
            IConversionResult cr = await FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .ExtractNthFrame(3, (n) => tmpFile)
                .Start(cancellationToken);

            result.Image = await File.ReadAllBytesAsync(tmpFile, cancellationToken);
            File.Delete(tmpFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        IReadOnlyList<MetadataEx.Directory>? meta = MetadataEx.ImageMetadataReader
            .ReadMetadata(filename);

        result.Metadata.DateTaken = GetDateTaken(meta);
        result.Metadata.GeoLocation = await GetGpsLocation(meta, cancellationToken);
        result.Metadata.Camera = GetCamera(meta);

        return result;
    }

    private CameraInfo? GetCamera(IReadOnlyList<MetadataEx.Directory> meta)
    {
        var make = GetMetadataValue(meta, "QuickTime Metadata Header/Make");
        var model = GetMetadataValue(meta, "QuickTime Metadata Header/Model");
        var software = GetMetadataValue(meta, "QuickTime Metadata Header/Software");

        if (make != null && model != null)
        {
            return new CameraInfo { Make = make, Model = model, Software = software };
        }

        return null;
    }

    public async Task<string> GeneratePreviewGifAsync(
        string filename,
        string? outfile,
        CancellationToken cancellationToken)
    {
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);

        IVideoStream video = mediaInfo.VideoStreams.FirstOrDefault();

        var frames = (video.Duration.TotalSeconds == 0 ? 1 : video.Duration.TotalSeconds) * video.Framerate;

        outfile ??= Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.gif");

        var totalFrames = 50.0;

        var rate = Math.Ceiling(frames / totalFrames / video.Duration.TotalSeconds);
        if (rate > video.Duration.TotalSeconds)
        {
            rate = video.Duration.TotalSeconds;
        }

        video
            .SetFramerate(rate)
            .SetCodec(VideoCodec.gif)
            .ChangeSpeed(2)
            .SetSize(VideoSize.Hd480);

        IConversion? conversion = FFmpeg.Conversions.New()
            .AddStream(video)
            .SetOutput(outfile);

        await conversion.Start(cancellationToken);



        return outfile;
    }

    public async Task<string> ConvertToWebMAsync(
        string filename,
        string? outfile,
        CancellationToken cancellationToken)
    {
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);

        IStream video = mediaInfo.VideoStreams.FirstOrDefault()
            .SetCodec(VideoCodec.vp8)
            .SetSize(VideoSize.Hd1080);

        IAudioStream audio = mediaInfo.AudioStreams.FirstOrDefault()
            .SetCodec(AudioCodec.libvorbis);

        outfile ??= Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.webm");

        IConversion? conversion = FFmpeg.Conversions.New()
            .AddStream(video, audio)
            .SetOutput(outfile);

        var result = await conversion.Start(cancellationToken);

        logger.LogInformation("Converted {Filename} to WebM in {Duration}. cmd: {FFmpeg}",
            filename,
            result.Arguments,
            result.Duration);

        return outfile;
    }

    public async Task<string> ConvertTo720Async(
        string filename,
        string? outfile,
        CancellationToken cancellationToken)
    {
        IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(filename, cancellationToken);

        IStream video = mediaInfo.VideoStreams.FirstOrDefault()
            ?.SetCodec(VideoCodec.h264)
            .SetSize(VideoSize.Hd720);

        IAudioStream audio = mediaInfo.AudioStreams.FirstOrDefault()
            ?.SetCodec(AudioCodec.aac);

        outfile ??= Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");

        IConversion? conversion = FFmpeg.Conversions.New()
            .AddStream(video, audio)
            .SetOutput(outfile);

        var result = await conversion.Start(cancellationToken);

        logger.LogInformation("Converted {Filename} to 720p in {Duration}. cmd: {FFmpeg}",
            filename,
            result.Arguments,
            result.Duration);

        return outfile;
    }

    private async Task<GeoLocation?> GetGpsLocation(
        IReadOnlyList<MetadataEx.Directory> meta,
        CancellationToken cancellationToken)
    {
        var geo = GetMetadataValue(meta, "QuickTime Metadata Header/GPS Location");

        if (geo != null)
        {
            var regex = new Regex(@"(\+|\-)(\d{2,}\.\d{2,})");
            MatchCollection? matches = regex.Matches(geo);

            var coordinates = new List<double>();

            foreach (string? value in matches.Select(x => x?.ToString()))
            {
                double coordValue;
                if (double.TryParse(value, out coordValue))
                {
                    if (coordValue > 0)
                    {
                        coordinates.Add(coordValue);
                    }
                }
            }

            if (coordinates.Count > 1)
            {
                var gps = new GeoLocation() { Latitude = coordinates[0], Longitude = coordinates[1] };

                if (coordinates.Count > 2)
                {
                    gps.Altitude = (int)coordinates[2];
                }

                return gps;
            }
        }

        return null;
    }

    private DateTime? GetDateTaken(IReadOnlyList<MetadataEx.Directory> meta)
    {
        var dateTime = GetMetadataValue(meta, "QuickTime Movie Header/Created");

        if (dateTime != null)
        {
            if (DateTime.TryParseExact(
                    dateTime,
                    QuickTimeDateFormat,
                    null,
                    DateTimeStyles.None,
                    out DateTime parsed))
            {
                return parsed;
            }
        }

        return null;
    }

    private string? GetMetadataValue(
        IReadOnlyList<MetadataEx.Directory>? meta, string path)
    {
        if (meta == null || string.IsNullOrEmpty(path))
        {
            return null;
        }

        var frags = path.Split('/');
        if (frags.Length < 2)
        {
            return null;
        }

        var directoryName = frags[0];
        var tagName = frags[1];

        var tagDescription = meta
            .Where(d => d.Name == directoryName)
            .SelectMany(d => d.Tags)
            .FirstOrDefault(t => t.Name == tagName)?
            .Description;

        return tagDescription;
    }
}
