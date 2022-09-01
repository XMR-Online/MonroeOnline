using System.Collections;
using System.Collections.Generic;

public sealed class UIInitial : UIBase<UIInitial> 
{
    public override void Init() 
    {
        transform.Find("exchangeButton").GetComponent<Button>().onClick.AddListener(OnExchangeButton); 
        transform.Find("poolButton").GetComponent<Button>().onClick.AddListener(OnPoolButton); 
        transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartButton); 
    }

    private void OnExchangeButton() 
    {
        Main.Instance.MandatoryReminder("None, nothing. If you want to put "Exchange" and "Mining Pool" into Monero Online, contact email: xmrOnlineMiner@gmail.com (attach contact information), where "Exchange Payment Address" and "Mining Pool User" are allowed to be used as wallets , put into wallet management");
    }

    private void OnPoolButton()
    {
        Main.Instance.MandatoryReminder("None, nothing. \r\nWant to put "Exchange" and "Mining Pool" into Monero Online, contact email: xmrOnlineMiner@gmail.com \r\n(attach contact information)");
    }

    private void OnStartButton() 
    {
        Main.Instance.GetUIComponent<UILogin>().Display();
        this.Conceal();
    }
}
