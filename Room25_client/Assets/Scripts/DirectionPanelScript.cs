using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DirectionPanelScript : MonoBehaviour {


    public GameManager gameManager;
    public Animator goAnim;
    public List<Image> arrows = new List<Image>();
    public List<Color> colors = new List<Color>();
	// Use this for initialization
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SendDirection(int dir)
    {
        MouseNotOver(dir);
        gameManager.SetDirection((Direction) dir);
        Close();
    }

    public void Close()
    {
        goAnim.SetBool("Active", false);
        Destroy(this.gameObject, 1.0f);
    }

    public void DisableDirection(Direction dir){
        arrows[(int)dir].enabled = false;
    }

    public void SetColor(ActionType action)
    {
        foreach(Image arr in arrows)
        {
            arr.color = colors[(int)action];
        }

    }

    public void MouseOver(int dir)
    {
        gameManager.DirectionHighlight((Direction)dir, true);
    }

    public void MouseNotOver(int dir)
    {
        gameManager.DirectionHighlight((Direction)dir, false);
    }
}
