using UnityEngine;
using TMPro;

public class SoundManager : Singleton<SoundManager>
{
    public TMP_Text Mute;

    AudioSource OST;

    private void Awake()
    {
        OST = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (OST.isPlaying)
            {
                OST.Stop();
                Mute.text = "Unmute : M";
            }
            else
            {
                OST.Play();
                Mute.text = "mute : M";
            }
        }
    }
}
