using KokoroSharp.Core;
using NAudio.Wave;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KokoroSharpUnity
{
    [Serializable]
    public class KokoroWaveOutEventUnity : KokoroWaveOutEvent
    {
        public AudioSource AudioSource;
        public MainThreadDispatcher MainThreadDispatcher;

        public override PlaybackState PlaybackState => state;
        private PlaybackState state = PlaybackState.Playing;

        public override void Dispose()
        {
            Stop();
            stream.Dispose();
        }
        public override async void Play()
        {
            state = PlaybackState.Playing;
            MainThreadDispatcher.Enqueue(async () =>
            {
                AudioClip clip = ToAudioClip(stream);
                await PlayAwaitable(clip);
                state = PlaybackState.Stopped;
            });
        }
        private async Task PlayAwaitable(AudioClip clip)
        {
            AudioSource.PlayOneShot(clip);

            float timer = 0;
            while (timer < clip.length)
            {
                timer += Time.deltaTime;
                await Task.Yield();
            }
        }

        public override void SetVolume(float volume)
        {
            throw new NotSupportedException();
        }

        public override void Stop()
        {
            AudioSource.Stop();
            state = PlaybackState.Stopped;
        }

        private AudioClip ToAudioClip(RawSourceWaveStream rawStream)
        {
            int sampleRate = rawStream.WaveFormat.SampleRate;
            int channels = rawStream.WaveFormat.Channels;
            int bitsPerSample = rawStream.WaveFormat.BitsPerSample;
            if (bitsPerSample != 16)
            {
                throw new ArgumentException("Only 16-bit PCM is supported!");
                ///  But it should never be not 16 bit, becuase <see cref="KokoroSharp.KokoroPlayback.waveFormat"/>
            }
            var sampleProvider = rawStream.ToSampleProvider();

            // Read all samples into a float array
            var allSamples = new List<float>();
            float[] buffer = new float[1024];
            int bytesRead;
            while ((bytesRead = sampleProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    allSamples.Add(buffer[i]);
                }
            }
            float[] samplesArray = allSamples.ToArray();

            // 2. Create the AudioClip in Unity
            int totalSamples = samplesArray.Length;

            AudioClip clip = AudioClip.Create("Kokoro Voice Clip", totalSamples, channels, sampleRate, false);
            // 'false' for streaming might need adjustment based on use case

            // 3. Set the data
            clip.SetData(samplesArray, 0);
            return clip;
        }
    }
}