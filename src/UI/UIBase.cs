using System.Collections;
using System.Collections.Generic;

public class ComponentBase : MonoBehaviour
{
    public virtual void ClosureProgram() 
    {
    }
}

public class UIBase : ComponentBase
{
    public virtual void Init()
    {
        Object.SetActive(false);
    }

    public virtual void Display()
    {
        Object.SetActive(true);
    }

    public virtual void Conceal()
    {
        Object.SetActive(false);
    }
}

public class UIBase<T> : UIBase where T : UIBase<T>
{

}
