using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace VisualSQL
{
    public partial class Form1 : Form
    {

        private static string hiddenString;


        public void getDBinfo(string constring, string qry, ListBox lst)
        {
            // get the db info
            SqlConnection myConn = new SqlConnection(constring);
            SqlDataReader reader;

            SqlCommand cmd = new SqlCommand(qry, myConn);
            cmd.CommandType = CommandType.Text;

            myConn.Open();
            try
            {
                Status.Text = string.Empty;
                reader = cmd.ExecuteReader();
                lst.DataSource = reader;
            }
            catch (Exception ex)
            {
                Status.Text = ex.ToString();
            }
            finally
            {
                if (myConn != null)
                    myConn.Dispose();
            }
        }

        protected void connectIons_SelectedIndexChanged(object sender, EventArgs e)
        {
            string db = connectIons.SelectedItem.ToString();
            string constring = ConfigurationManager.ConnectionStrings[db].ToString();
            string qry = "";
            string table = "%";
            hiddenString = constring;

            qry = "SELECT Name from Sysobjects where xtype = 'u'";
            getDBinfo(constring, qry, listBox1);

            qry = "select c.name as Name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name like '" + table + "' and t.type = 'U'";
            getDBinfo(constring, qry, listBox2);

            qry = "select name from sys.procedures";
            getDBinfo(constring, qry, listBox3);

            qry = "select name from sys.views";
            getDBinfo(constring, qry, listBox4);

        }

        public Form1()
        {
            InitializeComponent();

            foreach (ConnectionStringSettings c in System.Configuration.ConfigurationManager.ConnectionStrings)
            {
                connectIons.Items.Add(c.Name);
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string constring = hiddenString.ToString();

            string qry = "select c.name as Name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name like '" + connectIons.SelectedItem.ToString() + "' and t.type = 'U'";
            getDBinfo(constring, qry, listBox2);
        }

        protected void getSchemaInfo(string constring, string qry, RichTextBox txt)
        {

            // used for retrieving the db schema info rendering it in a label
            constring = hiddenString.ToString();

            SqlConnection myConn = new SqlConnection(constring);
            SqlDataReader reader;

            SqlCommand cmd = new SqlCommand(qry, myConn);
            cmd.CommandType = CommandType.Text;

            myConn.Open();
            try
            {
                Status.Text = string.Empty;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    txt.Text = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                Status.Text = ex.ToString();
            }
            finally
            {

                if (myConn != null)
                    myConn.Dispose();

                if (cmd != null)
                    cmd.Dispose();
            }

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
 
            string constring = hiddenString.ToString();

            string qry = "select ROUTINE_DEFINITION from INFORMATION_SCHEMA.ROUTINES Where ROUTINE_NAME='" + listBox2.SelectedItem.ToString() + "'";
            getSchemaInfo(constring, qry, richTextBox1);
        }

        protected void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string constring = hiddenString.ToString();

            string qry = "select VIEW_DEFINITION from INFORMATION_SCHEMA.VIEWS Where TABLE_NAME='" + listBox3.SelectedItem.ToString() + "'";
            getSchemaInfo(constring, qry, richTextBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string constring = hiddenString.ToString();
 
            SqlConnection myConn = new SqlConnection(constring);
            SqlDataReader reader;
            string qry = richTextBox1.ToString();
 
            SqlCommand cmd = new SqlCommand(qry, myConn);
            cmd.CommandType = CommandType.Text;
 
            myConn.Open();
            try
            {
                Status.Text = string.Empty;
                reader = cmd.ExecuteReader();
                grd.DataSource = reader;
                
            }
            catch (Exception ex)
            {
                Status.Text = ex.ToString();
                //throw;
            }
            finally
            {
 
                if (myConn != null)
                    myConn.Dispose();
 
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {           
                try
                {
                    StreamWriter sw = new StreamWriter("output.csv", false);
                    int iColCount = grd.Columns.Count;
                    for (int i = 0; i < iColCount; i++)
                    {
                        sw.Write(grd.Columns[i]);
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                    foreach (DataRow dr in grd.Rows)
                    {
                        for (int i = 0; i < iColCount; i++)
                        {
                            if (!Convert.IsDBNull(dr[i]))
                            {
                                sw.Write(dr[i].ToString());
                            }
                            if (i < iColCount - 1)
                            {
                                sw.Write(",");
                            }
                        }

                        sw.Write(sw.NewLine);
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
}

