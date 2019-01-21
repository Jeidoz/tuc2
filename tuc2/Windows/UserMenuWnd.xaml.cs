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
using tuc2.Entities;
using tuc2.Windows.UserControls;

namespace tuc2.Windows
{
    /// <summary>
    /// Interaction logic for UserMenuWnd.xaml
    /// </summary>
    public partial class UserMenuWnd : UserControl
    {
        public User LoginedUser { get; set; }

        public UserMenuWnd(User user)
        {
            LoginedUser = user;
            InitializeComponent();
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
            MainWindow mainWindow = null;
            switch (item.Name)
            {
                case "ItemHome":
                    usc = new TaskSolverWnd();
                    GridMain.Children.Add(usc);
                    break;
                case "ItemLogout":
                    mainWindow = (MainWindow)Window.GetWindow(this);
                    mainWindow.ShowLoginWindow();
                    break;
                default:
                    break;
            }
        }
    }
}
