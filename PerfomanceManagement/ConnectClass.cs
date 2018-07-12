using System;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Windows.Forms;

public class ConnectClass
{
    static string conn;

    public static MySqlConnection konec
    {
        get
        {
            return new MySqlConnection(conn);
        }
    }

	static ConnectClass()
	{
        string connectionLogin= "SERVER=localhost;" + "DATABASE=dbpacket;" + "UID=root;" + "PASSWORD=;" + "SSL Mode=none;";
        try
        {
            conn = connectionLogin;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Connection Failed ! : " + ex.Message);
        }
    }
}

public static class data
{
    static MySqlConnection conn;
    static MySqlDataAdapter da;

    public static void dgv(string query, string table, DataGridView dgv)
    {
        System.Data.DataTable dt = new System.Data.DataTable();
        conn = ConnectClass.konec;

        da = new MySqlDataAdapter(query, conn);

        try
        {
            conn.Open();
            da.Fill(dt);
            dgv.DataSource = dt;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Connection Failed ! : " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }

}
