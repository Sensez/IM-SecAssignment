using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace speechModality
{
    class Tts
    {
        SpeechSynthesizer tts = null;
        SpeechMod _sm;
        public Tts(SpeechMod _sm)
        {
            this._sm = _sm;
            tts = new SpeechSynthesizer();
            tts.SelectVoice("Microsoft Server Speech Text to Speech Voice (pt-PT, Helia)");
            tts.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(tts_SpeakCompleted);
            tts.SetOutputToDefaultAudioDevice();
        }
        public void Speak(string text)
        {
            _sm.waitForSpeakModule();
            tts.SpeakAsync(text);
        }

        void tts_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            if (_sm.recognizedGoodbye()) Environment.Exit(-1);
            _sm.startRecognitionModule();
        }
    }
}

