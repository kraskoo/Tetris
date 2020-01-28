using System;

[Serializable]
public class GameOptions
{
    public GameOptions(int level, bool advancedFigures, float soundVolume, float musicVolume)
    {
        this.Level = level;
        this.AdvancedFigures = advancedFigures;
        this.SoundVolume = soundVolume;
        this.MusicVolume = musicVolume;
    }

    public GameOptions() : this(1, true, 1f, 1f)
    {
    }

    public int Level { get; set; }

    public bool AdvancedFigures { get; set; }

    public float SoundVolume { get; set; }

    public float MusicVolume { get; set; }
}