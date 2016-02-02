using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImmediateActionsPanelScript : MonoBehaviour {

    public GameManager gameManager;

    public Image[] buttonsImages = new Image[4];
    public bool[] buttonsActive = new bool[4];


    public Animator anim;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AllowActions(bool push, bool control, bool move, bool look)
    {
        print("push : " + push + " control : " + control + " move : " + move + " look : " + look);
        buttonsActive[0] = push;
        buttonsActive[1] = control;
        buttonsActive[2] = move;
        buttonsActive[3] = look;
        UpdateButtonsImages();
        
    }

    private void UpdateButtonsImages()
    {
        for(int i = 0; i < 4; i++)
        {
            if (!buttonsActive[i]) buttonsImages[i].color = Color.grey;
            else buttonsImages[i].color = Color.white;
        }
    }
    public void Appear()
    {
        anim.SetBool("Active", true);
    }

    public void Disappear()
    {
        anim.SetBool("Active", false);
    }

    public void SelectAction(int i)
    {
        if (buttonsActive[i])
        {
            gameManager.ImmediateActionSet(i);
        }
    }
}
