using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SilverAlgebra
{
    public partial class AlgebraTextBox : UserControl
    {
        private string _Prompt = "SA> ";

        public AlgebraTextBox()
        {
            InitializeComponent();
            string copyright =
@"Silver Algebra 0.84  Alpha Edition

Copyright (c) 2010-2012 at Lost Particles Network [LPN]
All rights reserved for Ahmed Sadek 

Ahmed.Sadek@Lostparticles.net
Ahmed.Amara@gmail.com

Depends on SymbolicAlgebra Open Source Library 
http://SymbolicAlgebra.CodePlex.com
--------------------------------------------------------

SA> x+x   will produce 2*x

To Differentiate  use '|' operator
SA> (sin(x)*cos(x))|x   will produce  cos(x)^2-sin(x)^2

Enjoy
";
            ConsoleTextBox.Text = copyright;
            ConsoleTextBox.Text += (Prompt);
            ConsoleTextBox.Select(ConsoleTextBox.Text.Length, 0);
            ConsoleTextBox.Focus();
        }


        private string Prompt
        {
            get
            {
                return _Prompt;
            }
            set
            {
                _Prompt = value;
            }
        }

        List<string> ConsoleLines = new List<string>();

        int ConsoleLinesIndex = 0;
        private void ConsoleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                int lp = ConsoleTextBox.Text.LastIndexOf(Prompt);  // specify latest prompt
                //delete after that from text
                ConsoleTextBox.Text = ConsoleTextBox.Text.Substring(0, lp + Prompt.Length);

                ConsoleLinesIndex--;
                if (ConsoleLinesIndex > 0)
                {
                    var r = (ConsoleLines[ConsoleLinesIndex - 1]);
                    ConsoleTextBox.Text += r;
                }
                else
                {
                    ConsoleLinesIndex = 0;
                }

                e.Handled = true;
            }

            if (e.Key == Key.Down)
            {
                int lp = ConsoleTextBox.Text.LastIndexOf(Prompt);  // specify latest prompt
                //delete after that from text
                ConsoleTextBox.Text = ConsoleTextBox.Text.Substring(0, lp + Prompt.Length);

                ConsoleLinesIndex++;
                if (ConsoleLinesIndex <= ConsoleLines.Count)
                {
                    var r = (ConsoleLines[ConsoleLinesIndex - 1]);
                    ConsoleTextBox.Text += r;

                }
                else
                {
                    ConsoleLinesIndex = ConsoleLines.Count + 1;
                }

                //ConsoleTextBox.CaretIndex = ConsoleTextBox.Text.Length;
                ConsoleTextBox.Select(ConsoleTextBox.Text.Length, 0);
                e.Handled = true;
            }

        }


        private void ConsoleTextBox_KeyUp(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Enter)
            {
                //evaluate the expression 
                // and go to another line
                int lp = ConsoleTextBox.Text.LastIndexOf(Prompt);

                string LastLine = Regex.Split(ConsoleTextBox.Text, Environment.NewLine).Last();


                try
                {
                    if (LastLine.StartsWith(Prompt))
                    {
                        var s = ConsoleTextBox.Text.Substring(lp).Substring(Prompt.Length);

                        ConsoleTextBox.Text += (Environment.NewLine);

                        if (!string.IsNullOrEmpty(s))
                        {
                            if (ConsoleLines.Count == 0) ConsoleLines.Add(s);
                            else
                            {
                                if (ConsoleLines.Last() != s)
                                {
                                    //store in history only if the input string is not the same as latest string
                                    ConsoleLines.Add(s);
                                }
                            }
                        }

                        ConsoleLinesIndex = ConsoleLines.Count;

                        if (!string.IsNullOrEmpty(s))
                        {
                            var vv = SymbolicAlgebra.SymbolicVariable.Parse(s);

                            ConsoleTextBox.Text += "    " + vv.ToString();
                            ConsoleTextBox.Text += (Environment.NewLine);
                        }
                    }
                    else
                    {
                        ConsoleTextBox.Text += Environment.NewLine;
                    }
                }
                catch (Exception qse)
                {
                    //PrintError(qse);
                }
                finally
                {

                    ConsoleTextBox.Text += (Prompt);


                    //ConsoleTextBox.CaretIndex = ConsoleTextBox.Text.Length;
                    ConsoleTextBox.Select(ConsoleTextBox.Text.Length, 0);
                }
            }
        }

    }
}
