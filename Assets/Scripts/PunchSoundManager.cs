using System;
using UnityEngine;
using Random = UnityEngine.Random;

internal class PunchSoundManager : MonoBehaviour {
    public static PunchSoundManager Instance {
        get {
            if (_instance == null) {
                throw new InvalidOperationException();
            }

            return _instance;
        }
    }

    private static PunchSoundManager _instance;

    [SerializeField]
    private AudioClip _doubleClip;

    [SerializeField]
    private AudioClip[] _intensiveClips;

    [SerializeField]
    private AudioClip[] _weakClips;

    [SerializeField]
    private AudioSource _audioSource;

    public void PlayDouble() {
        _audioSource.PlayOneShot(_doubleClip);
    }

    public void Play(PunchIntensity intensity) {
        var arr = intensity is PunchIntensity.Powerful ? _intensiveClips : _weakClips;
        var rand = Random.Range(0, arr.Length);

        var clip = arr[rand];

        _audioSource.PlayOneShot(clip);
    }

    private void Awake() {
        _instance = this;
    }

    private void OnDestroy() {
        _instance = null;
    }
}