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
using SymbolicAlgebra;


namespace SilverAlgebra
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        SymbolicVariable CurrentExpression;
        private void EvaluateButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentExpression = SymbolicVariable.Parse(ExpressionTextBox.Text);
            ExpressionResult.Text = CurrentExpression.ToString();
            
            D3Chart.Title = CurrentExpression.ToString(); 

            ParametersPanel.Children.Clear();
            // create dynamic sliders for the expression.
            foreach (var p in CurrentExpression.InvolvedSymbols)
            {
                Slider s = new Slider() { Name = p + "Slider", Tag = p };
                s.Minimum = double.Parse(MinimumTextBox.Text);
                s.Maximum = double.Parse(MaximumTextBox.Text);
                s.Value = s.Minimum;

                s.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Sliders_ValueChanged);

                ParametersPanel.Children.Add(new TextBlock { Text = p });
                ParametersPanel.Children.Add(s);
            }

            
            lgraph.Points.Clear();

        }

        void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
            Dictionary<string, double> parameters = new Dictionary<string, double>();

            Slider VariableSlider = null;
            foreach (var sl in ParametersPanel.Children)
            {
                var s = sl as Slider;

                if (s != null)
                {
                    string p = (string)s.Tag;

                    if (p.Equals(VariableTextBox.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        VariableSlider = s;
                    }
                    else
                    {
                        parameters.Add(p, s.Value);
                    }
                }
            }

            // show slider
            if (VariableSlider != null)
            {
                var step = double.Parse(IntervalTextBox.Text);
                string vparameter = (string)VariableSlider.Tag;
                parameters.Add(vparameter, VariableSlider.Minimum);

                List<double> xl=new List<double>();
                List<double> yl=new List<double>();


                // show y=f(x)
                for (double x = VariableSlider.Minimum; x <= VariableSlider.Value; x += step)
                {
                    parameters[vparameter] = x;
                    xl.Add(x);
                    var y = CurrentExpression.Execute(parameters);
                    yl.Add(y);


                }

                lgraph.Plot(xl, yl);

                
                
            }

        }


    }
}
