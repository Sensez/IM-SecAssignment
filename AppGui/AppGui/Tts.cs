using Microsoft.Speech.Synthesis;
using System;

namespace AppGui
{
    class Tts
    {
        private Boolean speaking, goodbye;
        SpeechSynthesizer tts = null;
        public Tts()
        {
            speaking = false;
            goodbye = false;
            tts = new SpeechSynthesizer();
            tts.SelectVoice("Microsoft Server Speech Text to Speech Voice (pt-PT, Helia)");
            tts.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(tts_SpeakCompleted);
            tts.SetOutputToDefaultAudioDevice();
        }
        public void Speak(string text)
        {
            speaking = true;
            if (text.Contains("resto de um bom dia")) goodbye = true;
            tts.SpeakAsync(text);
        }

        void tts_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            speaking = false;
            if (goodbye == true) Environment.Exit(-1);
        }

        public Boolean getSpeaking()
        {
            return speaking;
        }
    }
}

