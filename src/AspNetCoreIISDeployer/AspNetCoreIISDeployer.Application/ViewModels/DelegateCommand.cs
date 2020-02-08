using System;
using System.Windows.Input;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private event EventHandler canExecuteChanged;

        private readonly Action<object> executor;

        public DelegateCommand(Action<object> executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public event EventHandler CanExecuteChanged
        {
            add { canExecuteChanged += value; }
            remove { canExecuteChanged -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            executor(parameter);
        }
    }
}