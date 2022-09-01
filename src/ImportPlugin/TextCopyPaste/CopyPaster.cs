using System;
using System.Collections;
using System.Collections.Generic;

enum ClickType
{
    CLICK_POPMENU,
    CLICK_BLANK,
    CLICK_INPUT
}

public class CopyPaster : MonoBehaviour
{
    private string RemoveColourLabel(string code)
    {
        return code.Replace("</color>", string.Empty).Replace("<color=green>", string.Empty).Replace("<color=red>", string.Empty).Replace("<color=YELLOW>", string.Empty)
            .Replace("<color=CYAN>", string.Empty).Replace("<color=MAGENTA>", string.Empty);
    }

    private RectTransform selfRect;
    public Object RightClickPanel;

    private Button copyBtn;
    private Button pasteBtn;
    private Button cutBtn;

    private InputField targetInput;
    private int strStartPos = -1;
    private int strEndPos = -1;

    private PointerEventData pointerEventData;
    private GraphicRaycaster graRayMainCanvas;
    private List<RaycastResult> results;
    private ClickType tempType;

    private void Start ()
	{
        this.copyBtn = this.RightClickPanel.transform.Find("CopyButton").GetComponent<Button>();
        this.pasteBtn = this.RightClickPanel.transform.Find("PasteButton").GetComponent<Button>();
        this.cutBtn = this.RightClickPanel.transform.Find("CutButton").GetComponent<Button>();

        this.copyBtn.onClick.AddListener(this.CopyBtnClick);
	    this.pasteBtn.onClick.AddListener(this.PasteBtnClick);
	    this.cutBtn.onClick.AddListener(this.CutBtnClick);

        this.selfRect = this.GetComponent<RectTransform>();
	}

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.tempType = this.JudgeClickType();
            if (this.tempType == ClickType.CLICK_BLANK || this.tempType == ClickType.CLICK_INPUT)
            {
                this.RightClickPanel.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(1))
	    {
            Vector2 pos;
	        this.tempType = this.JudgeClickType();
	        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.selfRect, Input.mousePosition,
	            Camera.main, out pos))
	        {
	            if (this.tempType == ClickType.CLICK_INPUT)
	            {
                    if (this.targetInput.selectionAnchorPosition > this.targetInput.selectionFocusPosition)
	                {
	                    this.strStartPos = this.targetInput.selectionFocusPosition;
	                    this.strEndPos = this.targetInput.selectionAnchorPosition;
                    }
	                else
	                {
	                    this.strStartPos = this.targetInput.selectionAnchorPosition;
	                    this.strEndPos = this.targetInput.selectionFocusPosition;
                    }
                    this.RightClickPanel.transform.localPosition = pos;
	                this.RightClickPanel.SetActive(true);
                }
            }
	    }
	}

    private ClickType JudgeClickType()
    {
        this.results = this.MouseResInMainCanvas();
        foreach (var item in this.results)
        {
            if (item.name.Contains("PopMenu"))
            {
                return ClickType.CLICK_POPMENU;
            }
            if (item.GetComponent<InputField>())
            {
                this.targetInput = item.GetComponent<InputField>();
                if (EventSystem.current.currentSelectedObject == null || !EventSystem.current.currentSelectedObject.GetComponent<InputField>())
                {
                    EventSystem.current.SetSelectedObject(this.targetInput);
                    this.targetInput.selectionAnchorPosition = this.targetInput.text.Length;
                }

                return ClickType.CLICK_INPUT;
            }
        }

        return ClickType.CLICK_BLANK;
    }

    private void CutBtnClick()
    {
        GUIUtility.systemCopyBuffer = this.targetInput.text.Substring(this.strStartPos, this.strEndPos - this.strStartPos);
        GUIUtility.systemCopyBuffer = RemoveColourLabel(GUIUtility.systemCopyBuffer);
        UserInputMng.Instance.SetCommand(new DoInputCommand(this.targetInput, this.targetInput.text));
        if (!this.targetInput.readOnly)
        {
            this.targetInput.text = this.targetInput.text.Remove(this.strStartPos, this.strEndPos - this.strStartPos);
        }
        this.RightClickPanel.SetActive(false);
    }

    private void PasteBtnClick()
    {
        if (this.targetInput.readOnly)
        {
            this.RightClickPanel.SetActive(false);
            return;
        }
        UserInputMng.Instance.SetCommand(new DoInputCommand(this.targetInput, this.targetInput.text));
        if (this.strStartPos == this.strEndPos && this.strEndPos == 0)
        {
            this.targetInput.text = GUIUtility.systemCopyBuffer + this.targetInput.text;
        }
        else if (this.strStartPos == this.strEndPos && this.strEndPos == this.targetInput.text.Length)
        {
            this.targetInput.text = this.targetInput.text + GUIUtility.systemCopyBuffer;
        }
        else if(this.strStartPos == this.strEndPos)
        {
            this.targetInput.text = this.targetInput.text.Insert(this.strStartPos, GUIUtility.systemCopyBuffer);
        }
        else
        {
            string temp = this.targetInput.text.Remove(this.strStartPos, this.strEndPos - this.strStartPos);
            this.targetInput.text = temp.Insert(this.strStartPos, GUIUtility.systemCopyBuffer);
        }
        this.RightClickPanel.SetActive(false);
    }

    private void CopyBtnClick()
    {
        GUIUtility.systemCopyBuffer = this.targetInput.text.Substring(this.strStartPos, this.strEndPos - this.strStartPos);
        GUIUtility.systemCopyBuffer = RemoveColourLabel(GUIUtility.systemCopyBuffer);
        this.RightClickPanel.SetActive(false);
    }

    private List<RaycastResult> MouseResInMainCanvas()
    {
        this.pointerEventData = new PointerEventData(EventSystem.current);
        this.pointerEventData.position = Input.mousePosition;
        this.results = new List<RaycastResult>();
        if (this.graRayMainCanvas == null)
        {
            this.graRayMainCanvas = this.GetComponent<GraphicRaycaster>();
        }
        this.graRayMainCanvas.Raycast(this.pointerEventData, this.results);
        return this.results;
    }
}
