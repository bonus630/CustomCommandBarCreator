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
using Vestris.ResourceLib;
using static System.Net.Mime.MediaTypeNames;

namespace CustomCommandBarCreator.Custom
{
    /// <summary>
    /// Interaction logic for ShortcutTextBox.xaml
    /// </summary>
    public partial class ShortcutTextBox : TextBox
    {
        private string prevText = string.Empty;
        private string prevTextFromKey = string.Empty;
        private Key prevKey = Key.None;
        private List<string> textList = new List<string>();
        private bool circle = false;
        public ShortcutTextBox()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            Console.WriteLine("PreviewKeyDown");
            //base.OnPreviewKeyDown(e);

            // Evento disparado quando uma tecla é pressionada antes da ação ser processada pelo sistema

            // Lógica adicional com base na tecla pressionada
            circle = true;
            prevKey = Key.None;
            prevText = string.Empty;
            Key key = e.Key;
            if (key == Key.System)
                key = e.SystemKey;
            if (key != Key.LeftCtrl && key != Key.LeftAlt && key != Key.LeftShift &&
                key != Key.RightCtrl && key != Key.RightAlt && key != Key.RightShift && key != Key.System)
            {
                prevKey = key;
                return;
              //  e.Handled = true;
            }
            e.Handled = true;

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Console.WriteLine("KeyDown");
            // Evento disparado quando uma tecla é pressionada

            // Lógica adicional com base na tecla pressionada

            //Vamos iniciar o ciclo neste evento



        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            Console.WriteLine("PreviewTextInput");
            // Evento disparado quando um caractere de texto é inserido no TextBox

            // Lógica adicional com base no caractere inserido
            prevText = string.Empty;
            if (e.Text.Length > 0)
            {
                prevText = e.Text.ToLower();
                e.Handled = true;
            }

            //Console.WriteLine(string.Format("OnPreviewTextInput:{0}", e.Text));

            //if (e.Text.Length > 0)
            //{
            //    ciclo = true;
            //    // Adiciona o texto minúsculo ou o código da tecla em maiúsculo

            //    string keyText = GetKeyText(e.Text);
            //    //if(CheckShift())
            //    //    e.Text.ToUpper() e.g
            //    //Text += keyText;
            //    AddText(keyText);
            //    Console.WriteLine(string.Format("OnPreviewTextInputFim:{0}", e.Text));
            //    e.Handled = true;
            //}

        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Console.WriteLine("TextChanged");
            // Evento disparado quando o conteúdo do TextBox é alterado

            // Lógica adicional com base na alteração do texto
            this.CaretIndex = this.Text.Length;

        }
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {

           Console.WriteLine("PreviewKeyUp");
            // Evento disparado quando uma tecla é solta antes da ação ser processada pelo sistema

            // Lógica adicional com base na tecla solta
            //Console.WriteLine("PreviewKeyUp:"+e.Key+" "+Keyboard.IsKeyDown(Key.LeftCtrl));

            //string text = Enum.GetName(typeof(Key), e.Key).ToUpper();

            //if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            //{
            //    ciclo = true;
            //}
            //if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            //{
            //    ciclo = true;
            //}

            //if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            //{
            //    // Ignora as teclas Shift pressionadas
            //    ciclo = true;
            //    e.Handled = true;

            //}

            //else if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            //{
            //    // Adiciona o prefixo da tecla modificadora + a tecla pressionada
            //    string prefix = GetModifierPrefix(e.KeyboardDevice.Modifiers);
            //    string keyText = GetKeyText(e.Key);
            //    AddText(prefix + keyText);
            //    e.Handled = true;
            //}
            //else
            Key key = e.Key;
            if (key == Key.System)
                key = e.SystemKey;
            prevTextFromKey = GetKeyString(key);
            if (key == Key.LeftCtrl || key == Key.LeftAlt || key == Key.LeftShift ||
               key == Key.RightCtrl || key == Key.RightAlt || key == Key.RightShift || key == Key.System)
            {
                e.Handled = true;
                return;
            }
            if (key == Key.Back)
            {

                //    // Remove o último caractere quando a tecla Backspace é pressionada

                if (textList.Count > 0)
                {
                    textList.RemoveAt(textList.Count - 1);
                    //        ciclo = true;
                }
                else
                    prevKey = key;

                //    // Text = Text.Substring(0, Text.Length - 1);
                e.Handled = true;

            }
            
            //if (!ciclo)
            //    AddText(text);

            UpdateText();
            base.OnPreviewKeyUp(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Console.WriteLine("KeyUp");

            // Evento disparado quando uma tecla é solta

            // Lógica adicional com base na tecla solta




            //Finalizamos nosso ciclo aqui
            //UpdateText();
        }
        private void UpdateText()
        {
            if (prevKey >= Key.D0 && prevKey <= Key.Z && (CheckCrtl() || CheckAlt()))
            {
                string text = GetModifierPrefix(Keyboard.Modifiers)+prevTextFromKey;
              
                textList.Add(text);
            }
            else if (prevKey >= Key.NumPad0 && prevKey <= Key.Divide)
            {
                string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                textList.Add(text);
            }
            else if(prevText == prevTextFromKey && CheckShift())
            {
                string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                textList.Add(text);
            }
            else if (prevKey != Key.Back && prevKey != Key.None)
            {
                string text = prevText;
                if (string.IsNullOrEmpty(text))
                    text = GetKeyString(prevKey);
                textList.Add(text);
            }
           
            Text = string.Join(",", textList);
            circle = false;
        }


        private string GetModifierPrefix(ModifierKeys modifiers)
        {
            if (modifiers == ModifierKeys.Shift)
                return "shift+";
            else if (modifiers == ModifierKeys.Control)
                return "ctrl+";
            else if (modifiers == ModifierKeys.Alt)
                return "alt+";
            else if (modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                return "ctrl+shift+";
            else if (modifiers == (ModifierKeys.Control | ModifierKeys.Alt))
                return "ctrl+alt+";
            else if (modifiers == (ModifierKeys.Shift | ModifierKeys.Alt))
                return "shift+alt+";
            else if (modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
                return "ctrl+shift+alt+";
            else
                return "";
        }

    
        private bool CheckCrtl()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
        private bool CheckAlt()
        {
            return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.System);
        }
        private bool CheckShift()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private string GetKeyText(string text)
        {
            string keyText = text;

            // Converte o texto para maiúsculas se a tecla modificadora for Caps Lock
            if (Keyboard.IsKeyToggled(Key.CapsLock))
                keyText = keyText.ToUpper();

            return keyText;
        }
 
        private void AddText(string text)
        {
            textList.Add(text);
            circle = true;
        }
        private string[] numOperator = new string[] {
        "MULTIPLY","ADD","SEPARATOR","SUBTRACT","DECIMAL","DIVIDE"  };
        public string GetKeyString(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                // Números de 0 a 9
                return ((int)(key - Key.D0)).ToString();
            }
            //else if (key >= Key.NumPad0 && key <= Key.NumPad9)
            //{
            //    // Números do teclado numérico
            //    return ((int)(key - Key.NumPad0)).ToString();
            //}
            else if (key >= Key.A && key <= Key.Z)
            {
                // Letras maiúsculas
                return key.ToString().ToLower();
            }
            else if (key >= Key.F1 && key <= Key.F24)
            {
                // Teclas de função
                return key.ToString();
            }
            else if(key >= Key.Multiply && key <= Key.Divide)
            {
                return numOperator[key - Key.Multiply];
            }
            else
            {
                //return string.Empty;
                // Outros casos
                return Enum.GetName(typeof(Key), key).ToUpper();
            }
        }

    }
}
