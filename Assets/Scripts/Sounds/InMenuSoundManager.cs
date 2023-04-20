using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using Random = UnityEngine.Random;

public class InMenuSoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundsAudioSource;
    [SerializeField] private AudioSource voiceChatAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip messageSoundClip;
    [SerializeField] private AudioClip[] musicSoundClips;

    private bool _shouldUpdateVoiceIconState = false;
    private bool _shouldUpdateMusicState = false;
    public static InMenuSoundManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
    }

    private void Update()
    {
        if (_shouldUpdateVoiceIconState == true && voiceChatAudioSource.isPlaying == false)
        {
            Actions.OnRemoveAllVoiceChatsIcons?.Invoke();
            _shouldUpdateVoiceIconState = false;
        }
        
        if (_shouldUpdateMusicState == true && musicAudioSource.isPlaying == false)
        {
            StopMusicClip();
            _shouldUpdateMusicState = false;
        }
    }

    public void PlayMessageClip()
    {
        soundsAudioSource.clip = messageSoundClip;
        soundsAudioSource.Play();
    }
    
    public string PlayMusicClip(int clipCount)
    {
        clipCount = Math.Abs(clipCount - 1);
        if (clipCount > musicSoundClips.Length) clipCount = musicSoundClips.Length - 1;

        musicAudioSource.clip = musicSoundClips[clipCount];
        musicAudioSource.Play();
        _shouldUpdateMusicState = true;
        return musicAudioSource.clip.name;
    }
    
    public void StopMusicClip()
    {
        musicAudioSource.Stop();
        MainMenuUi.Instance.RemoveSongItem();
    }

    public void PlayVoiceChat(byte[] destBuffer)
    {
        byte[] DestBuffer2 = new byte[22050 * 2];
        uint BytesWritten2;
        EVoiceResult ret = SteamUser.DecompressVoice(destBuffer, (uint) destBuffer.Length, DestBuffer2, (uint)DestBuffer2.Length, out BytesWritten2, 22050);
        if(ret == EVoiceResult.k_EVoiceResultOK && BytesWritten2 > 0)
        {
            voiceChatAudioSource.clip = AudioClip.Create(Random.Range(100,1000000).ToString(), 22050, 1, 22050, false);
 
            float[] test = new float[22050];
            for (int i = 0; i < test.Length; ++i)
            {
                test[i] = (short)(DestBuffer2[i * 2] | DestBuffer2[i * 2 + 1] << 8) / 32768.0f;
            }
            voiceChatAudioSource.clip.SetData(test, 0);
            voiceChatAudioSource.Play();
        }
        _shouldUpdateVoiceIconState = true;
    }

}
