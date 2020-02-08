using System;
using System.ComponentModel;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private event PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var handler = propertyChanged;

            if (!(handler is null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}