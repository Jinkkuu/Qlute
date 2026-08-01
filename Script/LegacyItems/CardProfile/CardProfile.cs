using Godot;
using System;

public partial class CardProfile : TextureRect
{
	private Label Accuracy;
	private Label Username;
	private Label pp;
	private Label Score;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Username = GetNode<Label>("Leftside/Username");
		pp = GetNode<Label>("Leftside/pp");
		Score = GetNode<Label>("Leftside/Score");
		Accuracy = GetNode<Label>("Accuracy");
		Username.Text = ApiOperator.Username;
		Score.Text = SettingsOperator.RankScore.ToString("N0");
		pp.Text = $"{SettingsOperator.ranked_points:N0}pp";
		Accuracy.Text = $"Accuracy - {SettingsOperator.OAccuracy:P2}";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Username.Text = ApiOperator.Username;
		Score.Text = SettingsOperator.RankScore.ToString("N0");
		pp.Text = $"{SettingsOperator.ranked_points:N0}pp";
		Accuracy.Text = $"Accuracy - {SettingsOperator.OAccuracy:P2}";
	}
}
