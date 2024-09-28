using System;
using System.Windows.Input;

namespace CustomCommandBarCreator
{
    
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<T> _execute;
        private Func<T,bool> _canExecute;
        private string displayName;
        public string DisplayName { get { return displayName; } }
        public RelayCommand(Action<T> execute):this(execute,null)
        {
            _execute = execute;
        }
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute,string displayName) : this(execute, null)
        {
            _execute = execute;
            _canExecute = canExecute;
            this.displayName = displayName;
        }
        public RelayCommand(Action<T> execute,Func<T,bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;

        }
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke((T)parameter);
        }

        public void RaiseCanExecuteChanged() 
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);   
        }

    }
}
