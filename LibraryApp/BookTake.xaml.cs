using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
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
    /// Interaction logic for BookTake.xaml
    /// </summary>
    public partial class BookTake : UserControl
    {
        SqlConnection conn;
        DataSet dataSet;
        SqlDataAdapter dataAdapter;
        string cs="";
        DataRowView DataRowView;
        int studentIdl;
        string bookId;
        int count = 100;
        MainWindow MainWindow;
        public BookTake(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = new MainWindow();
            MainWindow = mainWindow;
            conn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;
                conn.Open();
                dataSet = new DataSet();

                dataAdapter = new SqlDataAdapter("select* from Books", conn);
                mainDataGrid.ItemsSource = null;
                dataAdapter.Fill(dataSet, "mybook");
                mainDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
            }
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.myGrid.Children.Remove(this);
        }

        private void bookTakeBtn_Click(object sender, RoutedEventArgs e)
        {
            SqlTransaction transaction = null;



            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;
                conn.Open();
                transaction = conn.BeginTransaction();
                ++count;
                SqlCommand command = new SqlCommand("sp_SignInAndTakeBook", conn);
                command.CommandType = CommandType.StoredProcedure;



                var param1 = new SqlParameter();
                param1.Value = count++;
                param1.ParameterName = "@SCardId";
                param1.SqlDbType = SqlDbType.NVarChar;
                command.Parameters.Add(param1);



                var param2 = new SqlParameter();
                param2.Value = 2;
                param2.ParameterName = "@StudentId";
                param2.SqlDbType = SqlDbType.NVarChar;
                command.Parameters.Add(param2);
                command.Transaction = transaction;



                var param3 = new SqlParameter();
                param3.Value = bookId;
                param3.ParameterName = "@BookId";
                param3.SqlDbType = SqlDbType.NVarChar;
                command.Parameters.Add(param3);



                var param4 = new SqlParameter();
                param4.Value = DateTime.Now;
                param4.ParameterName = "@DateOut";
                param4.SqlDbType = SqlDbType.DateTime;
                command.Parameters.Add(param4);



                var param5 = new SqlParameter();
                param5.Value = DateTime.Now;
                param5.ParameterName = "@DateIn";
                param5.SqlDbType = SqlDbType.DateTime;
                command.Parameters.Add(param5);




                var param6 = new SqlParameter();
                param6.Value = 1;
                param6.ParameterName = "@IdLib";
                param6.SqlDbType = SqlDbType.NVarChar;
                command.Parameters.Add(param6);

                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    MessageBox.Show("Take Book Succesfly");
                    using (conn = new SqlConnection())
                    {
                        conn.ConnectionString = cs;
                        conn.Open();
                        dataSet = new DataSet();

                        dataAdapter = new SqlDataAdapter("select* from S_Cards", conn);
                        mainDataGrid.ItemsSource = null;
                        dataAdapter.Fill(dataSet, "mybook");
                        mainDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conn.Close();
                }
            }



        }



        private void MyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataRowView = mainDataGrid.SelectedItem as DataRowView;



            if (DataRowView != null)
            {
                bookId = DataRowView["Id"].ToString();
            }
        }
    }
}
