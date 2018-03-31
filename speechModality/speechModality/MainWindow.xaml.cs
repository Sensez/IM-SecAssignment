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
        private Tts _t;
        private Calculator _calc;
        private Boolean _beggining;
        private Boolean _confirmation;
        private string _lastNum1, _lastNum2, _lastOp;
        public MainWindow()
        {
            InitializeComponent();
            _sm = new SpeechMod();
            _t = new Tts();
            _calc = new Calculator();
            _sm.Recognized += _sm_Recognized;
            syntesisSpeak("Olá, eu sou a Cheila a Calculadora Falante! Em que lhe posso ser util?");
            _beggining = true;
            _confirmation = false;
            _lastNum1 = "";
            _lastNum2 = "";
            _lastOp = "";
        }

        private void _sm_Recognized(object sender, SpeechEventArg e)
        {
            Console.WriteLine(e.Semantic["ajuda"].Value.ToString() + "--------------->");
            string numberOne ="", numberTwo= "", operation= "";
            result.Text = e.Text;
            confidence.Text = e.Confidence+"";
            _calc.resetValues();
            if (_beggining == true && e.Confidence < 0.40)
            {
                syntesisSpeak("Podes contar comigo para efetuar operações com dois números de até 3 digitos." +
                    " As operações disponivéis são as de soma, multiplicação, divisão, subtração, raiz quadrada, " +
                    "e elevar a um numero. Um exemplo de utilização seria: Cheila, soma 5 e 5");
                _beggining = false;
            }
            else if(_confirmation == true && e.Confidence>= 0.7){
                _confirmation = false;
                if (e.Semantic["confirm"].Value.ToString().Equals("sim"))
                {
                    syntesisSpeak("O resultado da operação é " + _calc.makeCalculation(_lastNum1 + _lastOp + _lastNum2).ToString());
                }
                else
                {
                    syntesisSpeak("Então peço desculpa por ter percebido mal, poderia repetir de novo por favor?");
                }
            }
            else
            {
                if (_beggining == true) _beggining = false;
                if (e.Confidence > 0.8 && e.Semantic["ajuda"].Value.ToString().Equals("ajuda"))
                {
                    syntesisSpeak("Podes contar comigo para efetuar operações com dois números de até 3 digitos." +
                    " As operações disponivéis são as de soma, multiplicação, divisão, subtração, raiz quadrada, " +
                    "e elevar a um numero. Um exemplo de utilização seria: Cheila, soma 5 e 5");
                }
                else if (e.Confidence >= 0.90)
                {
                    if (!e.Semantic["number1"].Value.ToString().Equals("-1")) numberOne = e.Semantic["number1"].Value.ToString() + ",";
                    if (!e.Semantic["operator"].Value.ToString().Equals("")) operation = e.Semantic["operator"].Value.ToString() + ",";
                    if (!e.Semantic["number2"].Value.ToString().Equals("-1")) numberTwo = e.Semantic["number2"].Value.ToString();
                    syntesisSpeak("O resultado da operação é "+ _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                }
                else if (e.Confidence >= 0.80 && e.Confidence < 0.90)
                {
                    if (!e.Semantic["number1"].Value.ToString().Equals("-1")) numberOne = e.Semantic["number1"].Value.ToString() + ",";
                    if (!e.Semantic["operator"].Value.ToString().Equals("")) operation = e.Semantic["operator"].Value.ToString() + ",";
                    if (!e.Semantic["number2"].Value.ToString().Equals("-1")) numberTwo = e.Semantic["number2"].Value.ToString();
                    if(e.Semantic["number2"].Value.ToString().Equals("-1") || e.Semantic["number1"].Value.ToString().Equals("-1"))
                    {
                        if (e.Semantic["operator"].Value.ToString().Equals("raiz"))
                        {
                            syntesisSpeak("O resultado de raiz de " + numberTwo.ToString() + "é de " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                        }
                        else if(e.Semantic["operator"].Value.ToString().Equals("^,2"))
                        {
                            syntesisSpeak("O resultado de " + numberOne.ToString() + "ao quadrado é de: " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                        }
                        else
                        {
                            syntesisSpeak("O resultado de " + numberOne.ToString() + "ao cubo é de: " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                        }
                    }
                    else
                    {
                        syntesisSpeak("O resultado de " + numberOne.ToString() + " " + getOperador(e.Semantic["operator"].Value.ToString()) +" " + numberTwo.ToString() + " é " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                    }
                }
                else if (e.Confidence >= 0.45 && e.Confidence < 0.8)
                {
                    if (!e.Semantic["number1"].Value.ToString().Equals("-1")) numberOne = e.Semantic["number1"].Value.ToString() + ",";
                    if (!e.Semantic["operator"].Value.ToString().Equals("")) operation = e.Semantic["operator"].Value.ToString() + ",";
                    if (!e.Semantic["number2"].Value.ToString().Equals("-1")) numberTwo = e.Semantic["number2"].Value.ToString();
                    if (e.Semantic["number2"].Value.ToString().Equals("-1") || e.Semantic["number1"].Value.ToString().Equals("-1"))
                    {
                        if (e.Semantic["operator"].Value.ToString().Equals("raiz"))
                        {
                            syntesisSpeak("Deseja saber o resultado de raiz de " + numberTwo.ToString() + "certo?");
                        }
                        else if (e.Semantic["operator"].Value.ToString().Equals("^,2"))
                        {
                            syntesisSpeak("Deseja saber o resultado  de " + numberOne.ToString() + "ao quadrado certo?");
                        }
                        else
                        {
                            syntesisSpeak("Deseja saber o resultado de " + numberOne.ToString() + "ao cubo certo?");
                        }
                    }
                    else
                    {
                        syntesisSpeak("Deseja saber o resultado de " + numberOne.ToString() + " " + getOperador(e.Semantic["operator"].Value.ToString()) + " " + numberTwo.ToString() + " certo?");
                    }
                    _lastNum1 = numberOne;
                    _lastNum2 = numberTwo;
                    _lastOp = operation;
                    _confirmation = true;
                }
                else if (e.Confidence < 0.45)
                {
                    syntesisSpeak("Peço imensa desculpa, poderia repetir?");
                }
            }
        }

        public void syntesisSpeak(string message)
        {
            Console.WriteLine(message + "------------------------>");
            _sm.waitForSpeakModule();
            _t.Speak(message);
            _sm.startRecognitionModule();
        }

        public string getOperador(String op)
        {
            string operador = "";
            switch (op)
            {
                case "+": operador = "mais"; break;
                case "-": operador = "menos"; break;
                case "/": operador = "a divir por"; break;
                case "*": operador = "a multiplicar por"; break;
                case "^": operador = "elevado a"; break;
            }
            return operador;
        }
    }
}
