using System.Collections;
using TMPro;
using UnityEngine;

public class RaceMonitor : MonoBehaviour
{
    [SerializeField]
    private StartingPositionArchLights _startingPositionArchLights;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _countdownBeep;
    [SerializeField]
    private AudioClip _goBeep;
    [SerializeField]
    private TMP_Text _countdownText;

    [SerializeField]
    public static bool racing = false;

    void Start()
    {
        _countdownText.text = "";
                
        StartCoroutine(StartRaceCountdown());

    }


    void Update()
    {
        
    }

    public IEnumerator StartRaceCountdown()
    {
        yield return new WaitForSeconds(.5f);
        for(int i = 5; i >= 0; i--)
        {
            _startingPositionArchLights.SetLightsOn(i);
            switch (i)
            {
                case 5:
                    _audioSource.PlayOneShot(_countdownBeep);
                    _countdownText.color = Color.red;
                    _countdownText.text = "5";
                    break;
                case 4:
                    _audioSource.PlayOneShot(_countdownBeep);
                    _countdownText.color = Color.red;
                    _countdownText.text = "4";
                    break;
                case 3:
                    _audioSource.PlayOneShot(_countdownBeep);
                    _countdownText.color = Color.red;
                    _countdownText.text = "3";
                    break;
                case 2:
                    _audioSource.PlayOneShot(_countdownBeep);
                    _countdownText.color = Color.red;
                    _countdownText.text = "2";
                    break;
                case 1:
                    _audioSource.PlayOneShot(_countdownBeep);
                    _countdownText.color = Color.red;
                    _countdownText.text = "1";
                    break;
                case 0:
                    _audioSource.PlayOneShot(_goBeep);
                    _countdownText.color = Color.green;
                    _countdownText.text = "START";
                    racing = true;
                    break;
            }
            yield return new WaitForSeconds(1);            
        }
        _countdownText.text = "";
    }
}
