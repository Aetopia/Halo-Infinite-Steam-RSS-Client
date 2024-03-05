using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Xml;

class MainWindow : Window
{
    public MainWindow()
    {
        Title = "Halo Infinite Steam RSS Client";
        Width = MinWidth = 640;
        Height = MinHeight = 480;
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        UniformGrid uniformGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Columns = 2
        };
        Content = new ScrollViewer { Content = uniformGrid };

        List<Tuple<string, string, string>> tuples = [];
        try
        {
            using WebClient webClient = new();
            XmlDocument xmlDocument = new();
            xmlDocument.LoadXml(webClient.DownloadString("https://store.steampowered.com/feeds/news/app/1240440"));
            foreach (XmlNode xmlNode in xmlDocument.SelectNodes("rss/channel/item"))
                tuples.Add(new(xmlNode["enclosure"].GetAttribute("url"), xmlNode["title"].InnerText, xmlNode["link"].InnerText));
        }
        catch
        {
            MessageBox.Show("Couldn't fetch RSS Feed.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Environment.Exit(1);
        }

        foreach (Tuple<string, string, string> tuple in tuples)
        {
            int value = uniformGrid.Rows;
            uniformGrid.Rows++;

            Image image = new() { Source = new BitmapImage(new Uri(tuple.Item1)) };
            Grid.SetColumn(image, 0);
            Grid.SetRow(image, value);
            uniformGrid.Children.Add(image);

            Button button = new() { Content = new TextBlock { Text = tuple.Item2, TextWrapping = TextWrapping.Wrap } };
            button.Click += (sender, e) => Process.Start(tuple.Item3);
            Grid.SetColumn(button, 1);
            Grid.SetRow(button, value);
            uniformGrid.Children.Add(button);
        }
    }
}

class Program
{
    [STAThread]
    static void Main() { new MainWindow().ShowDialog(); }
}