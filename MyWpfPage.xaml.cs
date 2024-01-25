// MyWpfPage.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;

public partial class MyWpfPage : Page
{
    public event EventHandler SomethingHappened;

    public MyWpfPage()
    {
        InitializeComponent();
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        // When the button is clicked, notify subscribers (e.g., the main window)
        NotifySomethingHappened();
    }

    private void NotifySomethingHappened()
    {
        SomethingHappened?.Invoke(this, EventArgs.Empty);
    }
}
