using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace CSharp_Final_Project__Thomas_Nichols
{
    

    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
 
            toolTip1.SetToolTip(groupBox1,"You can add Info by pressing the add button. It will not work if it isn't a new program.\n" +
                "To use the change key enter an already exsisting program name, but change the user name or password.\n" +
                "To delete, simply put the program name in the corresponding box above.\n" +
                "Changes will take place AFTER reset of program.");//Tool tip to explain adding, changing, and deleting

            var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
            con.Open();
            string stm = "SELECT ProgramName FROM Info;";
            var UserReadTest = new SqliteCommand(stm, con);
            SqliteDataReader Read = UserReadTest.ExecuteReader();
            while(Read.Read())
            {
                lstPossible.Items.Add(Encoding.ASCII.GetString(Convert.FromBase64String(Read.GetString(0))));
            }
            con.Close();
        }
        private void lstPossible_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if(lstPossible.SelectedItem!=null)
            {
                txtName.Text = lstPossible.SelectedItem.ToString();
                txtNewProg.Text = lstPossible.SelectedItem.ToString();
                lstAnswer.Items.Clear();
                var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
                con.Open();
                string stm = "SELECT * FROM Info WHERE ProgramName=$Program";
                var UserReadTest = new SqliteCommand(stm, con);
                UserReadTest.Parameters.AddWithValue("$Program", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtName.Text)));
                SqliteDataReader Read = UserReadTest.ExecuteReader();

                while (Read.Read())
                {

                    lstAnswer.Items.Add(Encoding.ASCII.GetString(Convert.FromBase64String(Read.GetString(1))));
                    lstAnswer.Items.Add(Encoding.ASCII.GetString(Convert.FromBase64String(Read.GetString(2))));
                }
                con.Close();
            }
            
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            //Displays the username and password for supplied program name
            lstAnswer.Items.Clear();
            var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
            con.Open();
            string stm = "SELECT * FROM Info WHERE ProgramName=$Program";
            var UserReadTest = new SqliteCommand(stm, con);
            UserReadTest.Parameters.AddWithValue("$Program", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtName.Text)));
            SqliteDataReader Read = UserReadTest.ExecuteReader();

            while (Read.Read())
            {

                lstAnswer.Items.Add(Encoding.ASCII.GetString(Convert.FromBase64String(Read.GetString(1))));
                lstAnswer.Items.Add(Encoding.ASCII.GetString(Convert.FromBase64String(Read.GetString(2))));
            }
            con.Close();

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Adds an entry to the files
            lstAnswer.Items.Clear();
            var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
            con.Open();
            string stm = "SELECT * FROM Info WHERE ProgramName=$Program";
            var UserReadTest = new SqliteCommand(stm, con);
            UserReadTest.Parameters.AddWithValue("$Program", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewProg.Text)));
            SqliteDataReader Read = UserReadTest.ExecuteReader();
            var response = Read.Read();
            if(response==false)
            {

                var cmd = new SqliteCommand("INSERT INTO Info(ProgramName, Username, Password) VALUES($NewProgram,$User_Name,$Pass_Word)", con);
                cmd.Parameters.AddWithValue("$NewProgram", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewProg.Text)));
                cmd.Parameters.AddWithValue("$User_Name", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewUser.Text)));
                cmd.Parameters.AddWithValue("$Pass_Word", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewPass.Text)));

                cmd.ExecuteNonQuery();
                con.Close();
                RefreshList();

                txtNewProg.Text = "";
                txtNewUser.Text = "";
                txtNewPass.Text = "";
            }
            con.Close();
            lstAnswer.Items.Add("Program Already Exists");
            
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            //Changes a preexsisting file
            lstAnswer.Items.Clear();

            var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
            var cmd = new SqliteCommand("DELETE FROM Info WHERE ProgramName=$Program", con);
            cmd.Parameters.AddWithValue("$Program", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewProg.Text)));

            con.Open();
            cmd.ExecuteNonQuery();
            
            cmd = new SqliteCommand("INSERT INTO Info(ProgramName, Username, Password) VALUES($NewProgram,$User_Name,$Pass_Word)", con);
            cmd.Parameters.AddWithValue("$NewProgram", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewProg.Text)));
            cmd.Parameters.AddWithValue("$User_Name", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewUser.Text)));
            cmd.Parameters.AddWithValue("$Pass_Word", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewPass.Text)));

            cmd.ExecuteNonQuery();
            con.Close();

            RefreshList();
            lstAnswer.Items.Clear();
            txtNewProg.Text = "";
            txtNewUser.Text = "";
            txtNewPass.Text = "";
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Deletes an entry
            lstAnswer.Items.Clear();
            var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
            var cmd = new SqliteCommand("DELETE FROM Info WHERE ProgramName=$Program", con);
            cmd.Parameters.AddWithValue("$Program", Convert.ToBase64String(Encoding.ASCII.GetBytes(txtNewProg.Text)));

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            RefreshList();
            lstAnswer.Items.Clear();//Clears some text boxes and list box that displays responses
            txtNewProg.Text = "";
            txtNewUser.Text = "";
            txtNewPass.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();//Exit button
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstAnswer.Items.Clear();//Clears some text boxes and list box that displays responses
            txtNewProg.Text = "";
            txtNewUser.Text = "";
            txtNewPass.Text = "";
        }

        private void RefreshList()
        {
            lstPossible.Items.Clear();
            var con = new SqliteConnection(@"Data Source = AllInOneDb.db;");
            con.Open();
            string stm = "SELECT ProgramName FROM Info;";
            var UserReadTest = new SqliteCommand(stm, con);
            SqliteDataReader Read = UserReadTest.ExecuteReader();
            while (Read.Read())
            {
                lstPossible.Items.Add(Encoding.ASCII.GetString(Convert.FromBase64String(Read.GetString(0))));
            }
            con.Close();
        }
    }
}
