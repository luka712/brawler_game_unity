
/// <summary>
/// The zugai player.
/// </summary>
public class PlayerZugai : Player
{

    #region Unity Methods

    protected override void Start()
    {
        base.Start();
        State.Push(new PlayerIdleState());
    }

    private void FixedUpdate()
    {
        
        if(State.Count > 0)
            State.Peek().HandleInput(this);
    }

    protected override void Update()
    {
        base.Update();
        if(State.Count > 0)
            State.Peek().Update(this);
    }

    #endregion
}
