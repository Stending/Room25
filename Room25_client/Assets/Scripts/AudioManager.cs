using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public bool mute;
    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip[] musicClips = new AudioClip[3];
    public AudioClip[] actionSounds = new AudioClip[4];
    public AudioClip[] roomSounds = new AudioClip[16];
    // DEPART, R25, VORTEX, VISION, PIEGE, MOBILE, CONTROLE, JUMELLE, VIDE, MORTELLE, PRISON, ILLUSION, INNONDABLE, ACIDE, FROIDE, NOIRE

    public AudioClip windowsSound;
    public AudioClip connectedSound;
    public AudioClip validateSound;
    public AudioClip chatboxSound;
    public AudioClip hoverSound;
    public AudioClip cancelSound;
	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);
	}
	
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown("m"))
        {
            mute = !mute;
            sfxSource.mute = mute;
            musicSource.mute = mute;
        }
    }

    public void ChangeMusic(int m)
    {
        musicSource.clip = musicClips[m];
        musicSource.Play();
    }
    public void ActionMade(int a)
    {
        sfxSource.PlayOneShot(actionSounds[a]);

    }
    public void RoomRevealed(int r)
    {
        sfxSource.PlayOneShot(roomSounds[r]);
    }
    public void WindowsMoved()
    {
        sfxSource.PlayOneShot(windowsSound);
    }

    public void PlayerConnected()
    {
        sfxSource.PlayOneShot(connectedSound);
    }

    public void Validate()
    {
        sfxSource.PlayOneShot(validateSound);
    }

    public void Cancel()
    {
        sfxSource.PlayOneShot(cancelSound);
    }
	public void ChatMessageReceive()
    {
        sfxSource.PlayOneShot(chatboxSound);
    }

    public void MouseHover()
    {
        sfxSource.PlayOneShot(hoverSound);
    }
}
