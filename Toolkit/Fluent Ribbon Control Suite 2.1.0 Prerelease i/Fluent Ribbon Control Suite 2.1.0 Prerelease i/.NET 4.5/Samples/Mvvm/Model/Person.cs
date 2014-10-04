#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg., Weegen Patrick 2009-2013.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluent.Sample.Mvvm.Model
{
    /// <summary>
    /// Represents person
    /// </summary>
    public class Person : INotifyPropertyChanged
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

        // Name
        private string _name = "Untitled";
        // E-mail
        private string _email;
        // Phone
        private string _phone;
        // Photo
        private ImageSource _photo;

        #endregion

        #region Properies

        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                	return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets e-mail
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email == value)
                	return;
                _email = value;
                RaisePropertyChanged("Email");
            }
        }


        /// <summary>
        /// Gets or sets phone
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone == value)
                	return;
                _phone = value;
                RaisePropertyChanged("Phone");
            }
        }

        /// <summary>
        /// Gets or sets photo
        /// </summary>
        public ImageSource Photo
        {
            get { return _photo; }
            set
            {
                if (_photo == value)
                	return;
                _photo = value;
                RaisePropertyChanged("Photo");
            }
        }

        #endregion

        #region Constructor
        
        public Person()
        { }

        /// <summary>
        /// Creates new person
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="email">E-mail</param>
        /// <param name="phone">Phone</param>
        /// <param name="photo">Photo</param>
        /// <returns>Person</returns>
        public Person(string name, string email, string phone, ImageSource photo)
        {
            _name = name;
            _email = email;
            _phone = phone;
            _photo = photo;
        }

        #endregion
    }
}
