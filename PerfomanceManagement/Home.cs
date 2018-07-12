using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfomanceManagement
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }
        void loadLoss()
        {
            string query = "Select RBS,FRAMESLOST,Hour,Date from tbldata order by FRAMESLOST desc limit 10 offset 1";
            data.dgv(query, "", dgvLoss);
        }
        void loadDCH()
        {
            string query = "Select RBS,DCH_FRAMELOST,Hour,Date from tbldata order by DCH_FRAMELOST desc limit 10 offset 1";
            data.dgv(query, "", dgvDCH);
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
        private void Home_Load(object sender, EventArgs e)
        {
            loadLoss();
            loadDCH();
        }
    }
}
