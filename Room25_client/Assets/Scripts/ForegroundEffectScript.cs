using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForegroundEffectScript : MonoBehaviour {

    public Image effectImage;
    //DEPART, R25, VORTEX, VISION, PIEGE, MOBILE, CONTROLE, JUMELLE, VIDE, MORTELLE, PRISON, ILLUSION, INNONDABLE, ACIDE, FROIDE, NOIRE
    public Sprite vortexEffect;
    public Sprite visionEffect;
    public Sprite trapEffect;
    public Sprite mobileEffect;
    public Sprite controlEffect;

    public Sprite tunelEffect;

    public Sprite fireEffect;
    public Sprite prisonEffect;
    public Sprite illusionEffect;
    public Sprite waterEffect;
    public Sprite acidEffect;
    public Sprite freezeEffect;
    public Sprite blackEffect;  

    
    

    public Animator anim;
	// Use this for initialization
	void Start () {
	
	}
    
    public void DisplayEffect(RoomType room)
    {
        switch (room)
        {
            case RoomType.VORTEX:
                effectImage.sprite = vortexEffect;
                break;
            case RoomType.VISION:
                effectImage.sprite = visionEffect;
                break;
            case RoomType.PIEGE:
                effectImage.sprite = trapEffect;
                break;
            case RoomType.MOBILE:
                effectImage.sprite = mobileEffect;
                break;
            case RoomType.CONTROLE:
                effectImage.sprite = controlEffect;
                break;
            case RoomType.JUMELLE:
                break;
            case RoomType.MORTELLE:
                effectImage.sprite = fireEffect;
                break;
            case RoomType.PRISON:
                effectImage.sprite = prisonEffect;
                break;
            case RoomType.ILLUSION:
                effectImage.sprite = illusionEffect;
                break;
            case RoomType.INNONDABLE:
                effectImage.sprite = waterEffect;
                break;
            case RoomType.ACIDE:
                effectImage.sprite = acidEffect;
                break;
            case RoomType.FROIDE:
                effectImage.sprite = freezeEffect;
                break;
            case RoomType.NOIRE:
                effectImage.sprite = blackEffect;
                break;
            default:
                effectImage.enabled = false;
                effectImage.sprite = null;
                return;
        }
        Appear();   
        effectImage.enabled = true ;
    }
	
    public void Appear()
    {
        anim.SetBool("Active", true);
    }

    public void Disappear()
    {
        anim.SetBool("Active", false);
    }
}
