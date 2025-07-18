using Godot;
using System;

public partial class RamUsage : PanelContainer
{
	// Called when the node enters the scene tree for the first time.
	public Label  FreeRam {get;set;}
	public Label  UsedRam {get;set;}
	public Label  RealFPS {get;set;}
	public Label  TotalRam {get;set;}
	public override void _Ready()
	{
		UsedRam = GetNode<Label>("VSplit/UsedT/Used");
		RealFPS = GetNode<Label>("VSplit/RealtimeFPS/FPS");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public string ConvertBytes(int bytes){
		string[] suffix = new string[] {"B", "KB", "MB", "GB", "TB"};
		int i = 0;
		double dblSByte = bytes;
		if (bytes > 1024){
			for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024){
				dblSByte = bytes / 1024.0;
			}
		}
		return string.Format("{0:0.##} {1}", dblSByte, suffix[i]);
	}
	public override void _Process(double delta)
	{
		Vector2 mpos = GetViewport().GetMousePosition();
		UsedRam.Text = ConvertBytes(((int)OS.GetStaticMemoryUsage()));
		RealFPS.Text = (1 / delta).ToString("0") + "fps";
		Position = mpos-Size;
	}
}
