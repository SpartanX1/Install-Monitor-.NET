using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace InstallMonitor
{
    class ViewModel : INotifyPropertyChanged
    {

        public Helper.RegistryMonitor registryMonitor;
        public Helper.FileMonitor fileMonitor;

        #region Properties
        public bool HKEY_CLASSES_ROOT { get; set; }
        public bool HKEY_CURRENT_USER { get; set; }
        public bool HKEY_LOCAL_MACHINE { get; set; }
        public bool HKEY_USERS { get; set; }
        public bool HKEY_CURRENT_CONFIG { get; set; }
        public string SelectedDrive { get; set; }

        private string _info;
        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isCompBtnEnabled;
        public bool IsCompBtnEnabled
        {
            get
            {
                return _isCompBtnEnabled;
            }
            set
            {
                _isCompBtnEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isFileMonitorBtnEnabled;
        public bool IsFileMonitorBtnEnabled
        {
            get
            {
                return _isFileMonitorBtnEnabled;
            }
            set
            {
                _isFileMonitorBtnEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isRegSnapBtnEnabled;
        public bool IsRegSnapBtnEnabled
        {
            get
            {
                return _isRegSnapBtnEnabled;
            }
            set
            {
                _isRegSnapBtnEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public Helper.RelayCommand TakeRegistrySnapshotCommand
        {
            get
            {
                return new Helper.RelayCommand(cmd => TakeRegistrySnapshot(), cmd => true); ;
            }
            set
            {

            }
        }

        public Helper.RelayCommand CompareCommand
        {
            get
            {
                return new Helper.RelayCommand(cmd => PrepareCompareRegistry(), cmd => true); ;
            }
            set
            {

            }
        }

        public Helper.RelayCommand FileMonitorCommand
        {
            get
            {
                return new Helper.RelayCommand(cmd => StartFileMonitor(), cmd => true); ;
            }
            set
            {

            }
        }

        private string _registryText;
        public string RegistryText
        {
            get
            {
                return _registryText;
            }
            set
            {
                _registryText = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _drives;

        public event PropertyChangedEventHandler PropertyChanged;


        public List<string> Drives
        {
            get
            {
                var driveInfos = DriveInfo.GetDrives();
                foreach (var drive in driveInfos)
                {
                    if (drive.DriveType == DriveType.Fixed)
                        _drives.Add(drive.Name);
                }
                return _drives;
            }
            set
            {
                value = _drives;
            }
        }
        #endregion

        public ViewModel()
        {
            _drives = new List<string>();
            _registryText = string.Empty;
            registryMonitor = new Helper.RegistryMonitor();
            fileMonitor = new Helper.FileMonitor();
            _isRegSnapBtnEnabled = true;
            _isCompBtnEnabled = false;
            _isFileMonitorBtnEnabled = true;
            _info = string.Empty;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TakeRegistrySnapshot()
        {

            if (HKEY_CLASSES_ROOT || HKEY_CURRENT_CONFIG || HKEY_CURRENT_USER || HKEY_LOCAL_MACHINE || HKEY_USERS)
            {
                IsRegSnapBtnEnabled = false;
                Task.Factory.StartNew(() => registryMonitor.Start(HKEY_CLASSES_ROOT, HKEY_CURRENT_CONFIG, HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, HKEY_USERS, false)).ContinueWith(o => DoneWithSnapshot());
            }
            else
                MessageBox.Show("No registry key selected");
        }

        private void DoneWithSnapshot()
        {
            Helper.ConsoleManager.Hide();
            IsRegSnapBtnEnabled = true;
            Info = "Finished, you can now install the application!";
            IsCompBtnEnabled = true;
        }

        private void PrepareCompareRegistry()
        {
            Task.Factory.StartNew(() => registryMonitor.Start(HKEY_CLASSES_ROOT, HKEY_CURRENT_CONFIG, HKEY_CURRENT_USER, HKEY_LOCAL_MACHINE, HKEY_USERS,true));
        }

        private void StartFileMonitor()
        {
            IsFileMonitorBtnEnabled = false;
            fileMonitor.Watch(SelectedDrive);
        }

    }
}
