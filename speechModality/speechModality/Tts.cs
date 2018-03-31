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
        static SoundPlayer player = new SoundPlayer();

        public Tts()
        {
            tts = new SpeechSynthesizer();
            tts.SelectVoice("Microsoft Server Speech Text to Speech Voice (pt-PT, Helia)");
            //set function to play audio after synthesis is complete
            tts.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(tts_SpeakCompleted);
        }
        public void Speak(string text)
        {
            while (player.Stream != null)
            {
                //Console.WriteLine("Waiting...");
            }

            //create audio stream with speech
            player.Stream = new System.IO.MemoryStream();
            tts.SetOutputToWaveStream(player.Stream);
            tts.SpeakAsync(text);
        }

        void tts_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            if (player.Stream != null)
            {
                player.Stream.Position = 0;
                player.Play();
                player.Stream = null;
            }
        }
    }
}

