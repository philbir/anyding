using System.Net.Http.Json;
using System.Text.Json;

namespace Anyding.Media;

public class MagicFaceDetectionService(IHttpClientFactory httpClientFactory) : IFaceDetectionService
{
    public async Task<IReadOnlyList<FaceDetectionResult>> DetectFacesAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        HttpClient client = httpClientFactory.CreateClient("Face");
        var request = new HttpRequestMessage(HttpMethod.Post, "face/detect");
        var multipart = new MultipartFormDataContent();
        multipart.Add(new StreamContent(stream), "file", "detectme.jpg");
        request.Content = multipart;

        HttpResponseMessage res = await client.SendAsync(request, cancellationToken);

        if (res.IsSuccessStatusCode)
        {
            List<FaceDetectionApiResponse>? faces = await res.Content
                .ReadFromJsonAsync<List<FaceDetectionApiResponse>>(
                    JsonSettings, cancellationToken: cancellationToken);

            return faces?.Select(x => new FaceDetectionResult
            {
                Id = x.Id,
                Box = ImageRactangle.FromBoundingBox(x.Box),
                Encoding = x.Encoding
            }).ToList();
        }
        else
        {
            string text = await res.Content.ReadAsStringAsync();
            throw new ApplicationException(text);
        }
    }

    private JsonSerializerOptions JsonSettings => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

class FaceDetectionApiResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ImageBoundingBox Box { get; set; }
    public double[]? Encoding { get; set; }
}

