using System;
using System.Windows.Input;

namespace Child_Talker.Utilities.Autoscan
{
    public class AutoscanCommands : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}