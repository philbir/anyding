using Anyding.Media;
using Anyding.Media.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anyding;

public static class MediaServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddMediaPipelines(this IAnydingServerBuilder builder)
    {
        builder.Services.AddImagePipeline();
        builder.Services.AddVideoPipeline();

        builder.Services.AddMediaServices();

        builder.AddMagicFaceDetection();
        builder.AddFFmpeg();

        return builder;
    }

    public static IAnydingServerBuilder AddMagicFaceDetection(this IAnydingServerBuilder builder)
    {
        builder.Services.AddOptions<FaceDetectionOptions>()
            .Bind(builder.Configuration.GetSection("FaceDetection"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddHttpClient("Face", (provider, client) =>
        {
            FaceDetectionOptions options = provider.GetRequiredService<IOptions<FaceDetectionOptions>>().Value;

            client.BaseAddress = new Uri(options.ApiUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Anyding");
        });

        builder.Services.AddSingleton<IFaceDetectionService, MagicFaceDetectionService>();

        return builder;
    }

    private static IAnydingServerBuilder AddFFmpeg(this IAnydingServerBuilder builder)
    {
        builder.Services.AddSingleton<IFFmpegInitializer, FFmpegInitializer>();
        builder.Services.AddOptions<FFmpegOption>()
            .Bind(builder.Configuration.GetSection("Video:FFmpeg"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    private static IServiceCollection AddMediaServices(this IServiceCollection services)
    {
        services.AddSingleton<IImageMetadataExtractor, ImageMetadataExtractor>();
        services.AddSingleton<IImagePreviewService, ImagePreviewService>();
        services.AddSingleton<IVideoProcessingService, VideoProcessingService>();
        services.AddSingleton<IImageCropService, ImageCropService>();

        return services;
    }

    private static IServiceCollection AddImagePipeline(this IServiceCollection services)
    {
        services.AddScoped<IWorkspacePipeline>(p => new ImagePipeline(p));

        services.AddScoped<IWorkspaceTask<ImageWorkspace>, AutoOrientTask>();
        services.AddScoped<IWorkspaceTask<ImageWorkspace>, ConvertToJpgTask>();
        services.AddScoped<IWorkspaceTask<ImageWorkspace>, ExtractImageMetadataTask>();
        services.AddScoped<IWorkspaceTask<ImageWorkspace>, CreatePreviewImagesTask>();
        services.AddScoped<IWorkspaceTask<ImageWorkspace>, DetectFacesTask>();
        services.AddScoped<IWorkspaceTask<ImageWorkspace>, CropFaceImagesTask>();
        services.AddScoped<IWorkspaceTask<ImageWorkspace>, ReverseGeoCodeImageTask>();

        services.AddScoped<IWorkspaceProvider, ImageWorkspaceProvider>();

        return services;
    }

    private static IServiceCollection AddVideoPipeline(this IServiceCollection services)
    {
        services.AddScoped<IWorkspacePipeline>(p => new VideoPipeline(p));
        services.AddScoped<IWorkspaceTask<VideoWorkspace>, ExtractVideoDataTask>();
        services.AddScoped<IWorkspaceTask<VideoWorkspace>, CreateVideoPreviewTask>();
        services.AddScoped<IWorkspaceTask<VideoWorkspace>, CreatePreviewImagesFromVideoTask>();
        services.AddScoped<IWorkspaceTask<VideoWorkspace>, ReverseGeoCodeVideoTask>();

        services.AddScoped<IWorkspaceProvider, VideoWorkspaceProvider>();
        return services;
    }
}


public class FaceDetectionOptions
{
    public string? ApiUrl { get; set; }
}
