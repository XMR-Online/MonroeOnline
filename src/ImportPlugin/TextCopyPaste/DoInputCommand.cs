using System;
using System.Collections.Generic;

public class DoInputCommand : ICommand
{
    private InputField targetInput;
    private string undoStr;
    private string redoStr;

    public DoInputCommand(InputField targetInput,string sourceStr)
    {
        this.targetInput = targetInput;
        this.undoStr = sourceStr;
    }

    public void Execute()
    {
    }

    public void ReDo()
    {
        this.undoStr = this.targetInput.text;
        this.targetInput.text = this.redoStr;
    }

    public void UnDo()
    {
        this.redoStr = this.targetInput.text;
        this.targetInput.text = this.undoStr;
    }
}
