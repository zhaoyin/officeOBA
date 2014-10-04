#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg., Weegen Patrick 2009-2013.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

using System.ComponentModel;
using System.Windows.Input;
using Fluent.Sample.Mvvm.Comands;
using Fluent.Sample.Mvvm.Model;

namespace Fluent.Sample.Mvvm.ViewModels
{
    /// <summary>
    /// Represents main view model
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // Raise PropertyChanged event
        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Fields

        // Current person
        Person current;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets current person
        /// </summary>
        public Person Current
        {
            get { return current; }
            set
            {
                if (current == value) return;
                current = value;
                RaisePropertyChanged("Current");
            }
        }

        /// <summary>
        /// Gets persons
        /// </summary>
        public PersonCollection Persons { get; private set; }

        #endregion

        #region Commands

        #region Exit

        private RelayCommand _exitCommand;

        /// <summary>
        /// Exit from the application
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(x => System.Windows.Application.Current.Shutdown());
                return _exitCommand;
            }
        }

        #endregion
        
        #region Delete

        private RelayCommand _deleteCommand;

        /// <summary>
        /// Delete this person
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                    _deleteCommand = new RelayCommand(x => DeleteCurrentPerson(), x => current != null);
                return _deleteCommand;
            }
        }

        // Deletes current person
        void DeleteCurrentPerson()
        {
            if (current == null)
            	return;
            Person deleted = current;

            if (Persons.Count != 1)
            {
                int index = Persons.IndexOf(deleted);
                Current = Persons[index == 0 ? 1 : index - 1];
            }
            else
            {
                Current = null;
                _deleteCommand.RaiseCanExecuteChanged();
            }

            Persons.Remove(deleted);
            
        }

        #endregion

        #region Create

        private RelayCommand _createCommand;

        /// <summary>
        /// Delete this person
        /// </summary>
        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                    _createCommand = new RelayCommand(x => CreatePerson());
                return _createCommand;
            }
        }

        // Creates person
        void CreatePerson()
        {
        	Persons.Insert(0, new Person());
            Current = Persons[0];
            _deleteCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel()
        {
        	Persons = PersonCollection.Generate();
            if (Persons.Count > 0)
            	Current = Persons[0];
        }

        #endregion
    }
}