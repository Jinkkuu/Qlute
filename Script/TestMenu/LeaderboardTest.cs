using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;

public partial class LeaderboardTest : ColorRect
{
	private PanelContainer Leaderboard { get; set; }
	private void _reset()
	{
		ApiOperator.LeaderboardList = new List<LeaderboardEntry>();
		Random randomizer = new Random();
		for (int i = 0; i < 50; i++)
		{
			ApiOperator.LeaderboardList.Add(new LeaderboardEntry
			{
				username = $"Player{i.ToString("N0")}",
				points = randomizer.Next(1, 1000),
				score = randomizer.Next(1, 1823476),
				combo = randomizer.Next(1, 3265),
				MAX = randomizer.Next(1, 1000),
				GOOD = randomizer.Next(1, 1000),
				MEH = randomizer.Next(1, 1000),
				BAD = randomizer.Next(1, 1000),
				mods = "HD",
				time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			});
		}
		_kill();
		Leaderboard = GD.Load<PackedScene>("res://Panels/GameplayElements/Customizable/LeaderboardPanel.tscn").Instantiate().GetNode<PanelContainer>(".");
		Leaderboard.Name = "Leaderboard";
		Leaderboard.Position = new Vector2(405, 34);
		AddChild(Leaderboard);
	}
	private void _kill()
	{
		try
		{
			Leaderboard.QueueFree();
		}
		catch (Exception err)
		{
			GD.Print(err);
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		_reset();
	}

	private void _changed(float value)
	{
		// This method is called when the value of the slider changes.
		// You can implement functionality here to handle the change.
		SettingsOperator.Gameplaycfg.Score = (int)value;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
