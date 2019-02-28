using LiteDB;
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
using Tuc2DDL;

namespace tuc2.Windows
{
    /// <summary>
    /// Interaction logic for AuthorizeWnd.xaml
    /// </summary>
    public partial class AuthorizeWnd : UserControl
    {
        private DbContext db;

        public AuthorizeWnd()
        {
            this.db = new DbContext(); 
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = this.txtUsername.Text;
            var password = this.txtPassword.Password;
            var authUserRole = UserRoles.None;

            var loginedUser = this.db.GetUser(username);

            if (this.db.IsUserExist(username) && this.db.IsUserPasswordCorrect(username, password))
            {
                if (loginedUser.Role.Type == "admin")
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
            var loginedUserViewModel = new UserViewModel
            {
                Id = loginedUser.Id,
                Login = loginedUser.Login,
                Password = password,
                FirstName = loginedUser.FirstName,
                LastName = loginedUser.LastName,
                RoleType = authUserRole == UserRoles.Admin ? RolesInfo.Admin : RolesInfo.User
            };
            mainWindow.HideLoginWindow(authUserRole, loginedUserViewModel);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var thisWindow = Window.GetWindow(this);
            thisWindow.Close();
        }
    }
}
