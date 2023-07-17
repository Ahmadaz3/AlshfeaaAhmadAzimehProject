using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VisualProgrammingProjectTest1
{
    public partial class Login : Form
    {
        //Thread th;
        public Login()
        {
            InitializeComponent();
        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
     (
         int nLeftRect,
         int nTopRect,
         int nRightRect,
         int nBottomRect,
         int nWidthEllipse,
         int nHeightEllipse
     );

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void exitPicBox_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            
            panel1.Location = new Point(
                this.ClientSize.Width / 2 - panel1.Size.Width / 2,
                this.ClientSize.Height / 2 - panel1.Size.Height / 2);
            panel1.Anchor = AnchorStyles.None;

            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width,
            panel1.Height, 30, 30));
          //  usernameTxt.TextAlign = HorizontalAlignment.Left;
           //usernameTxt.RightToLeft = RightToLeft.No;
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            // Check if the user filled all fields
            if (usernameTxt.Text == string.Empty || passwordTxt.Text == string.Empty)
            {
                MessageBox.Show("Please fill in all fields.");
            }
            else
            {
                if (usernameTxt.Text == "Admin" && passwordTxt.Text == "AlshefaaAdmin372")
                {
                    AdminMainForm adminMainForm = new AdminMainForm();
                    this.Hide();
                    adminMainForm.ShowDialog();  // Use ShowDialog to block the code until CentersMainForm is closed
                    this.Close();
                }
                else
                {
                    using (SqlConnection sqlConnection = new SqlConnection("Data Source=DESKTOP-8EEUQH9\\SQLEXPRESS;Initial Catalog=AlshefaaManagementSystem;Integrated Security=True"))
                    {
                        sqlConnection.Open();

                        string query = "SELECT center_id, center_name FROM Centers WHERE center_name = @CenterName AND password = @Password";
                        SqlCommand command = new SqlCommand(query, sqlConnection);
                        command.Parameters.AddWithValue("@CenterName", usernameTxt.Text);
                        command.Parameters.AddWithValue("@Password", passwordTxt.Text);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                           CenterID.CenterId = (int)reader["center_id"];
                            CenterID.Cname = (string)reader["center_name"];
                            if (CenterID.CenterId >= 1)
                            {
                                CentersMainForm centersMainForm = new CentersMainForm(CenterID.CenterId);
                                this.Hide();
                                centersMainForm.ShowDialog();  // Use ShowDialog to block the code until CentersMainForm is closed
                                this.Close();  // Close Form1 after CentersMainForm is closed
                            }

                            // Login successful
                        }
                        else
                        {
                            // Login unsuccessful
                            MessageBox.Show("Invalid center name or password. Please try again.");
                        }

                        reader.Close();
                    }
                }
            }
        }

        }
}
