using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartScale.Model;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Phone.Controls;

namespace SmartScale.SmartScale
{
    public static class SettingsManager
    {
        // Nazwa pliku
        private const string FILE_NAME = "calibrationData.xml";


        //
        // Zapisuje ustawienia do pliku
        //
        public static void Save(ScaleSettings settingsData, string dataFile = FILE_NAME)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(dataFile, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ScaleSettings));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, settingsData);
                    }
                }
            }
        }


        //
        // Odczytuje ustawienia z pliku
        //
        public static ScaleSettings Load(string fileName = FILE_NAME)
        {
            ScaleSettings tempData = null;
            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(fileName))
                    {
                        using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(fileName, FileMode.Open))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ScaleSettings));
                            tempData = ((ScaleSettings)serializer.Deserialize(stream));
                        }
                    }
                    else
                    {
                        //CustomMessageBox msg = new CustomMessageBox()
                        //{
                        //    Title = "Dane",
                        //    Content = "Brak pliku danych",
                        //    LeftButtonContent = "Zamknij"
                        //};
                        //msg.Show();
                    }
                }
            }
            catch
            {
                //add some code here
            }

            return tempData;
        }


        //
        // Usuwa plik z ustawieniami
        //
        public static void DeleteSettings(string fileName = FILE_NAME)
        {
            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(fileName))
                    {
                        // Jeśli plik istnieje to go usuń
                        myIsolatedStorage.DeleteFile(fileName);
                    }
                    else
                    {
                        //CustomMessageBox msg = new CustomMessageBox()
                        //{
                        //    Title = "Dane",
                        //    Content = "Brak pliku danych",
                        //    LeftButtonContent = "Zamknij"
                        //};
                        //msg.Show();
                    }
                }
            }
            catch
            {
                //add some code here
            }
        }

    }
}
