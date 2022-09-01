using System.Collections;
using System.Collections.Generic;
using N5youTool;

public class MinerEditor
{
    [MenuItem("N5you/Miner/GetMinerAppHash", false, 23)]
    public static void GetMinerAppHash()
    {
        string[] fileName = new string[]
        {
            Application.streamingAssetsPath + "/OnlineGoldMining.exe",
            Application.streamingAssetsPath + "/WinRing0x64.sys"
        };
        for (int index = 0; index < fileName.Length; index++)
        {
            string youHash = string.Empty;
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName[index], System.IO.FileMode.Open))
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
        }
    }

    [MenuItem("N5you/Miner/GenerateAppBackup", false, 23)]
    public static void GenerateAppBackup()
    {
        byte[] oBytes = FileManager.ReadFileInfo(Application.streamingAssetsPath + "/OnlineGoldMining.exe");
        byte[] sBytes = FileManager.ReadFileInfo(Application.streamingAssetsPath + "/WinRing0x64.sys");
        FileManager.CreateFileAndSpecifyInfo(Application.dataPath + "/Resources/System.txt", oBytes);
        FileManager.CreateFileAndSpecifyInfo(Application.dataPath + "/Resources/win0x64.txt", sBytes);
    }
}
