public static class OptionsStreamer
{
    private static readonly Streamer<GameOptions> Streamer = new Streamer<GameOptions>("options.dat");

    public static void SaveOptions(GameOptions options) => Streamer.SaveOptions(options);

    public static GameOptions LoadOptions() => Streamer.LoadOptions();

    public static bool IsFileExist() => Streamer.IsFileExist();
}