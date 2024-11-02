using JetBrains.Annotations;

public struct LevelMessage
{
    public string LevelName { get; set; }
    [CanBeNull] public Level Level { get; set; }
    public LevelOperation Operation { get; set; }
    
    public LevelMessage(string levelName,LevelOperation operation, Level level = null)
    {
        LevelName = levelName;
        Operation = operation;
        Level = level;
    }
}