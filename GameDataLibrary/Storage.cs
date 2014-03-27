using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;

namespace GameDataLibrary
{
    public static class Storage
    {
        public static T LoadXml<T>(string fileName) where T : class, new()
        {
            T loadedObject = null;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
#if WINDOWS_PHONE
            using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
                if (storageFile.FileExists(fileName))
                    using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Open, storageFile))
#else
            if (File.Exists(fileName))
                using (var stream = new FileStream(fileName, FileMode.Open))
#endif
                {
                    if (stream != null)
                    {
                        if (stream.Length > 0)
                        {
                            loadedObject = (T)serializer.Deserialize(stream);
                        }
                        stream.Close();
                    }
                }
            return loadedObject;
        }

        public static void SaveXml<T>(string fileName, T objectToSave)
        {
#if WINDOWS_PHONE
			using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
			using (var stream
				= new IsolatedStorageFileStream(fileName, FileMode.Create, storageFile))
#else
            using (var stream = new FileStream(fileName, FileMode.Create))
#endif
            {
                if (stream != null)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(stream, objectToSave);
                    stream.Flush();
                }
            }
        }
    }
}
