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
        bool shifted = false;
        private readonly Dictionary<Key,string> shiftSpecialKeys = new Dictionary<Key, string>
        {
            { Key.Oem1, ";" },      // ;
            { Key.OemPlus, "=" },   // =
            { Key.OemComma, "," },  // ,
            { Key.OemMinus, "-" },  // -
            { Key.OemPeriod, "." }, // .
            { Key.OemQuestion, "/" }, // /
            { Key.OemOpenBrackets, "[" }, // [
            { Key.OemCloseBrackets, "]" }, // ]
            { Key.OemQuotes, "'" },  // '
            { Key.OemBackslash, "\\" }
            // \
        };

        private readonly Dictionary<Key, string> shiftSpecialKeysShifted = new Dictionary<Key, string>
        {
            { Key.Oem1, ":" },      // :
            { Key.OemPlus, "+" },   // +
            { Key.OemComma, "<" },  // <
            { Key.OemMinus, "_" },  // _
            { Key.OemPeriod, ">" }, // >
            { Key.OemQuestion, "?" }, // ?
            { Key.OemOpenBrackets, "{" }, // {
            { Key.OemCloseBrackets, "}" }, // }
            { Key.OemQuotes, "\"" },  // "
            { Key.OemBackslash, "|" } // |
            // |
        };
        public ShortcutTextBox()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            Console.WriteLine("PreviewKeyDown");

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
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            Console.WriteLine("PreviewTextInput");

            prevText = string.Empty;
            if (e.Text.Length > 0)
            {
                prevText = e.Text.ToLower();
                e.Handled = true;
            }
        }
 

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Console.WriteLine("TextChanged ");
            this.CaretIndex = this.Text.Length;
        }
    
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            Console.WriteLine("PreviewKeyUp");
             shifted = false;

            Key key = e.Key;
            if (key == Key.System)
                key = e.SystemKey;
            prevTextFromKey = GetKeyString(key,out shifted);
            if (key == Key.LeftCtrl || key == Key.LeftAlt || key == Key.LeftShift ||
               key == Key.RightCtrl || key == Key.RightAlt || key == Key.RightShift || key == Key.System)
            {
                e.Handled = true;
                return;
            }
            if (key == Key.Back)
            {

                if (textList.Count > 0)
                {
                    textList.RemoveAt(textList.Count - 1);
                }
                else
                {
                    prevKey = key;
                    prevTextFromKey = "Backspace";
                    string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                    textList.Add(text);
                }

                e.Handled = true;

            }
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                prevKey = key;
                prevTextFromKey = "Enter";
                string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                textList.Add(text);
            }
            UpdateText();
            base.OnPreviewKeyUp(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Console.WriteLine("KeyUp");
        }
        private void UpdateText()
        {
           if (prevKey >= Key.D0 && prevKey <= Key.Z && (CheckCrtl() || CheckAlt()))
            {
                string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                textList.Add(text);
            }
            else if ((prevKey >= Key.NumPad0 && prevKey <= Key.Divide)||(prevKey >= Key.F1 && prevKey <= Key.F24) )
            {
                string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                textList.Add(text);
            }
            else if (prevText == prevTextFromKey && CheckShift())
            {
                string text = GetModifierPrefix(Keyboard.Modifiers) + prevTextFromKey;

                textList.Add(text);
            }
            else if (prevKey != Key.Back && prevKey != Key.None && prevKey != Key.Enter)
            {
                string text = prevText;
                if (string.IsNullOrEmpty(text))
                    text = GetKeyString(prevKey, out shifted);
                textList.Add(text);
            }
            textList.RemoveAll(r => r.Equals("\b"));
            Text = string.Join(",", textList);
            circle = false;
        }

        private string GetModifierPrefix(ModifierKeys modifiers)
        {
            string shift = "shift+";
            if (shifted)
                shift = "";
            if (modifiers == ModifierKeys.Shift)
                return shift;
            else if (modifiers == ModifierKeys.Control)
                return "ctrl+";
            else if (modifiers == ModifierKeys.Alt)
                return "alt+";
            else if (modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                return "ctrl+"+shift;
            else if (modifiers == (ModifierKeys.Control | ModifierKeys.Alt))
                return "ctrl+alt+";
            else if (modifiers == (ModifierKeys.Shift | ModifierKeys.Alt))
                return shift+"alt+";
            else if (modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
                return "ctrl+"+shift+"alt+";
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
        public string GetKeyStringold(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                return ((int)(key - Key.D0)).ToString();
            }
            //else if (key >= Key.NumPad0 && key <= Key.NumPad9)
            //{
            //    // Números do teclado numérico
            //    return ((int)(key - Key.NumPad0)).ToString();
            //}
            else if (key >= Key.A && key <= Key.Z)
            {
                return key.ToString().ToLower();
            }
            else if (key >= Key.F1 && key <= Key.F24)
            {
                return key.ToString();
            }
            else if (key >= Key.Multiply && key <= Key.Divide)
            {
                return numOperator[key - Key.Multiply];
            }
            else
            {
                return Enum.GetName(typeof(Key), key).ToUpper();
            }
        }
        public string GetKeyString(Key key, out bool isShiftModified)
        {
            isShiftModified = false; 
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (key >= Key.D0 && key <= Key.D9)
            {
                int digit = (int)(key - Key.D0);
                if (isShiftPressed)
                {
                    string[] shiftChars = { ")", "!", "@", "#", "$", "%", "^", "&", "*", "(" };
                    isShiftModified = true;
                    return shiftChars[digit];
                }
                return digit.ToString();
            }
            else if (key >= Key.A && key <= Key.Z)
            {
                if (isShiftPressed)
                {
                    return key.ToString(); 
                }
                return key.ToString().ToLower(); 
            }
            else if (key >= Key.F1 && key <= Key.F24)
            {
                return key.ToString();
            }
            else if (key >= Key.Multiply && key <= Key.Divide)
            {
                return numOperator[key - Key.Multiply];
            }
            else
            {
                string specialKey = GetSpecialKeyWithShift(key, isShiftPressed, out isShiftModified);
                return specialKey ?? Enum.GetName(typeof(Key), key)?.ToUpper();
            }
        }
        private string GetSpecialKeyWithShift(Key key, bool isShiftPressed, out bool isShiftModified)
        {
            isShiftModified = false;
            if (isShiftPressed && shiftSpecialKeysShifted.ContainsKey(key))
            {
                isShiftModified = true;
                return shiftSpecialKeysShifted[key];
            }

            if (!isShiftPressed && shiftSpecialKeys.ContainsKey(key))
            {
                return shiftSpecialKeys[key];
            }
            return null;
        }
    }
}
