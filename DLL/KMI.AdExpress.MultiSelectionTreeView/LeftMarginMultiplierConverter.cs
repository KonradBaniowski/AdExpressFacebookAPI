﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MultiSelectionTreeView
{
    public class LeftMarginMultiplierConverter : IValueConverter 
    { 
        public double Length { get; set; } 
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            var item = value as MultipleSelectionTreeViewItem; 

            if (item == null)            
                return new Thickness(0); 
            
            return new Thickness(Length * item.GetDepth(), 0, 0, 0); 
        } 
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            return null;
        } 
    }
}

