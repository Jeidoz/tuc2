using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tuc2.ViewModels;
using tuc2.Windows.AdminControls;

namespace tuc2.Windows
{
    /// <summary>
    /// Interaction logic for AdminMenuWnd.xaml
    /// </summary>
    public partial class AdminMenuWnd : UserControl
    {
        public UserViewModel LoginedUser { get; set; }

        public AdminMenuWnd(UserViewModel loginedUser)
        {
            InitializeComponent();
            LoginedUser = loginedUser;
            InitializeDefaultWindow();
        }

        private void InitializeDefaultWindow()
        {
            UserControl usc = new UsersCrudWnd(LoginedUser);
            GridMain.Children.Clear();
            GridMain.Children.Add(usc);
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();
            ListViewItem item = ((ListView)sender).SelectedItem as ListViewItem;

            switch (item.Name)
            {
                case "ItemLogout":
                    MainWindow wnd = (MainWindow)Window.GetWindow(this);
                    wnd.ShowLoginWindow();
                    break;
                case "ItemUsers":
                    usc = new UsersCrudWnd(LoginedUser);
                    GridMain.Children.Add(usc);
                    break;
                case "ItemTasks":
                    usc = new TasksCrudWnd();
                    GridMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }
    }
}
