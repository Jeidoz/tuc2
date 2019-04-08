using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tuc2DDL;

namespace tuc2
{
    public static class WpfHelper
    {
        public static DbContext Database = new DbContext("Tuc2.db", "0951431404Tuc2");
        public static MainWindow GetMainWindow(UserControl control)
        {
            return (MainWindow)Window.GetWindow(control);
        }
    }
}
