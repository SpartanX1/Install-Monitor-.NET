using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;

namespace InstallMonitor.Helper
{
    class RegistryMonitor
    {
       
        public string[] subKeys;
        private XmlDocument doc;
        private XmlElement rootElement;

        public RegistryMonitor()
        {
            doc = new XmlDocument();
            rootElement = doc.CreateElement("Root");
            doc.AppendChild(rootElement);
        }
        public void Iterate(RegistryKey key, XmlElement keyElement = null)
        {

            if (keyElement == null)
            {
                keyElement = doc.CreateElement("Key");
                keyElement.SetAttribute("Name",key.Name);
                rootElement.AppendChild(keyElement);
            }

            if (key.GetSubKeyNames().Length > 0)
            {
                subKeys = key.GetSubKeyNames();

                foreach (var sb in subKeys)
                {
                    XmlElement element = doc.CreateElement("Key");
                    element.SetAttribute("Name", sb);
                    keyElement.AppendChild(element);
                    try
                    {
                        Console.WriteLine("Reading registry key : " + key + "\\" + sb);
                        Iterate(key.OpenSubKey(sb), element);
                    }
                    catch (SecurityException)
                    {
                        Console.WriteLine("Got security exception for registry key : " + key + "\\" + sb);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public void Save(string path)
        {
            doc.Save(path);
        }

        public bool Start(bool HKEY_CLASSES_ROOT, bool HKEY_CURRENT_CONFIG, bool HKEY_CURRENT_USER, bool HKEY_LOCAL_MACHINE, bool HKEY_USERS,bool isCompare)
        {
            ConsoleManager.Show();
            if (isCompare)
                Console.WriteLine("Taking registry snapshot after install");
            if (HKEY_CLASSES_ROOT)
            {
                XmlElement keyRootElement = doc.CreateElement("Key");
                keyRootElement.SetAttribute("Name", "HKEY_CLASSES_ROOT");
                var key = Registry.ClassesRoot;
                Iterate(key, keyRootElement);
                rootElement.AppendChild(keyRootElement);
            }
            if (HKEY_CURRENT_CONFIG)
            {
                XmlElement keyRootElement = doc.CreateElement("Key");
                keyRootElement.SetAttribute("Name", "HKEY_CURRENT_CONFIG");
                var key = Registry.CurrentConfig;
                Iterate(key, keyRootElement);
                rootElement.AppendChild(keyRootElement);
            }
            if (HKEY_CURRENT_USER)
            {
                XmlElement keyRootElement = doc.CreateElement("Key");
                keyRootElement.SetAttribute("Name", "HKEY_CURRENT_USER");
                var key = Registry.CurrentUser;
                Iterate(key, keyRootElement);
                rootElement.AppendChild(keyRootElement);
            }
            if (HKEY_LOCAL_MACHINE)
            {
                XmlElement keyRootElement = doc.CreateElement("Key");
                keyRootElement.SetAttribute("Name", "HKEY_LOCAL_MACHINE");
                var key = Registry.LocalMachine;
                Iterate(key, keyRootElement);
                rootElement.AppendChild(keyRootElement);
            }
            if (HKEY_USERS)
            {
                XmlElement keyRootElement = doc.CreateElement("Key");
                keyRootElement.SetAttribute("Name", "HKEY_USERS");
                var key = Registry.Users;
                Iterate(key, keyRootElement);
                rootElement.AppendChild(keyRootElement);
            }
            if (isCompare)
            {
                Save("After_Install.xml");
                try
                {
                    Console.WriteLine("Starting diff engine..");
                    CompareXml("After_Install.xml", "Before_Install.xml", "Diff.xml");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Save("Before_Install.xml");
                ConsoleManager.Hide();
            }
            return true;
        }

        public void CompareXml(string file1, string file2, string diffFileName)
        {
            Console.WriteLine("Pls wait..");
            try
            {
                String[] linesA = File.ReadAllLines(file1);
                String[] linesB = File.ReadAllLines(file2);

                IEnumerable<String> onlyB = linesB.Except(linesA);
                IEnumerable<String> onlyA = linesA.Except(linesB);

                XmlDocument result = new XmlDocument();
                XmlElement root = result.CreateElement("Root");
                result.AppendChild(root);
                XmlElement added = result.CreateElement("Added");
                foreach (var line in onlyB)
                {
                    XmlElement addedLine = result.CreateElement("Key");
                    addedLine.SetAttribute("Name", line.Replace("Key Name=", string.Empty));
                    added.AppendChild(addedLine);
                }
                root.AppendChild(added);

                XmlElement removed = result.CreateElement("Removed");
                foreach (var line in onlyA)
                {
                    XmlElement removedLine = result.CreateElement("Key");
                    removedLine.SetAttribute("Name", line.Replace("Key Name=", string.Empty));
                    removed.AppendChild(removedLine);
                }
                root.AppendChild(removed);

                result.Save(diffFileName);

                Console.WriteLine("Done!");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                ConsoleManager.Hide();
                System.Diagnostics.Process.Start(diffFileName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
