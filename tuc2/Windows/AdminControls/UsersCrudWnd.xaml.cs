using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace tuc2.Windows.AdminControls
{
    /// <summary>
    /// Interaction logic for UsersCrudWnd.xaml
    /// </summary>
    public partial class UsersCrudWnd : UserControl
    {
        private readonly ApplicationContext context;
        private ObservableCollection<string> usersList;
        private User loginedUser;
        private bool isNewUser = false;

        public int SelectedIndexValue { get; set; }

        public UsersCrudWnd(User usr)
        {
            context = new ApplicationContext();
            loginedUser = usr;
            usersList = new ObservableCollection<string>(context.Users.Select(user => user.Login));
            SelectedIndexValue = usersList.IndexOf(loginedUser.Login);

            DataContext = this;

            InitializeComponent();
            this.ListViewUsers.ItemsSource = usersList;
            
        }

        private void ListViewUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var userLogin = usersList[this.ListViewUsers.SelectedIndex];
            var userInfo = context.Users.Include(u => u.Role).SingleOrDefault(user => user.Login == userLogin);
            FillOrClearFields(userInfo.Login, userInfo.Password, userInfo.FirstName, userInfo.LastName, userInfo.Role.Id == RolesInfo.AdminId ? 0 : 1);
        }

        private void FillOrClearFields(string login = "", string password = "", string firstName = "", string lastName = "", int roleId = -1)
        {
            this.txtUsername.Text = login;
            this.txtPassword.Text = password;
            this.txtFirstName.Text = firstName;
            this.txtLastName.Text = lastName;
            this.cmbRole.SelectedIndex = roleId;
        }

        private void BtnAddNewUser_Click(object sender, RoutedEventArgs e)
        {
            FillOrClearFields();
            isNewUser = true;
        }

        private bool IsLoginReserved(string login)
        {
            return context.Users.Any(u => u.Login == login);
        }

        private bool IsAllFieldsFilled()
        {
            bool login = !string.IsNullOrWhiteSpace(this.txtUsername.Text);
            bool password = !string.IsNullOrWhiteSpace(this.txtPassword.Text);
            bool firstName = !string.IsNullOrWhiteSpace(this.txtFirstName.Text);
            bool lastName = !string.IsNullOrWhiteSpace(this.txtLastName.Text);
            bool role = this.cmbRole.SelectedIndex != -1;

            return (login && password && firstName && lastName && role);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if(!IsAllFieldsFilled())
            {
                MessageBox.Show("Для збереження користувача необхідно заповнити всі поля і обрати його роль", "Пусті поля", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var newUser = new User
            {
                Login = this.txtUsername.Text,
                Password = this.txtPassword.Text,
                FirstName = this.txtFirstName.Text,
                LastName = this.txtLastName.Text,
                Role = context.Roles.SingleOrDefault(role => role.Id == this.cmbRole.SelectedIndex + 1)
            };
            
            if (isNewUser)
            {
                if (IsLoginReserved(newUser.Login))
                {
                    var errorText = $"Користувач із іменем {newUser.Login} уже існує в базі даних";
                    var errorHeader = "Ім'я користувача зайняте";
                    MessageBox.Show(errorText, errorHeader, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
                context.Users.Add(newUser);
                context.SaveChanges();
                usersList.Add(newUser.Login);
                isNewUser = false;
            }
            else
            {
                var selectedUserIndex = this.ListViewUsers.SelectedIndex;
                var selectedUserLogin = usersList[selectedUserIndex];
                var selectedUser = context.Users.SingleOrDefault(u => u.Login == selectedUserLogin);
                if (selectedUser == null)
                    return;

                selectedUser.Login = newUser.Login;
                selectedUser.Password = newUser.Password;
                selectedUser.FirstName = newUser.FirstName;
                selectedUser.LastName = newUser.LastName;
                selectedUser.Role = newUser.Role;

                context.SaveChanges();
            }
            
            var index = usersList.IndexOf(newUser.Login);
            this.ListViewUsers.SelectedIndex = index;
            var messageText = $"Користувач із іменем {newUser.Login} був успішно збережений у базу даних";
            var messageHeader = "Успішне збереження користувача";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var oldIndex = this.ListViewUsers.SelectedIndex;
            var username = usersList[oldIndex];
            var user = context.Users.SingleOrDefault(usr => usr.Login == username);
            context.Users.Remove(user);
            context.SaveChanges();
            this.ListViewUsers.SelectedIndex = oldIndex - 1;
            usersList.Remove(username);

            var messageText = $"Користувач із іменем {username} був успішно видалений із бази даних";
            var messageHeader = "Успішне видалення користувача";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
