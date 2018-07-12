using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;


namespace PerfomanceManagement
{
    public partial class DataManagement : Form
    {
        MySqlConnection con = new MySqlConnection("SERVER=localhost;" + "DATABASE=dbpacket;" + "UID=root;" + "PASSWORD=;" + "SSL Mode=none;");
        
        public DataManagement()
        {
            InitializeComponent();
        }
        private void load()
        {
            Chart1.Series.Clear();
            Chart1.DataBindCrossTable(GetData().DefaultView, "RBS", "Date", "FRAMESLOST","");
            con.Close();
            string query = "SELECT `tbldata_id`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date, `Hour`, `RBS`, `RNC`, `FRAMESLOST`, `DCH_FRAMELOST`, `pmEdchIubLimitingRatio`, `IUB_CAP_HS_LIMIT` FROM `tbldata`";
            data.dgv(query, "", dgvTampil);
        }
        private DataTable GetData()
        {
            DataTable dataTable = new DataTable();

            using (MySqlCommand cmd = new MySqlCommand("SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,MAX(FRAMESLOST) AS FRAMESLOST FROM `tbldata` GROUP BY RBS,Date", con))
            {
                cmd.CommandText = "SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,MAX(FRAMESLOST) AS FRAMESLOST FROM `tbldata` GROUP BY RBS,Date";
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }
            return dataTable;
        }
        private DataTable GetDataSpecific()
        {
            DataTable dataTable = new DataTable();

            using (MySqlCommand cmd = new MySqlCommand("SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,MAX(FRAMESLOST) AS FRAMESLOST FROM `tbldata` where Date = '"+txtRange1.Text+"' && Hour = '"+txtRange2.Text+"' GROUP BY RBS,Date", con))
            {
                cmd.CommandText = "SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,MAX(FRAMESLOST) AS FRAMESLOST FROM `tbldata`  where RBS = '" + txtObject.Text + "' && Hour = '"+txtRange2.Text+"' GROUP BY RBS,Date";
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }
            return dataTable;
        }

        private void kembaliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load();
            con.Close();
        }

        private void DataManagement_Load(object sender, EventArgs e)
        {
            load();
            Chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            Chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            Chart1.MouseWheel += chart1_MouseWheel;
            WindowState = FormWindowState.Maximized;
        }
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }
        private void dgvTampil_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTampil.CurrentRow.Index != -1)
            {
                txtObject.Text = dgvTampil.CurrentRow.Cells[3].Value.ToString();
                txtRange1.Text = dgvTampil.CurrentRow.Cells[1].Value.ToString();
                txtRange2.Text = dgvTampil.CurrentRow.Cells[2].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Chart1.Series.Clear();
            try
            {
                Chart1.DataBindCrossTable(GetDataSpecific().DefaultView, "RBS", "Date", "FRAMESLOST", "");
                string query = "SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,MAX(FRAMESLOST) AS FRAMESLOST FROM `tbldata` where RBS = '" + txtObject.Text + "' && Hour = '" + txtRange2.Text + "' GROUP BY RBS,Date";
                data.dgv(query, "", dgvTampil);
                Chart1.Series[0].Color = Color.MediumPurple;
                Chart1.Series[0].ChartType = SeriesChartType.Spline;
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please select a data : "+ex.Message,"The value is empty!" ,MessageBoxButtons.OK,MessageBoxIcon.Information);
                con.Close();
            }
            
            txtObject.Text = "";
            txtRange1.Text = "";
            txtRange2.Text = "";
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
