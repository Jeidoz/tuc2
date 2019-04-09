using System.Windows;
using System.Windows.Controls;
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
            LoginedUser = loginedUser;

            InitializeComponent();
            InitializeDefaultWindow();

            this.txtLoginedUsername.Text = LoginedUser.Login;
        }

        private void InitializeDefaultWindow()
        {
            var usc = new UsersCrudWnd(LoginedUser);
            GridMain.Children.Add(usc);
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonUserInfo.Visibility = Visibility.Visible;
        }
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonUserInfo.Visibility = Visibility.Collapsed;
        }
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();
            var item = ((ListView)sender).SelectedItem as ListViewItem;

            switch (item.Name)
            {
                case "ItemLogout":
                    var wnd = WpfHelper.GetMainWindow(this);
                    wnd.ShowLoginWindow();
                    return;
                case "ItemUsers":
                    usc = new UsersCrudWnd(LoginedUser);
                    break;
                case "ItemTasks":
                    usc = new TasksCrudWnd();
                    break;
                default:
                    break;
            }
            GridMain.Children.Add(usc);
        }
    }
}
