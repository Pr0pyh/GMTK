using Godot;
using System;

public class Level : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public bool final;
    AnimationPlayer animPlayer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("fadeIn");
    }

    public void _on_AnimationPlayer_animation_finished(String animName)
    {
        if(animName == "fadeIn" && final == true)
            animPlayer.Play("ending");
        else if(animName == "ending")
            GetTree().ChangeScene("res://scripts/Levels/MainMenu.tscn");

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
