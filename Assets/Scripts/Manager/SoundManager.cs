using System.Collections.Generic;
using PoolSystem;
using UnityEngine;

namespace Manager
{
    public class SoundManager : MonoBehaviour
    {
        #region Variables

        public List<AudioClip> sounds;
        public static SoundManager instance;
        private readonly Dictionary<string, AudioClip> nameToSound = new Dictionary<string, AudioClip>();

        #endregion

        #region Unity Method

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            foreach (var sound in sounds)
            {
                nameToSound.Add(sound.name, sound);
            }
        }

        #endregion

        #region Public Method

        public void PlaySound(AudioClip clip, bool loop = false)
        {
            if (clip != null)
            {
                var soundFx = ObjectPooler.Instance.Spawn("SoundFX", Vector3.one);
                soundFx.GetComponent<SoundFx>().Play(clip, loop);
            }
        }

        public void PlaySound(string soundName, bool loop = false)
        {
            var clip = nameToSound[soundName];
            if (clip != null)
            {
                PlaySound(clip, loop);
            }
        }

        #endregion
    }
}