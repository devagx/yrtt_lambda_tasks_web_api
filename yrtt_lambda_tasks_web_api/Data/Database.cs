//using Amazon.Lambda.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using yrtt_lambda_tasks_web_api.Logging;
using yrtt_lambda_tasks_web_api.Models;

namespace yrtt_lambda_tasks_web_api.Data
{
    public class Database : IDatabase
    {
        private MySqlConnection connection;
        private MySqlDataReader dbReader;
        private MySqlTransaction dbTrans;
        private string dbHost;
        private string dbPort;
        private string dbName;
        private string dbUser;
        private string dbPassword;
        private string connectionString;
        private enum Models
        {
            TaskListModel,
            RatingModel,
            WalkModel,
            WalkAvgRatingModel,
            WalkMonthlyRatingModel,
            RouteModel
        }
        public Database()
        {
            Logger.LogDebug("Calling Initialize", "Database", "Database");
            Initialize();
        }
        public void AddTask(TaskListModel task)
        {
            try
            {
                string queryStatement =
                "INSERT INTO " +
                "   task " +
                "(" +
                "   taskId," +
                "   userId," +
                "   description," +
                "   completed" +
                ")" +
                "VALUES " +
                "(" +
                "   UUID(), " +
                "   @userId, " +
                "   @description, " +
                "   @completed" +
                ")";
                MySqlParameter[] dbParams = {
                new MySqlParameter("@userId",task.UserId),
                new MySqlParameter("@description",task.Description),
                new MySqlParameter("@completed",task.Completed)
             };
                Logger.LogDebug("Performing DB operations", "AddTask", "Database");
                this.OpenConnection();
                this.BeginTransaction();
                this.InsertData(queryStatement, dbParams);
                this.CommitTransaction();
                this.CloseConnection();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue adding task to the DB", "AddTask", "Database", ex.Message);

                this.RollbackTransaction();
                this.CloseConnection();
                throw new Exception(ex.Message);
            }

        }
        public void UpdateTask(TaskListModel task)
        {
            try
            {
                string queryStatement = "" +
                "UPDATE " +
                "   task " +
                "SET " +
                "   userId = @userId, " +
                "   description = @description, " +
                "   completed = @completed " +
                "WHERE " +
                "   taskId = @taskId";
                MySqlParameter[] dbParams = {
                new MySqlParameter("@taskId",task.TaskId),
                new MySqlParameter("@userId",task.UserId),
                new MySqlParameter("@description",task.Description),
                new MySqlParameter("@completed",task.Completed)
             };
                Logger.LogDebug("Performing DB operations", "UpdateTask", "Database");
                this.OpenConnection();
                this.BeginTransaction();
                this.UpdateData(queryStatement, dbParams);
                this.CommitTransaction();
                this.CloseConnection();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue updating task to the DB", "UpdateTask", "Database", ex.Message);

                this.RollbackTransaction();
                this.CloseConnection();
                throw new Exception(ex.Message);
            }
        }
        public void DeleteTask(string taskId)
        {
            try
            {
                string queryStatement = "" +
                "DELETE FROM " +
                "   task " +
                "WHERE " +
                "   taskId = @taskId";
                MySqlParameter[] dbParams = {
                new MySqlParameter("@taskId",taskId)
             };
                Logger.LogDebug("Performing DB operations", "DeleteTask", "Database");
                this.OpenConnection();
                this.BeginTransaction();
                this.DeleteData(queryStatement, dbParams);
                this.CommitTransaction();
                this.CloseConnection();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue deleting task to the DB", "DeleteTask", "Database", ex.Message);

                this.RollbackTransaction();
                this.CloseConnection();
                throw new Exception(ex.Message);
            }
        }
        public ArrayList GetTasks()
        {
            try
            {
                ArrayList data = null;

                string queryStatement = "" +
                    "SELECT " +
                    "   * " +
                    "FROM " +
                    "   task";

                Logger.LogDebug("Performing DB operations", "GetTasks", "Database");
                this.OpenConnection();
                data = this.GetData(queryStatement, null, Models.TaskListModel);
                this.CloseConnection();

                return data;
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue getting tasks from the DB", "GetTasks", "Database", ex.Message);
                this.CloseConnection();
                throw new Exception(ex.Message);
            }

        }
        public ArrayList GetTasks(string taskId)
        {
            try
            {
                ArrayList data = null;

                string queryStatement = "" +
                    "SELECT " +
                    "   * " +
                    "FROM " +
                    "   task " +
                    "WHERE " +
                    "   taskId = @taskId";
                MySqlParameter[] dbParams = {
                new MySqlParameter("@taskId",taskId)
                };

                Logger.LogDebug("Performing DB operations", "GetTasks", "Database");
                this.OpenConnection();
                data = this.GetData(queryStatement, dbParams, Models.TaskListModel);
                this.CloseConnection();

                return data;
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue getting tasks from the DB", "GetTasks", "Database", ex.Message);
                this.CloseConnection();
                throw new Exception(ex.Message);
            }
        }
        private void Initialize()
        {
            dbHost = System.Environment.GetEnvironmentVariable("DB_HOST");
            dbPort = System.Environment.GetEnvironmentVariable("DB_PORT");
            dbName = System.Environment.GetEnvironmentVariable("DB_NAME");
            dbUser = System.Environment.GetEnvironmentVariable("DB_USER");
            dbPassword = System.Environment.GetEnvironmentVariable("DB_PASSWORD");

            connectionString =
                "SERVER=" + dbHost + ";" +
                "DATABASE=" + dbName + ";" +
                "PORT=" + dbPort + ";" +
                "USER=" + dbUser + ";" +
                "PASSWORD=" + dbPassword + ";";
            try
            {
                Logger.LogDebug("Creating MySQL connection", "Initialize", "Database");
                connection = new MySqlConnection(connectionString);
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue initializing connection to the DB", "Initialize", "Database", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private void OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    Logger.LogDebug("Opening connection", "OpenConnection", "Database");
                    connection.Open();
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Logger.LogError("Issue connecting to the server", "OpenConnection", "Database", ex.Message);
                        break;
                    case 1045:
                        Logger.LogError("Invalid username or password", "OpenConnection", "Database", ex.Message);
                        break;
                    default:
                        Logger.LogError("Issue opening connection", "OpenConnection", "Database", ex.Message);
                        break;
                }
                throw new Exception(ex.Message);
            }
        }
        private void BeginTransaction()
        {
            try
            {
                Logger.LogDebug("Beggining transaction", "BeginTransaction", "Database");
                dbTrans = connection.BeginTransaction();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue beggining transaction", "BeginTransaction", "Database", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private void CommitTransaction()
        {
            try
            {
                Logger.LogDebug("Commiting transaction", "CommitTransaction", "Database");
                dbTrans.Commit();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue commiting transaction", "CommitTransaction", "Database", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private void RollbackTransaction()
        {
            try
            {
                Logger.LogDebug("Rolling back transaction", "RollbackTransaction", "Database");
                dbTrans.Rollback();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue rolling back transaction", "RollbackTransaction", "Database", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private void CloseConnection()
        {
            try
            {
                Logger.LogDebug("Closing connection", "CloseConnection", "Database");
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Issue closing connection", "CloseConnection", "Database", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private void InsertData(string queryStatement, MySqlParameter[] dbParams)
        {
            MySqlCommand command = new MySqlCommand(queryStatement, connection);
            if (dbParams != null)
            {
                command.Parameters.AddRange(dbParams);

                Logger.LogDebug("Inserting data", "InsertData", "Database");
                Logger.LogDebug("queryStatement = " + queryStatement, "InsertData", "Database");
                Logger.LogDebug("Looping parameters", "GetData", "Database");
                foreach (var param in dbParams)
                {
                    Logger.LogDebug($"dbParam name = {param.ParameterName} | Value = {param.Value}", "GetData", "Database");
                }
            }
            command.ExecuteNonQuery();
        }
        private void UpdateData(string queryStatement, MySqlParameter[] dbParams)
        {
            MySqlCommand command = new MySqlCommand(queryStatement, connection);
            if (dbParams != null)
            {
                command.Parameters.AddRange(dbParams);

                Logger.LogDebug("Updating data", "UpdateData", "Database");
                Logger.LogDebug("queryStatement = " + queryStatement, "UpdateData", "Database");
                Logger.LogDebug("Looping parameters", "GetData", "Database");
                foreach (var param in dbParams)
                {
                    Logger.LogDebug($"dbParam name = {param.ParameterName} | Value = {param.Value}", "GetData", "Database");
                }
            }
            command.ExecuteNonQuery();
        }
        private void DeleteData(string queryStatement, MySqlParameter[] dbParams)
        {
            MySqlCommand command = new MySqlCommand(queryStatement, connection);
            if (dbParams != null)
            {
                command.Parameters.AddRange(dbParams);

                Logger.LogDebug("Deleting data", "DeleteData", "Database");
                Logger.LogDebug("queryStatement = " + queryStatement, "DeleteData", "Database");
                Logger.LogDebug("Looping parameters", "GetData", "Database");
                foreach (var param in dbParams)
                {
                    Logger.LogDebug($"dbParam name = {param.ParameterName} | Value = {param.Value}", "GetData", "Database");
                }
            }
            command.ExecuteNonQuery();
        }
        private ArrayList GetData(string queryStatement, MySqlParameter[] dbParams, Models objectModelType)
        {
            ArrayList data = new ArrayList();
            object obj;

            MySqlCommand command = new MySqlCommand(queryStatement, connection);
            command.CommandText = queryStatement;
            if (dbParams != null)
            {
                command.Parameters.AddRange(dbParams);

                Logger.LogDebug("Getting data", "GetData", "Database");
                Logger.LogDebug("queryStatement = " + queryStatement, "GetData", "Database");
                Logger.LogDebug("Looping parameters", "GetData", "Database");
                foreach (var param in dbParams)
                {
                    Logger.LogDebug($"dbParam name = {param.ParameterName} | Value = {param.Value}", "GetData", "Database");
                }
            }
            dbReader = command.ExecuteReader();

            if (dbReader.HasRows)
            {
                while (dbReader.Read())
                {
                    switch (objectModelType)
                    {
                        case Models.TaskListModel:
                            obj = new TaskListModel
                            {
                                TaskId = dbReader.GetString("taskId"),
                                UserId = dbReader.GetString("userId"),
                                Description = dbReader.GetString("description"),
                                Completed = dbReader.GetBoolean("completed")
                            };
                            data.Add(obj);
                            break;
                    }
                }
            }
            dbReader.Close();
            return data;
        }
    }
}
