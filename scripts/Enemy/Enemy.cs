using Godot;
using System;

public class Enemy : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float speed;

    public enum ENEMY_STATE {
        MOVING,
        STUN
    };
    ENEMY_STATE enemyState;
    Node world;
    Player player;
    Area hurtArea;
    Timer stunTimer;
    Timer hurtTimer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        world = GetParent<Node>();
        stunTimer = GetNode<Timer>("StunTimer");
        hurtTimer = GetNode<Timer>("HurtTimer");
        hurtArea = GetNode<Area>("Area");
        player = GetParent().GetNode<Player>("Player");
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
        }

        
    }

    public void moving()
    {
        LookAt(player.Translation, new Vector3(0.0f, 1.0f, 0.0f));
        Vector3 movePos = (player.Translation - Translation).Normalized();
        MoveAndSlide(movePos * speed);
    }

    public void stun()
    {
        LookAt(player.Translation, new Vector3(0.0f, 1.0f, 0.0f));
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
            player.heal(5);
        }
    }

    public void _on_HurtTimer_timeout()
    {
        hurtArea.SetDeferred("monitoring", true);
    }
}
