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
            syntesisSpeak(chooseRandomSpeech("greeting"));
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
                syntesisSpeak(chooseRandomSpeech("help"));
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
                    syntesisSpeak(chooseRandomSpeech("repeat"));
                }
            }
            else
            {
                if (_beggining == true) _beggining = false;
                if (e.Confidence > 0.8 && e.Semantic["ajuda"].Value.ToString().Equals("ajuda"))
                {
                    syntesisSpeak(chooseRandomSpeech("help"));
                }
                else if (e.Confidence >= 0.90)
                {
                    numberOne = getNumberTranslated(1, e.Semantic);
                    numberTwo = getNumberTranslated(2, e.Semantic);
                    if (!e.Semantic["operator"].Value.ToString().Equals("")) operation = e.Semantic["operator"].Value.ToString() + ",";
                    syntesisSpeak("O resultado da operação é "+ _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                }
                else if (e.Confidence >= 0.80 && e.Confidence < 0.90)
                {
                    numberOne = getNumberTranslated(1, e.Semantic);
                    numberTwo = getNumberTranslated(2, e.Semantic);
                    if (!e.Semantic["operator"].Value.ToString().Equals("")) operation = e.Semantic["operator"].Value.ToString() + ",";
                    if(numberTwo.Equals("") || numberOne.Equals(""))
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
                    numberOne = getNumberTranslated(1, e.Semantic);
                    numberTwo = getNumberTranslated(2, e.Semantic);
                    if (!e.Semantic["operator"].Value.ToString().Equals("")) operation = e.Semantic["operator"].Value.ToString() + ",";
                    if (numberTwo.Equals("") || numberOne.Equals(""))
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
                    syntesisSpeak(chooseRandomSpeech("repetition"));
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

        public string getNumberTranslated(int number, SemanticValue semantic)
        {
            Console.WriteLine("--->"+ semantic["number7"].Value.ToString() + semantic["number5"].Value.ToString() + semantic["number3"].Value.ToString() + semantic["number1"].Value.ToString());
            string result = "";
            if(number == 1)
            {
                if (!semantic["number7"].Value.ToString().Equals("-1"))
                    result += semantic["number7"].Value.ToString();

                if (!result.Equals("") && semantic["number5"].Value.ToString().Equals("-1"))
                    result += "0";

                if (!semantic["number5"].Value.ToString().Equals("-1"))
                    result += semantic["number5"].Value.ToString();

                if (!result.Equals("") && semantic["number3"].Value.ToString().Equals("-1") && (semantic["number1"].Value.ToString().Length == 1 || semantic["number1"].Value.ToString().Equals("-1")))
                    result += "0";

                if (!semantic["number3"].Value.ToString().Equals("-1"))
                    result += semantic["number3"].Value.ToString();

                if (!result.Equals("") && semantic["number1"].Value.ToString().Equals("-1"))
                    result += "0";

                if (!semantic["number1"].Value.ToString().Equals("-1"))
                    result += semantic["number1"].Value.ToString();

                if (!result.Equals(""))
                    result += ",";

                return result;
            }
            else
            {
                if (!semantic["number8"].Value.ToString().Equals("-1"))
                    result += semantic["number8"].Value.ToString();

                if (!result.Equals("") && semantic["number6"].Value.ToString().Equals("-1"))
                    result += "0";

                if (!semantic["number6"].Value.ToString().Equals("-1"))
                    result += semantic["number6"].Value.ToString();

                if (!result.Equals("") && semantic["number4"].Value.ToString().Equals("-1") && (semantic["number2"].Value.ToString().Length == 1 || semantic["number2"].Value.ToString().Equals("-1")))
                    result += "0";

                if (!semantic["number4"].Value.ToString().Equals("-1"))
                    result += semantic["number4"].Value.ToString();

                if (!result.Equals("") && semantic["number2"].Value.ToString().Equals("-1"))
                    result += "0";

                if (!semantic["number2"].Value.ToString().Equals("-1"))
                    result += semantic["number2"].Value.ToString();

                return result;
            }
        }

        public string chooseRandomSpeech(string type)
        {
            Random rnd = new Random();
            int random = rnd.Next(0, 3);

            String[] greeting = { "Ola, eu sou a Cheila a calculadora falante, em que lhe posso ser util ?",
                "Bem vindo ! Em que lhe posso ser util hoje ?",
                "Ora aqui estou eu, Cheila, a magnifica calculadora ! Que contas vamos fazer hoje ?" };

            String[] repetition = { "Peço imensa desculpa, poderia repetir?",
                "Bolas, essa escapou-me, poderia dizer novamente por favor?",
                "Hoje estou lenta, dormi pouco, peço imensa desculpa ! Poderia repetir de novo por favor?" };

            String[] help = { "Pode contar comigo para efetuar operações com dois números de até 4 digitos." +
                    " As operações disponíveis são as de soma, multiplicação, divisão, subtração, raiz quadrada, " +
                    "e elevar a um numero. Um exemplo de utilização seria: Cheila, soma 5 e 5",
                "Tenho todo o prazer em o ajudar, tente por exemplo algo como, quanto é um numero mais outro, ou melhor ainda, " +
                "quanto é a soma de um numero com outro. Pode dividir, somar, subtrair, multiplicar e ainda aplicar raizes e elevar numeros. Todas as operações são válidas" +
                "para numeros com até 4 digitos.",
                "Pode contar comigo para o auxiliar a efetuar operações tais como as de soma, multiplicação, divisão e subtração a numeros com até 4 digitos." +
                "Porque não tenta efetuar desde já uns calculos teste, como por exemplo, somar um numero com outro" };

            switch (type)
            {
                case "greeting": return greeting[random];
                case "repetition": return repetition[random];
                case "help": return help[random];
                default: return "";
            }
        }
    }
}
