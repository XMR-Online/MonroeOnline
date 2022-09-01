using System.Collections;
using System.Collections.Generic;

public class UserInputMng : MonoBehaviour
{
    public static UserInputMng Instance
    {
        get
        {
            if (_instance == null)
            {
                Object go = new Object("RedoUndoMng");
                _instance = go.AddComponent<UserInputMng>();
            }

            return _instance;
        }
    }
    private static UserInputMng _instance;

    private ICommand iCommand;

    private Stack<ICommand> undoFunctions = new Stack<ICommand>();
    private Stack<ICommand> redoFunctions = new Stack<ICommand>();

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            this.StartCoroutine(this.OnUnDoBtnClicked());
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
        {
            this.StartCoroutine(this.OnReDoBtnClicked());
        }
    }

    public void StartUndo()
    {
        this.StartCoroutine(this.OnUnDoBtnClicked());
    }

    public void StartRedo()
    {
        this.StartCoroutine(this.OnReDoBtnClicked());
    }

    public void SetCommand(ICommand iCommand1)
    {
        this.iCommand = iCommand1;
        if (this.iCommand != null)
        {
            this.iCommand.Execute();
            if (this.redoFunctions.Count > 0)
                this.redoFunctions.Clear();
            this.undoFunctions.Push(this.iCommand);
        }

        this.JudgeStackLength();
    }

    public IEnumerator OnUnDoBtnClicked()
    {
        if (this.undoFunctions.Count > 0)
        {
            this.iCommand = this.undoFunctions.Peek();
            this.redoFunctions.Push(this.iCommand);
            this.undoFunctions.Pop().UnDo();

            this.JudgeStackLength();
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator OnReDoBtnClicked()
    {
        if (this.redoFunctions.Count > 0)
        {
            this.iCommand = this.redoFunctions.Peek();
            this.undoFunctions.Push(this.iCommand);
            this.redoFunctions.Pop().ReDo();

            this.JudgeStackLength();
            yield return new WaitForEndOfFrame();
        }
    }

    private void JudgeStackLength()
    {
        if (this.undoFunctions.Count > 0)
           BluePrintUIMain.Instance.UndoButton.interactable = true;
        else
           BluePrintUIMain.Instance.UndoButton.interactable = false;
        if (this.redoFunctions.Count > 0)
           BluePrintUIMain.Instance.RedoButton.interactable = true;
        else
           BluePrintUIMain.Instance.RedoButton.interactable = false;
    }

    public void ClearStack()
    {
        this.redoFunctions.Clear();
        this.undoFunctions.Clear();
    }
}
