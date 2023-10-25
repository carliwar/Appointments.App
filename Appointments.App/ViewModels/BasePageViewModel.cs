﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Appointments.App.ViewModels
{
    public class BasePageViewModel : INotifyPropertyChanged
    {
        public DateTime Today { get => DateTime.Today; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<TData>(ref TData storage, TData value, [CallerMemberName] string propertyName = "")
        {
            if (storage?.Equals(value) == true)
                return;

            storage = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
