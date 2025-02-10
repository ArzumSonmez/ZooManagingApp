using System;
using System.Collections.Generic;
using System.Collections;
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

namespace WPF_ZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["WPF_ZooManager.Properties.Settings.qhTConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAllAnimals();
        
        }

        private void ShowAllAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    listAllAnimals.DisplayMemberPath = "Name";
                    listAllAnimals.SelectedValuePath = "Id";
                    listAllAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

           
        }
        private void ShowZoos()
        {
            try
            {
                string query = "select * from Zoo";
                // the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);
                    //Which Information of the Table in DataTable should be shown in our ListBox?
                    listZoos.DisplayMemberPath = "Locations";
                    //Which Value should be delivered, when an Item from our ListBox is selected?
                    listZoos.SelectedValuePath = "Id";
                    //The Reference to the Data the ListBox should populate
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch(Exception e) 
            {
                MessageBox.Show(e.ToString());
            }
        }


        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = "select * from Animal a inner join ZooAnimal " +
                    "za on a.Id = za.Animalid where za.Zooid = @Zooid";

                //To be able to use the @zooid as a variable put sql command
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //this time instead of query and connection string we are passing sqlCommand to be able to use @zooid
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);
                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
               // MessageBox.Show(e.ToString());
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            ShowSelectedZoosInTextBox();
        }

    
        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where Id = @Zooid";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally 
            { 
                sqlConnection.Close();
                ShowZoos();
            }  
            
        }

        private void AddZoo_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Locations)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Locations", myTextBox.Text);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@Zooid, @Animalid)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Animalid", listAllAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Animalid)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Animalid", myTextBox.Text);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllAnimals();
            }
        }

       private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where Id = @Animalid";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Animalid", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }
       

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where Id = @Animalid";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Animalid", listAllAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllAnimals();
            }
        }

        private void ShowSelectedZoosInTextBox()
        {
            try
            {
                string query = "select Locations from Zoo where Id = @Zooid";

                //To be able to use the @zooid as a variable put sql command
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //this time instead of query and connection string we are passing sqlCommand to be able to use @zooid
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);
                    DataTable zooDataTable = new DataTable();

                    sqlDataAdapter.Fill(zooDataTable);
                    myTextBox.Text = zooDataTable.Rows[0]["Locations"].ToString();
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        private void ShowSelectedAnimalsInTextBox()
        {
            try
            {
                string query = "select name from Animal where Id = @Animalid";

                //To be able to use the @zooid as a variable put sql command
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //this time instead of query and connection string we are passing sqlCommand to be able to use @zooid
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@Animalid", listAllAnimals.SelectedValue);
                    DataTable zooDataTable = new DataTable();

                    sqlDataAdapter.Fill(zooDataTable);
                    myTextBox.Text = zooDataTable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        private void listAllAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalsInTextBox();
        }
        private void UpdateZoo_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo Set Locations = @Locations where Id = @Zooid";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Zooid", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Locations", myTextBox.Text);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void UpdateAnimal_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Animal Set Name = @Name where Id = @Animalid";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                //Easy way to execute sql command which execute query with sql connection
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Animalid", listAllAnimals.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllAnimals();
            }
        }
    }
}