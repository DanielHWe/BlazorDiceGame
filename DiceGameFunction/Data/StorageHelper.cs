using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DiceGameFunction.Data
{
    public class StorageHelper
    {
        private static String _connectionString;
        private static CloudStorageAccount storageAccount;
        private static CloudTableClient _tableClient;
        private static CloudStorageAccount _storageAccount;

        public static string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (value != null && value.Equals(_connectionString)) return;
                _connectionString = value;
         
                Reset();
         
            }
        }

        public static void Reset()
        {            
            _tableClient = null;
            storageAccount = CloudStorageAccount.Parse(ConnectionString);
            TableHelper.ResetCache();

            _tableClient = storageAccount.CreateCloudTableClient();                            
        }

        internal void LoadConnectionString()
        {
            ConnectionString = ReadSetting("AzureWebJobsStorage");
        }

        private static String ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? Environment.GetEnvironmentVariable(key);
            }
            catch (ConfigurationErrorsException)
            {
                return null;
            }
        }

        public CloudTable GetTable(String tableName)
        {
            if (_tableClient == null)
                GetCloudTableClient();

            CloudTable table = _tableClient.GetTableReference(tableName);

            return table;
        }

        private void GetCloudTableClient()
        {
            if (String.IsNullOrEmpty(ConnectionString))
            {
                LoadConnectionString();
            }



            _tableClient = StorageAccount.CreateCloudTableClient();
        }

        private static CloudStorageAccount StorageAccount
        {
            get
            {
                if (_storageAccount == null)
                {
                    _storageAccount = CloudStorageAccount.Parse(ConnectionString);
                }
                return _storageAccount;
            }
        }
    }
}
