using UnityEngine;
using System.Collections;

public class CanvasScript : MonoBehaviour {

    public GameObject inGameUI;
    public GameObject connectionUI;
	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetInGameMode()
    {
        inGameUI.SetActive(true);
        connectionUI.SetActive(false);
    }

    public void SetConnectionMode()
    {
        inGameUI.SetActive(false);
        connectionUI.SetActive(true);
    }
}
