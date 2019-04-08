using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using tuc2.ViewModels;
using Tuc2DDL;
using Tuc2DDL.Entities;

namespace tuc2.Windows.AdminControls
{
    /// <summary>
    /// Interaction logic for UsersCrudWnd.xaml
    /// </summary>
    public partial class UsersCrudWnd : UserControl
    {
        private DbContext db;
        private ObservableCollection<string> usersList;
        private UserViewModel loginedUser;
        private bool isNewUser = false;

        public int SelectedIndexValue { get; set; }

        public UsersCrudWnd(UserViewModel usr)
        {
            db = WpfHelper.Database;
            loginedUser = usr;
            var users = db.Users.FindAll();
            usersList = new ObservableCollection<string>();
            foreach(var user in users)
            {
                usersList.Add(user.Login);
            }
            SelectedIndexValue = usersList.IndexOf(loginedUser.Login);

            DataContext = this;

            InitializeComponent();
            this.ListViewUsers.ItemsSource = usersList;
        }

        //TO-DO
        //REPLACE "CHANGE PASSWORD" TO "make new password"
        private void ListViewUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedIndexValue = this.ListViewUsers.SelectedIndex;
            if (SelectedIndexValue == -1)
                SelectedIndexValue = 0;
            var userLogin = usersList[SelectedIndexValue];
            var selectedUser = db.GetUser(userLogin);
            FillOrClearFields(selectedUser.Login,
                null,
                selectedUser.FirstName,
                selectedUser.LastName,
                selectedUser.Role.Type == "admin" ? RolesInfo.Admin : RolesInfo.User);
        }
        private void FillOrClearFields(string login = "", string password = "", string firstName = "", string lastName = "", string roleType = RolesInfo.User)
        {
            this.txtUsername.Text = login;
            this.txtPassword.Text = password;
            this.txtFirstName.Text = firstName;
            this.txtLastName.Text = lastName;
            this.cmbRole.SelectedIndex = roleType == RolesInfo.Admin ? 0 : 1;
        }
        private void BtnAddNewUser_Click(object sender, RoutedEventArgs e)
        {
            FillOrClearFields();
            isNewUser = true;
        }
        private bool IsLoginReserved(string login)
        {
            return db.IsUserExist(login);
        }
        private bool IsAllFieldsFilled()
        {
            bool login = !string.IsNullOrWhiteSpace(this.txtUsername.Text);
            //bool password = !string.IsNullOrWhiteSpace(this.txtPassword.Text);
            bool firstName = !string.IsNullOrWhiteSpace(this.txtFirstName.Text);
            bool lastName = !string.IsNullOrWhiteSpace(this.txtLastName.Text);
            bool role = this.cmbRole.SelectedIndex != -1;

            return (login && firstName && lastName && role);
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if(!IsAllFieldsFilled())
            {
                MessageBox.Show(
                    "Для збереження користувача необхідно заповнити всі поля і обрати його роль",
                    "Пусті поля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                return;
            }

            // obj that save into DB
            var newUser = new User
            {
                FirstName = this.txtFirstName.Text,
                LastName = this.txtLastName.Text,
                Login = this.txtUsername.Text,
                PasswordSalt = this.db.GetSalt(),
                Role = this.cmbRole.SelectedIndex == 0 ? this.db.GetRole("admin") : this.db.GetRole("user")
            };
            newUser.PasswordHash = this.db.GetSaltedHash(this.txtPassword.Text, newUser.PasswordSalt);

            if (isNewUser)
            {
                if (IsLoginReserved(newUser.Login))
                {
                    var errorText = $"Користувач із іменем {newUser.Login} уже існує в базі даних";
                    var errorHeader = "Ім'я користувача зайняте";
                    MessageBox.Show(errorText, errorHeader, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
                this.db.Users.Insert(newUser);
                usersList.Add(newUser.Login);
                isNewUser = false;
                SelectedIndexValue = usersList.IndexOf(newUser.Login);
            }
            else
            {
                var selectedUserIndex = this.ListViewUsers.SelectedIndex;
                var selectedUserLogin = usersList[selectedUserIndex];
                var selectedUser = this.db.GetUser(selectedUserLogin);
                if (selectedUser == null)
                    return;

                selectedUser.Login = newUser.Login;
                if(!string.IsNullOrWhiteSpace(this.txtPassword.Text))
                {
                    selectedUser.PasswordHash = this.db.GetSaltedHash(this.txtPassword.Text, selectedUser.PasswordSalt);
                }
                selectedUser.FirstName = newUser.FirstName;
                selectedUser.LastName = newUser.LastName;
                selectedUser.Role = newUser.Role;

                this.db.Users.Update(selectedUser);
                usersList[selectedUserIndex] = selectedUser.Login;
                SelectedIndexValue = selectedUserIndex;
            }
            
            this.ListViewUsers.SelectedIndex = SelectedIndexValue;
            var messageText = $"Користувач із іменем {usersList[SelectedIndexValue]} був успішно збережений у базу даних";
            var messageHeader = "Успішне збереження користувача";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var oldIndex = this.ListViewUsers.SelectedIndex;
            var username = usersList[oldIndex];
            var user = this.db.GetUser(username);
            this.db.Users.Delete(user.Id);
            this.ListViewUsers.SelectedIndex = oldIndex - 1;
            usersList.Remove(username);

            var messageText = $"Користувач із іменем {username} був успішно видалений із бази даних";
            var messageHeader = "Успішне видалення користувача";
            MessageBox.Show(messageText, messageHeader, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
