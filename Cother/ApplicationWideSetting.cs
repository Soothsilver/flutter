using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace Cother
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ApplicationWideSetting : System.Attribute
    {
    
    }
    public static class ApplicationSettingsManagement
    {
        public static Exception Error = null;
        public static string SettingsFileName;
        static ApplicationSettingsManagement()
        {
            SettingsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fluttershy", "session.dat");
        }
        /// <summary>
        /// Loads application-wide settings from file into the "container".
        /// In the container, you must specify fields  to save with [ApplicationWideSetting] attribute.
        /// They must be [Serializable].
        /// </summary>
        /// <param name="container">The object that has the specified fields</param>
        /// <returns>Were the settings loaded from file?</returns>
        public static bool LoadSettings(object container)
        {
            if (!System.IO.File.Exists(SettingsFileName))
                return true;
            try
            {
                List<SerializablePair> pairs = null;
                using (FileStream fs = new FileStream(SettingsFileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pairs = (List<SerializablePair>)bf.Deserialize(fs);
                    fs.Flush();
                }
                foreach (SerializablePair pair in pairs)
                {
                    pair.FieldInfo.SetValue(container, pair.Contents);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Saves application-wide settings into a file from the container.
        /// In the container, you must specify fields to save with [ApplicationWideSetting] attribute.
        /// They must be [Serializable].
        /// </summary>
        /// <param name="container">The object that has the specified fields</param>
        /// <returns>Were the settings successfully saved?</returns>
        public static bool SaveSettings(object container)
        {
            Type typ = container.GetType();
            FieldInfo[] fields = typ.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            List<SerializablePair> pairs = new List<SerializablePair>();
            foreach (FieldInfo fi in fields)
            {
                if (fi.GetCustomAttributes(typeof(ApplicationWideSetting), false).Length > 0)
                {
                    object value = fi.GetValue(container);
                    pairs.Add(new SerializablePair(fi, value));
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFileName));
            using (FileStream fs = new FileStream(SettingsFileName, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, pairs);
                fs.Flush();
            }
            return true;
        }

        [Serializable]
        private class SerializablePair
        {
            public FieldInfo FieldInfo;
            public object Contents;
            public SerializablePair(FieldInfo fieldInfo, object contents)
            {
                FieldInfo = fieldInfo;
                Contents = contents;
            }
        }
       
    }
}
