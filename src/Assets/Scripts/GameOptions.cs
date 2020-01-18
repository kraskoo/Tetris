using System;

[Serializable]
public class GameOptions
{
    public GameOptions(int level, float soundVolume, float musicVolume)
    {
        this.Level = level;
        this.SoundVolume = soundVolume;
        this.MusicVolume = musicVolume;
    }

    public GameOptions() : this(1, 1f, 1f)
    {
    }

    public int Level { get; set; }

    public float SoundVolume { get; set; }

    public float MusicVolume { get; set; }
}