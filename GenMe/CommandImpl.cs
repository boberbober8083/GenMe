using ICommand = System.Windows.Input.ICommand;
using Action = System.Action;
using EventHandler = System.EventHandler;

namespace GenMe
{
    public sealed class CommandImpl : ICommand
    {
        Action _action;

        public CommandImpl(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
