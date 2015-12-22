using System;
using System.Windows.Input;

namespace RolexLoader.Helpers
{
  public  class RelayCommand : ICommand
    {
        #region private fields
        private readonly Action execute;
        private readonly Func<bool> canExecute;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RelayCommand class
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the RelayCommand class
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this.execute = execute;
            this.canExecute = canExecute;
        }
        #endregion

        #region Implementation of ICommand

        /// <summary>
        /// Définit la méthode à appeler lorsque la commande est appelée.
        /// </summary>
        /// <param name="parameter">Données utilisées par la commande.Si la commande ne requiert pas que les données soient passées, cet objet peut avoir la valeur null.</param>
        public void Execute(object parameter)
        {
            execute();
        }

        /// <summary>
        /// Définit la méthode qui détermine si la commande peut s'exécuter dans son état actuel.
        /// </summary>
        /// <returns>
        /// true si cette commande peut être exécutée ; sinon false.
        /// </returns>
        /// <param name="parameter">Données utilisées par la commande.Si la commande ne requiert pas que les données soient passées, cet objet peut avoir la valeur null.</param>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

       // public event EventHandler CanExecuteChanged;

        #endregion

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }
    }
}
