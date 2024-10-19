namespace Anyding;

public static class SampleData
{

    public static class Classes
    {
        public static ThingClass ImageClass => new()
        {
            Id = Guid.Parse("a4828c9d-cf1e-4b53-90cd-89431359988a"), Name = "Image", Type = ThingType.Media
        };

        public static ThingClass VideoClass =>
            new()
            {
                Id = Guid.Parse("bae242c1-0459-4da3-988d-ebb8a13ec205"), Name = "Video", Type = ThingType.Media
            };

        public static ThingClass PhoneClass =>
            new()
            {
                Id = Guid.Parse("9133fdd5-df0f-4ef2-8a5f-0655b5d4ac3d"), Name = "Phone", Type = ThingType.Device
            };
        public static ThingClass CameraClass =>
            new()
            {
                Id = Guid.Parse("f22f7e8e-8864-41b6-94e6-543f0386ec1c"), Name = "Camera", Type = ThingType.Device
            };

        public static IEnumerable<ThingClass> All => new List<ThingClass>() { ImageClass, VideoClass, PhoneClass };
    }

    public class Tags
    {
        public static TagDefinition FavoriteTag => new() { Id = Guid.Parse("04900209-b9a1-4453-8fcc-dc7bf37c12c0"), Name = "Favorite", Icon = "mdi-heart", Color = "#f01707"};
        public static TagDefinition AIImageTag => new() { Id = Guid.Parse("d074e310-cd33-4deb-b534-95f37adcf423"), Name = "AIImage", Icon = "mdi-robot-outline", Color = "#f01707"};
        public static TagDefinition AIImageCaption => new() { Id = Guid.Parse("25c70231-e6e6-4de7-9ae4-e96495bc969d"), Name = "AICaption", Icon = "mdi-image-outline", Color = "#f01707"};
        public static TagDefinition AIImageColor => new() { Id = Guid.Parse("d4cd5d46-5970-4597-a28f-9ae58bcfe74e"), Name = "AIColor", Icon = "mdi-palette", Color = "#f01707"};

        public static TagDefinition AIImageObject => new() { Id = Guid.Parse("a2af20b5-43b4-4496-a15f-c9ef6f014ac2"), Name = "AIObject", Icon = "mdi-vector-rectangle", Color = "#f01707"};
        public static IEnumerable<TagDefinition> All => new List<TagDefinition>() { FavoriteTag, AIImageTag, AIImageColor, AIImageCaption, AIImageObject };
    }
}
