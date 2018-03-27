public interface ICommand
{
    void Execute(Player player);
}

public class PushTeleportStateCommand : ICommand
{
    public void Execute(Player player)
    {
        player.State.Push(new PlayerTeleportingState());
    }
}

public class PopTeleportStateCommand : ICommand
{
    public void Execute(Player player)
    {
        if(player.State.Peek().GetType() == typeof(PlayerTeleportingState))
        {
            player.State.Pop();
        }
    }
}
