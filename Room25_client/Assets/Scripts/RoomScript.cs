using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class RoomScript : MonoBehaviour {

    public delegate void SelectRoomAction(int x, int y);
    public event SelectRoomAction OnEnter;
    public event SelectRoomAction OnExit;
    public event SelectRoomAction OnSelect;

    public int x, y;
    public RoomType roomType = RoomType.DEPART;
    public bool visible = false;
    public Animator maskAnim;
    public SpriteRenderer highlightMask;
    public List<Sprite> roomSprites = new List<Sprite>();

    public bool selectable = false;
    public SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
        if (visible)
            Destroy(maskAnim.gameObject);
        UpdateVisual();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateVisual()
    {
        spriteRenderer.sprite = roomSprites[(int)roomType];

    }

    void OnMouseEnter()
    {
        if (selectable)
            OnEnter(x, y);
    }

    void OnMouseExit()
    {
        if (selectable)
            OnExit(x, y);
    }

    void OnMouseDown()
    {
        if (selectable)
        {
            OnSelect(x, y);
        }
    }

    public void RevealHide()
    {
        Reveal();
        Invoke("Hide", 5.0f);
    }

    public void Reveal()
    {
        if (!visible)
        {
            maskAnim.SetBool("Active", false);
            visible = true;
        }
    }

    public void Hide()
    {
        if(visible)
        {
            maskAnim.SetBool("Active", true);
            visible = false;
        }

    }
    public void Highlight(Color col, float shade)
    {
        Color black = new Color(0, 0, 0, 0.3f);
        col.a = 0.3f;
        highlightMask.enabled = true;
        highlightMask.color = Color.Lerp(black, col, shade);
    }
    public void Unlight()
    {
        //if(!visible)
            highlightMask.enabled = false;
    }


    public IEnumerator Move(Vector2 pos, float t)
    {
        Vector3 pos3 = new Vector3(pos.x, pos.y, this.transform.position.z);
        Vector3 posPrec = this.transform.position;
        float c = 0.0f;
        for (c = 0.0f; c <= 1; c += (Time.deltaTime / t))
        {
            this.transform.position = Vector3.Lerp(posPrec, pos3, c);
            yield return null;
        }

        this.transform.position = pos3;
    }
}
