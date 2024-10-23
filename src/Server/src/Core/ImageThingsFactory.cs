namespace Anyding;


public class CreateThingsRequest
{
    public List<CreateThingInput> Things { get; set; } = new List<CreateThingInput>();

    public List<ThingConnection> Connections { get; set; } = new List<ThingConnection>();
}


public class CreateThingInput
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime Created { get; set; }

    public ThingSource Source { get; set; }

    public string ClassName { get; set; }

    public ThingType Type { get; set; }
    public string Details { get; set; }

    public List<ThingInputData> Data { get; set; } = new List<ThingInputData>();
}

public class ThingInputData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }

    public Func<Stream> LoadData { get; set; }
    public string Type { get; set; }
}

