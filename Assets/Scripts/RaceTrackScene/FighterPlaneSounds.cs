using System.Collections;
using Enums;
using UnityEngine;

namespace RaceTrackScene
{
    public class FighterPlaneSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource catapultWarningSound;
        [SerializeField] private AudioSource catapultCountdownSound;
        [SerializeField] private AudioSource lowHpSound;
        [SerializeField] private int delayBetweenLowHpAlarm;

        private IEnumerator _lowHpAudioCoroutine;
        
        public void ProcessCriticalHealthPointsSound(bool playSound)
        {
            if (playSound == true && lowHpSound.isPlaying == false)
            {
                _lowHpAudioCoroutine = LowHpAudioCoroutine();
                StartCoroutine(_lowHpAudioCoroutine);
            }
            else if (playSound == false)
            {
                if (_lowHpAudioCoroutine != null)
                {
                    StopCoroutine(_lowHpAudioCoroutine);
                }
                
                lowHpSound.Stop();
            }
        }

        public void CatapultCountdownAndWarningSoundPlay(FighterPlaneType fighterPlaneType)
        {
            CatapultWarningSoundPlay();
            CatapultCountdownSoundPlay();
        }
        
        public void DisableAllPlaneSounds()
        {
            catapultWarningSound.Stop();
            catapultCountdownSound.Stop();
            if (_lowHpAudioCoroutine != null)
            {
                StopCoroutine(_lowHpAudioCoroutine);
            }
            lowHpSound.Stop();
        }

        private void CatapultWarningSoundPlay()
        {
            if (catapultWarningSound.isPlaying == false)
            {
                catapultWarningSound.Play();
            }
        }
        
        private void CatapultCountdownSoundPlay()
        {
            if (catapultCountdownSound.isPlaying == false)
            {
                catapultCountdownSound.Play();
            }
        }
        
        private IEnumerator LowHpAudioCoroutine()
        {
            while (true)
            {
                lowHpSound.Play();
                yield return new WaitForSeconds(delayBetweenLowHpAlarm);
            }
        }
    }
}
