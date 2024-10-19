namespace Anyding.Search;

public class FaceIndex
{
    public string Id { get; set; }
    public string PersonId { get; set; }
    public string MediaId { get; set; }
    public int AgeInMonth { get; set; }
    public double[] Encoding { get; set; }
    public string Name { get; set; }
    public string? State { get; set; }
    public string RecognitionType { get; set; }
}
