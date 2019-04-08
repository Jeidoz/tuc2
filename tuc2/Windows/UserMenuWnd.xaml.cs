using System.Windows;
using System.Windows.Controls;
using tuc2.ViewModels;
using tuc2.Windows.UserControls;

namespace tuc2.Windows
{
    /// <summary>
    /// Interaction logic for UserMenuWnd.xaml
    /// </summary>
    public partial class UserMenuWnd : UserControl
    {
        public UserViewModel LoginedUser { get; set; }

        public UserMenuWnd(UserViewModel user)
        {
            LoginedUser = user;
            InitializeComponent();
            InitializeDefaultWindow();
        }

        private void InitializeDefaultWindow()
        {
            UserControl usc = new TaskSolverWnd();
             
            GridMain.Children.Add(usc);
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Hidden;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Hidden;
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
