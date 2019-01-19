using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using tuc2.Windows;

namespace tuc2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AuthorizeWnd loginWnd;
        public MainWindow()
        {
            InitializeComponent();
            loginWnd = new AuthorizeWnd();
            gridMainWindow.Children.Add(loginWnd);
        }

        public void ShowLoginWindow()
        {
            gridMainWindow.Children.Clear();
            loginWnd = new AuthorizeWnd();
            gridMainWindow.Children.Add(loginWnd);
        }

        public void HideLoginWindow(UserRoles userRole, User loginedUser)
        {
            gridMainWindow.Children.Remove(loginWnd);
            if (userRole == UserRoles.User)
            {
                var userInterface = new UserMenuWnd();
                this.gridMainWindow.Children.Add(userInterface);
            }
            else
            {
                var userInterface = new AdminMenuWnd(loginedUser);
                this.gridMainWindow.Children.Add(userInterface);
            }
        }
    }
}
