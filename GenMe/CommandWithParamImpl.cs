using ICommand = System.Windows.Input.ICommand;
using EventHandler = System.EventHandler;

namespace GenMe
{
    public class CommandWithParamImpl : ICommand
    {
        private readonly System.Action<object> _action;

        public CommandWithParamImpl(System.Action<object> action)
        {
            System.Diagnostics.Debug.Assert(action != null);
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

        protected void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged.Invoke(this, new System.EventArgs());
            }
        }
    }
}
