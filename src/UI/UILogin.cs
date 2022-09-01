using N5youTool;
using System.Collections;
using System.Collections.Generic;
using winforms = System.Windows.Forms;

[System.Runtime.Serialization.DataContract]
public sealed class MinerDataInfo 
{
    [System.Runtime.Serialization.DataContract]
    public sealed class AMD 
    {
        [System.Runtime.Serialization.DataMember(Name = "enable")]
        public bool enable;

        public string GetArguments()
        {
            if (enable)
            {
                return " --opencl";
            }
            return string.Empty;
        }
    }

    [System.Runtime.Serialization.DataContract]
    public sealed class NVIDIA 
    {
        [System.Runtime.Serialization.DataMember(Name = "enable")]
        public bool enable;

        public string GetArguments()
        {
            if (enable)
            {
                return " --cuda";
            }
            return string.Empty;
        }
    }

    [System.Runtime.Serialization.DataContract]
    public sealed class CPU 
    {
        [System.Runtime.Serialization.DataMember(Name = "enable")]
        public bool enable;
        [System.Runtime.Serialization.DataMember(Name = "t")]
        public string t; 

        public string GetArguments()
        {
            if (enable)
            {
                return " --no-cpu";
            }
            if (string.IsNullOrEmpty(t))
            {
                return string.Empty;
            }
            string arguments = " -t " + t;
            return arguments;
        }
    }

    [System.Runtime.Serialization.DataContract]
    public sealed class HTTPAPI
    {
        [System.Runtime.Serialization.DataMember(Name = "enable")]
        public bool enable;
        [System.Runtime.Serialization.DataMember(Name = "api_worker_id")]
        public string api_worker_id;
        [System.Runtime.Serialization.DataMember(Name = "http_host")]
        public string http_host; 
        [System.Runtime.Serialization.DataMember(Name = "http_port")]
        public string http_port; 
        [System.Runtime.Serialization.DataMember(Name = "http_access_token")]
        public string http_access_token; 
        [System.Runtime.Serialization.DataMember(Name = "http_no_restricte")]
        public bool http_no_restricte; 

        public string GetArguments()
        {
            if (enable)
            {
                string arguments = " --api-worker-id " + api_worker_id;
                arguments = arguments + " --http-host " + http_host;
                arguments = arguments + " --http-port " + http_port;
                arguments = arguments + " --http-access-token " + http_access_token;
                if (http_no_restricte)
                {
                    arguments = arguments + " --http-no-restricted";
                }
                return arguments;
            }
            return string.Empty;
        }
    }

    [System.Runtime.Serialization.DataContract]
    public sealed class Acting 
    {
        [System.Runtime.Serialization.DataMember(Name = "enable")]
        public bool enable;
        [System.Runtime.Serialization.DataMember(Name = "o")]
        public string o; 
        [System.Runtime.Serialization.DataMember(Name = "rig_id")]
        public string rig_id; 

        public string GetArguments()
        {
            if (enable)
            {
                return " --rig-id " + rig_id;
            }
            return string.Empty;
        }
    }

    [System.Runtime.Serialization.DataMember(Name = "a")]
    public byte a; 
    [System.Runtime.Serialization.DataMember(Name = "o")]
    public string o; 
    [System.Runtime.Serialization.DataMember(Name = "u")]
    public string u; 
    [System.Runtime.Serialization.DataMember(Name = "p")]
    public string p; 
    [System.Runtime.Serialization.DataMember(Name = "daemon")]
    public bool daemon; 
    [System.Runtime.Serialization.DataMember(Name = "TLS")]
    public bool TLS; 
    [System.Runtime.Serialization.DataMember(Name = "amd")]
    public AMD amd = new AMD();
    [System.Runtime.Serialization.DataMember(Name = "nvidia")]
    public NVIDIA nvidia = new NVIDIA();
    [System.Runtime.Serialization.DataMember(Name = "cpu")]
    public CPU cpu = new CPU();
    [System.Runtime.Serialization.DataMember(Name = "httpapi")]
    public HTTPAPI httpapi = new HTTPAPI();
    [System.Runtime.Serialization.DataMember(Name = "acting")]
    public Acting acting = new Acting();
    [System.Runtime.Serialization.DataMember(Name = "coin")]
    public sbyte coin;
    [System.Runtime.Serialization.DataMember(Name = "isPrintfLog")]
    public bool isPrintfLog;

    public string GetArguments()
    {
        string parameter = " -o " + o.Replace("\n", string.Empty).Replace("\r", string.Empty);
        parameter = parameter + " -u " + u.Replace("\n", string.Empty).Replace("\r", string.Empty);
        string ac = acting.GetArguments();
        if (string.IsNullOrEmpty(ac))
        {
            if (string.IsNullOrEmpty(p))
            {
                parameter = parameter + " -p x";
            }
            else
            {
                parameter = parameter + " -p " + p.Replace("\n", string.Empty).Replace("\r", string.Empty);
            }
        }
        else
        {
            parameter = parameter + ac;
        }
        if (daemon)
        {
            parameter = parameter + " --daemon";
        }
        if (TLS)
        {
            parameter = parameter + " --tls";
        }
        return parameter;
    }
}

public sealed class UILogin : UIBase<UILogin>
{
    private sealed class CPUImage 
    {
        public Toggle noCpuToggle; 
        public InputField tInputField; 
        private Transform transform;
        public CPUImage(Transform transform)
        {
            this.transform = transform;
            tInputField = transform.Find("tInputField").GetComponent<InputField>();
            noCpuToggle = transform.Find("noCpuToggle").GetComponent<Toggle>();
        }

        public MinerDataInfo.CPU GetInfo() 
        {
            MinerDataInfo.CPU cpu = new MinerDataInfo.CPU();
            cpu.enable = noCpuToggle.isOn;
            cpu.t = tInputField.text;
            return cpu;
        }

        public void UpdataInfo(MinerDataInfo.CPU info)
        {
            noCpuToggle.isOn = info.enable;
            tInputField.text = info.t;
        }
    }

    private sealed class AMDImage
    {
        public Toggle noAMDToggle; 
        public AMDImage(Transform transform)
        {
            noAMDToggle = transform.Find("noAMDToggle").GetComponent<Toggle>();
        }

        public MinerDataInfo.AMD GetInfo()
        {
            MinerDataInfo.AMD amd = new MinerDataInfo.AMD();
            amd.enable = noAMDToggle.isOn;
            return amd;
        }

        public void UpdataInfo(MinerDataInfo.AMD amd)
        {
            noAMDToggle.isOn = amd.enable;
        }
    }

    private sealed class NVIDIAImage
    {
        public Toggle noNVIDIAToggle;
        public NVIDIAImage(Transform transform)
        {
            noNVIDIAToggle = transform.Find("noNVIDIAToggle").GetComponent<Toggle>();
        }

        public MinerDataInfo.NVIDIA GetInfo()
        {
            MinerDataInfo.NVIDIA nvidia = new MinerDataInfo.NVIDIA();
            nvidia.enable = noNVIDIAToggle.isOn;
            return nvidia;
        }

        public void UpdataInfo(MinerDataInfo.NVIDIA info)
        {
            noNVIDIAToggle.isOn = info.enable;
        }
    }

    private sealed class ActingBGImage 
    {
        public Toggle noActingToggle;
        public InputField oInputField;
        public InputField rigIdInputField;
        private Button ClosureButton;
        private Transform transform;
        public ActingBGImage(Transform transform)
        {
            this.transform = transform;
            noActingToggle = transform.Find("noActingToggle").GetComponent<Toggle>();
            oInputField = transform.Find("oInputField").GetComponent<InputField>();
            rigIdInputField = transform.Find("rigIdInputField").GetComponent<InputField>();
            ClosureButton = transform.Find("ClosureButton").GetComponent<Button>();
            ClosureButton.onClick.AddListener(OnClosureButton);
        }

        private void OnClosureButton()
        {
            transform.SetActive(false);
        }

        public void SetActive(bool active)
        {
            transform.SetActive(active);
        }

        public static explicit operator MinerDataInfo.Acting(ActingBGImage cActing)
        {
            MinerDataInfo.Acting acting = new MinerDataInfo.Acting();
            acting.enable = cActing.noActingToggle.isOn;
            acting.o = cActing.oInputField.text;
            acting.rig_id = cActing.rigIdInputField.text;
            return acting;
        }

        public void UpdataInfo(MinerDataInfo.Acting info)
        {
            noActingToggle.isOn = info.enable;
            oInputField.text = info.o;
            rigIdInputField.text = info.rig_id;
        }
    }

    private sealed class HTTPAPIbgImage
    {
        public Toggle noHTTPAPIToggle;
        public Toggle httpNoRestrictedToggle;
        public InputField apiWorkerIdInputField;
        public InputField httpHostInputField;
        public InputField httpPortInputField;
        public InputField httpAccessTokenInputField;
        private Button ClosureButton;
        private Transform transform;
        public HTTPAPIbgImage(Transform transform)
        {
            this.transform = transform;
            noHTTPAPIToggle = transform.Find("noHTTPAPIToggle").GetComponent<Toggle>();
            httpNoRestrictedToggle = transform.Find("httpNoRestrictedToggle").GetComponent<Toggle>();
            apiWorkerIdInputField = transform.Find("apiWorkerIdInputField").GetComponent<InputField>();
            httpHostInputField = transform.Find("httpHostInputField").GetComponent<InputField>();
            httpPortInputField = transform.Find("httpPortInputField").GetComponent<InputField>();
            httpAccessTokenInputField = transform.Find("httpAccessTokenInputField").GetComponent<InputField>();
            ClosureButton = transform.Find("ClosureButton").GetComponent<Button>();
            ClosureButton.onClick.AddListener(OnClosureButton);
        }

        private void OnClosureButton()
        {
            transform.SetActive(false);
        }

        public void SetActive(bool active)
        {
            transform.SetActive(active);
        }

        public static explicit operator MinerDataInfo.HTTPAPI(HTTPAPIbgImage httpapi)
        {
            MinerDataInfo.HTTPAPI http = new MinerDataInfo.HTTPAPI();
            http.enable = httpapi.noHTTPAPIToggle.isOn;
            http.http_no_restricte = httpapi.httpNoRestrictedToggle.isOn;
            http.api_worker_id = httpapi.apiWorkerIdInputField.text;
            http.http_host = httpapi.httpHostInputField.text;
            http.http_port = httpapi.httpPortInputField.text;
            http.http_access_token = httpapi.httpAccessTokenInputField.text;
            return http;
        }

        public void Updata(MinerDataInfo.HTTPAPI info)
        {
            noHTTPAPIToggle.isOn = info.enable;
            httpNoRestrictedToggle.isOn = info.http_no_restricte;
            apiWorkerIdInputField.text = info.api_worker_id;
            httpHostInputField.text = info.http_host;
            httpPortInputField.text = info.http_port;
            httpAccessTokenInputField.text = info.http_access_token;
        }
    }

    private InputField oInputField;
    private InputField uInputField;
    private InputField pInputField;
    private Toggle daemonToggle; 
    private Toggle TLSToggle;
    private Dropdown aDropdown; 
    private Button actingButton; 
    private Button HTTPAPIButton; 
    private Button startButton;
    private Button saveButton;
    private MinerDataInfo minerDataInfo = new MinerDataInfo();
    private CPUImage cPUImage;
    private AMDImage aMDImage;
    private NVIDIAImage nVIDIAImage;
    private ActingBGImage actingBGImage;
    private HTTPAPIbgImage hTTPAPIbgImage;

    private Dropdown coinDropdown; 
    private Toggle logToggle; 

    public MinerDataInfo GetMinerDataInfo
    {
        get
        {
            return minerDataInfo;
        }
    }

    public override void Init()
    {
        cPUImage = new CPUImage(transform.Find("CPUImage"));
        aMDImage = new AMDImage(transform.Find("AMDImage"));
        nVIDIAImage = new NVIDIAImage(transform.Find("NVIDIAImage"));
        actingBGImage = new ActingBGImage(transform.Find("actingBGImage"));
        actingBGImage.oInputField.onEndEdit.AddListener(OnActingOInputField);
        hTTPAPIbgImage = new HTTPAPIbgImage(transform.Find("HTTPAPIbgImage"));
        daemonToggle = transform.Find("Basis/daemonToggle").GetComponent<Toggle>();
        TLSToggle = transform.Find("Basis/TLSToggle").GetComponent<Toggle>();
        oInputField = transform.Find("Basis/oInputField").GetComponent<InputField>();
        uInputField = transform.Find("Basis/uInputField").GetComponent<InputField>();
        pInputField = transform.Find("Basis/pInputField").GetComponent<InputField>();
        aDropdown = transform.Find("Basis/aDropdown").GetComponent<Dropdown>();
        startButton = transform.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(OnStartButton);
        saveButton = transform.Find("SaveButton").GetComponent<Button>();
        saveButton.onClick.AddListener(OnSaveButton);
        actingButton = transform.Find("actingButton").GetComponent<Button>();
        actingButton.onClick.AddListener(OnActingButton);
        HTTPAPIButton = transform.Find("HTTPAPIButton").GetComponent<Button>();
        HTTPAPIButton.onClick.AddListener(OnHTTPAPIButton);
        transform.Find("ReadButton").GetComponent<Button>().onClick.AddListener(OnReadButton);
        coinDropdown = transform.Find("coinDropdown").GetComponent<Dropdown>();
        logToggle = transform.Find("logToggle").GetComponent<Toggle>();
    }

    private void OnReadButton() 
    {
        byte[] configData = FileManager.ReadFileInfo(Application.dataPath + "/Config.json");
        MinerDataInfo config = null;
        if (configData != null)
        {
            string configString = System.Text.Encoding.UTF8.GetString(configData);
            config = DataTool.FromJSON<MinerDataInfo>(configString);
            this.UpdateInfo(config);
            if (config != null)
            {
                Main.Instance.MandatoryReminder("Loaded successfully");
                return;
            }
            Main.Instance.MandatoryReminder("Failed to load");
            return;
        }
        Main.Instance.MandatoryReminder("config file not found");
    }

    private void OnActingOInputField(string text)
    {
        oInputField.text = text;
    }

    public void UpdateInfo(MinerDataInfo loginInfo) 
    {
        this.minerDataInfo = loginInfo;
        aDropdown.value = loginInfo.a;
        oInputField.text = loginInfo.o;
        uInputField.text = loginInfo.u;
        pInputField.text = loginInfo.p;
        cPUImage.UpdataInfo(loginInfo.cpu);
        aMDImage.UpdataInfo(loginInfo.amd);
        nVIDIAImage.UpdataInfo(loginInfo.nvidia);
        actingBGImage.UpdataInfo(loginInfo.acting);
        hTTPAPIbgImage.Updata(loginInfo.httpapi);
        daemonToggle.isOn = loginInfo.daemon;
        TLSToggle.isOn = loginInfo.TLS;
        coinDropdown.value = loginInfo.coin;
        logToggle.isOn = loginInfo.isPrintfLog;
    }

    private void SynchronizationMinerDataInfo() 
    {
        minerDataInfo.a = (byte)aDropdown.value;
        minerDataInfo.o = oInputField.text;
        minerDataInfo.u = uInputField.text;
        minerDataInfo.p = pInputField.text;
        minerDataInfo.cpu = cPUImage.GetInfo();
        minerDataInfo.amd = aMDImage.GetInfo();
        minerDataInfo.nvidia = nVIDIAImage.GetInfo();
        minerDataInfo.acting = (MinerDataInfo.Acting)actingBGImage;
        minerDataInfo.httpapi = (MinerDataInfo.HTTPAPI)hTTPAPIbgImage;
        minerDataInfo.daemon = daemonToggle.isOn;
        minerDataInfo.TLS = TLSToggle.isOn;
        minerDataInfo.coin = (sbyte)coinDropdown.value;
        minerDataInfo.isPrintfLog = logToggle.isOn;
    }

    private void OnActingButton()
    {
        actingBGImage.SetActive(true);
    }

    private void OnHTTPAPIButton()
    {
        hTTPAPIbgImage.SetActive(true);
    }

    private void OnStartButton() 
    {
        this.SynchronizationMinerDataInfo(); 
        if (string.IsNullOrEmpty(minerDataInfo.o))
        {
            if (LayoutManager.Visible)
            {
                System.Windows.Forms.MessageBox.Show("No mining pool address will not work", "error", winforms.MessageBoxButtons.OK, winforms.MessageBoxIcon.Error);
                return;
            }
            Main.Instance.MandatoryReminder("No mining pool address will not work");
            return;
        }
        if (string.IsNullOrEmpty(minerDataInfo.u))
        {
            if (LayoutManager.Visible)
            {
                System.Windows.Forms.MessageBox.Show("If you don't have a wallet, you will not be able to receive income", "error", winforms.MessageBoxButtons.OK, winforms.MessageBoxIcon.Error);
                return;
            }
            Main.Instance.MandatoryReminder("If you don't have a wallet, you will not be able to receive income");
            return;
        }
        string parameter = string.Empty;
        if (aDropdown.value != 0) 
        {
            string a = "-a " + aDropdown.captionText.text + " ";
            parameter = a + parameter;
        }
        if (coinDropdown.value != 0) 
        {
            string coin = " --coin=" + coinDropdown.captionText.text + " "; 
            parameter = parameter + coin;
        }
        parameter = parameter + minerDataInfo.cpu.GetArguments() + minerDataInfo.httpapi.GetArguments() + minerDataInfo.amd.GetArguments()
            + minerDataInfo.nvidia.GetArguments() + minerDataInfo.GetArguments();
        if (logToggle.isOn) 
        {
            string dir = System.Environment.CurrentDirectory + @"\log";
            if (System.IO.Directory.Exists(dir) == false) 
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            string fileName = System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day + "." + System.DateTime.Now.Hour + "." +
    System.DateTime.Now.Minute + "." + System.DateTime.Now.Second + ".log.txt";
            string path = dir + @"\" + fileName;
            System.IO.FileStream logFileStream = System.IO.File.Create(path);
            parameter = parameter + " --log-file=" + path;
        }
        Main.Instance.StartToWork(parameter + " --no-color");
    }

    public void StartWork()
    {
        this.OnStartButton();
    }

    private void OnSaveButton()
    {
        this.SynchronizationMinerDataInfo();
        string json = DataTool.ToJSON(minerDataInfo);
        string path = Application.dataPath + "/Config.json";
        if (FileManager.Exists(path))
        {
            FileManager.DeleteFile(path);
        }
        FileManager.OverwriteCreationFile(path, json);
        if (FileManager.Exists(path))
        {
            Main.Instance.MandatoryReminder("Successfully saved");
            return;
        }
        Main.Instance.MandatoryReminder("Failed to save");
    }
}
