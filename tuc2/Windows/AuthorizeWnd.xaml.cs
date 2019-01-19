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

namespace tuc2.Windows
{
    /// <summary>
    /// Interaction logic for AuthorizeWnd.xaml
    /// </summary>
    public partial class AuthorizeWnd : UserControl
    {
        private readonly ApplicationContext dbContext;
        public AuthorizeWnd()
        {
            InitializeComponent();
            dbContext = new ApplicationContext();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = this.txtUsername.Text;
            var password = this.txtPassword.Password;
            var authUserRole = UserRoles.None;
            var user = dbContext.Users.SingleOrDefault(u => u.Login == username && u.Password == password);
            if (user != null)
            {
                if (user.Role.Id == RolesInfo.AdminId)
                    authUserRole = UserRoles.Admin;
                else
                    authUserRole = UserRoles.User;
            }
            else
            {
                var boxHeader = "Некоректні дані для входу";
                var boxText = "Ви ввели неправильний логін/пароль!";
                MessageBox.Show(boxText, boxHeader, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.HideLoginWindow(authUserRole, user);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var thisWindow = Window.GetWindow(this);
            thisWindow.Close();
        }
    }
}
