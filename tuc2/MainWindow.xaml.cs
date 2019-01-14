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

namespace tuc2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext dbContext;
        public MainWindow()
        {
            InitializeComponent();

            dbContext = new ApplicationContext();
            dbContext.Users.Load();
            this.DataContext = dbContext.Users.Local.ToBindingList();
        }

        // добавление
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            UserWindow UserWindow = new UserWindow(new User());
            if (UserWindow.ShowDialog() == true)
            {
                User User = UserWindow.User;
                dbContext.Users.Add(User);
                dbContext.SaveChanges();
            }
        }
        // редактирование
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (UsersList.SelectedItem == null) return;
            // получаем выделенный объект
            User User = UsersList.SelectedItem as User;

            UserWindow UserWindow = new UserWindow(new User
            {
                Id = User.Id,
                Login = User.Login,
                Password = User.Password,
                FirstName = User.FirstName,
                LastName = User.LastName
            });

            if (UserWindow.ShowDialog() == true)
            {
                // получаем измененный объект
                User = dbContext.Users.Find(UserWindow.User.Id);
                if (User != null)
                {
                    User.Login = UserWindow.User.Login;
                    User.FirstName = UserWindow.User.FirstName;
                    User.LastName = UserWindow.User.LastName;
                    User.Password = UserWindow.User.Password;
                    dbContext.Entry(User).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }
        // удаление
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (UsersList.SelectedItem == null) return;
            // получаем выделенный объект
            User User = UsersList.SelectedItem as User;
            dbContext.Users.Remove(User);
            dbContext.SaveChanges();
        }
    }
}
