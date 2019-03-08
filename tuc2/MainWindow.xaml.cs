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
using tuc2.Windows;
using Tuc2DDL;

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
