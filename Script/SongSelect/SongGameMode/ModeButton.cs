using Godot;
using System;

public partial class ModeButton : Button
{
	[Export]
	public bool NewGameMode = false;
	private Label NewGameModeNode { get; set; } 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NewGameModeNode = GetNode<Label>("NewLabel");
		NewGameModeNode.Visible = NewGameMode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
