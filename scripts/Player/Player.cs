using Godot;
using System;

public class Player : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float speed;
    [Export]
    public String level;
    public int health;
    public int inv;
    bool finish;
    Vector3 moveVector;
    Sprite3D sprite;
    AnimationPlayer animPlayer;
    AnimationPlayer knifePlayer;
    Particles particles;
    Timer particlesTimer;
    Timer particlesTimer2;
    Area attackBox;
    CameraMovement camera;
    ColorRect colorRect;
    ColorRect colorRect2;
    ColorRect colorRect3;
    AudioStreamPlayer audioPlayer;
    AudioStreamPlayer stepPlayer;
    [Export]
    public bool canHurt;
    Label label;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite3D>("Sprite3D");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        knifePlayer = GetNode<AnimationPlayer>("AnimationPlayer2");
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        stepPlayer = GetNode<AudioStreamPlayer>("WalkingAudio");
        particles = GetNode<Particles>("Particles");
        particlesTimer = GetNode<Timer>("ParticlesTimer");
        particlesTimer2 = GetNode<Timer>("ParticlesTimer2");
        attackBox = GetNode<Area>("Area");
        camera = GetNode<CameraMovement>("Camera");
        colorRect = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("Control").GetNode<ColorRect>("ColorRect");
        colorRect2 = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("Control").GetNode<ColorRect>("ColorRect2");
        colorRect3 = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("Control").GetNode<ColorRect>("ColorRect3");
        label = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("Control").GetNode<Label>("Label");
        finish = false;
        colorRect.Visible = false;
        colorRect2.Visible = false;
        colorRect3.Visible = false;
        label.Visible = false;
        attackBox.Monitoring = false;
        health = 15;
        inv = 10;
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
        {
            camera.trauma = 0.02f;
            animPlayer.Play("run_left");
            label.Visible = false;
            if(stepPlayer.Playing == false)
                stepPlayer.Play();
        }
        else if(animPlayer.IsPlaying())
        {
            animPlayer.Stop();
            if(stepPlayer.Playing == true)
                stepPlayer.Stop();
            sprite.Frame = 0;
        }

        if(Input.IsActionJustPressed("ui_accept") && canHurt)
        {
            damage(5);
        }

        if(Input.IsActionJustPressed("ui_cancel"))
        {
            label.Visible = true;
        }

        if(Input.IsActionJustPressed("ui_accept") && label.Visible == true)
        {
            GetTree().Quit();
        }

        if(finish && inv >= 1)
            colorRect3.Visible = true;
        else
            colorRect3.Visible = false;

        MoveAndSlide(moveVector * speed);
    }

    public void damage(int amount)
    {
        health -= amount;
        sprite.Modulate -= new Color(0.0f, 0.2f, 0.2f, 0.0f);
        particles.Emitting = true;
        attackBox.Monitoring = true;
        knifePlayer.Play("stab");
        audioPlayer.Play();
        camera.trauma = 0.3f;
        colorRect.Visible = true;
        particlesTimer.Start();
        if(finish && health < 0 && inv >= 1)
            GetTree().ChangeScene(level);
        else if(health < 0)
            GetTree().ReloadCurrentScene();
    }

    public void heal(int amount)
    {
        if(health < 15)
        {
            health += amount;
            // if(sprite.Modulate < new Color(1.0f, 1.0f, 1.0f, 1.0f))
                  
            // else
            // {
            //     if(sprite.Modulate.a >= 0.0f)
            //         sprite.Modulate -= new Color(0.0f, 0.0f, 0.0f, 0.5f);
            // }
            sprite.Modulate += new Color(0.0f, 0.2f, 0.2f, 0.0f); 
        }
        else if(inv > 0)
        {
            inv -= amount;
            sprite.Modulate -= new Color(0.0f, 0.0f, 0.0f, 0.5f);
        }
        particlesTimer2.Start();
        colorRect2.Visible = true;
    }

    public void _on_ParticlesTimer_timeout()
    {
        particles.Emitting = false;
        attackBox.Monitoring = false;
        GD.Print("prolazak");
        colorRect.Visible = false;
    }

    public void _on_ParticlesTimer2_timeout()
    {
        GD.Print("hilovanje");
        colorRect2.Visible = false;
    }

    public void _on_Finish_body_entered(Node body)
    {
        if(body.Name == "Player")
            finish = true;
    }
    
    public void _on_Finish_body_exited(Node body)
    {
        if(body.Name == "Player")
            finish = false;
    }

    public void _on_Area_body_entered(Node body)
    {
        if(body.IsInGroup("enemies") && finish == false)
        {
            Enemy enemy = (Enemy)body;
            // if(sprite.Modulate.a < 1.0f)
            //     sprite.Modulate += new Color(0.0f, 0.0f, 0.0f, 0.3f);
            enemy.hurt();
        }
    }
}
