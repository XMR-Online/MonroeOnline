using N5youTool;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIMining : UIBase<UIMining>
{
    private InputField speedText;
    private InputField HintText;
    public override void Init()
    {
        speedText = transform.Find("speedText").GetComponent<InputField>();
        HintText = transform.Find("BGImage").GetComponent<InputField>();
        transform.Find("PauseButton").GetComponent<Button>().onClick.AddListener(OnPauseButton);
        transform.Find("ClosureButton").GetComponent<Button>().onClick.AddListener(OnClosureButton);
        transform.Find("CloseGraphButton").GetComponent<Button>().onClick.AddListener(OnCloseGraphButton);
        base.Init();
    }

    private void FixedUpdate()
    {
        lock (miningText)
        {
            HintText.text = miningText.ToString();
        }
        lock (speed)
        {
            speedText.text = speed;
        }
    }

    private string speed = string.Empty;
    private StringBuilder miningText = new StringBuilder();
    private Miner myMiner = null;
    private System.Threading.Thread listenMessageThread = null;

    public bool AtWork
    {
        get
        {
            return myMiner != null;
        }
    }

    public void ListenForMessages(Miner miner) 
    {
        if (Main.Instance.GetSetUpInfo.HideTaskbar) 
        {
            miner.RunMiner(false);
            this.OnCloseGraphButton();
            return;
        }
        WriteLine("<color=red>This graphics tool will have performance overhead. If you don't need graphics display, you can use the following</color><color=red>Turn off the graphics and continue mining</color><color=red>The button is turned off, the miner will continue to work, and the graphics tool will no longer have any performance overhead</color>");
        try
        {
            this.myMiner = miner;
            listenMessageThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ReceiveMessage));
            myMiner.RunMiner(false);
            listenMessageThread.Start(); 
        }
        catch (System.Exception ex)
        {
            WriteLine("<color=red>The program encountered an error " + ex.GetHashCode() + " May cause miners to stop working. If it has stopped, try again</color>");
        }
    }

    public override void ClosureProgram()
    {
        if (listenMessageThread != null)
        {
            listenMessageThread.Abort();
            listenMessageThread = null;
        }
        if (myMiner != null)
        {
            myMiner.Dispose();
            myMiner = null;
        }
        lock (this.miningText)
        {
            this.miningText.Remove(0, this.miningText.Length);
        }
        this.HintText.text = string.Empty;
        lock (this.speed)
        {
            this.speed = string.Empty;
        }
        this.speedText.text = string.Empty;
    }

    public override void Conceal()
    {
        this.ClosureProgram();
        base.Conceal();
    }

    private void WriteLineReverseOrder(string text)
    {
        if (1000 < miningText.Length)
        {
            int startIndex = miningText.Length - 900;
            int stopIndex = miningText.Length - 1;
            miningText = miningText.Remove(startIndex, stopIndex);
        }
        miningText = text + "\n" + miningText;
        HintText.text = miningText;
    }

    private void WriteLine(string text) 
    {
        try
        {
            lock (miningText)
            {
                if (1050 < miningText.Length)
                {
                    int length = miningText.ToString().Split('\n')[0].Length + 2;
                    miningText.Remove(0, length);
                }
                miningText.AppendLine(text);
            }
        }
        catch (System.Exception ex)
        {
            WriteLine("<color=red>The program encountered an error " + ex.GetHashCode() + " May cause miners to stop working. If it has stopped, try again</color>");
        }
    }

    private void ReceiveMessage() 
    {
        string text = string.Empty;
        while (true)
        {
            System.Threading.Thread.Sleep(500);
            lock (myMiner)
            {
                if (myMiner.HasExited())
                {
                    if (Main.Instance.GetSetUpInfo.NoSuspension)
                    {
                        myMiner.RunMiner(false);
                    }
                    else
                    {
                        break;
                    }
                }
                text = myMiner.ReadLine();
            }
            if (string.IsNullOrEmpty(text) || text.Equals(string.Empty))
            {
                continue;
            }
            if (text.Contains("new job from"))
            {
                string[] texts = text.Split(new string[] { "net", "new job from", "diff" }, System.StringSplitOptions.RemoveEmptyEntries);
                text = texts[0] + "<color=green>from</color> " + Main.Instance.GetMinerDataInfo.o + " <color=green>Mining new blocks</color> diff" + texts[2];
            }
            else if (text.Contains("speed"))
            {
                string[] texts = text.Split(new string[] { "miner", "speed"}, System.StringSplitOptions.RemoveEmptyEntries);
                text = texts[0] + "<color=aqua>Hashrate (approximately):</color>" + texts[1]; 
                lock (speed)
                {
                    speed = "Hashrate (approximately):" + texts[1];
                }
            }
            WriteLine(text);
        }
        text = "<color=red>Miners have gone on strike, mining has stopped</color>";
        WriteLine(text);
    }

    private void OnPauseButton() 
    {
        try
        {
            myMiner.Exited();
        }
        catch (System.Exception ex)
        {
            WriteLine("<color=red>The program encountered an error " + ex.GetHashCode() + " May cause miners to stop working. If it has stopped, try again</color>");【
        }
    }

    private void OnClosureButton() 
    {
        try
        {
            this.Conceal();
            Main.Instance.GetUIComponent<UILogin>().Display();
        }
        catch (System.Exception ex)
        {
            WriteLine("<color=red>The program encountered an error " + ex.GetHashCode() + " May cause miners to stop working. If it has stopped, try again</color>");
        }
    }

    internal void OnCloseGraphButton() 
    {
        string pathHideWorkID = Application.dataPath + "/HideWorkID.json";
        if (Main.Instance.GetSetUpInfo.Exhale)
        {
            List<HideWorkID> hideWorkIDList = null;
            if (FileManager.Exists(pathHideWorkID))
            {
                byte[] configHideWorkID = FileManager.ReadFileInfo(pathHideWorkID);
                if (configHideWorkID != null)
                {
                    string stringHideWorkID = System.Text.Encoding.UTF8.GetString(configHideWorkID);
                    HideWorkID[] hideWorkIDs = DataTool.FromJSON<HideWorkID[]>(stringHideWorkID);
                    hideWorkIDList = new List<HideWorkID>(hideWorkIDs);
                    FileManager.DeleteFile(pathHideWorkID);
                }
            }
            else
            {
                hideWorkIDList = new List<HideWorkID>();
            }
            hideWorkIDList.Add(new HideWorkID(myMiner.MyMinerProcess.Id, Main.ID));
            string jsonHideWorkID = DataTool.ToJSON(hideWorkIDList.ToArray());
            FileManager.OverwriteCreationFile(pathHideWorkID, jsonHideWorkID);
        }

        if (listenMessageThread != null)
        {
            listenMessageThread.Abort();
            listenMessageThread = null;
        }
        if (!Main.Instance.Quit(true))
        {
        }
        System.Diagnostics.Process cur = System.Diagnostics.Process.GetCurrentProcess();
        cur.Kill();
    }

    internal bool Exhale(int id) 
    {
        try
        {
            myMiner = Miner.GetXMRById(id); 
            if (myMiner.MyMinerProcess != null)
            {
                WriteLine("Call out hidden miners <color=green>" + Main.ID + "</color> success");
                listenMessageThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ReceiveMessage));
                listenMessageThread.Start(); 
                return true;
            }
        }
        catch (System.Exception ex)
        {
            WriteLine("<color=red>The program encountered an error " + ex.GetHashCode() + " May cause miners to stop working. If it has stopped, try again</color>");
        }
        return false;
    }
}
