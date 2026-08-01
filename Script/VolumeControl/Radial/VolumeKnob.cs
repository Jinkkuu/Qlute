using Godot;
using System;

public partial class VolumeKnob : TextureProgressBar
{
    [Signal]
    public delegate void ValueChangedEventHandler(float value);

    [Export] public new float MinValue { get; set; } = 0f;
    [Export] public new float MaxValue { get; set; } = 100f;
     public static float SavedValue { get; set; } = 100f;

    private float _currentValue = 0f;

    /// <summary> The current visual value of the knob. </summary>
    [Export]
    public float CurrentValue
    {
        get => _currentValue;
        set
        {
            // If the node isn't inside the tree yet, just store the value
            if (!IsNodeReady())
            {
                _currentValue = value;
                _targetValue = value;
                return;
            }
            SetTargetValue(value);
        }
    }

    [Export] public float StartAngleDegrees { get; set; } = 270f;
    [Export] public float SmoothSpeed { get; set; } = 15f;
    [Export] public string VolumeTitle { get; set; } = "Master";
    [Export] public bool ShowMute { get; set; } = false;

    private Label VolumeTitleNode;
    private Label VolumeLabel;
    private ButtonFade Mute;
    private float _targetValue = 0f;
    private float oldval = 0f;
    private bool _isDragging = false;
    private bool ishoveringmute = false;
    private Tween Tween;

    public override void _Ready()
    {
        VolumeTitleNode = GetNodeOrNull<Label>("Title");
        VolumeLabel = GetNodeOrNull<Label>("Volume");
        Mute = GetNodeOrNull<ButtonFade>("Mute");
        Mute.Visible = ShowMute;
        
        if (VolumeTitleNode != null)
            VolumeTitleNode.Text = VolumeTitle;

        FillMode = (int)FillModeEnum.CounterClockwise;
        base.MinValue = MinValue;
        base.MaxValue = MaxValue;

        // Preserve initial value set from Inspector or external script
        Value = MinValue; 
        SetTargetValue((float)CurrentValue);
    }

    public override void _Process(double delta)
    {
        VolumeLabel.Text = $"{Value:N0}%";
        if (oldval != Value && !dontstart)
        {
            oldval = (float)Value;
            EmitSignal(SignalName.ValueChanged, Value);
        }

        if (ShowMute && AudioPlayer.isMasterMuted())
        {
            Mute.StringText = "Unmute";
        }
        else
        {
            Mute.StringText = "Mute";
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseBtn && mouseBtn.ButtonIndex == MouseButton.Left)
        {
            _isDragging = mouseBtn.Pressed;
            if (_isDragging)
            {
                UpdateTargetFromMouse(mouseBtn.Position);
            }
        }
        else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
        {
            UpdateTargetFromMouse(mouseMotion.Position);
        }
    }

    private void UpdateTargetFromMouse(Vector2 mousePos)
    {
        if (ishoveringmute) return;
        dontstart = false;
        Vector2 center = Size / 2f;
        Vector2 dir = mousePos - center;

        float angleRad = Mathf.Atan2(dir.Y, dir.X);
        float angleDeg = Mathf.RadToDeg(angleRad);

        float adjustedAngle = Mathf.PosMod(StartAngleDegrees - angleDeg, 360f);
        float pct = adjustedAngle / 360f;
        float calculatedValue = Mathf.Lerp(MinValue, MaxValue, pct);

        SavedValue = calculatedValue;
        SetTargetValue(calculatedValue);
    }

    private bool dontstart = true;
    public void SetTargetValue(float newValue)
    {
        Tween?.Kill();
        Tween = CreateTween();
        Tween.TweenProperty(this, "value", newValue, 0.3f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        Tween.Play();
    }

    private void block() => ishoveringmute = true;
    private void unblock() => ishoveringmute = false;


    private void mute()
    {
        dontstart = false;
        if (AudioPlayer.isMasterMuted())
        {
            SetTargetValue(SavedValue);
        }
        else
        {
            SetTargetValue(0);
        }
    }
}