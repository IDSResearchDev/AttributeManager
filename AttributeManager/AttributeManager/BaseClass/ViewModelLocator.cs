﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ConnectionCreator
{
    public static class ViewModelLocator
    {
        public static bool GetAutoViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoViewModelProperty);
        }

        public static void SetAutoViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoViewModelProperty =
            DependencyProperty.RegisterAttached("AutoViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, AutoWireViewModelChanged));

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;
            var viewType = d.GetType();
            var viewTypeName = viewType.FullName;

            var viewModelTypeName = viewTypeName + "Model";
            var viewModelType = Type.GetType(viewModelTypeName);
            var viewModel = Activator.CreateInstance(viewModelType);
            ((FrameworkElement)d).DataContext = viewModel;
        }


    }
}
