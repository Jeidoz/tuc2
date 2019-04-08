using System.Windows;
using System.Windows.Controls;
using tuc2.ViewModels;
using tuc2.Windows;

namespace tuc2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLoginWindow();
        }

        public void ShowLoginWindow()
        {
            gridMain.Children.Clear();
            gridMain.Children.Add(new AuthorizeWnd());
        }
        public void HideLoginWindow(UserViewModel loginedUser)
        {
            gridMain.Children.Clear();

            UserControl nextWnd;
            if (loginedUser.RoleType == RolesInfo.Admin)
                nextWnd = new AdminMenuWnd(loginedUser);
            else
                nextWnd = new UserMenuWnd(loginedUser);

            gridMain.Children.Add(nextWnd);
        }
    }
}
