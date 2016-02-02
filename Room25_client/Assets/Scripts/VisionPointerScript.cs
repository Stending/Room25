using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisionPointerScript : MonoBehaviour {

    public Image characterImage;

    public Sprite[] characterSprites = new Sprite[6];
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void SetCharacter(int c)
    {
        characterImage.sprite = characterSprites[c];
    }
}
