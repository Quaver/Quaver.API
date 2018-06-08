namespace Quaver.API.Maps.Processors.Scoring.Data
{
    public enum HitStatType
    {
        Hit, // Input was involved in this HitStatType. (User hit, released, or early missed)
        Miss, // User completely missed the note.
    }
}