using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;

namespace speechModality
{
    public class SpeechEventArg
    {
        public string Text { get; set; }
        public double Confidence { get; set; }
        public bool Final { get; set; }
        public SemanticValue Semantic { get; set; }
    }
}
