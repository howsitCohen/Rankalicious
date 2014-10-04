using System;
using System.Windows;

namespace RankaliciousWPF.Controls
{
    public interface IViewLocator
    {
        UIElement GetOrCreateViewType(Type viewType);
    }
}