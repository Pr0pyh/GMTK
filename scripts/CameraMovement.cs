using Godot;
using System;

public class CameraMovement : Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    double amount;
    double maxHOffset;
    double maxVOffset;
    public float trauma;
    Player player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetParent<Player>();
        GD.Randomize();
        maxHOffset = 1;
        maxVOffset = 1;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    public override void _PhysicsProcess(float delta)
    {
        GlobalTranslation = GlobalTranslation.LinearInterpolate(player.GlobalTranslation, 0.9f);
        Translation += new Vector3(0.0f, 7.0f, 8.0f);
        if(!(trauma < 0.0))
        {
            shake();
            trauma = Mathf.Max((float)(trauma - 0.8*delta), 0.0f);
        }
    }

    public void shake()
    {
        amount = trauma;
        HOffset = (float)(maxHOffset * amount * GD.RandRange(-1, 1));
        VOffset = (float)(maxVOffset * amount * GD.RandRange(-1, 1));
    }
}
