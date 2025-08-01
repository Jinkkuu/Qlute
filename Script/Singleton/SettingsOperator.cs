using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;



public class DanceCounter {
        public int time {get;set;}
        public bool flash {get;set;}
}

public partial class SettingsOperator : Node
{
    public static string UpdatedRank = "#0";
    public static string Updatedpp = "0pp";
    public static string homedir = OS.GetUserDataDir().Replace("\\", "/");
	public static string tempdir => homedir + "/temp";
	public static string beatmapsdir => homedir + "/beatmaps";
    public static float ppbase = 0.035f;
	public static string downloadsdir => homedir + "/downloads";
	public static string replaydir => homedir + "/replays";
	public static string screenshotdir => homedir + "/screenshots";
	public static string skinsdir => homedir + "/skins";
    public static string settingsfile => homedir + "/settings.cfg";
    public static string Qlutedb => homedir + "/qlute.db";
    public static bool loopaudio = false;
    public static bool jukebox = false;
    public int backgrounddim {get;set;}
    public int MasterVol {get;set;}
    public int SampleVol {get;set;}
    public int scrollspeed {get;set;}
    public static bool SpectatorMode { get; set; } = false;
    public static float AllMiliSecondsFromBeatmap { get; set; }
    public static float MiliSecondsFromBeatmap {get;set;}
    public static int MiliSecondsFromBeatmapTimes {get;set;}
    public static List<BeatmapLegend> Beatmaps = new List<BeatmapLegend>();
	public static float AudioOffset { get; set; } = 0;
    public static int LeaderboardType = 1;
    public static void ResetRank()
    {
        UpdatedRank = "#0";
        Updatedpp = "0pp";
    }

    public void RefreshFPS()
    {
        var fps = int.TryParse(GetSetting("fpsmode").ToString(), out int fpsm) ? (int)fpsm : 1;
        if (fps == 0)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
        }
        else if (fps == 1)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            Engine.MaxFps = (int)DisplayServer.ScreenGetRefreshRate() * 2;
        }
        else if (fps == 2)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            Engine.MaxFps = (int)DisplayServer.ScreenGetRefreshRate() * 4;
        }
        else if (fps == 3)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            Engine.MaxFps = (int)DisplayServer.ScreenGetRefreshRate() * 8;
        }
        else if (fps == 4)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            Engine.MaxFps = 0; // Unlimited FPS
        }
    }
    public static Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>
    {
        { "scaled", false },
        { "windowmode", 0 },
        { "master", 80 },
        { "sample", 80 },
        { "backgrounddim", 70 },
        { "audiooffset", 0 },
        { "skin", null },
        { "username", null },
        { "password", null },
        { "stayloggedin", true },
        { "api", "https://qlute.pxki.us.to/" },
        { "client-id", null },
        { "client-secret", null },
        { "scrollspeed", (int)1346 }, // 11485 ms max
        { "fpsmode", 1 },
        { "showfps", false },
        { "leaderboardtype", 1 },
    };
    public Dictionary<string, object> Configurationbk {get; set;}


    public static Texture2D LoadImage(string path) // I am going to make this better and not lag the game when loading images
    {
        if (!FileAccess.FileExists(path))
        {
            Notify.Post("Image could not be loaded, because it doesn't exist!");
            return null;
        }

        try
        {
            using var image = Image.LoadFromFile(path);
            return ImageTexture.CreateFromImage(image);
        }
        catch (Exception)
        {
            Notify.Post("Failed to load image!");
            return null;
        }
    }
    public static int RndSongID(){
        int id = new Random().Next(0, Beatmaps.Count);
        return id;
    }
    public static bool CreateSelectingBeatmap { get; set; }
    public static bool MultiSelectingBeatmap { get; set; }
    public static bool Start_reloadLeaderboard {get; set; } = false;
    public static List<int> MarathonMapPaths { get; set; } = new List<int>();
    public static bool Marathon { get; set; } = false; // Marathon mode flag
    public static int MarathonID { get; set; } = -1; // ID of the current marathon song
    public static int PerfectJudge { get; set; } = 500; // Judge Perfect
    public static int GreatJudge { get; set; } = -1; // Judge Great
    public static int MehJudge { get; set; } = -1; // Judge Meh
    public void SelectSongID(int id)
    {
        if (Beatmaps.ElementAt(id) != null)
        {
            ApiOperator.LeaderboardStatus = 0; // Reset leaderboard status
            ApiOperator.LeaderboardList.Clear(); // Clear the leaderboard list
            if (!Marathon) MarathonMapPaths.Clear(); // Clear marathon map paths if not in marathon mode
            Start_reloadLeaderboard = true;
            var beatmap = Beatmaps[id];
            Sessioncfg["SongID"] = id;
            Sessioncfg["beatmapurl"] = beatmap.Rawurl;
            Sessioncfg["beatmaptitle"] = beatmap.Title;
            Sessioncfg["beatmapartist"] = beatmap.Artist;
            Sessioncfg["beatmapdiff"] = beatmap.Version;
            Sessioncfg["beatmapbpm"] = (int)beatmap.Bpm;
            Gameplaycfg.TimeTotalGame = beatmap.Timetotal * 0.001f;
            Sessioncfg["beatmapmapper"] = beatmap.Mapper;
            Sessioncfg["beatmapaccuracy"] = (int)beatmap.Accuracy;
            LevelRating = (int)Math.Round(beatmap.Levelrating);
            Sessioncfg["osubeatid"] = (int)beatmap.Osubeatid;
            Sessioncfg["osubeatidset"] = (int)beatmap.Osubeatidset;
            var Texture = LoadImage(beatmap.Path.ToString() + beatmap.Background.ToString());
            Sessioncfg["background"] = (Texture2D)Texture;
            Gameplaycfg.maxpp = beatmap.pp;
            string audioPath = beatmap.Path + "" + beatmap.Audio;
            if (System.IO.File.Exists(audioPath))
            {
                AudioStream filestream = null;
                if (audioPath.EndsWith(".mp3"))
                {
                    filestream = AudioPlayer.LoadMP3(audioPath);
                }
                else if (audioPath.EndsWith(".wav"))
                {
                    filestream = AudioPlayer.LoadWAV(audioPath);
                }
                else if (audioPath.EndsWith(".ogg"))
                {
                    filestream = AudioPlayer.LoadOGG(audioPath);
                }
                AudioPlayer.Instance.Stream = filestream;
                AudioPlayer.Instance.Play();
            }
            else
            {
                GD.PrintErr("Audio file not found: " + audioPath);
            }
        }
        else { GD.PrintErr("Can't select a song that don't exist :/"); }
    }
    public static double Get_ppvalue(int max, int great, int meh, int bad, float multiplier = 1,int combo = 0){
        //bad = Math.Max(1,bad);
        var ppvalue = 0.0;
        ppvalue = max * ppbase;
        ppvalue -= ppbase/2 * great;
        ppvalue -= ppbase/3 * meh;
        ppvalue -= ppbase * bad;
        ppvalue += combo * ppbase;
        ppvalue *=multiplier;
        return Math.Max(0,ppvalue);
        }
    public static void Parse_Beatmapfile(string filename, int SetID = 0){
        using var file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
        var text = file.GetAsText();
        var lines = text.Split("\n");
        string songtitle = "";
        string artist = "";
        string version = "";
        float timetotal = 0;
        int bpm = 0;
        int osubeatid = 0;
        int accuracy = 0;
        int osubeatidset = 0;
        int qlbeatid = 0;
        int qlbeatidset = 0;
        double ppvalue = 0;
        string mapper = "";
        float levelrating = 0;
        int notetime = 0;
        string background = "";
        string audio = "";
        string rawurl = filename;
        var hitob = 0;
        List<DanceCounter> dance = [];
        var isHitObjectSection = false;
        string path = filename.Replace(filename.Split("/").Last(),"");
        int keycount = 4;
        foreach (var line in lines)
        {
            if (line.StartsWith("Title:"))
            {
            songtitle = line.Split(":")[1].Trim();
            }
            if (line.StartsWith("Artist:"))
            {
            artist = line.Split(":")[1].Trim();
            }
            if (line.StartsWith("CircleSize:"))
            {
            //keycount = (int)float.Parse(line.Split(":")[1].Trim());
            keycount = float.TryParse(line.Split(":")[1].Trim(), out float keycountv) ? (int)keycountv : 4;
            }if (line.StartsWith("OverallDifficulty:"))
            {
            //keycount = (int)float.Parse(line.Split(":")[1].Trim());
            accuracy = float.TryParse(line.Split(":")[1].Trim(), out float accuracyv ) ? (int)accuracyv : 0;
            }
            if (line.StartsWith("AudioFilename:"))
            {
            audio = line.Split(":")[1].Trim();
            }
            if (line.StartsWith("BeatmapID:"))
            {
            osubeatid = int.TryParse(line.Split(":")[1].Trim(), out int osubeatidv) ? (int)osubeatidv : 0;
            }
            if (line.StartsWith("BeatmapSetID:"))
            {
            osubeatidset = int.TryParse(line.Split(":")[1].Trim(), out int osubeatidsetv) ? (int)osubeatidsetv : 0;
            }
            if (line.StartsWith("QluteBeatID:"))
            {
            qlbeatid = int.TryParse(line.Split(":")[1].Trim(), out int qlbeatidv) ? (int)qlbeatidv : 0;
            }
            if (line.StartsWith("QluteBeatIDSet:"))
            {
            qlbeatidset = int.TryParse(line.Split(":")[1].Trim(), out int qlbeatidsetv) ? (int)qlbeatidsetv : 0;
            }
            if (line.StartsWith("Creator:"))
            {
            mapper = line.Split(":")[1].Trim();
            }
            if (line.StartsWith("Version:"))
            {
            version = line.Split(":")[1].Trim();
            }
            if (line.StartsWith("0,0,\"") && line.Contains("\""))
            {
            var parts = line.Split("\"");
            if (parts.Length > 1)
            {
                background = parts[1].Trim();
            }
            }
            if (line.StartsWith("[TimingPoints]"))
            {
            var timingPointLines = lines.SkipWhile(l => !l.StartsWith("[TimingPoints]")).Skip(1);
            bool foundbpm = false;
            foreach (var timingLine in timingPointLines)
            {
                if (string.IsNullOrWhiteSpace(timingLine) || timingLine.StartsWith("["))
                break;

                var timingParts = timingLine.Split(",");
                if (timingParts.Length > 1 && float.TryParse(timingParts[1], out float bpmValue) && !foundbpm)
                {
                bpmValue = 60000 / bpmValue;
                bpm = (int)bpmValue;
                foundbpm = true;
                }
                if (timingParts.Length > 1 && int.TryParse(timingParts.First(), out int timecount) && int.TryParse(timingParts.Last(), out int flashtime)){
                    dance.Add(new DanceCounter { time = timecount, flash = flashtime == 1 });
                }
            }
            }
            if (line.Trim() == "[HitObjects]")
            {
                isHitObjectSection = true;
                continue;
            }

            if (isHitObjectSection)
            {
                // Break if we reach an empty line or another section
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('['))
                {
                    ppvalue = Get_ppvalue(hitob,0,0,0,combo: hitob);
                    isHitObjectSection = !isHitObjectSection;
                    levelrating = hitob * 0.005f;
                    timetotal = notetime;
                    break;
                }
                var notecfg = line.Split(new[] { ',', ':' });
                notetime = float.TryParse(notecfg[2], out float notetimev) ? (int)notetimev : 0;
                var notesect = notecfg[0];
                hitob++;
            }
        }
        if (keycount == 4)
        {
            Beatmaps.Add(new BeatmapLegend
            {
                ID = Beatmaps.Count,
                SetID = SetID,
                Title = songtitle,
                Artist = artist,
                Mapper = mapper,
                KeyCount = keycount,
                Version = version,
                pp = ppvalue,
                Osubeatid = osubeatid,
                Osubeatidset = osubeatidset,
                Beatid = qlbeatid,
                Beatidset = qlbeatidset,
                Bpm = bpm,
                Dance = dance,
                Timetotal = timetotal,
                Levelrating = levelrating,
                Accuracy = accuracy,
                Background = background,
                Audio = audio,
                Rawurl = rawurl,
                Path = path
            });
        }
    }
    public static void Addms(float ms)
    {
        AllMiliSecondsFromBeatmap += ms;
        MiliSecondsFromBeatmapTimes++;
        UnstableRate.Rate.Add(ms);
    }
    public static float Getms(){
        return AllMiliSecondsFromBeatmap/MiliSecondsFromBeatmapTimes;
    }
    public static void Resetms(){
        AllMiliSecondsFromBeatmap = 0;
        MiliSecondsFromBeatmapTimes = 0;
    }
    public static void ResetScore(){
        Gameplaycfg.Score = 0;
        Gameplaycfg.pp = 0;
        Gameplaycfg.Max = 0;
        Gameplaycfg.Great = 0;
        Gameplaycfg.Meh = 0;
        Gameplaycfg.Bad = 0;
        Gameplaycfg.Accuracy = 100;
        Gameplaycfg.Time = 0;
        Gameplaycfg.Combo = 0;
        Gameplaycfg.MaxCombo = 0;
        Gameplaycfg.Avgms = 0;
        Gameplaycfg.ms = 0;
        Gameplaycfg.Timeframe = 0;
    }
    public static class Gameplaycfg
    {
        public static int Score { get; set; }
        public static double Timeframe { get; set; }
        public static double pp { get; set; }
        public static double maxpp { get; set; }
        public static float Time { get; set; }
        public static float TimeTotal { get; set; }
        public static float TimeTotalGame { get; set; }
        public static float Accuracy { get; set; }
        public static int Combo { get; set; }
        public static int MaxCombo { get; set; }
        public static int Max { get; set; }
        public static int Great { get; set; }
        public static int Meh { get; set; }
        public static int Bad { get; set; }
        public static float ms { get; set; }
        public static int Avgms { get; set; }


    }
    public static int ranked_points { get; set; }
    
    public static int SongIDHighlighted { get; set; } = -1; // Highlighted song ID for the song select screen
    public static int LevelRating { get; set; } = -1; // Highlighted song ID for the song select screen
    public static Dictionary<string, object> Sessioncfg { get; set; } = new Dictionary<string, object>
    {
        { "TopPanelSlideip", false },
        { "toppanelhide", false },
        { "Reloadmap", false },
        { "reloaddb", false },
        { "localpp", (double)0 },
        { "loggedin", false },
        { "loggingin", false },
        { "showaccountpro", false },
        { "settingspanelv", false },
        { "chatboxv", false },
        { "notificationpanelv", false },
        { "ranknumber", null },
        { "playercolour", null },
        { "SongID", -1 },
        { "totalbeatmaps", 0 },
        { "beatmapurl", null },
        { "beatmaptitle", null },
        { "beatmapartist", null },
        { "beatmapmapper", null },
        { "beatmapbpm", (int)160 },
        { "osubeatid", 0 },
        { "osubeatidset", 0 },
        { "beatmapaccuracy", (int)0 },
        { "beatmapdiff", null },
        { "customapi", false},
        { "multiplier" , 1.0f},
        { "fps" , 0},
        { "ms" , 0.0f},
        { "background" , null},
        { "client-id", null },
        { "client-secret", null },
    };
    public void toppaneltoggle(){
		Sessioncfg["toppanelhide"] = !(bool)Sessioncfg["toppanelhide"];
		AnimationPlayer Ana = GetTree().Root.GetNode<AnimationPlayer>("TopPanelOnTop/TopPanel/Wabamp");
		if (((bool)Sessioncfg["toppanelhide"] == true)) Ana.PlayBackwards("Bootup"); else Ana.Play("Bootup");}
    public static float TopPanelPosition { get; set; } = 0.0f;
    private double oldtime = 0.0f;

    public override void _Process(double _delta)
    {
        var aud = GetSetting("audiooffset").ToString();
        if (aud == null) AudioOffset = 0; // AudioOffset Subsystem
        else AudioOffset = float.Parse(aud);


        TopPanelPosition = GetTree().Root.GetNode<ColorRect>("TopPanelOnTop/TopPanel/InfoBar").Position.Y + 50;
        if (Input.IsActionJustPressed("Hide Panel"))
        {
            toppaneltoggle();
        }
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - oldtime > 2)
        {
            oldtime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ResetVol();
        }




    }
    public override void _Ready()
    {
        Configurationbk = new Dictionary<string, object>(Configuration);
        GD.Print("Please wait...");
        GD.Print("Checking if config file is saved...");
        if (System.IO.File.Exists(settingsfile))
        {
            GD.Print("Config file found... Loading it up :)");
            using var saveFile = FileAccess.Open(settingsfile, FileAccess.ModeFlags.Read);
            var json = saveFile.GetAsText();
            Configuration = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            foreach (string s in Configurationbk.Keys)
            {
                if (!Configuration.ContainsKey(s))
                {
                    Configuration[s] = Configurationbk[s];
                }
            }
            saveFile.Close();
        }
        else
        {
            GD.Print("Creating config...");
            SaveSettings();
        }


        // Check if temp folder is not empty, then delete its contents
        if (System.IO.Directory.Exists(tempdir))
        {
            var files = System.IO.Directory.GetFiles(tempdir);
            var dirs = System.IO.Directory.GetDirectories(tempdir);
            if (files.Length > 0 || dirs.Length > 0)
            {
                foreach (var file in files)
                {
                    try { System.IO.File.Delete(file); } catch { }
                }
                foreach (var dir in dirs)
                {
                    try { System.IO.Directory.Delete(dir, true); } catch { }
                }
            }
        }
        

        backgrounddim = int.TryParse(GetSetting("backgrounddim").ToString(), out int bkd) ? bkd : 70;
        SampleVol = int.TryParse(GetSetting("sample").ToString(), out int smp) ? smp : 80;
        MasterVol = int.TryParse(GetSetting("master").ToString(), out int mtr) ? mtr : 80;
        ResetVol();
        var resolutionIndex = int.TryParse(GetSetting("windowmode")?.ToString(), out int mode) ? mode : 0;
        changeres(resolutionIndex);
        RefreshFPS();
        Replay.Init();
        LeaderboardType = int.TryParse(GetSetting("leaderboardtype").ToString(), out int lbtm) ? (int)lbtm : 1;
		if (LeaderboardType < 0 && LeaderboardType > 1) LeaderboardType = 1;
    }
    public void ResetVol()
    {
        AudioPlayer.Instance.VolumeDb = (int)(Math.Log10(MasterVol / 100.0) * 20) - 5; // -5 to adjust the volume to a more NOT loud level and cap it
        Sample.Instance.VolumeDb = (int)(Math.Log10(SampleVol / 100.0) * 20) - 5;
    }
    public void changeres(int index)
    {
        GD.Print(string.Format("Resolution changed to {0}", index));
        if (index == 0)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }
        else if (index == 1)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else if (index == 2)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
        SetSetting("windowmode", index);
    }
    // Set a setting
    public void SetSetting(string key, object value)
    {
        if (Configuration.ContainsKey(key))
            Configuration[key] = value;
            SaveSettings();
    }

    // Save settings to file
    public void SaveSettings()
    {
    using var saveFile = FileAccess.Open(settingsfile, FileAccess.ModeFlags.Write);
    var json = JsonSerializer.Serialize(Configuration);
    saveFile.StoreString(json);
    saveFile.Close();
    }

    // Get a setting
    public static object GetSetting(string key)
    {
        return Configuration.ContainsKey(key) ? Configuration[key] : null;
    }
}
