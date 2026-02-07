namespace Atoms.Core;

public static class Constants
{
    public const int Rows = 6;
    public const int Columns = 10;
    public const int MinPlayers = 2;
    public const int MaxPlayers = 4;

    public static class StorageKeys
    {
        public const string LocalStorageId = nameof(LocalStorageId);
        public const string UserName = nameof(UserName);
        public const string ColourScheme = nameof(ColourScheme);
        public const string AtomShape = nameof(AtomShape);
        public const string Sound = nameof(Sound);
        public const string GameMenuOptions = nameof(GameMenuOptions);
    }

    public static class Cookies
    {
        public const string VisitorId = nameof(VisitorId);
    }
}
