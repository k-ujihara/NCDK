namespace NCDK.IO.Setting
{
    class SettingManager_Example
    {
        void Main()
        {
            {
                #region 
                // create the manager and add a setting
                var manager = new SettingManager<BooleanIOSetting>();
                manager.Add(new BooleanIOSetting("Sample", IOSetting.Importance.Medium, "This is a sample?", "true"));

                // check the setting is present (case insensitive)
                if (manager.Has("sample"))
                {
                    // access requiring multiple lines of code
                    BooleanIOSetting setting = manager["sample"];
                    string v1 = setting.Setting;
                    // single line access (useful for conditional statements)
                    string v2 = manager["sample"].Setting;
                }
                #endregion
            }
            {
                IOSetting.Importance importance = IOSetting.Importance.Medium;
                string some = null;
                #region Add
                var manager = new SettingManager<BooleanIOSetting>();
                BooleanIOSetting setting1 = manager.Add(new BooleanIOSetting("use.3d", importance, some, some));
                BooleanIOSetting setting2 = manager.Add(new BooleanIOSetting("use.3d", importance, some, some));

                // setting1 == setting2 and so changing a field in setting1 will also change the field
                // in setting2
                #endregion
            }
            {
                IOSetting.Importance importance = IOSetting.Importance.Medium;
                string some = null;
                #region get
                var manager = new SettingManager<BooleanIOSetting>();
                manager.Add(new BooleanIOSetting("name", importance, some, some));

                BooleanIOSetting setting1 = manager["Name"]; // okay
                // OptionIOSetting setting2 = manager["Name"]; // failed to compile
                #endregion
            }
        }
    }
}
