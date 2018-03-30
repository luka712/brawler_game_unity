public interface ICommand
{
    void Execute(Player player);
}

/// <summary>
/// Call this command whenever push to teleport state is needed.
/// </summary>
public class PushTeleportStateCommand : ICommand
{
    public void Execute(Player player)
    {
        player.State.Push(new PlayerTeleportingState());
    }
}

/// <summary>
/// Call this command whenever teleport state exit is needed.
/// </summary>
public class PopTeleportStateCommand : ICommand
{
    public void Execute(Player player)
    {
        if(player.State.Count > 0 && 
            player.State.Peek().GetType() == typeof(PlayerTeleportingState))
        {
            player.State.Pop();
        }
    }
}
