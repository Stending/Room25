using UnityEngine;
using System.Collections;



public class PlayerScript : MonoBehaviour {

    public CaracterType character = CaracterType.BG;
    public UserInfos infos;
    public int x = 2, y = 2;

    public Sprite[] sprites = new Sprite[6];

    // Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator Move(Vector2 pos, float t) {
        Vector3 pos3 = new Vector3(pos.x, pos.y, this.transform.position.z);
        Vector3 posPrec = this.transform.position;
        float c = 0.0f;
        for(c = 0.0f; c<=1;c+=(Time.deltaTime/ t)) {

            this.transform.position = Vector3.Lerp(posPrec, pos3, c);
            yield return null;
        }
    }

    public void Initialize(UserInfos infos)
    {
        this.character = infos.Type;
        this.infos = infos;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        this.GetComponent<SpriteRenderer>().sprite = sprites[(int)character];
    }

    public IEnumerator FlyAndLand(Transform father, float t)
    {
        this.transform.parent = this.transform.parent.parent;
        yield return new WaitForSeconds(t);
        this.transform.parent = father;
    }
}
