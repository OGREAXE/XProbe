using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.VisualBasic;
using Microsoft.Win32;

using MGCPCB;
using MGCPCBAutomationLicensing;
//using MGCPCBEngines;
namespace AutoGen
{
    public partial class Form1 : Form
    {
        private enum MsgSvr { Info, Err };
        MGCPCB.ExpeditionPCBApplication pcbApp = null;
        MGCPCB.Document pcbDoc = null;
        OpenFileDialog openFile = null;

        //True  表示从 Capture ---> Xpedition
        //False 表示从 Xpedition ---> Capture
        static private bool bFromCapture2xpedition = false;
        private bool crossProbeEnable = true;
        private bool fitSelectedEnable = false;
        private bool moveSelectedEnable = false;
        
        MGCPCB.Components compsCollection = null;
        MGCPCB.Component _comp = null;
        MGCPCB.Nets netsCollection = null;
        MGCPCB.Net _net = null;
        MGCPCB.Pins pinsCollection = null;
        MGCPCB.Pin _pin = null;
        
        //测试用
        string strComps = "";
        string strNets = "";
        string strPins = "";
        //测试用

        //列表合并，待定 ...
        List<string> listComps2capture = new List<string>();
        List<string> listNets2capture = new List<string>();
        List<string> listPins2capture = new List<string>();

        List<string> listComps2xpedition = new List<string>();
        List<string> listNets2xpedition = new List<string>();
        List<string> listPins2xpedition = new List<string>();

        private string strPcbDocPath = "";                  //PCB文件名，包括工程路径
        private string strPcbFolderPath = "";               //PCB路径，不包括工程文件名
        private string strPcbName = "";                     //PCB路径，不包括工程文件名
        private int flag = 0;

        public Form1()
        {
            InitializeComponent();
            CheckExpeditionRunning();
            textBoxBrowse.Hide();
            buttonBrowse.Hide();
            //异常
            startMonitorWinRegistry();
        }

        //反向与 OrCAD Capture CIS 交互
        //每次操作完整体数据后需要清空列表
        //To Do Somthing ...
        private void CrossProbe2capture()
        {
            //
            if (listComps2capture.Count != 0)
            {
                //结合ActiveMode ，To Do Something
                Debug.WriteLine("Comps Selected : " + strComps);
                listComps2capture.Clear();
                compsCollection = null;
            }
            if (listNets2capture.Count != 0)
            {
                //结合ActiveMode ，To Do Something
                Debug.WriteLine("Nets Selected : " + strNets);
                listNets2capture.Clear();
                netsCollection = null;
            }
            if (listPins2capture.Count != 0)
            {
                //结合ActiveMode ，To Do Something
                Debug.WriteLine("Pins Selected : " + strPins);
                listPins2capture.Clear();
                netsCollection = null;
            }
            pcbDoc.UnHighlightAll();
            //pcbDoc.UnSelectAll();
        }

        //正向与 Mentor Expedition 交互
        //每次操作完整体数据后需要清空列表
        private void CrossProbe2xpedition(bool select)
        {
            if (crossProbeEnable == true)
            {
                //这里会反过去响应点击的事件，导致不循环
                //需要按正反向单独存放列表吗？
                for (int i = 0; i < listComps2xpedition.Count; i++)
                {
                    _comp = pcbDoc.FindComponent(listComps2xpedition[i]);
                    //_comp = pcbDoc.FindComponent("JA0V1");
                    if (_comp != null)
                    {
                        _comp.Selected = select;
                        _comp.Highlighted = select;
                    }
                }
                for (int i = 0; i < listNets2xpedition.Count; i++)
                {
                    _net = pcbDoc.FindNet(listNets2xpedition[i]);
                    if (_net != null)
                    {
                        _net.Selected = select;
                        _net.Highlighted = select;
                    }
 
                }
                for (int i = 0; i < listPins2xpedition.Count; i++)
                {
                    //如Pin脚为“R1-1”，则分为代表位号的“R1”和该位号的Pin脚号“1”
                    string[] _ref = listPins2xpedition[i].Split('-');
                    Component aComp = FindComponent(_ref[0]);
                    if (aComp != null)
                    {
                        _pin = aComp.FindPin(_ref[1]);
                        if (_pin != null)
                        {
                            _pin.Selected = select;
                            _pin.Highlighted = select;
                        }
                    }
                }
                //如果在布局模式下
                if (pcbApp.Gui.ActiveMode == EPcbMode.epcbModePlace)
                {
                    //Todo Something
                    if (fitSelectedEnable && select)
                    {
                        pcbDoc.ActiveView.SetExtentsToSelection(true, false);
                        if (moveSelectedEnable)
                        {
                            //
                            pcbApp.Gui.ProcessCommand("Edit->Move", true);
                        }
                        else
                        {
                            //Nothing To Do ...
                        }
                    }
                    else
                    {
                        //Nothing To Do ...
                    }
                }
                else
                {
                    if (fitSelectedEnable && select)
                    {
                        pcbDoc.ActiveView.SetExtentsToSelection(true, false);
                    }
                    else
                    {
                        //Nothing To Do ...
                    }
                }
                //每次操作完整体数据后需要清空
                listComps2xpedition.Clear();
                listNets2xpedition.Clear();
                listPins2xpedition.Clear();
            }
            else
            {
                WriteLog(MsgSvr.Info, "正向操作交互已关闭 ！");
                pcbDoc.UnSelectAll();
            }
        }

        private MonitorWindowsReg T;
        private void startMonitorWinRegistry()
        {
            try
            {
                Microsoft.Win32.RegistryKey _Key = Microsoft.Win32.Registry.CurrentUser;
                _Key = _Key.OpenSubKey("SoftWare");
                _Key = _Key.OpenSubKey("VB and VBA Program Settings");
                _Key = _Key.OpenSubKey("XProbe");
                _Key = _Key.OpenSubKey("OrCAD Capture");
                T = new MonitorWindowsReg(_Key);
                T.UpReg += new MonitorWindowsReg.UpdataReg(T__UpdateReg);
                T.BUpReg += new MonitorWindowsReg.BeforeUpReg(T__BeforeUpdateReg);
                T.Star();
            }
            catch (Exception e)
            {
                MessageBox.Show("open registry error!");
            }

        }

        void T__BeforeUpdateReg()
        {
            pcbDoc.UnSelectAll();
        }
        void T__UpdateReg(string OldText, object OldValue, string NewText, object NewValue)
        {
            object Old = OldValue;
            object New = NewValue;
            if (Old == null) Old = "";
            if (New == null) New = "";
           // MessageBox.Show(OldText + ":" + Old.ToString(), NewText + ":" + New.ToString());
            if (Old.ToString().Length > 0)
            {
                String strValue = Old.ToString();
                string[] valStrArray = strValue.Split(',');
                for (int i = 0; i < valStrArray.Length; i++)
                {
                    string aVal = valStrArray[i];

                    if (NewText.Equals("Parts"))
                    {
                        listComps2xpedition.Add(aVal);
                    }
                    else if (NewText.Equals("Nets"))
                    {
                        listNets2xpedition.Add(aVal);
                    }
                    else if (NewText.Equals("Pins"))
                    {
                        listPins2xpedition.Add(aVal);
                    }
                }

                CrossProbe2xpedition(false);
            }

            if (New.ToString().Length > 0)
            {
                String strValue = New.ToString();
                string[] valStrArray = strValue.Split(',');
                for (int i = 0; i < valStrArray.Length; i++)
                {
                    string aVal = valStrArray[i];

                    if (NewText.Equals("Parts"))
                    {
                        listComps2xpedition.Add(aVal);
                    }
                    else if (NewText.Equals("Nets"))
                    {
                        listNets2xpedition.Add(aVal);
                    }
                    else if (NewText.Equals("Pins"))
                    {
                        listPins2xpedition.Add(aVal);
                    }
                }

                CrossProbe2xpedition(true);
            }
            
        }

        private void checkBoxProbe_CheckedChanged(object sender, EventArgs e)
        {
            CheckExpeditionRunning();
            if (checkBoxProbe.Checked)
            {
                crossProbeEnable = true;
                if (flag != 0)
                {
                    buttonBrowse.Hide();
                    textBoxBrowse.Hide();
                    Thread.Sleep(1000);
                    OpenExpedition();
                }
                else
                {
                    buttonBrowse.Show();
                    textBoxBrowse.Show();
                    Thread.Sleep(1000);
                    buttonBrowse.PerformClick();
                    //OpenExpedition(ref pcbApp, ref pcbDoc);
                }
            }
            else
            {
                crossProbeEnable = false;
                textBoxBrowse.Text = "";
                textBoxBrowse.Hide();
                buttonBrowse.Hide();
                if (pcbDoc != null)
                {
                    pcbDoc.UnSelectAll();
                    //此处不能 Null，否则Uncheck后，交互函数会抛出无句柄异常
                    //pcbDoc = null;
                    //pcbApp = null;
                }
                else
                {
                    //
                    WriteLog(MsgSvr.Info, "没有打开的Xpedition程序 ！");
                }
                
                WriteLog(MsgSvr.Info, "交互进程已关闭 ！");
            }

        }
        private void checkBoxFit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFit.Checked)
            {
                fitSelectedEnable = true;
            }
            else
            {
                fitSelectedEnable = false;
            }
        }
        private void checkBoxSel_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSel.Checked)
            {
                moveSelectedEnable = true;
                pcbApp.Gui.ActiveMode = EPcbMode.epcbModePlace;
            }
            else
            {
                moveSelectedEnable = false;
            }
        }

        //以下为测试代码，从Capture ---> Xpedition 功能调试用
        //获取 OrCAD Capture CIS选定的集合存入List后调用正向操作函数即可
        private void buttonComp_Click(object sender, EventArgs e)
        {
            //先设定成从 OrCAD ---> Xpedition
            //待调用完后重新设定成默认 Xpedition ---> OrCAD
            bFromCapture2xpedition = true;
            pcbDoc.UnSelectAll();
            pcbDoc.UnHighlightAll();
            listComps2xpedition.Add("R1");
            listComps2xpedition.Add("R2");
            CrossProbe2xpedition(true);
            bFromCapture2xpedition = false;
        }
        private void buttonNet_Click(object sender, EventArgs e)
        {
            bFromCapture2xpedition = true;
            pcbDoc.UnSelectAll();
            pcbDoc.UnHighlightAll();
            listNets2xpedition.Add("IN");
            listPins2xpedition.Add("R2-1");
            CrossProbe2xpedition(true);
            bFromCapture2xpedition = false;
        }
        private void buttonSelective_Click(object sender, EventArgs e)
        {
            bFromCapture2xpedition = true;
            pcbDoc.UnSelectAll();
            pcbDoc.UnHighlightAll();
            listComps2xpedition.Add("R1");
            listNets2xpedition.Add("OUT");
            CrossProbe2xpedition(true);
            bFromCapture2xpedition = false;
        }

        //Close Expedition
        private void CloseExpedition()
        {
            if (pcbDoc != null)
            {
                if (pcbDoc.Save())
                {
                    Thread.Sleep(1000);
                    WriteLog(MsgSvr.Info, "Saving Document ...\t Done !");
                }
                else
                {
                    WriteLog(MsgSvr.Err, "Saving Document ...\t Failed !");
                }
                if (pcbDoc.Close(true))
                {
                    Thread.Sleep(1000);
                    WriteLog(MsgSvr.Info, "Closing Document ...\t Done !");
                }
                else
                {
                    WriteLog(MsgSvr.Err, "Closing Document ...\t Failed !");
                }
                pcbDoc = null;
            }
            else
            {
                WriteLog(MsgSvr.Err, "Closing Document ...\t Error !");
            }
            if (pcbApp != null)
            {
                pcbApp.Quit();
                Thread.Sleep(1000);
                pcbApp = null;
                WriteLog(MsgSvr.Info, "Closing Application ...\t Done !");
            }
            else
            {
                WriteLog(MsgSvr.Err, "Closing Application ...\t Failed !");
            }
        }
        //点击 Xpedition关闭按钮时，由该程序关闭
        //还需要监测软件异常退出，没有实例句柄存在而造成软件抛出异常！
        bool OnPreClose()
        {
            CloseExpedition();
            return true;
            //throw new NotImplementedException();
        }
        //Validating Server
        private bool ValidateServer()
        {
            bool bOkValidateServer = true;
            try
            {
                //Wrong Method !
                //MGCPCBAutomationLicensing.Application licensing = new MGCPCBAutomationLicensing.Application();

                //Right Method !
                MGCPCBAutomationLicensing.Application licensing = null;
                System.Type t = System.Type.GetTypeFromProgID("MGCPCBAutomationLicensing.Application");
                licensing = (MGCPCBAutomationLicensing.Application)System.Activator.CreateInstance(t);

                int key = pcbDoc.Validate(0);
                WriteLog(MsgSvr.Info, "Got License Key : \t" + key.ToString());
                int token = licensing.GetToken(key);
                WriteLog(MsgSvr.Info, "Got License Token : \t" + token.ToString());
                licensing = null;
                if (pcbDoc.Validate(token) == 0)
                {
                    return bOkValidateServer;
                }
                else
                {
                    WriteLog(MsgSvr.Err, "Failed to Validate Server Automation !");
                    return false;
                }
            }
            catch (Exception ex)
            {
                string msg = "Validating Server Error :\r\n";
                msg += ex.ToString();
                WriteLog(MsgSvr.Err, msg);
                return false;
            }
        }
        //Check ExpeditionPCB Running
        private void CheckExpeditionRunning()
        {
            flag = 0;
            System.Diagnostics.Process[] _processList = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process _process in _processList)
            {
                if (_process.ProcessName == "ExpeditionPCB")
                {
                    flag++;
                }
            }
        }
        //Write Log
        private void WriteLog(MsgSvr severity, string message)
        {
            string msg = "";
            switch (severity)
            {
                case MsgSvr.Info:
                    msg = "# Info. :" + message;
                    break;
                case MsgSvr.Err:
                    msg = "# Error :" + message;
                    break;
            }
            this.textBoxLog.Text += msg + System.Environment.NewLine;
            this.textBoxLog.SelectionStart = this.textBoxLog.Text.Length;
            this.textBoxLog.ScrollToCaret();
            System.Windows.Forms.Application.DoEvents();
            Debug.WriteLine(msg);
        }
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFile = new OpenFileDialog();
            openFile.InitialDirectory = "D:\\";
            openFile.Filter = "(.pcb文件)|*.pcb|全部文件|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                strPcbDocPath = openFileDialog.FileName;
                if (strPcbDocPath.ToUpper().EndsWith(".PCB"))
                {
                    strPcbFolderPath = Path.GetDirectoryName(strPcbDocPath);
                    strPcbName = Path.GetFileNameWithoutExtension(strPcbDocPath).ToUpper();
                    textBoxBrowse.Text = strPcbDocPath;
                    WriteLog(MsgSvr.Info, "PCB Path : \t" + strPcbDocPath);
                    OpenExpedition();
                    buttonBrowse.Hide();
                }
                else
                {
                    WriteLog(MsgSvr.Info, "Invalid PCB Name ,Plz Try Again !");
                    buttonBrowse.Show();
                }
            }
            else
            {
                WriteLog(MsgSvr.Info, "No File Chose ,Plz Try Again !");
                checkBoxProbe.CheckState = CheckState.Unchecked;
                buttonBrowse.Show();
            }
        }
        //Open Expedition
        private void OpenExpedition()
        {
            //WriteLog(MsgSvr.Info, "Openning Expedition ... ...");
            if (flag != 0)
            {
                try
                {
                    pcbApp = (MGCPCB.ExpeditionPCBApplication)Microsoft.VisualBasic.Interaction.GetObject(null, "MGCPCB.ExpeditionPCBApplication");
                }
                catch (Exception ex)
                {
                    string msg = "Getting Expedition Instance Error !";
                    msg += ex.ToString();
                    WriteLog(MsgSvr.Err, msg);
                }
                Thread.Sleep(1000);
                try
                {
                    pcbDoc = pcbApp.ActiveDocument;
                }
                catch (Exception ex)
                {
                    string msg = "Opening Document Error :\r\n";
                    msg += ex.ToString();
                    WriteLog(MsgSvr.Err, msg);
                    //pcbApp.Quit();
                }
            }
            else
            {
                try
                {
                    System.Type t = System.Type.GetTypeFromProgID("MGCPCB.ExpeditionPCBApplication");
                    pcbApp = (MGCPCB.ExpeditionPCBApplication)System.Activator.CreateInstance(t);
                }
                catch (Exception ex)
                {
                    string msg = "Creating Expedition Instance Error !";
                    msg += ex.ToString();
                    WriteLog(MsgSvr.Err, msg);
                }
                Thread.Sleep(1000);
                try
                {
                    pcbDoc = pcbApp.OpenDocument(strPcbDocPath);
                }
                catch (Exception ex)
                {
                    string msg = "Opening Document Error :\r\n";
                    msg += ex.ToString();
                    WriteLog(MsgSvr.Err, msg);
                    pcbApp.Quit();
                }
            }
            if (this.ValidateServer())
            {
                pcbApp.UnlockServer(true);
                pcbApp.Gui.ActiveMode = EPcbMode.epcbModePlace;
                pcbApp.Visible = true;
                pcbApp.Gui.CursorBusy(false);
                pcbApp.Gui.SuppressTrivialDialogs = true;
                //compsCollection = pcbDoc.get_Components(EPcbSelectionType.epcbSelectAll, EPcbComponentType.epcbCompGeneral, EPcbCelltype.epcbCelltypePackage, "");
                //netsCollection = pcbDoc.get_Nets(EPcbSelectionType.epcbSelectAll, true, "");
                //此处需将所有网络，器件全部统计归集到集合里面再做处理
                pcbDoc.OnSelectionChange += new _IMGCPCBDocumentEvents_OnSelectionChangeEventHandler(OnSelectionChange);
                pcbDoc.OnPreClose += new _IMGCPCBDocumentEvents_OnPreCloseEventHandler(OnPreClose);
            }
            else
            {
                WriteLog(MsgSvr.Err, "Failed to Validate PCB Document !");
            }
        }
        //反向与OrCAD交互，从 Xpedition ---> Capture
        void OnSelectionChange(EPcbOnSelectionType eType)
        {
            strComps = "";
            strNets = "";
            strPins = "";
            //从 OrCAD ---> Xpedition 选定交互的过程中有Slected操作
            //会同时调用该点击事件处理函数，因此必须检测是正向还是反向操作
            //在正向操作时给定值为True，处理完毕后再重新设定回默认反向操作
            if (bFromCapture2xpedition)
            {
                Console.WriteLine("点击原理图时又调用了一次该选定事件！");
                //bFromCapture2xpedition = false;
            }
            else
            {
                if (crossProbeEnable)
                {
                    //*
                    switch (pcbApp.Gui.ActiveMode)
                    {
                        case EPcbMode.epcbModeDrawing:
                            break;
                        case EPcbMode.epcbModeModeless:
                            compsCollection = pcbDoc.get_Components(EPcbSelectionType.epcbSelectSelected, EPcbComponentType.epcbCompGeneral, EPcbCelltype.epcbCelltypePackage, "");
                            netsCollection = pcbDoc.get_Nets(EPcbSelectionType.epcbSelectSelected, true, "");
                            break;
                        case EPcbMode.epcbModePlace:
                            compsCollection = pcbDoc.get_Components(EPcbSelectionType.epcbSelectSelected, EPcbComponentType.epcbCompGeneral, EPcbCelltype.epcbCelltypePackage, "");
                            break;
                        case EPcbMode.epcbModeRF:
                            break;
                        case EPcbMode.epcbModeRoute:
                            netsCollection = pcbDoc.get_Nets(EPcbSelectionType.epcbSelectSelected, true, "");
                            pinsCollection = pcbDoc.get_Pins(EPcbSelectionType.epcbSelectSelected);
                            break;
                        default:
                            break;
                    }
                    //*/

                    if (compsCollection != null)
                    {
                        foreach (MGCPCB.Component comp in compsCollection)
                        {
                            listComps2capture.Add(comp.RefDes);
                            strComps += comp.RefDes + ",";
                        }
                    }
                    if (netsCollection != null)
                    {
                        foreach (MGCPCB.Net net in netsCollection)
                        {
                            listNets2capture.Add(net.Name);
                            strNets += net.Name + ",";
                        }
                    }
                    if (pinsCollection != null)
                    {
                        foreach (MGCPCB.Pin pin in pinsCollection)
                        {
                            listPins2capture.Add(pin.Component.RefDes + "-" + pin.Name);
                            strPins += pin.Component.RefDes + "-" + pin.Name + ",";
                        }
                    }
                    //以下代码供测试用，左侧调用后，清除集合内容（但是正向操作时选定操作会同时调用此过程？）
                    //同时检测crossProbeEnable选项是否打开，以判断是否对 CaptureCIS 原理图作操作
                    CrossProbe2capture();
                    //bFromCapture2xpedition = true;
                }
                else
                {
                    WriteLog(MsgSvr.Info, "反向操作交互已关闭 !");
                }
            }
        }
    }
}
