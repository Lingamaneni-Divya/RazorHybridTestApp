// MainWindow.xaml.cs
using System;
using System.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
{
    InitializeComponent();

    MyWpfPage myWpfPage = new MyWpfPage();
    myWpfPage.SomethingHappened += HandleSomethingHappened;

    MainFrame.Navigate(myWpfPage);

    WebView.NavigationCompleted += WebView_NavigationCompleted;
    WebView.Source = new Uri("wwwroot/index.html", UriKind.Relative);
}


    private void HandleSomethingHappened(object sender, EventArgs e)
    {
        MessageBox.Show("Button clicked in WPF Page!");
    }

    private async void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
        await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
            "function notifyWpfApplication() {" +
            "  DotNet.invokeMethod('WpfApp', 'HandleSomethingHappened');" +
            "}");
    }
}
