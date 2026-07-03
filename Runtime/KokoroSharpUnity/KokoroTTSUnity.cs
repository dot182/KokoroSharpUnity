using KokoroSharp;
using KokoroSharp.Core;
using System.IO;
using UnityEngine;

namespace KokoroSharpUnity
{
    public class KokoroTTSUnity : MonoBehaviour
    {
        public string OnnxPath;
        public string VoicePath;
        public bool UsePersistentDataPath;
        public KokoroWaveOutEventUnity KokoroAudioPlayer;

        public KokoroTTS TTS;
        void Start()
        {
            if (UsePersistentDataPath)
            {
                OnnxPath = Path.Combine(Application.persistentDataPath, OnnxPath);
                VoicePath = Path.Combine(Application.persistentDataPath, VoicePath);
            }
            KokoroVoiceManager.LoadVoicesFromPath(VoicePath);
            TTS = new(OnnxPath, KokoroAudioPlayer);
        }
        public SynthesisHandle Speak(string speech, KokoroVoice voice)
            => TTS.Speak(speech, voice);
        public SynthesisHandle SpeakFast(string speech, KokoroVoice voice)
            => TTS.SpeakFast(speech, voice);
    }
}