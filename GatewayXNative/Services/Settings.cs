using System;
using Android.App;
using Android.Content;
using Android.Preferences;

namespace GatewayXNative.Services
{
    public class Settings
    {
        readonly object locker = new object();
        public static Settings instance = null;

        public static Settings SharedInstance()
        {
            if (instance == null)
            {
                instance = new Settings();
            }
            return instance;
        }

        public Settings()
        {
        }

        private string baseId;
        public string BaseId
        {
            get
            {
                if (String.IsNullOrEmpty(baseId))
                {
                    lock (locker)
                    {
                        using (var sharedPreferences = GetSharedPreference())
                        {
                            return sharedPreferences.GetString("BaseID", null);
                        }
                    }
                }
                return baseId;
            }
            set
            {
                baseId = value;
                lock (locker)
                {
                    using (var sharedPreferences = GetSharedPreference())
                    {
                        using (var editor = sharedPreferences.Edit())
                        {
                            editor.PutString("BaseID", value);
                            editor.Commit();
                        }
                    }
                }
            }
        }

        private ISharedPreferences GetSharedPreference()
        {
            return Application.Context.GetSharedPreferences("DottedGateway", FileCreationMode.Private);
        }
    }
}
