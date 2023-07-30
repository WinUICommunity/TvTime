﻿using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class LayoutSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public List<string> breadCrumbBarCollection = new();

    public IJsonNavigationViewService JsonNavigationViewService;
    public LayoutSettingViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        BreadCrumbBarCollection.Add(App.Current.Localizer.GetLocalizedString("LayoutSettingPage_BreadCrumbBar"));
    }

    [RelayCommand]
    private void GoToStyleSettingPage(object sender)
    {
        var item = sender as SettingsCard;
        if (item.Tag != null)
        {
            Type pageType = Application.Current.GetType().Assembly.GetType($"TvTime.Views.{item.Tag}");

            if (pageType != null)
            {
                SlideNavigationTransitionInfo entranceNavigation = new SlideNavigationTransitionInfo();
                JsonNavigationViewService.NavigateTo(pageType, item.Header, false, entranceNavigation);
            }
        }
    }
}
