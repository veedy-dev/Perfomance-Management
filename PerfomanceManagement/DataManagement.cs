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
            dgvTampil.Columns.Clear();
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

            using (MySqlCommand cmd = new MySqlCommand("SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,FRAMESLOST FROM tbldata where DATE_FORMAT(Date, '"+txtRange1.Text+"') && RBS = '"+txtObject.Text+"' GROUP BY RBS,Hour", con))
            {
                cmd.CommandText = "SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,FRAMESLOST FROM tbldata where DATE_FORMAT(Date, '" + txtRange1.Text + "') && RBS = '" + txtObject.Text + "' GROUP BY RBS,Hour";
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }
            return dataTable;
        }

        private void designChart()
        {
            for(int i = 0; i < 19; i++)
            {
                Chart1.Series[i].ChartType = SeriesChartType.Spline;
            }                     
        }

        private void kembaliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load();
            designChart();
            con.Close();
            txtObject.Text = "";
            txtRange1.Text = "";
            txtRange2.Text = "";
            Chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            Chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
        }
        private void loadform()
        {
            try
            {
                load();
                designChart();
                lbl3.Text = "";
                lbl4.Text = "";
                lbl5.Text = "";
                Chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
                Chart1.ChartAreas[0].AxisX.IsStartedFromZero = false;
                Chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                Chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                Chart1.MouseWheel += chart1_MouseWheel;
                chartSelectionZoom();
                WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                if(MessageBox.Show("Could not Load data! : " + ex.Message, "Connection Failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                {
                    loadform();
                }
                else
                {
                    this.Close();
                }
            }
        }
        private void DataManagement_Load(object sender, EventArgs e)
        {
            loadform();
        }

        private void chartSelectionZoom()
        {
            Chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            Chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            Chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            Chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            Chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            Chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            Chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            Chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
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

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 3;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 3;

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
                txtObject.Text = dgvTampil.CurrentRow.Cells["RBS"].Value.ToString();                
                txtRange1.Text = dgvTampil.CurrentRow.Cells["Date"].Value.ToString();
                txtRange2.Text = dgvTampil.CurrentRow.Cells["Hour"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Chart1.Series.Clear();            
            try
            {
                Chart1.DataBindCrossTable(GetDataSpecific().DefaultView, "RBS", "Hour", "FRAMESLOST", "");
                string query = "SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS Date,`Hour`,FRAMESLOST FROM tbldata where DATE_FORMAT(Date, '" + txtRange1.Text + "') && RBS = '" + txtObject.Text + "' GROUP BY RBS,Hour";
                data.dgv(query, "", dgvTampil);
                Chart1.Series[0].ChartType = SeriesChartType.Spline;
                Chart1.Series[0].Color = Color.MediumPurple;                
                con.Close();
                Chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                Chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
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

        private void Chart1_MouseClick(object sender, MouseEventArgs e)
        {
            HitTestResult hit = Chart1.HitTest(e.X, e.Y);
            Series s = null;
            if (hit != null) s = hit.Series;
            if (s != null)
            {
                string text = s.LegendText != "" ? s.LegendText : s.Name;
                txtObject.Text = text;
            }
        }

        private void Chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Series s = null;
            Point mousePoint = new Point(e.X, e.Y);
            Chart1.ChartAreas[0].CursorX.Interval = 0;
            Chart1.ChartAreas[0].CursorY.Interval = 0;

            Chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
            Chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);

            HitTestResult result = Chart1.HitTest(e.X, e.Y);
            if (result != null) s = result.Series;
            if (s != null)
            {
                string text = s.LegendText != "" ? s.LegendText : s.Name;
                lbl4.Text = text;
            }
            if (result.PointIndex > -1 && result.ChartArea != null)
            {
                lbl3.Text = "FRAMESLOST: " + result.Series.Points[result.PointIndex].YValues[0].ToString();
                lbl5.Text = "Hour: "+ result.Series.Points[result.PointIndex].XValue.ToString();
            }
        }
    }
}
