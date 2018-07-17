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
    public partial class Home : Form
    {
        MySqlConnection con = new MySqlConnection("SERVER=localhost;" + "DATABASE=dbpacket;" + "UID=root;" + "PASSWORD=;" + "SSL Mode=none;");
        public Home()
        {
            InitializeComponent();
        }
        void loadLoss()
        {
            string query = "SELECT `RBS`,MAX(FRAMESLOST) AS `FRAMESLOST`,DATE_FORMAT(Date,'%d/%m/%Y') AS `Date`, `Hour` FROM `tbldata` GROUP BY RBS ORDER BY FRAMESLOST limit 10 OFFSET 1";
            data.dgv(query, "", dgvLoss);
        }
        void loadDCH()
        {
            string query = "SELECT `RBS`,MAX(DCH_FRAMELOST) AS `DCH_FRAMELOST`,DATE_FORMAT(Date,'%d/%m/%Y') AS `Date`, `Hour` FROM `tbldata` GROUP BY RBS ORDER BY DCH_FRAMELOST limit 10 OFFSET 1";
            data.dgv(query, "", dgvDCH);
        }
        private DataTable GetData()
        {
            DataTable dataTable = new DataTable();

            using (MySqlCommand cmd = new MySqlCommand("SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS `Date`, `Hour`,MAX(FRAMESLOST) AS `FRAMESLOST` FROM `tbldata` GROUP BY RBS ORDER BY FRAMESLOST limit 10 OFFSET 1", con))
            {
                cmd.CommandText = "SELECT `RBS`,DATE_FORMAT(Date,'%d/%m/%Y') AS `Date`, `Hour`,MAX(FRAMESLOST) AS `FRAMESLOST` FROM `tbldata` GROUP BY RBS ORDER BY FRAMESLOST limit 10 OFFSET 1";
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }
            return dataTable;
        }
        private void designChart()
        {  
           for(int i = 0;i<10;i++)
            {
                Chart1.Series[i].ChartType = SeriesChartType.Line;
            }           
        }
        private void dataManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (DataManagement frm = new DataManagement())
            {
                frm.ShowDialog();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void load()
        {
            try
            {
                Chart1.Series.Clear();
                lbl3.Text = "";
                lbl4.Text = "";
                Chart1.DataBindCrossTable(GetData().DefaultView, "RBS", "Date", "FRAMESLOST", "");
                loadLoss();
                loadDCH();
                con.Close();
                Chart1.MouseWheel += chart1_MouseWheel;
            }
            catch (Exception ex)
            {
                if (MessageBox.Show("Could not Load data! : " + ex.Message, "Connection Failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                {
                    load();
                }
                else
                {
                    this.Close();
                }
            }
        }
        private void Home_Load(object sender, EventArgs e)
        {
            load();
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
                lbl3.Text = "";
            }
            if (result.PointIndex > -1 && result.ChartArea != null)
            {
                lbl3.Text = "FRAMESLOST: " + result.Series.Points[result.PointIndex].YValues[0].ToString();
            }
        }
    }
}
