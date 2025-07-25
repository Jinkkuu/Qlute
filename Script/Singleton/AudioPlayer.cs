using Godot;
using System;
using NVorbis;

public partial class AudioPlayer : AudioStreamPlayer
{
    public static AudioStreamPlayer Instance;
    private bool _isPlaying = false;
    public static bool _isogg = false;
    private float _seekPosition = 0.0f;

    public override void _Ready()
    {
        Instance = this;
        Instance.Bus = "Master";
    }
    public override void _Process(double delta)
    {
        if (SettingsOperator.loopaudio) AudioLoop();
        SettingsOperator.Gameplaycfg.Time = GetPlaybackPosition();
        SettingsOperator.Gameplaycfg.TimeTotal = (float)(Stream?.GetLength() ?? 0);
    }

    public static AudioStreamMP3 LoadMP3(string path)
    {
        using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        var sound = new AudioStreamMP3();
        _isogg = false;
        sound.Data = file.GetBuffer((long)file.GetLength());
        return sound;
    }
	public void AudioLoop(){
		if (SettingsOperator.Gameplaycfg.TimeTotal - GetPlaybackPosition() < 0.1 && SettingsOperator.loopaudio)
		{
			AudioPlayer.Instance.Play();
		}
	}
	 public static AudioStreamOggVorbis LoadOGG(string path)
	 {
        return AudioStreamOggVorbis.LoadFromFile(path);
	 }
    public static AudioStreamWav LoadWAV(string path)
    {
        _isogg = false;
        using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        var sound = new AudioStreamWav();
        sound.Data = file.GetBuffer((long)file.GetLength());
        return sound;
    }
}
