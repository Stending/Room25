using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterButtonScript : MonoBehaviour {

    public Animator darkMaskAnim;
    public Animator bandAnim;
    public Text playerNameText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Disable(string playerName)
    {
        darkMaskAnim.SetBool("Active", true);
        bandAnim.SetBool("Active", true);
        playerNameText.gameObject.SetActive(true);
        playerNameText.text = playerName;
    }

    public void Enable()
    {
        darkMaskAnim.SetBool("Active", false);
        bandAnim.SetBool("Active", false);
        playerNameText.gameObject.SetActive(false);
    }
}
