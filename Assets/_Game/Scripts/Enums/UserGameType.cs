namespace DLS.Enums
{
    [System.Flags]
    public enum UserGameType
    {
        General = 1 << 0, // 1
        FpsSinglePlayer = 1 << 1, // 2
        FpsMultiPlayer = 1 << 2, // 4
        RpgSinglePlayer = 1 << 3, // 8
        RpgMultiPlayer = 1 << 4 // 16
    }
}