using ICommand = System.Windows.Input.ICommand;
using EventHandler = System.EventHandler;

namespace GenMe
{
    public class CommandImpl : ICommand
    {
        private readonly System.Action _action;

        public CommandImpl(System.Action action)
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
            _action.Invoke();
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
