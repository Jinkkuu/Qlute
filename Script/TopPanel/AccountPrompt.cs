using Godot;
using System;
using System.Security.Cryptography;
using System.Text;

public partial class AccountPrompt : Control
{
	public static LineEdit Password { get; set; }
	public static LineEdit User { get; set; }
	public static HttpRequest LoginApi { get; set; }
	public static HttpRequest RankingApi { get; set; }
	public string Username = "Guest";
	public string PasswordHash = null;
	public Control Log {get;set;}
	public MarginContainer NotLog {get;set;}
	public Label Ranking {get;set;}
	public Label PerformanceNumber {get;set;}
	public Label PlayerName {get;set;}
	private SettingsOperator SettingsOperator { get; set; }
	private void _create()
	{
		var Signup = GD.Load<PackedScene>("res://Panels/Screens/Signup.tscn").Instantiate().GetNode<ColorRect>(".");
		GetTree().CurrentScene.AddChild(Signup);
		Hide();
	}
	public override void _Process(double _delta)
	{
		if ((bool)SettingsOperator.Sessioncfg["loggedin"] == true)
		{
			Log.Visible = true;
			NotLog.Visible = false;
		}
		else
		{
			Log.Visible = false;
			NotLog.Visible = true;
		}
		Ranking = GetNode<Label>("AccPanel/Log/Panel/RankingNumber");
		PerformanceNumber = GetNode<Label>("AccPanel/Log/Panel/PerformanceNumber");
		PlayerName = GetNode<Label>("AccPanel/Log/Panel/UsernameSection/Username");
		PlayerName.Text = SettingsOperator.GetSetting("username")?.ToString();
		Ranking.Text = "#" + SettingsOperator.Sessioncfg["ranknumber"]?.ToString();
		PerformanceNumber.Text = $"{SettingsOperator.ranked_points.ToString("N0")}pp";
	}
	public override void _Ready()
	{
		SettingsOperator = GetNode<SettingsOperator>("/root/SettingsOperator");
		Password = GetNode<LineEdit>("AccPanel/Notlog/VBoxContainer/Password");
		User = GetNode<LineEdit>("AccPanel/Notlog/VBoxContainer/Username");
		NotLog = GetNode<MarginContainer>("AccPanel/Notlog");
		Log = GetNode<Control>("AccPanel/Log");
		User.Text = SettingsOperator.GetSetting("username")?.ToString();
	}
	private void _on_login_pressed(){
		PasswordHash = ApiOperator.ComputeSha256Hash(Password.Text);   
		Label Notice = GetNode<Label>("AccPanel/Notlog/VBoxContainer/HBoxContainer/Notice");
		Notice.Text = "Connecting...";
		Username = User.Text;
		Notice.Text = ApiOperator.Login(Username,PasswordHash);
	}
	private void _logout(){
		Log.Visible = false;
		NotLog.Visible = true;
		SettingsOperator.SetSetting("username",null);
		SettingsOperator.SetSetting("password",null);
		SettingsOperator.Sessioncfg["loggedin"] = false;
		Ranking.Visible = false;
		PlayerName.Text = "Guest";
	}
}
