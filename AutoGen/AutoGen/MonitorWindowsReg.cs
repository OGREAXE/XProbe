using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.Win32;

namespace AutoGen
{
    public class MonitorWindowsReg
    {
        [DllImport("advapi32.dll", EntryPoint = "RegNotifyChangeKeyValue")]
        private static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree, int dwNotifyFilter, int hEvent, bool fAsynchronus);
        [DllImport("advapi32.dll", EntryPoint = "RegOpenKey")]
        private static extern int RegOpenKey(uint hKey, string lpSubKey, ref IntPtr phkResult);
        [DllImport("advapi32.dll", EntryPoint = "RegCloseKey")]
        private static extern int RegCloseKey(IntPtr hKey);
        private static uint HKEY_CLASSES_ROOT = 0x80000000;
        private static uint HKEY_CURRENT_USER = 0x80000001;
        private static uint HKEY_LOCAL_MACHINE = 0x80000002;
        private static uint HKEY_USERS = 0x80000003;
        private static uint HKEY_PERFORMANCE_DATA = 0x80000004;
        private static uint HKEY_CURRENT_CONFIG = 0x80000005;
        private static uint HKEY_DYN_DATA = 0x80000006;
        private static int REG_NOTIFY_CHANGE_NAME = 0x1;
        private static int REG_NOTIFY_CHANGE_ATTRIBUTES = 0x2;
        private static int REG_NOTIFY_CHANGE_LAST_SET = 0x4;
        private static int REG_NOTIFY_CHANGE_SECURITY = 0x8;
        /// <summary> 
        /// 打开的注册表句饼 
        /// </summary> 
        private IntPtr _OpenIntPtr = IntPtr.Zero;
        private RegistryKey _OpenReg;
        private Hashtable _Date = new Hashtable();
        /// <summary> 
        /// 监视注册表  
        /// </summary> 
        /// <param name="MonitorKey">Microsfot.Win32.RegistryKey</param> 
        public MonitorWindowsReg(RegistryKey MonitorKey)
        {
            if (MonitorKey == null) throw new Exception("注册表参数不能为NULL");
            _OpenReg = MonitorKey;
            string[] _SubKey = MonitorKey.Name.Split('\\');
            uint _MonitorIntPrt = HKEY_CURRENT_USER;
            switch (_SubKey[0])
            {
                case "HKEY_CLASSES_ROOT":
                    _MonitorIntPrt = HKEY_CLASSES_ROOT;
                    break;
                case "HKEY_CURRENT_USER":
                    _MonitorIntPrt = HKEY_CURRENT_USER;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    _MonitorIntPrt = HKEY_LOCAL_MACHINE;
                    break;
                case "HKEY_USERS":
                    _MonitorIntPrt = HKEY_USERS;
                    break;
                case "HKEY_PERFORMANCE_DATA":
                    _MonitorIntPrt = HKEY_PERFORMANCE_DATA;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    _MonitorIntPrt = HKEY_CURRENT_CONFIG;
                    break;
                case "HKEY_DYN_DATA":
                    _MonitorIntPrt = HKEY_DYN_DATA;
                    break;
                default:
                    break;
            }
            string _Text = MonitorKey.Name.Remove(0, MonitorKey.Name.IndexOf('\\') + 1);
            RegOpenKey(_MonitorIntPrt, _Text, ref _OpenIntPtr);
        }
        /// <summary> 
        /// 开始监控 
        /// </summary> 
        public void Star()
        {
            if (_OpenIntPtr == IntPtr.Zero)
            {
                throw new Exception("不能打开的注册项！");
            }
            GetOldRegData();

            System.Threading.Thread _Thread = new System.Threading.Thread(new System.Threading.ThreadStart(Monitor));
            StarMonitor = true;
            _Thread.Start();
        }
        /// <summary> 
        /// 更新老的数据表 
        /// </summary> 
        private void GetOldRegData()
        {
            _Date.Clear();
            string[] OldName = _OpenReg.GetValueNames();
            for (int i = 0; i != OldName.Length; i++)
            {
                _Date.Add(OldName[i], _OpenReg.GetValue(OldName[i]));
            }
        }
        /// <summary> 
        /// 停止监控 
        /// </summary> 
        public void Stop()
        {
            StarMonitor = false;
            RegCloseKey(_OpenIntPtr);
        }
        /// <summary> 
        /// 停止标记 
        /// </summary> 
        private bool StarMonitor = false;
        /// <summary> 
        /// 开始监听 
        /// </summary> 
        public void Monitor()
        {
            while (StarMonitor)
            {
                RegNotifyChangeKeyValue(_OpenIntPtr, false, REG_NOTIFY_CHANGE_NAME + REG_NOTIFY_CHANGE_ATTRIBUTES + REG_NOTIFY_CHANGE_LAST_SET + REG_NOTIFY_CHANGE_SECURITY, 0, false);
                GetUpdate();
            }
        }
        /// <summary> 
        /// 检查数据 
        /// </summary> 
        private void GetUpdate()
        {
            //if (BUpReg != null) BUpReg();

            string[] NewName = _OpenReg.GetValueNames();  //获取当前的名称 
            if (NewName.Length < _Date.Count)    //如果当前少了说明是删除 
            {
                foreach (string Key in _Date.Keys)  //循环刚开始的记录的名称 
                {
                    bool _Del = true;
                    for (int i = 0; i != NewName.Length; i++)     //循环比较 
                    {
                        if (Key == NewName[i])
                        {
                            _Del = false;
                            break;
                        }
                    }
                    if (_Del == true)
                    {
                        if (UpReg != null) UpReg(Key, _Date[Key], "", "");      //删除  
                        GetOldRegData();
                        return;
                    }
                }
            }
            for (int i = 0; i != NewName.Length; i++)
            {
                if (_Date[NewName[i]] == null)
                {
                    if (UpReg != null) UpReg("", "", NewName[i], _OpenReg.GetValue(NewName[i]));   //添加 
                    GetOldRegData();
                    return;
                }
                else
                {
                    if (_Date[NewName[i]].ToString() == _OpenReg.GetValue(NewName[i]).ToString()) continue;
                    //修改 
                    if (UpReg != null) UpReg(NewName[i], _Date[NewName[i]], NewName[i], _OpenReg.GetValue(NewName[i]));
                    
                }
            }
            GetOldRegData();
            return;
        }
        public delegate void UpdataReg(string OldText, object OldValue, string NewText, object NewValue);
        public delegate void BeforeUpReg();

        public event UpdataReg UpReg;
        public event BeforeUpReg BUpReg;
    }
}
