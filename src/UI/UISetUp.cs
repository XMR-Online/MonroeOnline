using Microsoft.Win32;
using N5youTool;
using System.Collections;
using System.Collections.Generic;

public class UISetUp : UIBase<UISetUp> 
{
    private Toggle ProhibitMultipleRunsToggle; 
    private Toggle AutomaticStartToggle; 
    private Toggle WorkAutomaticallyToggle; 
    private Toggle NoSuspensionToggle; 
    private Toggle HideTaskbarToggle; 
    private Toggle ExitDirectlyToggle; 
    private Toggle ExhaleToggle; 
    private Button SaveAndReturnButton;
    public override void Init()
    {
        ProhibitMultipleRunsToggle = transform.Find("ProhibitMultipleRunsToggle").GetComponent<Toggle>();
        AutomaticStartToggle = transform.Find("AutomaticStartToggle").GetComponent<Toggle>();
        WorkAutomaticallyToggle = transform.Find("WorkAutomaticallyToggle").GetComponent<Toggle>();
        NoSuspensionToggle = transform.Find("NoSuspensionToggle").GetComponent<Toggle>();
        HideTaskbarToggle = transform.Find("HideTaskbarToggle").GetComponent<Toggle>();
        SaveAndReturnButton = transform.Find("SaveAndReturnButton").GetComponent<Button>();
        SaveAndReturnButton.onClick.AddListener(OnSaveAndReturnButton);
        transform.Find("ReturnButton").GetComponent<Button>().onClick.AddListener(this.Conceal);
        transform.Find("VersionText").GetComponent<Text>().text = "v" + Application.version;
        ExitDirectlyToggle = transform.Find("ExitDirectlyToggle").GetComponent<Toggle>();
        ExhaleToggle = transform.Find("ExhaleToggle").GetComponent<Toggle>();
        base.Init();
    }

    private SetUpInfo setUpInfo;
    public SetUpInfo GetSetUpInfo
    {
        get
        {
            return setUpInfo;
        }
    }

    private void OnSaveAndReturnButton()
    {
        setUpInfo.ProhibitMultipleRuns = ProhibitMultipleRunsToggle.isOn;
        if (setUpInfo.ProhibitMultipleRuns)
        {
            Microsoft.Win32.RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.SetValue(Application.productName, Application.dataPath);
            rk2.Close();
            rk.Close();
        }
        else 
        {
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.DeleteValue(Application.productName, false);
            rk2.Close();
            rk.Close();
        }
        setUpInfo.AutomaticStart = AutomaticStartToggle.isOn;
        setUpInfo.WorkAutomatically = WorkAutomaticallyToggle.isOn;
        setUpInfo.NoSuspension = NoSuspensionToggle.isOn;
        setUpInfo.HideTaskbar = HideTaskbarToggle.isOn;
        setUpInfo.ExitDirectly = ExitDirectlyToggle.isOn;
        setUpInfo.Exhale = ExhaleToggle.isOn;
        string json = DataTool.ToJSON(setUpInfo);
        string path = Application.dataPath + "/SetUpInfo.json";
        if (FileManager.Exists(path))
        {
            FileManager.DeleteFile(path);
        }
        FileManager.OverwriteCreationFile(path, json);
        if (FileManager.Exists(path))
        {
            Main.Instance.MandatoryReminder("Successfully saved");
        }
        else
        {
            Main.Instance.MandatoryReminder("Failed to save");
        }
        this.Conceal();
    }

    public void UpdateInfo(SetUpInfo setUpInfo)
    {
        if (setUpInfo != null)
        {
            this.setUpInfo = setUpInfo;
            ProhibitMultipleRunsToggle.isOn = setUpInfo.ProhibitMultipleRuns;
            AutomaticStartToggle.isOn = setUpInfo.AutomaticStart;
            WorkAutomaticallyToggle.isOn = setUpInfo.WorkAutomatically;
            NoSuspensionToggle.isOn = setUpInfo.NoSuspension;
            HideTaskbarToggle.isOn = setUpInfo.HideTaskbar;
            ExitDirectlyToggle.isOn = setUpInfo.ExitDirectly;
            ExhaleToggle.isOn = setUpInfo.Exhale;
            return;
        }
        this.setUpInfo = new SetUpInfo();
    }
}

[System.Runtime.Serialization.DataContract]
public class SetUpInfo 
{
    [System.Runtime.Serialization.DataMember(Name = "ProhibitMultipleRuns")]
    public bool ProhibitMultipleRuns = false;
    [System.Runtime.Serialization.DataMember(Name = "AutomaticStart")]
    public bool AutomaticStart = false;
    [System.Runtime.Serialization.DataMember(Name = "WorkAutomatically")]
    public bool WorkAutomatically = false;
    [System.Runtime.Serialization.DataMember(Name = "NoSuspension")]
    public bool NoSuspension = false; 
    [System.Runtime.Serialization.DataMember(Name = "HideTaskbar")]
    public bool HideTaskbar = false; 
    [System.Runtime.Serialization.DataMember(Name = "ExitDirectly")]
    public bool ExitDirectly = false;
    [System.Runtime.Serialization.DataMember(Name = "Exhale")]
    public bool Exhale = false;
}
