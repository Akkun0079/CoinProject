using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;

    //SE���i�[����z��
    public AudioClip[] sound;

    private int lastPlayedSoundIndex = -1; // 最後に再生された効果音のインデックス


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    public void SoundEvolveMiss()
    {
         audioSource.clip = sound[2];
        audioSource.PlayOneShot(audioSource.clip);
    }


    public void PlaySound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex >= sound.Length)
        {
            Debug.LogError("Invalid sound index");
            return;
        }

        // 同じ効果音が再生中でないか確認し、再生中なら停止する
        if (lastPlayedSoundIndex == soundIndex && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 効果音を再生する
        audioSource.PlayOneShot(sound[soundIndex]);

        // 最後に再生された効果音のインデックスを更新
        lastPlayedSoundIndex = soundIndex;
    }

    public void PlaySound2()
    {
        audioSource.clip = sound[3];
        audioSource.PlayOneShot(audioSource.clip);
    }
    
    public void cancelSound()
    {
        audioSource.clip = sound[4];
        audioSource.PlayOneShot(audioSource.clip);
    }
        public void PlaySound3()
    {
        audioSource.clip = sound[5];
        audioSource.PlayOneShot(audioSource.clip);
    }
}
