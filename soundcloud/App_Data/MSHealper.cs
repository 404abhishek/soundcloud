using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Configuration;
using System.Data;


class MSHealper : IDisposable
{
    
    public enum MType { Success, Error }
    public enum QueryMode { Insert, Update }
    private MySqlConnection connect;
    private bool bConnected = false;
    private int count;
    public double FINE = 5;
    private string connectstr = ConfigurationManager.ConnectionStrings["connlocal"].ToString();
   
    public MSHealper()
    {
        Connect();
    }
    private void Connect()
    {
        try
        {
            // string cns = RegistryHealper.ReadValues(utils.folder, "CNS");
           
                
          
                connect = new MySqlConnection(connectstr);
              
            
            connect.Open();
            bConnected = true;
        }
        catch (MySqlException ex)
        {
                 }
    }
    public void RunScript(String cmdtext)
    {
        int count = 0;
        try
        {
            if (bConnected == false)
            {
                Connect();
            }
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = cmdtext;
            cmd.Connection = connect;
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            count = cmd.ExecuteNonQuery();
             
        }
        catch (Exception xe)
        {  Debug(xe.Message); }
    }
    public Double ConvertToDouble(String val)
    {
        Double result = 0;
        if (String.IsNullOrEmpty(val))
            result = 0;
        else
            result = Convert.ToDouble(val);
        return result;
    }
    public int ConvertToInteger(String val)
    {
        int result = 0;
        if (String.IsNullOrEmpty(val))
            result = 0;
        else
            result = Convert.ToInt32(val);
        return result;
    }

  
    //--------------
    public int RecordCount(String Query)
    {
        DataTable dt = new DataTable();
        int RecordCount = 0;
        DataSet ds = new DataSet();
        try
        {
            MySqlDataAdapter dr = new MySqlDataAdapter(Query, connect);
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            dr.Fill(dt);
            RecordCount = dt.Rows.Count;
        }
        catch (Exception xe)
        {
            Debug(xe.Message);
        }
        return RecordCount;
    }
    public bool IsExist(string field, string table, string where)
    {
        String val = GetfirstValue(field, table, where);
        if (!string.IsNullOrEmpty(val))
            return true;
        else return false;
    }
    public String GetfirstValue(string field, string table, string where)
    {
        string value = string.Empty;
        DataSet ds = GetData(field, table, where + " LIMIT 1 ");
        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0) { value = dt.Rows[0][field].ToString(); }
        }
        return value;
    }
    //--------------
    public DataSet GetData(String Query)
    {
        DataSet ds = new DataSet();
        try
        {
            MySqlDataAdapter dr = new MySqlDataAdapter(Query, connect);
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            dr.Fill(ds);
            return ds;
        }
        catch (Exception xe)
        { }
        return ds;
    }
    public DataSet GetData(String fields, String table, String Where = "")
    {
        DataSet ds = new DataSet();
        try
        {
            string Query = string.Format("Select {0} from {1} {2}", fields, table, (Where.Length > 0 ? "where " + Where : ""));
            MySqlDataAdapter dr = new MySqlDataAdapter(Query, connect);
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            dr.Fill(ds);
            return ds;
        }
        catch (Exception xe)
        { }
        return ds;
    }
    public DataTable GetTable(String Query)
    {
        DataTable ds = new DataTable();
        try
        {
            MySqlDataAdapter dr = new MySqlDataAdapter(Query, connect);
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            dr.Fill(ds);
            return ds;
        }
        catch (Exception xe)
        { }
        return ds;
    }
    public DataTable GetTable(String fields, String table, String Where = "")
    {
        DataTable ds = new DataTable();
        try
        {
            string Query = string.Format("Select {0} from {1} {2}", fields, table, (Where.Length > 0 ? "where " + Where : ""));
            MySqlDataAdapter dr = new MySqlDataAdapter(Query, connect);
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            dr.Fill(ds);
            return ds;
        }
        catch (Exception xe)
        { }
        return ds;
    }
    //--------------
    public int Insert_Value(Hashtable data, String @tables)
    {
        String @fields = string.Empty, @Values = string.Empty;
        int counter = 1;
        IDictionaryEnumerator enums = data.GetEnumerator();
        MySqlCommand cmd = new MySqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        while (enums.MoveNext())
        {
            @fields += "," + enums.Key;
            @Values += ",?" + enums.Key;
            cmd.Parameters.AddWithValue(enums.Key.ToString(), enums.Value);
            counter++;
        }
        @fields = @fields.Remove(0, 1); @Values = @Values.Remove(0, 1).Trim();
        cmd.CommandText = string.Format("insert into {0} ({1}) values ({2})", @tables, @fields, @Values);
        cmd.Connection = connect;
        if (connect.State == System.Data.ConnectionState.Closed) { connect.Open(); }
        try
        {
            count = cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { Debug(ex.Message); }
        return count;
    }
    public int Update_Value(Hashtable data, String @tables, String @where)
    {
        String @Values = string.Empty;
        int counter = 1;
        IDictionaryEnumerator enums = data.GetEnumerator();
        MySqlCommand cmd = new MySqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        while (enums.MoveNext())
        {
            @Values += string.Format(",{0}=?{0}", enums.Key);
            cmd.Parameters.AddWithValue(enums.Key.ToString(), enums.Value);
            counter++;
        }
        @Values = @Values.Remove(0, 1);
        cmd.CommandText = string.Format("Update {0} Set {1} where {2}", @tables, @Values, @where);
        cmd.Connection = connect;
        if (connect.State == System.Data.ConnectionState.Closed) { connect.Open(); }
        try
        {
            count = cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { Debug(ex.Message); }
        return count;
    }
    //--------------
    public int ExecuteNonQuery(String Query)
    {
        int count = 0;
        try
        {
            if (bConnected == false)
            {
                Connect();
            }
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = Query;
            cmd.Connection = connect;
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            count = cmd.ExecuteNonQuery();
            return count;
        }
        catch (Exception xe)
        { return count = -1; Debug(xe.Message); }

        return count;
    }
    public int ExecuteNonQuery(String Query, MySqlParameter[] param)
    {
        int count = 0;
        try
        {
            if (bConnected == false)
            {
                Connect();
            }
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = Query;
            cmd.Connection = connect;
            foreach (MySqlParameter prm in param)
            {
                if (prm != null)
                    cmd.Parameters.Add(prm);
            }
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            count = cmd.ExecuteNonQuery();
            return count;
        }
        catch (Exception xe)
        { return count = -1; Debug(xe.Message); }

        return count;
    }
    //--------------
    public MySqlDataAdapter GetAdapter(String Query, ref DataSet ds, String table)
    {
        MySqlDataAdapter dr = new MySqlDataAdapter(Query, connect);
        try
        {
            if (connect.State == System.Data.ConnectionState.Closed)
            { connect.Open(); }
            dr.Fill(ds, table);
        }
        catch (Exception xe)
        { }
        finally
        { connect.Close(); }
        return dr;
    }
    public MySqlDataReader ExecuteReader(string query)
    {
        MySqlCommand cmd = new MySqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = query;
        cmd.Connection = connect;
        MySqlDataReader reader = null;
        try
        {
            if (connect.State == System.Data.ConnectionState.Closed)
            {
                connect.Open();
            }
            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        return reader;
    }
    //--------------
    internal int DeleteRecord(String Db, String[] param)
    {
        int count = ExecuteNonQuery(String.Format("Delete from {0} where {1}={2}", Db, param[0], param[1]));
        if (count > 0) { deletedRecord_log(Db, Db); }
        return count;
    }
    public int DeleteRecord(String table, String where)
    {
        int count = ExecuteNonQuery(String.Format("Delete from {0} where {1}", table, where));
        if (count > 0) { deletedRecord_log(table, table); }
        return count;
    }
    //--------------
    public void Debug(string error)
    {
        Console.WriteLine(error + "/n/r");
    }

   
    public void Dispose()
    {
        connect.Close();
    }

    public int deletedRecord_log(string fieldcode, string tablename)
    {
        int count = 0;
        Hashtable hst = new Hashtable();
        hst["FIELDCODE"] = fieldcode;
        hst["TABLENAME"] = tablename;
        hst["SCHOOLID"] = 1;
        
        hst["DELETEDATE"] = DateTime.Now.ToString("yyyy-MM-dd");
        count = Insert_Value(hst, "school_tableitem_deleted");
        return count;
    }
}
