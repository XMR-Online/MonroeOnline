using System.Collections;
using System.Collections.Generic;

public class UIMandatoryReminderBox : UIBase<UIMandatoryReminderBox>
{
    private InputField PromptBox;
    private Button DetermineButton;
    public override void Init()
    {
        PromptBox = transform.Find("PromptBox").GetComponent<InputField>();
        DetermineButton = transform.Find("DetermineButton").GetComponent<Button>();
        DetermineButton.onClick.AddListener(OnDetermineButton);
        base.Init();
    }

    private void OnDetermineButton()
    {
        PromptBox.text = string.Empty;
        this.Conceal();
    }

    public void Display(string text)
    {
        PromptBox.text = text;
        this.Display();
    }
}
