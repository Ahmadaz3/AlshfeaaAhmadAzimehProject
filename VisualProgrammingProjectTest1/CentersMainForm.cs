using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VisualProgrammingProjectTest1
{
    public partial class CentersMainForm : Form
    {
        private int centerID;
        public CentersMainForm(int centerID)
        {
            InitializeComponent();
            this.centerID = centerID;
        }

        private void CentersMainForm_Load(object sender, EventArgs e)
        {
            // label1.Text = CenterID.CenterId.ToString();
            CenterNameTitle.Text = CenterID.Cname.ToString() + " Alshefaa";
          
                using (SqlConnection sqlConnection = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True"))
                {
                    sqlConnection.Open();
                    string query = @"SELECT case_date, case_description, case_status, CaseType, CaseGender, CaseAge, FromPlace, ArrivedTo,Time
                     FROM EmergencyCases
                     WHERE center_id = @CenterID
                         AND MONTH(case_date) = MONTH(GETDATE())
                         AND YEAR(case_date) = YEAR(GETDATE())";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@CenterID", CenterID.CenterId);

                    SqlDataReader reader = command.ExecuteReader();

                    // Clear existing items in the ListView
                    lvMonthlyCases.Items.Clear();
                    //for (int i = 0; i < reader.FieldCount; i++)
                    //{
                    //    lvMonthlyCases.Columns.Add(reader.GetName(i));
                    //}
                    lvMonthlyCases.Columns.Add("Case Date");
                    lvMonthlyCases.Columns.Add("Case Description");
                    lvMonthlyCases.Columns.Add("Case Status");
                    lvMonthlyCases.Columns.Add("Case Type");
                    lvMonthlyCases.Columns.Add("Case Gender");
                    lvMonthlyCases.Columns.Add("Case Age");
                    lvMonthlyCases.Columns.Add("Take Case From");
                    lvMonthlyCases.Columns.Add("Take Case To");
                    lvMonthlyCases.Columns.Add("Case Time");

                    while (reader.Read())
                    {
                        DateTime caseDate = ((DateTime)reader["case_date"]).Date; // Retrieve the date component only
                        string formattedCaseDate = caseDate.ToString("MM/dd/yyyy"); // Format the date as month/day/year
                        ListViewItem item = new ListViewItem(formattedCaseDate);
                        item.SubItems.Add(reader["case_description"].ToString());
                        item.SubItems.Add(reader["case_status"].ToString());
                        item.SubItems.Add(reader["CaseType"].ToString());
                        item.SubItems.Add(reader["CaseGender"].ToString());
                        item.SubItems.Add(reader["CaseAge"].ToString());
                        item.SubItems.Add(reader["FromPlace"].ToString());
                        item.SubItems.Add(reader["ArrivedTo"].ToString());
                        item.SubItems.Add(reader["Time"].ToString());
                        lvMonthlyCases.Items.Add(item);
                    }
                    reader.Close();
                    lvMonthlyCases.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
            
        }

        private void btnAddVoulnteer_Click(object sender, EventArgs e)
        {
            if (Vname.Text == string.Empty || Vadd.Text == string.Empty || Vcon.Text == string.Empty || VcmbBloodType.SelectedIndex < 0)
            {
                MessageBox.Show("Please Fill all Fields");
            }
            else
            {
                string name = Vname.Text.ToString();
                string Address = Vadd.Text.ToString();
                string contact = Vcon.Text.ToString();
                int age = (int)VnumAge.Value;
                string blood = VcmbBloodType.SelectedItem.ToString();
                //string cmdText = "INSERT INTO Volunteers (center_id, volunteer_name, volunteer_address, volunteer_contact, volunteer_age, BloodType, Volunteer_WorkingDays) ";
                //cmdText += "VALUES (@CenterID, @Name, @Address, @Contact, @Age, @Blood, @WorkingDays)";
                string cmdText = "INSERT INTO Volunteers (center_id, volunteer_name, volunteer_address, volunteer_contact, volunteer_age, BloodType, Volunteer_WorkingDays, AmbulanceCharacter, SuitMeasurement, ShoeMeasurement) ";
                cmdText += "VALUES (@CenterID, @Name, @Address, @Contact, @Age, @Blood, @WorkingDays, @Ambulance, @Suit, @Shoe)";

                using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True"))
                {
                    using (SqlCommand command = new SqlCommand(cmdText, connection))
                    {
                        // Set the parameter values
                        command.Parameters.AddWithValue("@CenterID", CenterID.CenterId);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@Contact", contact);
                        command.Parameters.AddWithValue("@Age", age);
                        command.Parameters.AddWithValue("@Blood", blood);
                        // Get the selected working days from the checked list and join them by , in a string
                        string workingDays = string.Join(",", VcheckedList.CheckedItems.Cast<string>());
                        // Add the working days parameter
                        command.Parameters.AddWithValue("@WorkingDays", workingDays);
                        // Get the selected ambulance character from the combo box
                        string ambulanceCharacter = VcmbAmbulanceCharacter.SelectedItem.ToString();

                        // Add the ambulance character parameter
                        command.Parameters.AddWithValue("@Ambulance", ambulanceCharacter);

                        // Get the selected suit measurement from the combo box
                        string suitMeasurement = VcmbSuitMeasurement.SelectedItem.ToString();

                        // Add the suit measurement parameter
                        command.Parameters.AddWithValue("@Suit", suitMeasurement);

                        // Get the shoe measurement from the numeric up-down control
                        decimal shoeMeasurement = VnumericUpDownShoe.Value;

                        // Add the shoe measurement parameter
                        command.Parameters.AddWithValue("@Shoe", shoeMeasurement);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            MessageBox.Show("Volunteer added successfully.");
                            Vname.Text = String.Empty;
                            Vadd.Text = String.Empty;
                            Vcon.Text = String.Empty;
                            VnumAge.Value = VnumAge.Minimum;
                            VcmbBloodType.SelectedIndex = -1;
                            VcmbAmbulanceCharacter.SelectedIndex = -1;
                            VcmbSuitMeasurement.SelectedIndex = -1;
                            VnumericUpDownShoe.Value = VnumericUpDownShoe.Minimum;
                            for (int i = 0; i < VcheckedList.Items.Count; i++)
                            {
                                VcheckedList.SetItemChecked(i, false);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message);
                        }
                    }

                }
            }
        }

        private void BtnFindBlood_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True");

            if (VcmbBloodTypes.SelectedIndex <= -1)
            {
                MessageBox.Show("Please Select a Blood Type");
            }
            else
            {
                try
                {
                    conn.Open();
                    string selectedBloodType = VcmbBloodTypes.SelectedItem.ToString();
                    string query = "SELECT volunteer_name, volunteer_address, volunteer_contact FROM Volunteers WHERE BloodType = @BloodType AND center_id = @CenterID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BloodType", selectedBloodType);
                    cmd.Parameters.AddWithValue("@CenterID", CenterID.CenterId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear existing items in the ListView
                    lvAvaBlood.Items.Clear();
                    lvAvaBlood.Columns.Clear();
                    // Add column headers to the ListView
                    //for (int i = 0; i < reader.FieldCount; i++)
                    //{
                    //    lvAvaBlood.Columns.Add(reader.GetName(i));
                    //}
                    lvAvaBlood.Columns.Add("Name");
                    lvAvaBlood.Columns.Add("Address");
                    lvAvaBlood.Columns.Add("Contact");
                    int volunteerCount = 0;
                    while (reader.Read())
                    {
                        // Get volunteer details
                        string volunteerName = reader.GetString(0);
                        string volunteerAddress = reader.GetString(1);
                        string volunteerContact = reader.GetString(2);

                        // Add volunteer details to the ListView
                        ListViewItem item = new ListViewItem(volunteerName);
                        item.SubItems.Add(volunteerAddress);
                        item.SubItems.Add(volunteerContact);
                        lvAvaBlood.Items.Add(item);
                        volunteerCount++;
                    }
                    reader.Close();
                    NbAvaLabel.Text = "Number of Avaiable Of this Type: " + volunteerCount;
                    // Resize columns to fit content
                    // lvAvaBlood.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    lvAvaBlood.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True");

            if (VcmbBloodTypes.SelectedIndex <= -1)
            {
                MessageBox.Show("Please Select a Blood Type");
            }
            else
            {
                try
                {
                    conn.Open();
                    string selectedBloodType = VcmbBloodTypes.SelectedItem.ToString();
                    string query = "SELECT volunteer_name, volunteer_address, volunteer_contact FROM Volunteers WHERE BloodType = @BloodType";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BloodType", selectedBloodType);
                    //cmd.Parameters.AddWithValue("@CenterID", CenterID.CenterId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear existing items in the ListView
                    lvAvaBlood.Items.Clear();
                    lvAvaBlood.Columns.Clear();
                    // Add column headers to the ListView
                    lvAvaBlood.Columns.Add("Name");
                    lvAvaBlood.Columns.Add("Address");
                    lvAvaBlood.Columns.Add("Contact");
                    int volunteerCount = 0;
                    while (reader.Read())
                    {
                        // Get volunteer details
                        string volunteerName = reader.GetString(0);
                        string volunteerAddress = reader.GetString(1);
                        string volunteerContact = reader.GetString(2);

                        // Add volunteer details to the ListView
                        ListViewItem item = new ListViewItem(volunteerName);
                        item.SubItems.Add(volunteerAddress);
                        item.SubItems.Add(volunteerContact);
                        lvAvaBlood.Items.Add(item);
                        volunteerCount++;
                    }
                    reader.Close();
                    NbAvaLabel.Text = "Number of Avaiable Of this Type: " + volunteerCount;
                    lvAvaBlood.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private void btnAddExp_Click(object sender, EventArgs e)
        {
            if (exCmbCategory.SelectedIndex <= -1 || exAmount.Text == string.Empty || exAddNotes.Text == string.Empty)
            {
                MessageBox.Show("Please Fill All Fields");
            }
            else
            {
                try
                {

                    SqlConnection sqlConnection = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True");
                    sqlConnection.Open();
                    string query = "INSERT INTO MonthlyExpenses (center_id,expense_category,expense_date,amount,AdditionalNotes) VALUES (@CenterID, @ExpenseCategory, @ExpenseDate, @Amount, @AdditionalNotes)";
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@CenterID", CenterID.CenterId); // Replace centerID with the actual value of the center ID
                    command.Parameters.AddWithValue("@ExpenseCategory", exCmbCategory.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@ExpenseDate", exdtbDate.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Amount", decimal.Parse(exAmount.Text));
                    command.Parameters.AddWithValue("@AdditionalNotes", exAddNotes.Text);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Expenses Added Successfulyy");
                    exCmbCategory.SelectedIndex = -1;
                    exdtbDate.Value = DateTime.Now;
                    exAmount.Text = string.Empty;
                    exAddNotes.Text = string.Empty;
                    sqlConnection.Close();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
             
            }
        private void btnGetExpenses_Click(object sender, EventArgs e)
        {
            if(cmbMonth.SelectedIndex <= -1)
            {
                MessageBox.Show("Please Select A month First");
            }
            else { 
            string selectedMonth = cmbMonth.SelectedItem.ToString();
            string connectionString = "Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
                {
                    try
                    {


                        connection.Open();

                        string query = "SELECT expense_date, expense_category, amount FROM MonthlyExpenses WHERE MONTH(expense_date) = @Month AND center_id = @CenterID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Month", selectedMonth);
                        // command.Parameters.AddWithValue("@Year", selectedYear);
                        command.Parameters.AddWithValue("@CenterID", CenterID.CenterId);

                        // Clear the ListView
                        lvMonthExpenses.Items.Clear();
                        lvMonthExpenses.Columns.Clear();
                        SqlDataReader reader = command.ExecuteReader();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            lvMonthExpenses.Columns.Add(reader.GetName(i));
                        }
                        // Execute the query and retrieve the expenses
                        decimal totalAmount = 0;
                        while (reader.Read())
                        {
                            DateTime expenseDate = (DateTime)reader["expense_date"];
                            string formattedExpenseDate = expenseDate.ToString("yyyy-MM-dd");
                            string expenseCategory = (string)reader["expense_category"];
                            decimal amount = (decimal)reader["amount"];
                            totalAmount += amount;
                            // Create a new ListViewItem and add the expense details
                            ListViewItem item = new ListViewItem(formattedExpenseDate);
                            item.SubItems.Add(expenseCategory);
                            item.SubItems.Add(amount.ToString());

                            // Add the item to the ListView
                            lvMonthExpenses.Items.Add(item);
                        }
                        reader.Close();
                        lvMonthExpenses.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        lbTotal.Text = "Your Total Amount of Expenses This Mounth is :" + totalAmount + "$";

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally { connection.Close(); }
                }
            }

        }
        private void btnAddCase_Click(object sender, EventArgs e)
        {
            if (CaseStatus.SelectedIndex <= -1 || CaseMoveFrom.Text == string.Empty || CaseArrivedTo.Text == string.Empty
              || CaseType.SelectedIndex <= -1 || CaseAge.Text == string.Empty || CaseTime.Text == string.Empty
              || CaseDescription.Text == string.Empty || CaseGender.SelectedIndex <= -1)
            {
                MessageBox.Show("please fill All informations Before");
            }
            else
            {
                SqlConnection sqlConnection = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True");
                try
                {
                    sqlConnection.Open();
                    string query = "INSERT INTO EmergencyCases (center_id,case_date,case_description,case_status,CaseType,CaseGender,CaseAge,FromPlace,ArrivedTo,Time) VALUES " +
                    "(@CenterID, @CaseDate, @CaseDescription,@Casestatus,@CaseType,@caseGender,@caseAge,@fromplace,@arrivedto,@time)";
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@CenterID", CenterID.CenterId);
                    command.Parameters.AddWithValue("@CaseDate", CaseDate.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@CaseDescription", CaseDescription.Text.ToString());
                    command.Parameters.AddWithValue("@Casestatus", CaseStatus.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@CaseType", CaseType.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@caseGender", CaseGender.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@CaseAge", CaseAge.Text.ToString());
                    command.Parameters.AddWithValue("@fromplace", CaseMoveFrom.Text.ToString());
                    command.Parameters.AddWithValue("@arrivedto", CaseArrivedTo.Text.ToString());
                    command.Parameters.AddWithValue("@time", CaseTime.Text.ToString());
                    command.ExecuteNonQuery();
                    MessageBox.Show("Case Added Successfulyy");
                    CaseMoveFrom.Text = string.Empty;
                    CaseArrivedTo.Text = string.Empty;
                    CaseAge.Text = string.Empty;
                    CaseTime.Text = string.Empty;
                    CaseDescription.Text = string.Empty;
                    CaseStatus.SelectedIndex = -1;
                    CaseType.SelectedIndex = -1;
                    CaseGender.SelectedIndex = -1;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        private void weekDays_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadListViewData();
        }
        private void LoadListViewData()
        {
            // Clear the existing items and columns in the ListView
            lvWorkingDays.Items.Clear();
            lvWorkingDays.Columns.Clear();

            // Add the columns to the ListView
            lvWorkingDays.Columns.Add("Volunteer Name");
            lvWorkingDays.Columns.Add("Ambulance Character");

            // Retrieve the selected center ID and working day from the combo box
            int centerID = CenterID.CenterId;
            string workingDay = weekDays.SelectedItem.ToString();

            // Retrieve the data from the database
            string connectionString = "Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True";
            string query = "SELECT [volunteer_name], [AmbulanceCharacter] FROM [dbo].[Volunteers] WHERE [center_id] = @CenterID AND [Volunteer_WorkingDays] LIKE '%' + @WorkingDay + '%'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CenterID", centerID);
                command.Parameters.AddWithValue("@WorkingDay", workingDay);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Read the data and populate the ListView
                while (reader.Read())
                {
                    string volunteerName = reader["volunteer_name"].ToString();
                    string ambulanceCharacter = reader["AmbulanceCharacter"].ToString();

                    ListViewItem item = new ListViewItem(volunteerName);
                    item.SubItems.Add(ambulanceCharacter);
                    lvWorkingDays.Items.Add(item);
                }
                lvWorkingDays.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                reader.Close();
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }

        }
 



