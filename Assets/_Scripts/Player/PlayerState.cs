using System.Collections;
using UnityEngine;


/***************************************************************************
 * IDLE_STATE       -> ( MOVE_STATE, JUMP_STATE, ATTACK_STATE)  
 * MOVE_STATE       -> ( IDLE_STATE, JUMP_STATE, ATTACK_STATE)  
 * JUMP_STATE       -> ( IDLE_STATE, MOVE_STATE, DOUBLE_JUMP)              
 * IS_TELEPORTING   -> ( IDLE_STATE )
 * ATTACK_STATE     -> PREVIOUS 
 * DOUBLE_JUMP      -> ( IDLE_STATE, MOVE_STATE )  
 * 
 * 
 * 
 * 
 * 
 * HIERARCHY 
 * lvl 1     IDLE_STATE -- MOVE_STATE -- JUMP_STATE -- IS_TELEPORTING  -- DOUBLE_JUMP
 *               |             |             |               |                 |
 *               V             V             V               V                 v
 *          ATTACK_STATE  ATTACK_STATE   
 * 
 * 
 * **************************************************************************/

/// <summary>
/// State interface
/// </summary>
public interface IPlayerState
{
    void HandleInput(Player player);
    void Update(Player player);
}

public abstract class PlayerState : IPlayerState
{
    protected float direction;
    protected bool jump;
    protected bool attack;
    protected bool jumpPress;

    public virtual void HandleInput(Player player)
    {

        direction = Input.GetAxis(player.MoveLookup.HorizontalAxis);
        jump = Input.GetButtonDown(player.MoveLookup.JumpButton);
        attack = Input.GetButtonDown(player.MoveLookup.AttackButton);
        jumpPress = Input.GetButtonDown(player.MoveLookup.JumpButton);
    }

    public abstract void Update(Player player);

    public virtual void MoveLeft(Player player)
    {
        player.MoveLeft();
        player.LookLeft();
    }

    public virtual void MoveRight(Player player)
    {
        player.MoveRight();
        player.LookRight();
    }

    public virtual void Move(Player player)
    {
        if (direction > 0)
        {
            MoveRight(player);
        }
        else if (direction < 0)
        {
            MoveLeft(player);
        }
    }
}


public class PlayerIdleState : PlayerState, IPlayerState
{
    private bool isIdle;

    public override void Update(Player player)
    {
        // no need to constantly set animation.
        if (!isIdle)
        {
            isIdle = true;
            player.PlayMoveAnimation(play: false);
        }

        if (direction != 0)
        {
            player.State.Pop();
            player.State.Push(new PlayerMoveState());
        }

        if (jump)
            player.State.Push(new PlayerJumpState());

        // TODO
        //if (attack)
        //    player.State.Push(new PlayerAttackState());

    }
}

public class PlayerMoveState : PlayerState, IPlayerState
{

    public override void Update(Player player)
    {
        if (direction > 0)
        {
            MoveRight(player);
            player.PlayMoveAnimation();
        }
        else if (direction < 0)
        {
            MoveLeft(player);
            player.PlayMoveAnimation();
        }
        else
        {
            player.State.Pop();
            player.State.Push(new PlayerIdleState());
        }

        if (jump)
        {
            player.PlayMoveAnimation(play: false);
            player.State.Push(new PlayerJumpState());
        }
    }
}

public class PlayerJumpState : PlayerState, IPlayerState
{
    private bool isInJump;

    public override void Update(Player player)
    {
        if (!isInJump)
        {
            isInJump = true;
            player.Jump();
            player.PlayJumpAnimation();
        }

        Move(player);

        if (player.CheckIfPlayerIsGrounded() == true)
        {
            player.PlayJumpAnimation(play: false);
            player.PlayLandingAnimation();
            player.State.Pop();
            if (direction == 0)
                player.State.Push(new PlayerIdleState());
            else
                player.State.Push(new PlayerMoveState());
        }

        if (isInJump && jumpPress)
        {
            JumpPress(player);
        }
    }

    // Made this abstract, double jump state is same and does nothing on jump
    public virtual void JumpPress(Player player)
    {
        player.DoubleJump();
        player.State.Pop();
        player.State.Push(new PlayerDoubleJumpState());
    }
}

public class PlayerDoubleJumpState : PlayerJumpState, IPlayerState
{
    public override void JumpPress(Player player) { }
}

public class PlayerTeleportingState : IPlayerState
{
    public void HandleInput(Player player) { }

    public void Update(Player player)
    {
    }
}

