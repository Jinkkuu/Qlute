using Godot;
using System;

public partial class TaikoDrum : Sprite2D
{
	/// <summary>
	/// Drum type
	/// 0 - Red
	/// 1 - Blue
	/// </summary>
	[Export] public DrumTypeGroup DrumType { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		switch (DrumType)
		{
			case DrumTypeGroup.Inside : SelfModulate = new Color("#eb452c"); break;
			case DrumTypeGroup.Outside : SelfModulate = new Color("#448dab"); break;
		}
		SetProcess(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
