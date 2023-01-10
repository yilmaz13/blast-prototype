using UnityEngine;

public class SoundFx : MonoBehaviour
{
    #region Variables

    private AudioSource _audioSource;

    #endregion

    #region Unity Method

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    #endregion

    #region Public Method

    public void Play(AudioClip clip, bool loop = false)
    {
        if (clip == null)
        {
            return;
        }

        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
        Invoke("DisableSoundFx", clip.length + 0.1f);
    }

    #endregion

    #region Private Method

    private void DisableSoundFx()
    {
        GetComponent<PoolObject>().GoToPool();
    }

    #endregion
}