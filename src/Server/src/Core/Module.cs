using GreenDonut;

[assembly: DataLoaderDefaults(
    ServiceScope = DataLoaderServiceScope.DataLoaderScope,
    AccessModifier = DataLoaderAccessModifier.PublicInterface)]

[assembly: DataLoaderModule("AnydingApplication")]
