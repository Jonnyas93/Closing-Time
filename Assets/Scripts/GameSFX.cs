using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameSFX : MonoBehaviour
{
    public AudioSource gameAudioSource;
    public AudioClip[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        gameAudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int soundNum)
    {
        if (audioClips != null)
        {
            gameAudioSource.PlayOneShot(audioClips[soundNum]);
        }
        else
        {
            Debug.Log("Audio clips is empty");
        }
    }

    public void PlaySound(int soundNum, float vol)
    {
        if (audioClips != null)
        {
            gameAudioSource.PlayOneShot(audioClips[soundNum], vol);
        }
        else
        {
            Debug.Log("Audio clips is empty");
        }
    }

    public IEnumerator PlaySoundWait(int soundNum, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (audioClips != null)
        {
            gameAudioSource.PlayOneShot(audioClips[soundNum]);
        }
        else
        {
            Debug.Log("Audio clips is empty");
        }
    }
    public IEnumerator PlaySoundWait(int soundNum, float vol, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (audioClips != null)
        {
            gameAudioSource.PlayOneShot(audioClips[soundNum], vol);
        }
        else
        {
            Debug.Log("Audio clips is empty");
        }
    }
}