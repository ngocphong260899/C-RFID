using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Data.SqlClient;
using System.Data.Odbc;
using Microsoft.Office.Core;
//using Microsoft.Office.Interop.Excel;
using Excel_12 = Microsoft.Office.Interop.Excel;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string InputData = String.Empty;
        string InputDataTime = String.Empty;
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-U790TPU\WINCC;Initial Catalog=rfid;Integrated Security=True");
        SqlDataAdapter da;
        DataTable dt;
        SqlCommand cmd;
        static int i = 1;
        //int rowIndex = 0;
       // int light;
        delegate void SetTextCallback(string text);

        

        public Form1()
        {
           
            InitializeComponent();
           
            getAvailblePort();
            KetNoiCSDL();
            if (dataGridView1.Rows.Count - 1 > 0)
            {
                i = Int32.Parse(dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value.ToString());
                i++;
            }
            else i = 1;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
            timer1.Start();
        }
        private void KetNoiCSDL()
        {
           

            string sql = "select * from rfid_i4";
            SqlCommand com = new SqlCommand(sql, con);
            com.CommandType = CommandType.Text;
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            con.Close();
            dataGridView1.DataSource = dt;
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;

        }
        
        private void KetNoiCSDL1()
        {

            string sql = "select * from rfid_act";
            SqlCommand com = new SqlCommand(sql, con);
            com.CommandType = CommandType.Text;
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            con.Close();
            dataGridView2.DataSource = dt;
            dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.RowCount - 1;
        }
        
        void getAvailblePort()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            string[] portsCOM = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(portsCOM);
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBox2.Items.AddRange(BaudRate);

        }

        private void SetText(string text)
        {
            if (this.rfid_num.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.rfid_num.Text = text;
                rfid_num.SelectionStart = rfid_num.Text.Length;
                rfid_num.ScrollToCaret();
            }
            //add 
            rfid_num.Text = string.Empty;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'rfidDataSet1.rfid_act' table. You can move, or remove it, as needed.
            this.rfid_actTableAdapter.Fill(this.rfidDataSet1.rfid_act);
            // TODO: This line of code loads data into the 'rfidDataSet.rfid_i4' table. You can move, or remove it, as needed.
            this.rfid_i4TableAdapter.Fill(this.rfidDataSet.rfid_i4);
            // TODO: This line of code loads data into the 'done_doluongDataSet.doluong2' table. You can move, or remove it, as needed.
            //this.doluong2TableAdapter.Fill(this.done_doluongDataSet.doluong2);

        }

        private void DataReceive(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
               
                InputData = serialPort1.ReadLine();
                InputDataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return;
            }

            if (InputData != String.Empty)
            {
                SetText(InputData);
            }

            //AutoUpdateDatabases(i, InputDataTime, null);
            //i++;
        }
        private delegate void dlgAutoUpdateDatabases( string ten, string times);
        private void AutoUpdateDatabases( string ten, string times)
        {
            
            if (this.dataGridView2.InvokeRequired)
            {
                this.Invoke(new dlgAutoUpdateDatabases(AutoUpdateDatabases), ten, times);
            }
            else
            {
                con.Open();
                cmd = new SqlCommand("INSERT INTO rfid_act (ten,times) VALUES (@ten,@times)", con);
                cmd.Parameters.Add("@ten", txt_name.Text);
                cmd.Parameters.Add("@times", label10.Text);
                try
                {
                    //run database
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    con.Close();
                    return;
                }
                KetNoiCSDL1();
            }
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           label10.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getAvailblePort();
            KetNoiCSDL();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                if (comboBox2.Text.Length != 0)
                {
                    if (comboBox1.Text.Length != 0)
                    {
                        try
                        {
                            serialPort1.PortName = comboBox1.Text;
                            serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                            serialPort1.Open();
                        }
                        catch
                        {
                            MessageBox.Show(comboBox1.Text + " is denied", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        if (serialPort1.IsOpen)
                        {
                            button1.Text = ("Ngắt Kết nối");
                            label9.Text = ("Đã kết nối");
                            label9.ForeColor = Color.Green;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Access to the port 'COM1' is denied", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn Baud Rate", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

                serialPort1.Close();
                button1.Text = ("Kết nối");
                label9.Text = ("Chưa kết nối");
                label9.ForeColor = Color.Red;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            rfid_num.ReadOnly = true;
            int i;
            i = dataGridView1.CurrentRow.Index;
            txt_id.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
            txt_name.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            textBox5.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("delete from rfid_i4", con); // xoá hết dữ liệu Table
            cmd.ExecuteNonQuery();
            KetNoiCSDL();
            i = 0;//Nếu xoá sạch database thì lưu dữ liệu với STT từ đầu
        }

        private void button4_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "delete from doluong2 where id = '" + txt_id.Text + "'";
            cmd.ExecuteNonQuery();
            KetNoiCSDL();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "update doluong2 set  date_time ='" + txt_name.Text + "',  so_luong = '" + textBox5.Text + "' where id = '" + txt_id.Text + "'";
            cmd.ExecuteNonQuery();
            KetNoiCSDL();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "insert into rfid_i4(rfid_num, name_use) values('" + textBox5.Text.ToString() + "','" + txt_name.Text.ToString() + "')";
            cmd.ExecuteNonQuery();
            KetNoiCSDL();
            MessageBox.Show("Đã thêm thẻ từ", "OK", MessageBoxButtons.OK);
            con.Close();
        }

        private void rfid_num_TextChanged(object sender, EventArgs e)
        {
            
            String r_num = rfid_num.Text;
            DataTable dataTable = new DataTable();
            string sql = "select * from rfid_i4 where rfid_num like " + r_num;
            String id ="",name = "", rfid = "";
            if (rfid_num.Text != "")
            {
                try
                {
                    SqlCommand com = new SqlCommand(sql, con);
                    com.CommandType = CommandType.Text;
                    da = new SqlDataAdapter(com);

                    da.Fill(dataTable);
                    id = dataTable.Rows[0][0].ToString();
                    name = dataTable.Rows[0][2].ToString();
                    rfid = dataTable.Rows[0][1].ToString();

                    txt_id.Text = id;
                    txt_name.Text = name;
                    textBox5.Text = rfid;
                    //add them
                    AutoUpdateDatabases(name, label10.Text);

                    //txt_name.Text = string.Empty;
                }
                catch (Exception eee)
                {
                    MessageBox.Show("Error card"+id+"|"+name, "OK", MessageBoxButtons.OK);
                }

                con.Close();
               
            }
           
        }
        public static void ExportDataGridViewTo_Excel12(DataGridView myDataGridViewQuantity)
        {

            Excel_12.Application oExcel_12 = null; //Excel_12 Application 

            Excel_12.Workbook oBook = null; // Excel_12 Workbook 

            Excel_12.Sheets oSheetsColl = null; // Excel_12 Worksheets collection 

            Excel_12.Worksheet oSheet = null; // Excel_12 Worksheet 

            Excel_12.Range oRange = null; // Cell or Range in worksheet 

            Object oMissing = System.Reflection.Missing.Value;

            // Create an instance of Excel_12. 

            oExcel_12 = new Excel_12.Application();
            // Make Excel_12 visible to the user. 

            oExcel_12.Visible = true;


            // Set the UserControl property so Excel_12 won't shut down. 

            oExcel_12.UserControl = true;

            // System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US"); 

            //object file = File_Name;

            //object missing = System.Reflection.Missing.Value;



            // Add a workbook. 

            oBook = oExcel_12.Workbooks.Add(oMissing);

            // Get worksheets collection 

            oSheetsColl = oExcel_12.Worksheets;

            // Get Worksheet "Sheet1" 

            oSheet = (Excel_12.Worksheet)oSheetsColl.get_Item("Sheet1");
            oSheet.Name = "ChamCong";




            // Export titles 

            for (int j = 0; j < myDataGridViewQuantity.Columns.Count; j++)
            {

                oRange = (Excel_12.Range)oSheet.Cells[1, j + 1];

                oRange.Value2 = myDataGridViewQuantity.Columns[j].HeaderText;

            }

            // Export data 

            for (int i = 0; i < myDataGridViewQuantity.Rows.Count; i++)
            {

                for (int j = 0; j < myDataGridViewQuantity.Columns.Count; j++)
                {
                    oRange = (Excel_12.Range)oSheet.Cells[i + 2, j + 1];

                    oRange.Value2 = myDataGridViewQuantity[j, i].Value;

                }

            }
            oBook = null;
            oExcel_12.Quit();
            oExcel_12 = null;
            GC.Collect();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                ExportDataGridViewTo_Excel12(dataGridView2);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
