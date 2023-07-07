using Godot;
using System;

public class Player : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float speed;
    public int health;
    Vector3 moveVector;
    Sprite3D sprite;
    AnimationPlayer animPlayer;
    Particles particles;
    Timer particlesTimer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite3D>("Sprite3D");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        particles = GetNode<Particles>("Particles");
        particlesTimer = GetNode<Timer>("ParticlesTimer");
        health = 10;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {

    // }

    public override void _PhysicsProcess(float delta)
    {
        moveVector = new Vector3(0.0f, 0.0f, 0.0f);
        if(Input.IsActionPressed("ui_up"))
            moveVector -= new Vector3(0.0f, 0.0f, 1.0f);
        if(Input.IsActionPressed("ui_down"))
            moveVector += new Vector3(0.0f, 0.0f, 1.0f);
        if(Input.IsActionPressed("ui_right"))
            moveVector += new Vector3(1.0f, 0.0f, 0.0f);
        if(Input.IsActionPressed("ui_left"))
            moveVector -= new Vector3(1.0f, 0.0f, 0.0f);
        moveVector = moveVector.Normalized();

        if(moveVector.x < 0.0f)
            sprite.FlipH = true;
        else if(moveVector.x > 0.0f)
            sprite.FlipH = false; 
        
        if(moveVector != new Vector3(0.0f, 0.0f, 0.0f))
            animPlayer.Play("run_left");
        else if(animPlayer.IsPlaying())
        {
            animPlayer.Stop();
            sprite.Frame = 0;
        }

        if(Input.IsActionJustPressed("ui_accept"))
            damage(5);

        MoveAndSlide(moveVector * speed);
    }

    public void damage(int amount)
    {
        health -= amount;
        particles.Emitting = true;
        particlesTimer.Start();
        if(health < 0)
            GetTree().ReloadCurrentScene();
    }

    public void _on_ParticlesTimer_timeout()
    {
        particles.Emitting = false;
    }
    
}
