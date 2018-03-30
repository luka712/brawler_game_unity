using UnityEngine;


/***************************************************************************
 * IDLE_STATE       -> ( MOVE_STATE, JUMP_STATE, ATTACK_STATE)  
 * MOVE_STATE       -> ( IDLE_STATE, JUMP_STATE, ATTACK_STATE)  
 * JUMP_STATE       -> ( IDLE_STATE, MOVE_STATE)              
 * IS_TELEPORTING   -> ( IDLE_STATE )
 * ATTACK_STATE     -> PREVIOUS 
 * 
 * 
 * 
 * 
 * 
 * 
 * HIERARCHY 
 * lvl 1     IDLE_STATE -- MOVE_STATE -- JUMP_STATE -- IS_TELEPORTING
 *               |             |             |               |
 *               V             V             V               V
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


public class PlayerIdleState : IPlayerState
{
    private bool isIdle;
    private float direction;
    private bool jump;
    private bool attack;

    public void HandleInput(Player player)
    {
        direction = Input.GetAxis(player.MoveLookup.HorizontalAxis);
        jump = Input.GetButtonDown(player.MoveLookup.JumpButton);
        attack = Input.GetButtonDown(player.MoveLookup.AttackButton);
    }

    public void Update(Player player)
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

        if (attack)
            player.State.Push(new PlayerAttackState());

    }
}

public class PlayerMoveState : IPlayerState
{
    private float direction;
    private bool jump;
    private bool attack;

    public void HandleInput(Player player)
    {
        direction = Input.GetAxis(player.MoveLookup.HorizontalAxis);
        jump = Input.GetButtonDown(player.MoveLookup.JumpButton);
        attack = Input.GetButtonDown(player.MoveLookup.AttackButton);
    }

    public void Update(Player player)
    {
        if (direction > 0)
        {
            player.MoveRight();
            player.LookRight();
            player.PlayMoveAnimation();
        }
        else if (direction < 0)
        {
            player.MoveLeft();
            player.LookLeft();
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

        if (attack)
            player.State.Push(new PlayerAttackState());
    }
}

public class PlayerJumpState : IPlayerState
{
    private bool isInJump;
    private float direction;

    public void HandleInput(Player player) =>
        direction = Input.GetAxis(player.MoveLookup.HorizontalAxis);


    public void Update(Player player)
    {
        if (!isInJump)
        {
            isInJump = true;
            player.Jump();
            player.PlayJumpAnimation();
        }

        if (direction > 0)
        {
            player.LookRight();
            player.MoveRight();
        }
        else if (direction < 0)
        {
            player.MoveLeft();
            player.LookLeft();
        }


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
    }
}

public class PlayerTeleportingState : IPlayerState
{
    public void HandleInput(Player player) { }

    public void Update(Player player)
    {
        
    }
}

public class PlayerAttackState : IPlayerState
{
    public void HandleInput(Player player) { }

    public void Update(Player player)
    {
        player.ActivateAttackCollider();
        player.PlayAttackAnimation();
        player.ActivateAttackCollider(false);
        player.State.Pop();
    }
}
