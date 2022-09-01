using N5youTool;
using System;
using System.Collections;
using System.Collections.Generic;
using winforms = System.Windows.Forms;

public sealed class Main : MonoBehaviour
{
    private static int id = System.Diagnostics.Process.GetProcessesByName("XMR-Online").Length;
    public static int ID
    {
        get
        {
            return id;
        }
    }

    private LayoutManager layoutManager = new LayoutManager();
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwd, int cmdShow);
    private static Main instance;
    public static Main Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        int xMax = (int)(Screen.currentResolution.width * 0.9f);
        int yMax = (int)(Screen.currentResolution.height * 0.9f);
        if (xMax < 800 && yMax < 600)
        {
            Screen.SetResolution(xMax, yMax, FullScreenMode.Windowed);
        }
        Screen.SetResolution(680, 400, FullScreenMode.Windowed);
        instance = this;
    }

    private Dictionary<Type, UIBase> uiDictionary = new Dictionary<Type, UIBase>(); 
    private UIInitial initial;
    private UILogin login;
    private UIMining mining;
    private UISetUp setUp;
    private UIMandatoryReminderBox mandatoryReminderBox;
    private UIStatement statement;
    public SetUpInfo GetSetUpInfo
    {
        get
        {
            return setUp.GetSetUpInfo;
        }
    }
    public MinerDataInfo GetMinerDataInfo
    {
        get
        {
            return login.GetMinerDataInfo;
        }
    }
    private void Start()
    {
        initial = transform.Find("initial").GetComponent<UIInitial>();
        initial.Init();
        login = transform.Find("Login").GetComponent<UILogin>();
        login.Init();
        mining = transform.Find("Mining").GetComponent<UIMining>();
        mining.Init();
        setUp = transform.Find("SetUp").GetComponent<UISetUp>();
        setUp.Init();
        byte[] setUpData = FileManager.ReadFileInfo(Application.dataPath + "/SetUpInfo.json");
        SetUpInfo setUpInfo = null;
        if (setUpData != null)
        {
            string seUpString = System.Text.Encoding.UTF8.GetString(setUpData);
            setUpInfo = DataTool.FromJSON<SetUpInfo>(seUpString);
            if (setUpInfo.HideTaskbar)
            {
                var hwd = GetForegroundWindow();
                ShowWindow(hwd, 0);
            }
        }
        setUp.UpdateInfo(setUpInfo);
        byte[] configData = FileManager.ReadFileInfo(Application.dataPath + "/Config.json");
        if (configData != null)
        {
            MinerDataInfo config = null;
            string configString = System.Text.Encoding.UTF8.GetString(configData);
            config = DataTool.FromJSON<MinerDataInfo>(configString);
            login.UpdateInfo(config);
            if (setUpInfo != null && setUpInfo.Exhale)
            {
                string pathHideWorkID = Application.dataPath + "/HideWorkID.json";
                byte[] configHideWorkID = FileManager.ReadFileInfo(pathHideWorkID);
                if (configHideWorkID != null)
                {
                    string stringHideWorkID = System.Text.Encoding.UTF8.GetString(configHideWorkID);
                    HideWorkID[] hideWorkIDs = DataTool.FromJSON<HideWorkID[]>(stringHideWorkID);
                    if (hideWorkIDs.Length > 0)
                    {  
                        List<HideWorkID> hideWorkIDList = new List<HideWorkID>(hideWorkIDs);
                        System.IO.DirectoryInfo[] thisDirectory = System.IO.Directory.GetParent(Application.streamingAssetsPath).GetDirectories();
                        for (int index = 0; index < thisDirectory.Length; index++)
                        {
                            System.IO.DirectoryInfo directory = thisDirectory[index];
                            string directoryName = directory.Name;
                            if (!hideWorkIDList.Exists(o => { return directoryName.Equals(o.PID); }))
                            {
                                System.IO.Directory.Delete(directory.FullName);
                            }
						}
                        HideWorkID HideWorkID = hideWorkIDList[0];
                        hideWorkIDList.Remove(hideWorkIDList[0]);
                        string jsonHideWorkID = DataTool.ToJSON(hideWorkIDList.ToArray());
                        FileManager.DeleteFile(pathHideWorkID);
                        FileManager.OverwriteCreationFile(pathHideWorkID, jsonHideWorkID);
                        id = HideWorkID.PID;
                        if (mining.Exhale(HideWorkID.ID))
                        {
                            mining.Display();
                            login.Conceal();
                        }
                        else
                        {
                            string directoey = Application.streamingAssetsPath + "/" + id;
                            if (System.IO.Directory.Exists(directoey)) 
                            {
                                System.IO.File.Delete(N5youTool.Miner.MINER_PATH);
                                System.IO.File.Delete(N5youTool.Miner.MINER_RELY_PATH);
                                System.IO.Directory.Delete(directoey);
                            }
                        }
                    }
                    else
                    {
                        if (setUpInfo.WorkAutomatically)
                        {
                            login.StartWork(); 
                        }
                    }
                }
                else
                {
                    if (setUpInfo.WorkAutomatically)
                    {
                        login.StartWork(); 
                    }
                }
            }
            else
            {
                if (setUpInfo != null && setUpInfo.WorkAutomatically)
                {
                    login.StartWork(); 
                }
            }
        }
        transform.Find("BGImage/LogoImage").GetComponent<Button>().onClick.AddListener(OnLogoButton);
        uiDictionary.Add(initial.GetType(), initial);
        uiDictionary.Add(login.GetType(), login);
        uiDictionary.Add(mining.GetType(), mining);
        uiDictionary.Add(setUp.GetType(), setUp);
        mandatoryReminderBox = transform.Find("MandatoryReminderBox").GetComponent<UIMandatoryReminderBox>();
        mandatoryReminderBox.Init();
        uiDictionary.Add(mandatoryReminderBox.GetType(), mandatoryReminderBox);
        statement = transform.Find("Statement").GetComponent<UIStatement>();
        statement.Init();
        transform.Find("SetUp/StatementButton").GetComponent<Button>().onClick.AddListener(OnStatementButton);
        layoutManager.Init();
        Application.wantsToQuit += Close;
    }

    private bool isClosureProgram = false; 
    public void ClosureProgram() 
    {
        string appPath = Application.streamingAssetsPath + "/" + Main.ID;
        if (System.IO.Directory.Exists(appPath)) 
        {
            System.IO.File.Delete(N5youTool.Miner.MINER_PATH);
            System.IO.File.Delete(N5youTool.Miner.MINER_RELY_PATH);
            System.IO.Directory.Delete(appPath);
        }
        foreach (var value in uiDictionary.Values)
        {
            value.ClosureProgram();
        }
        this.isClosureProgram = true;
    }

    private bool Close() 
    {
        if (setUp.GetSetUpInfo.ExitDirectly || isClosureProgram) 
        {
            return true;
        }
        layoutManager.HideTaskBar();
        return false;
    }

    private void OnStatementButton()
    {
        statement.Display();
    }

    public void StartToWork(string parameter)
    {
        if (mining.AtWork) 
        {
            System.Windows.Forms.MessageBox.Show("Miners are already working hard", this.GetType().Name, winforms.MessageBoxButtons.OK, winforms.MessageBoxIcon.Error);
            return;
        }
        Miner miner = Miner.StartXMR(parameter);
        login.Conceal(); 
        mining.Display(); 
        mining.ListenForMessages(miner); 
    }

    public T GetUIComponent<T>() where T:UIBase
    {
        return (T)uiDictionary[typeof(T)];
    }

    public void MandatoryReminder(string text) 
    {
        mandatoryReminderBox.Display(text);
    }

    private void OnLogoButton() 
    {
        setUp.Display();
    }

    public bool Quit(bool isExitDirectly) 
    {
        this.isClosureProgram = true;
        if (!isExitDirectly)
        {
            this.ClosureProgram();
        }
        Application.Quit();
        return false;
    }
}

[System.Runtime.Serialization.DataContract]
public class HideWorkID
{
    [System.Runtime.Serialization.DataMember(Name = "id")]
    private int id;
    [System.Runtime.Serialization.DataMember(Name = "pid")]
    private int pid;

    public int ID
    {
        get
        {
            return id;
        }
    }

    public int PID
    {
        get
        {
            return pid;
        }
    }

    private HideWorkID()
    {
    }

    public HideWorkID(int id, int pid)
    {
        this.id = id;
        this.pid = pid;
    }
}
