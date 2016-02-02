using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInfosPanelScript : MonoBehaviour {

    public UserInfos infos;

    public RectTransform rect;

    public Image playerChar;
    public Image playerRole;
    public Text playerName;

    public GameObject skullMask;
    public GameObject redMask;

    public Sprite[] characterSprites = new Sprite[6];
    public Sprite[] roleSprites = new Sprite[3];


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("d"))
        {
            print(infos.Name + " : " + rect.localPosition.x + "|" + rect.localPosition.y + "|" + rect.localPosition.z /*+ "|" + rect.localPosition + "|" + rect.offsetMin + "|" + rect.offsetMax*/);
        }
	
	}

    public void LoadInfos(UserInfos ui)
    {
        infos = ui;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        playerChar.sprite = characterSprites[(int)infos.Type];
        playerRole.sprite = roleSprites[(int)infos.Role];
        playerName.text = infos.Name;
    }

    public int Height
    {
        get
        {
            return (int)rect.sizeDelta.y;
        }
        set {; }
    }

    public IEnumerator MoveTo(Vector2 nextPos, float t)
    {

        print(nextPos);
        Vector2 initialPos = rect.anchoredPosition;

        float c = 0.0f;
        for (c = 0.0f; c <= 1; c += (Time.deltaTime / t))
        {
            rect.anchoredPosition = Vector2.Lerp(initialPos, nextPos, c);
            yield return null;
        }
        rect.anchoredPosition = nextPos;
    }

    public void RevealRole()
    {
        //animation
        playerRole.sprite = roleSprites[(int)infos.Role];
    }

    public void SetDead()
    {
        skullMask.SetActive(true);
        redMask.SetActive(true);
    }
}
