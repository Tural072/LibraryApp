using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace LibraryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        string cs;
        int count;
        public MainWindow()
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
        }

        private void goBtn_Click(object sender, RoutedEventArgs e)
        {
            SqlTransaction transaction = null;
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;
                conn.Open();
                transaction = conn.BeginTransaction();
                ++count;
                SqlCommand command = new SqlCommand("sp_CheckStudents", conn);
                command.CommandType = CommandType.StoredProcedure;



                var param1 = new SqlParameter();
                param1.Value = nameTxtbx.Text.ToString();
                param1.ParameterName = "@Firstname";
                param1.SqlDbType = SqlDbType.NVarChar;
                command.Parameters.Add(param1);



                var param2 = new SqlParameter();
                param2.Value = surenameTxtbx.Password;
                param2.ParameterName = "@Password";
                param2.SqlDbType = SqlDbType.NVarChar;
                command.Parameters.Add(param2);
                command.Transaction = transaction;


                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    BookTake BookTake = new BookTake(this);
                    myGrid.Children.Add(BookTake);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conn.Close();
                }
            }
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
