using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace speechModality
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SpeechMod _sm;
        public MainWindow()
        {
            InitializeComponent();
            _sm = new SpeechMod();
            _sm.Recognized += _sm_Recognized;
        }

        private void _sm_Recognized(object sender, SpeechEventArg e)
        {
            string numberOne ="", numberTwo= "", operation= "";
            Calculator calc = new Calculator();

            result.Text = e.Text;
            confidence.Text = e.Confidence+"";

            if (e.Final)
            {
                if(e.Semantic["number1"].Value.ToString() != "-1") numberOne = e.Semantic["number1"].Value.ToString()+",";
                if (e.Semantic["operator"].Value.ToString() != "") operation = e.Semantic["operator"].Value.ToString() + ",";
                if (e.Semantic["number2"].Value.ToString() != "-1") numberTwo = e.Semantic["number2"].Value.ToString();
                Console.WriteLine(calc.makeCalculation(numberOne+operation+numberTwo));
                result.FontWeight = FontWeights.Bold;
            }
            else result.FontWeight = FontWeights.Normal;
        }
    }
}
