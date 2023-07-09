using Godot;
using System;

public class Enemy : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float speed;
    Vector3 movePos;

    public enum ENEMY_STATE {
        MOVING,
        STUN,
        DETECTED
    };
    ENEMY_STATE enemyState;
    Node world;
    Player player;
    Area hurtArea;
    Timer stunTimer;
    Timer hurtTimer;
    AnimationPlayer animPlayer;
    AudioStreamPlayer audioPlayer;
    RayCast rayCast;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        world = GetParent<Node>();
        stunTimer = GetNode<Timer>("StunTimer");
        hurtTimer = GetNode<Timer>("HurtTimer");
        hurtArea = GetNode<Area>("Area");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        player = GetParent().GetNode<Player>("Player");
        rayCast = GetNode<RayCast>("RayCast");

        movePos = (player.Translation - Translation).Normalized();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    public override void _PhysicsProcess(float delta)
    {
        switch(enemyState)
        {
            case ENEMY_STATE.MOVING:
                moving();
                break;
            case ENEMY_STATE.STUN:
                stun();
                break;
            case ENEMY_STATE.DETECTED:
                detected();
                break;
        }

        
    }

    public void detected()
    {
        if(!animPlayer.IsPlaying())
            animPlayer.Play("run");
        // LookAt(player.Translation, new Vector3(0.0f, 1.0f, 0.0f));
        Vector3 currPos = (player.Translation - Translation).Normalized();
        player.Rotation = new Vector3(player.Rotation.x, 0.0f, 0.0f);
        movePos = currPos;
        MoveAndSlide(movePos * speed * 1.5f);
        if(this.Translation.DistanceTo(player.Translation) > 10.0f)
            enemyState = ENEMY_STATE.MOVING;
    }

    public void moving()
    {
        if(!animPlayer.IsPlaying())
            animPlayer.Play("run");
        Vector3 currPos = new Vector3(1.0f, 0.0f, 0.0f);
        if(this.IsOnWall())
            movePos *= -1;
        MoveAndSlide(movePos*speed);
        if(this.Translation.DistanceTo(player.Translation) < 7.0f)
            enemyState = ENEMY_STATE.DETECTED;
    }

    public void stun()
    {
        if(animPlayer.IsPlaying())
            animPlayer.Stop();
        // LookAt(player.Translation, new Vector3(0.0f, 1.0f, 0.0f));
        player.Rotation = new Vector3(player.Rotation.x, 0.0f, 0.0f);
        Vector3 movePos = -(player.Translation - Translation).Normalized();
        MoveAndSlide(movePos * speed * 8);
    }

    public void hurt()
    {
        enemyState = ENEMY_STATE.STUN;
        stunTimer.Start();
    }

    public void _on_StunTimer_timeout()
    {
        enemyState = ENEMY_STATE.MOVING;
    }

    public void _on_Area_body_entered(Node body)
    {
        if(body.Name == "Player")
        {
            hurtArea.SetDeferred("monitoring", false);
            hurtTimer.Start();
            Player player = (Player)body;
            audioPlayer.Play();
            player.heal(5);
        }
    }

    public void _on_HurtTimer_timeout()
    {
        hurtArea.SetDeferred("monitoring", true);
    }
}
