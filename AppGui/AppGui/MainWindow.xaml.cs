using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
        private Tts _t;
        private Calculator _calc;
        private Boolean _beggining, _confirmation;
        private double confidence;
        private string _lastNum1, _lastNum2, _lastOp, confirmation, goodbye, ajuda,
                        oprt, n1, n2, n3, n4, n5, n6, n7, n8;
        public MainWindow()
        {
            InitializeComponent();
            _t = new Tts();
            _calc = new Calculator();
            _t.Speak(chooseRandomSpeech("greeting"));
            _beggining = true; _confirmation = false;
            _lastNum1 = ""; _lastNum2 = ""; _lastOp = "";
            confidence = 0;
            InitializeComponent();
            mmiC = new MmiCommunication("localhost", 8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();
        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            if (!_t.getSpeaking()) { 
                var doc = XDocument.Parse(e.Message);
                var com = doc.Descendants("command").FirstOrDefault().Value;
                dynamic json = JsonConvert.DeserializeObject(com);
                fillValuesWithMessage(json);
            
                string numberOne = "", numberTwo = "", operation = "";
                _calc.resetValues();

                if (_beggining == true && confidence < 0.40)
                {
                    _t.Speak(chooseRandomSpeech("help"));
                    _beggining = false;
                }
                else if (_confirmation == true)
                {
                    if (confirmation.Equals("sim"))
                    {
                        Console.WriteLine("----" + _lastNum1 + " " + _lastOp + " " + _lastNum2 + "---");
                        _t.Speak("O resultado da operação é " + _calc.makeCalculation(_lastNum1 + _lastOp + _lastNum2).ToString());
                        _confirmation = false;
                    }
                    else if (confirmation.Equals("nao"))
                    {
                        _t.Speak(chooseRandomSpeech("repetition"));
                        _confirmation = false;
                    }
                }
                else
                {
                    if (_beggining == true) _beggining = false;
                    if (confidence > 0.7 && goodbye.Equals("goodbye"))
                    {
                        _t.Speak(chooseRandomSpeech("goodbye"));
                    }
                    if (confidence > 0.7 && ajuda.Equals("ajuda"))
                    {
                        _t.Speak(chooseRandomSpeech("help"));
                    }
                    else if (confidence >= 0.90)
                    {
                        numberOne = getNumberTranslated(1);
                        numberTwo = getNumberTranslated(2);
                        if (!oprt.Equals("")) operation = oprt + ",";
                        Console.WriteLine("----" + numberOne.ToString() + " " + getOperador(oprt) + " " + numberTwo.ToString() + "---");
                        _t.Speak("O resultado da operação é " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                    }
                    else if (confidence >= 0.80 && confidence < 0.90)
                    {
                        numberOne = getNumberTranslated(1);
                        numberTwo = getNumberTranslated(2);
                        if (!oprt.Equals("")) operation = oprt + ",";
                        Console.WriteLine("----" + numberOne.ToString() + " " + getOperador(oprt) + " " + numberTwo.ToString() + "---");
                        if (numberTwo.Equals("") || numberOne.Equals(""))
                        {
                            if (oprt.Equals("raiz"))
                            {
                                _t.Speak("O resultado de raiz de " + numberTwo.ToString() + "é de " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                            }
                            else if (oprt.Equals("^,2"))
                            {
                                _t.Speak("O resultado de " + numberOne.ToString() + "ao quadrado é de: " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                            }
                            else
                            {
                                _t.Speak("O resultado de " + numberOne.ToString() + "ao cubo é de: " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                            }
                        }
                        else
                        {
                            _t.Speak("O resultado de " + numberOne.ToString() + " " + getOperador(oprt) + " " + numberTwo.ToString() + " é " + _calc.makeCalculation(numberOne + operation + numberTwo).ToString());
                        }
                    }
                    else if (confidence >= 0.45 && confidence < 0.8)
                    {
                        numberOne = getNumberTranslated(1);
                        numberTwo = getNumberTranslated(2);
                        if (!oprt.Equals("")) operation = oprt + ",";
                        Console.WriteLine("----" + numberOne.ToString() + " " + getOperador(oprt) + " " + numberTwo.ToString() + "---");
                        if (numberTwo.Equals("") || numberOne.Equals(""))
                        {
                            if (oprt.Equals("raiz"))
                            {
                                _t.Speak("Deseja saber o resultado de raiz de " + numberTwo.ToString() + "certo?");
                            }
                            else if (oprt.Equals("^,2"))
                            {
                                _t.Speak("Deseja saber o resultado  de " + numberOne.ToString() + "ao quadrado certo?");
                            }
                            else
                            {
                                _t.Speak("Deseja saber o resultado de " + numberOne.ToString() + "ao cubo certo?");
                            }
                        }
                        else
                        {
                            _t.Speak("Deseja saber o resultado de " + numberOne.ToString() + " " + getOperador(oprt) + " " + numberTwo.ToString() + " certo?");
                        }
                        _lastNum1 = numberOne;
                        _lastNum2 = numberTwo;
                        _lastOp = operation;
                        _confirmation = true;
                    }
                    else if (confidence < 0.45)
                    {
                        _t.Speak(chooseRandomSpeech("repetition"));
                    }
                }
            }
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

        public string getNumberTranslated(int number)
        {
            string result = "";
            if (number == 1)
            {
                if (!n7.Equals("-1"))
                    result += n7;

                if (!result.Equals("") && n5.Equals("-1"))
                    result += "0";

                if (!n5.Equals("-1"))
                    result += n5;

                if (!result.Equals("") && n3.Equals("-1") && (n1.Length == 1 || n1.Equals("-1")))
                    result += "0";

                if (!n3.Equals("-1"))
                    result += n3;

                if (!result.Equals("") && n1.Equals("-1"))
                    result += "0";

                if (!n1.Equals("-1"))
                    result += n1;

                if (!result.Equals(""))
                    result += ",";

                return result;
            }
            else
            {
                if (!n8.Equals("-1"))
                    result += n8;

                if (!result.Equals("") && n6.Equals("-1"))
                    result += "0";

                if (!n6.Equals("-1"))
                    result += n6;

                if (!result.Equals("") && n4.Equals("-1") && (n2.Length == 1 || n2.Equals("-1")))
                    result += "0";

                if (!n4.Equals("-1"))
                    result += n4;

                if (!result.Equals("") && n2.Equals("-1"))
                    result += "0";

                if (!n2.Equals("-1"))
                    result += n2;

                return result;
            }
        }

        public string chooseRandomSpeech(string type)
        {
            Random rnd = new Random();
            int random = rnd.Next(0, 3);

            String[] greeting = { "Olá, eu sou a Cheila a calculadora falante, em que lhe posso ser util ?",
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

            String[] goodbye = {
                 "Ate amanhã, nem que seja para me dizer olá, porque eu merêço! Tenha um resto de um bom dia",
                "Sempre às ordens ! Tenha um resto de um bom dia.",
                "Espero ter ajudado, que tenha o resto de um bom dia." };

            switch (type)
            {
                case "greeting": return greeting[random];
                case "repetition": return repetition[random];
                case "help": return help[random];
                case "goodbye": return goodbye[random];
                default: return "";
            }
        }
        public void fillValuesWithMessage(dynamic json)
        {
            confidence = Convert.ToDouble((string)json.recognized[0].ToString());
            confirmation = (string)json.recognized[1].ToString();
            goodbye = (string)json.recognized[2].ToString();
            ajuda = (string)json.recognized[3].ToString();
            oprt = (string)json.recognized[4].ToString();
            n1 = (string)json.recognized[5].ToString();
            n2 = (string)json.recognized[6].ToString();
            n3 = (string)json.recognized[7].ToString();
            n4 = (string)json.recognized[8].ToString();
            n5 = (string)json.recognized[9].ToString();
            n6 = (string)json.recognized[10].ToString();
            n7 = (string)json.recognized[11].ToString();
            n8 = (string)json.recognized[12].ToString();
        }
    }
}
