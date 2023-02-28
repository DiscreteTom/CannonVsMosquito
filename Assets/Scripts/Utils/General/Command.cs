namespace DT.General {
  public interface ICommand<T> {
    void Exec(T core);
  }
  public interface ICommand : ICommand<IIoCC> { }

  public interface ICommandBus<T> {
    void Exec(ICommand<T> command);
    void Exec<C>() where C : ICommand<T>, new();
  }
  public interface ICommandBus : ICommandBus<IIoCC> { }

  public class CommandBus<T> : ICommandBus<T> {
    T core;

    public CommandBus(T core) {
      this.core = core;
    }

    /// <summary>
    /// Execute a command.
    /// Override this method to add custom logic.
    /// </summary>
    protected virtual void HandleCommand(ICommand<T> command) {
      command.Exec(this.core);
    }

    public void Exec(ICommand<T> command) {
      this.HandleCommand(command);
    }

    public void Exec<C>() where C : ICommand<T>, new() {
      this.Exec(new C());
    }
  }
  public class CommandBus : CommandBus<IIoCC>, ICommandBus {
    public CommandBus(IIoCC core) : base(core) { }
  }
}