using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace gunggme
{
    // 오디오 유형을 정의하는 열거형
    public enum SoundType {
        VFX,                // 비주얼 효과 오디오
        BackgroundMusic     // 배경 음악 오디오
    }

    public class SoundManager : Singletone<SoundManager>
    {
        public AudioMixer audioMixer;           // 오디오 믹서
        public AudioClip[] vfxClips;            // 비주얼 효과 오디오 클립 배열
        public AudioClip[] backgroundMusicClips;// 배경 음악 오디오 클립 배열
        
        AudioSource vfxAudioSource;            // 비주얼 효과용 오디오 소스
        AudioSource backgroundMusicAudioSource; // 배경 음악용 오디오 소스

        protected override void Awake()
        {
            base.Awake();
            // 비주얼 효과용 오디오 소스 생성
            GameObject vfxObject = new GameObject("VFXAudioSource");
            vfxObject.transform.parent = transform;
            vfxAudioSource = vfxObject.AddComponent<AudioSource>();
        
            // 배경 음악용 오디오 소스 생성
            GameObject backgroundMusicObject = new GameObject("BackgroundMusicAudioSource");
            backgroundMusicObject.transform.parent = transform;
            backgroundMusicAudioSource = backgroundMusicObject.AddComponent<AudioSource>();
        
            // 오디오 소스들이 출력할 믹서 그룹 설정
            vfxAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VFX")[0];
            backgroundMusicAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BackgroundMusic")[0];
        }

        // 오디오 재생 함수
        public void PlaySound(SoundType soundType, int index)
        {
            AudioClip clip = null;
            AudioSource audioSource = null;
        
            // 오디오 타입에 따라 적절한 클립과 오디오 소스 지정
            switch (soundType)
            {
                case SoundType.VFX:
                    if (index >= 0 && index < vfxClips.Length)
                    {
                        clip = vfxClips[index];
                        audioSource = vfxAudioSource;
                    }
                    break;
                case SoundType.BackgroundMusic:
                    if (index >= 0 && index < backgroundMusicClips.Length)
                    {
                        clip = backgroundMusicClips[index];
                        audioSource = backgroundMusicAudioSource;
                        audioSource.loop = true;
                    }
                    break;
            }
            
            // 클립이 존재하고 오디오 소스가 지정되었을 경우 재생
            if (clip != null && audioSource != null && soundType == SoundType.BackgroundMusic)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            }
            else if (clip != null && audioSource != null && soundType == SoundType.VFX)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        // 볼륨 조절 함수
        public void SetVolume(SoundType soundType, float volume)
        {
            // 해당 오디오 믹서 그룹의 볼륨 조절
            audioMixer.SetFloat(soundType.ToString(), Mathf.Log10(volume) * 20);
        }
    }
}
