namespace Quaver.API.Enums
{
    /// <summary>
    ///     Indicates the type of a hit object
    /// </summary>
    public enum HitObjectType
    {
        Normal, // Regular hit object. It should be hit normally.
        Mine // A mine object. It should not be hit, and hitting it will result in a miss.
    }
}