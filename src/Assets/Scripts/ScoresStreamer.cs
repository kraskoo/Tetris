public class ScoresStreamer
{
    private static readonly Streamer<GameScores> Streamer = new Streamer<GameScores>("scores.dat");

    public static void SaveOptions(GameScores options) => Streamer.SaveOptions(options);

    public static GameScores LoadOptions() => Streamer.LoadOptions();

    public static bool IsFileExist() => Streamer.IsFileExist();
}