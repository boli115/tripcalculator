using System;
using System.Linq;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TripCalculator
{
    class Settings
    {

        private string settingsFileName = "";
        private XDocument docSettings;

        public Settings()
        {

        }

        public Settings(string fileName)
        {
            settingsFileName = fileName;
            if (!File.Exists(settingsFileName)) CreateNew();

            docSettings = XDocument.Load(settingsFileName);
        }

        public void CreateNew()
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Settings"));

            if (!Directory.Exists(Path.GetDirectoryName(settingsFileName))) Directory.CreateDirectory(Path.GetDirectoryName(settingsFileName));

            doc.Save(settingsFileName);

        }

        public string Read(string key, string defaultValue = "")
        {
            var data = from item in docSettings.Root.Elements("Setting")
                       where item.Element("Key").Value == key
                       select item.Element("Value").Value;
   

            string retVal = "";

            foreach (var p in data)
            {
                Console.WriteLine(p.ToString());
                retVal = p.ToString();

            }

            if (retVal == "")
            {
                XElement newClient = new XElement("Setting",
                   new XElement("Key", key),
                   new XElement("Value", defaultValue)

                   );

                return defaultValue;
            }
            else
            {
                return retVal;
            }

        }

        public bool Write(string key, string value)
        {
            var data = from item in docSettings.Root.Elements("Setting")
                       where item.Element("Key").Value == key
                       select item.Element("Value").Value;

            if (data.Count() == 0)
            {


                XElement newClient = new XElement("Setting",
                   new XElement("Key", key),
                   new XElement("Value", value)

                );
                docSettings.Root.Add(newClient);
            }
            else //Client section exists.
            {

                var x = docSettings.Descendants("Setting");
                foreach (var p in x)
                {
                    if (p.Element("Key").Value == key)
                        p.Element("Value").Value = value;

                }
                //x.Value = value;
            }

            docSettings.Save(settingsFileName);

            return true;

        }


        
        public void SaveWindowLocation(Form frm, string key = "Window Location")
        {
            if (frm.WindowState == FormWindowState.Normal)
            {
                Write(key, frm.Location.X + "," + frm.Location.Y + "," + frm.Size.Width + "," + frm.Size.Height);
            }
            else
            {
                string sLoc = Read(key);
                if (sLoc != "")
                {
                    string[] s = sLoc.Split(',');
                    if (s.Length > 3) Write(key, s[0] + "," + s[1] + "," + s[2] + "," + s[3] + "," + frm.WindowState.ToString());
                    //frm.Location.
                }
            }
        }

        public void SetWindowLocation(Form frm, string key = "Window Location")
        {
            try
            {
                string sLoc = Read(key);
                if (sLoc != "")
                {
                    string[] s = sLoc.Split(',');
                    frm.Location = new Point(System.Convert.ToInt32(s[0]), System.Convert.ToInt32(s[1]));
                    frm.Size = new Size(System.Convert.ToInt32(s[2]), System.Convert.ToInt32(s[3]));
                    if (s.Length == 5) frm.WindowState = (s[4] == "Maximized" ? FormWindowState.Maximized : FormWindowState.Minimized);
                }
                else
                {
                    // First time, so save something
                    Write(key, frm.Location.X + "," + frm.Location.Y + "," + frm.Size.Width + "," + frm.Size.Height);
                }
            }
            catch
            {
            }
        }



    }
}
