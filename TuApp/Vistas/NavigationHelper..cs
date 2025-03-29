using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Vistas;
public static class NavigationHelper
{
    public static async Task Navegar(Page page)
    {
        if (Application.Current.MainPage is FlyoutPage flyout &&
            flyout.Detail is NavigationPage navPage)
        {
            await navPage.Navigation.PushAsync(page);
        }
    }
}