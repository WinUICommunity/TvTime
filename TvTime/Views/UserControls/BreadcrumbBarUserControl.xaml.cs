﻿using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class BreadcrumbBarUserControl : UserControl
{
    public List<string> Items
    {
        get => (List<string>) GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register("Items", typeof(List<string>), typeof(BreadcrumbBarUserControl), new PropertyMetadata(null));

    public string SingleItem
    {
        get => (string) GetValue(SingleItemProperty);
        set => SetValue(SingleItemProperty, value);
    }

    public static readonly DependencyProperty SingleItemProperty =
        DependencyProperty.Register("SingleItem", typeof(string), typeof(BreadcrumbBarUserControl), new PropertyMetadata(default(string)));

    public BreadCrumbBarViewModel ViewModel { get; }

    public BreadcrumbBarUserControl()
    {
        ViewModel = App.GetService<BreadCrumbBarViewModel>();
        this.InitializeComponent();
        Loaded += BreadcrumbBarUserControl_Loaded;
    }

    private void BreadcrumbBarUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.BreadcrumbBarCollection.Add(Application.Current.Resources["BreadCrumbBarRootText"] as string);
        if (Items != null)
        {
            foreach (var item in Items)
            {
                ViewModel.BreadcrumbBarCollection.Add(item);
            }
        }
        else
        {
            ViewModel.BreadcrumbBarCollection.Add(SingleItem);
        }
    }
}

