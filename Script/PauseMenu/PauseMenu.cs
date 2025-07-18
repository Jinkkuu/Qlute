using Godot;
using System;

public partial class PauseMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	private SettingsOperator SettingsOperator { get; set; }
	public override void _Ready()
	{
		SettingsOperator = GetNode<SettingsOperator>("/root/SettingsOperator");
		if (GetTree().CurrentScene is CanvasItem canvasScene) canvasScene.Modulate = new Color(1f, 1f, 1f, 1f);
		if (Gameplay.Dead)
		{
			GetNode<Button>("PanelContainer/VBoxContainer/Continue").Visible = false;
			GetNode<Label>("PauseLabel/Text").Visible = true;
			GetNode<Label>("PauseLabel").Text = "Game Over";
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private void _continue(){
			GetTree().Paused = false;
			Visible = false;
			QueueFree();
	}
	private void _retry(){
			GetTree().Paused = false;
			BeatmapBackground.FlashEnable = true;
			SettingsOperator.toppaneltoggle();
			GetNode<SceneTransition>("/root/Transition").Switch("res://Panels/Screens/SongLoadingScreen.tscn");
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pausemenu")){
			_continue();
		}else if (Input.IsActionJustPressed("retry")){
			_retry();
}
	}
	private void _exit(){
			GetTree().Paused = false;
			BeatmapBackground.FlashEnable = true;
			SettingsOperator.toppaneltoggle();
			GetNode<SceneTransition>("/root/Transition").Switch("res://Panels/Screens/song_select.tscn");
	}
}
