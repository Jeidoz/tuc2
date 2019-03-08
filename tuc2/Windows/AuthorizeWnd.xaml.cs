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
            this.db = WpfHelper.Database;
            InitializeComponent();
        }

        private bool IsAuthorizationFailed(string username, string password)
        {
            bool isUsernameExist = this.db.IsUserExist(username);
            bool isPasswordCorrect = this.db.IsUserPasswordCorrect(username, password);
            return !(isUsernameExist && isPasswordCorrect);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = this.txtUsername.Text;
            var password = this.txtPassword.Password;
            var loginedUser = this.db.GetUser(username);

            if (IsAuthorizationFailed(username, password))
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
                RoleType = loginedUser.Role.Type
            };
            mainWindow.HideLoginWindow(loginedUserViewModel);
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var thisWindow = Window.GetWindow(this);
            thisWindow.Close();
        }
    }
}
