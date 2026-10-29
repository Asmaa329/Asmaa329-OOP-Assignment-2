using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.src
{
    public class AppConfig
    {
        public static int LoadCount;
        public string DbConnection { get; set; }
        public string Theme { get; set; }
        private static readonly Object _async = new Object();
        private static AppConfig _instance;
        public static AppConfig Instance
        {
            get
            {
                lock (_async)
                {
                    if (_instance == null)
                    {
                        _instance = new AppConfig();
                    }
                    return _instance;

                }
            }
        }
        private AppConfig()
        {
            LoadCount++;
            Console.WriteLine($"[AppConfig] Loading settings from disk... (load #{LoadCount})");
            Thread.Sleep(300);
            DbConnection = "Server=localhost;Db=School";
            Theme = "Light";

        }


    }
    public class DatabaseService
    {
        public AppConfig Config
        {
            get
            {
                return AppConfig.Instance;
            }
        }
        public void Connect() => Console.WriteLine($"Connecting to {Config.DbConnection}");

    }
    public class UiService
    {
        public AppConfig Config
        {
            get
            {
                return AppConfig.Instance;
            }
        }

        public void Render() => Console.WriteLine($"UI is using theme: {Config.Theme}");
    }

}
