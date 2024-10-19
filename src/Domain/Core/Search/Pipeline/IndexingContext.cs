using Anyding.Data;

namespace Anyding.Search;

public class IndexingContext
{
    public CancellationToken Cancelled { get; set; }
    public AnydingDbContext Database { get; set; }
    public IndexingRequest Request { get; set; }
    internal Dictionary<string, MediaIndex> Media { get; } = new();
    internal Dictionary<string, PersonIndex> Persons { get; } = new();
    internal Dictionary<string, FaceIndex> Faces { get; } = new();

    public void AddMedia(MediaIndex media)
    {
        Media[media.Id] = media;
    }

    public void AddPerson(PersonIndex person)
    {
        Persons[person.Id] = person;
    }

    public void AddFace(FaceIndex face)
    {
        Faces[face.Id] = face;
    }
}
