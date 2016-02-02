using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ActionButtonScript : MonoBehaviour {

    public ActionType actualAction = 0;
    public bool panelActive = false;

    public ActionButtonScript otherButton;

    public Animator freezeAnim; 

    public Image image;

    public Animator actionPanel;

    public Button button;
    public Button[] actionButtons = new Button[5];
    public Sprite[] actionSprites = new Sprite[5];
    public bool[] actionsEnabled = new bool[5];
    public int alreadyTaken = -1;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Freeze()
    {
        freezeAnim.SetBool("Active", true);
    }

    public void Unfreeze()
    {
        freezeAnim.SetBool("Active", false);
    }

    public void OpenClosePanel()
    {
        if (panelActive)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        Transform canvas = this.transform.parent;
        this.transform.SetParent(transform.parent.parent);
        this.transform.SetParent(canvas);

        otherButton.ClosePanel();
        actionPanel.SetBool("Active", true);
        panelActive = true;
    }
    public void ClosePanel()
    {
        
        actionPanel.SetBool("Active", false);
        panelActive = false;
    }

    public void setAction(int a)
    {
        if (actionsEnabled[a])
        {
            if (a == alreadyTaken)
            {
                int prevAct = (int)actualAction;
                actualAction = (ActionType)a;
                image.sprite = actionSprites[a];
                otherButton.setAlreadyTakenAction(a);
                otherButton.setAction(prevAct);
            }
            else
            {
                actualAction = (ActionType)a;
                image.sprite = actionSprites[a];
                if (a == 0)
                    otherButton.setAlreadyTakenAction(-1);
                else
                    otherButton.setAlreadyTakenAction(a);
            }
        }
    }
    
    public void enableAction(int a, bool enabled)
    {
        actionsEnabled[a] = enabled;
    }

    public void setAlreadyTakenAction(int a)
    {
        alreadyTaken = a;
    }

}
