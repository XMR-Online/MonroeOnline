using System.Collections;
using System.Collections.Generic;

public class UIStatement : UIBase<UIStatement>
{
    private Button DetermineButton;
    public override void Init()
    {
        base.Init();
        DetermineButton = transform.Find("DetermineButton").GetComponent<Button>();
        DetermineButton.onClick.AddListener(OnDetermineButton);
    }

    private void OnDetermineButton()
    {
        this.Conceal();
    }
}
