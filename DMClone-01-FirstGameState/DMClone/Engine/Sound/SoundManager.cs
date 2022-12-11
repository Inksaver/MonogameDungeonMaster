using DMClone.Engine.States;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace DMClone.Engine.Sound
{
    internal class SoundManager
    {
        private int soundtrackIndex = -1;
        private List<SoundEffectInstance> soundtracks = new List<SoundEffectInstance>();
        private Dictionary<string, SoundEffectInstance> soundtracksDict = new Dictionary<string, SoundEffectInstance>();
        private Dictionary<Type, SoundBankItem> soundBank = new Dictionary<Type, SoundBankItem>();

        public void AddSoundtrackToDictionary(string key, SoundEffectInstance track)
        {
            if (!soundtracksDict.ContainsKey(key))
                soundtracksDict.Add(key, track);
        }
        public void SetSoundtrack(List<SoundEffectInstance> tracks)
        {
            soundtracks = tracks;
            soundtrackIndex = soundtracks.Count - 1;
        }
        public void SetSoundtrack(string[] tracks)
        {
            // setup a list of separate sounds from the dictionary
            soundtracks.Clear();
            foreach (string track in tracks)
            {
                if(soundtracksDict.ContainsKey(track))
                    soundtracks.Add(soundtracksDict[track]);
            }
            soundtrackIndex = soundtracks.Count - 1;
        }

        public void OnNotify(BaseGameStateEvent gameEvent)
        {
            if (soundBank.ContainsKey(gameEvent.GetType()))
            {
                var sound = soundBank[gameEvent.GetType()];
                // if sound not playing
                sound.Sound.Play(sound.Attributes.Volume, sound.Attributes.Pitch, sound.Attributes.Pan);
            }
        }
        public void OnNotify(BaseGameStateEvent gameEvent, int index, bool looping)
        {
            if (soundBank.ContainsKey(gameEvent.GetType()))
            {
                var sound = soundBank[gameEvent.GetType()];
                // if sound not playing
                sound.Sound.Play(sound.Attributes.Volume, sound.Attributes.Pitch, sound.Attributes.Pan);
            }
        }

        public void PlaySoundtrack()
        {
            /// Play all soundtracks in the list
            var numTracks = soundtracks.Count;

            if (numTracks <= 0)
            {
                return;
            }

            var currentTrack = soundtracks[soundtrackIndex];
            var nextTrack = soundtracks[(soundtrackIndex + 1) % numTracks];

            if (currentTrack.State == SoundState.Stopped)
            {
                nextTrack.Play();
                soundtrackIndex++;

                if (soundtrackIndex >= soundtracks.Count)
                {
                    soundtrackIndex = 0;
                }
            }
        }
        public void PlaySoundtrack(int soundtrackIndex, bool looping = false)
        {
            /// Play a specific soundtrack from the list, optional looping
            var currentTrack = soundtracks[soundtrackIndex];
            currentTrack.IsLooped = looping;
            if (currentTrack.State == SoundState.Stopped)
            {
                currentTrack.Play();
            }
        }
        public void PlaySoundtrack(string soundtrackKey, bool looping = false)
        {
            /// Play a specific soundtrack from the list, optional looping
            var currentTrack = soundtracksDict[soundtrackKey];
            currentTrack.IsLooped = looping;
            if (currentTrack.State == SoundState.Stopped)
            {
                currentTrack.Play();
            }
        }
        public void StopSoundTrack(int soundtrackIndex)
        {
            /// stop a specific sound track
            soundtracks[soundtrackIndex].Stop();
        }
        public void StopSoundTrack(string soundtrackKey)
        {
            /// stop a specific sound track
            soundtracksDict[soundtrackKey].Stop();
        }
        public void RegisterSound(BaseGameStateEvent gameEvent, SoundEffect sound)
        {
            /// Add a sound to the soundBank Dictionary using it's Type as the Dictionary key. No options
            RegisterSound(gameEvent, sound, 1.0f, 0.0f, 0.0f);
        }
        public void RegisterSound(BaseGameStateEvent gameEvent, SoundEffect sound, float volume, float pitch, float pan)
        {
            /// Add a sound to the soundBank Dictionary using it's Type as the Dictionary key. Options for volume, pitch and pan
            soundBank.Add(gameEvent.GetType(), new SoundBankItem(sound, new SoundAttributes(volume, pitch, pan)));
        }
    }
}
