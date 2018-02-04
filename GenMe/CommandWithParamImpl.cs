using ICommand = System.Windows.Input.ICommand;
using EventHandler = System.EventHandler;

namespace GenMe
{
    public sealed class CommandWithParamImpl : ICommand
    {
        private readonly System.Action<object> _action;

        public CommandWithParamImpl(System.Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
