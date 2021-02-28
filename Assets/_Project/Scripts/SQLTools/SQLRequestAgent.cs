using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.Data;
using PowerInspector;
using MySql.Data.MySqlClient;
using System.Threading;
using System;

[LuaCallCSharp]
public class SQLRequestAgent : ShortLifeSingleton<SQLRequestAgent>
{
    #region Data & Definations
    MySqlConnection sqlConnection;
    public string server;
    public string port;
    public string database;
    public string uid;
    public string pwd;
    [ReadOnly]
    public string connectionMethod;

    [CSharpCallLua]
    public delegate void SQLDataReaderCallback(MySqlDataReader reader);
    #endregion

    #region Unity Life Cycle
    void Start()
    {
        sqlThread = new Thread(SQLProcessingThread);
        sqlThread.Start();
    }

    private void Update()
    {
        lock(responseQueue)
        {
            while(responseQueue.Count!=0)
            {
                SQLResponse response = responseQueue.Dequeue();
                if (response == null)
                    continue;

                response.callback?.Invoke(response.result);
                //处理完一定要关闭掉，一个Connection只能给一个Result
                if(response.result!=null)
                {
                    response.result.Close();
                    response.result.Dispose();
                }    
            }
        }
    }

    private void OnValidate()
    {
        connectionMethod = string.Format("server={0};port={1};database={2};uid={3};pwd={4}", server, port ,database, uid, pwd);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (sqlConnection != null)
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
        }
    }
    #endregion

    #region SQL Thread Processing
    protected class SQLRequest
    {
        public string query;
        public SQLDataReaderCallback callback;
        public SQLRequest(string query, SQLDataReaderCallback callback)
        {
            this.query = query;
            this.callback = callback;
        }
    }

    protected class SQLResponse
    {
        public MySqlDataReader result;
        public SQLDataReaderCallback callback;
        public SQLResponse(MySqlDataReader reader, SQLDataReaderCallback callback)
        {
            this.result = reader;
            this.callback = callback;
        }
    }

    protected Queue<SQLRequest> requestQueue = new Queue<SQLRequest>();
    protected Queue<SQLResponse> responseQueue = new Queue<SQLResponse>();

    void OpenConnection()
    {
        if (sqlConnection != null)
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
        }
        sqlConnection = new MySqlConnection(connectionMethod);
        sqlConnection.Open();
    }

    SQLResponse MakeQuery(SQLRequest req)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(req.query, sqlConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
            return new SQLResponse(reader, req.callback);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }

    Thread sqlThread = null;
    void SQLProcessingThread()
    {
        while(true)
        {
            if (sqlConnection == null)
            {
                OpenConnection();
            }

            if (sqlConnection.State==ConnectionState.Broken ||
                sqlConnection.State==ConnectionState.Closed)
            {
                OpenConnection();
                Thread.Sleep(50);
                continue;
            }

            if (sqlConnection.State == ConnectionState.Connecting || 
                sqlConnection.State == ConnectionState.Executing || 
                sqlConnection.State == ConnectionState.Fetching)
            {
                Thread.Sleep(50);
                continue;
            }

            if (requestQueue.Count == 0)
            {
                Thread.Sleep(50);
                continue;
            }

            SQLResponse response = null;
            lock (requestQueue)       
                response=MakeQuery(requestQueue.Dequeue());
            

            lock(responseQueue)
                responseQueue.Enqueue(response);

            Thread.Sleep(50);
        }
    }
    #endregion


    #region Exposed API
    public void PerformSQLRequest(string query, SQLDataReaderCallback callback)
    {
        lock(requestQueue)
            requestQueue.Enqueue(new SQLRequest(query, callback));
    }

    public void Toast(string content)
    {
        AndroidJavaObject apkInstaller = new AndroidJavaObject("com.SamaelXXX.APKTools.APKInstaller");
        if (apkInstaller != null)
        {
            apkInstaller.Call<bool>("showToast", content);
        }
    }

    [ContextMenu("Test")]
    public void Test()
    {
        string query = "select steam_id,user_name from players_basic limit 10";
        PerformSQLRequest(query, (reader) =>
         {
             while(reader.Read())
             {
                 string steamId = reader.GetString("steam_id");
                 string userName = reader.GetString("user_name");
                 Debug.Log("SteamID:"+steamId+",UserName:"+userName);
             }
         });
    }
    #endregion
}
