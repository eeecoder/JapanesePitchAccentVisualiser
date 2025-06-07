using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace JapanesePitchAccentVisualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Grid? contourGrid;
        public Slider? wordSlider;
        public Slider? particleSlider;
        public Slider? accentSlider;
        public Label? wordLabel;
        public Label? particleLabel;
        public Label? accentLabel;
        public Label? debugLabel;
        public Label? accentPatternLabel;
        public CheckBox? accentlessCheckbox;
        public AccentVisualisation? accentVisualisation;
        private const int ACCENTLESS_INDEX = 0;

        private static class SliderConfig
        {
            public static int WORD_SLIDER_MIN = 1;
            public static int WORD_SLIDER_MAX = 10;
            public static int ACCENT_SLIDER_MIN = 1;
            public static int ACCENT_SLIDER_MAX = 10;
            public static int PARTICLE_SLIDER_MIN = 1;
            public static int PARTICLE_SLIDER_MAX = 4;
            public static int MORA_WIDTH = 50;
            public static int SLIDER_HEIGHT = 50;
        }

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }


        public void Init()
        {
            FindWidgets();
            RefreshPhonologicalWord();
        }

        private void FindWidgets()
        {
            contourGrid = (Grid)FindName("ContourGrid");
            accentSlider = (Slider)FindName("AccentSlider");
            wordSlider = (Slider)FindName("WordSlider");
            particleSlider = (Slider)FindName("ParticleSlider");
            accentLabel = (Label)FindName("AccentLabel");
            wordLabel = (Label)FindName("WordLabel");
            particleLabel = (Label)FindName("ParticleLabel");
            accentPatternLabel = (Label)FindName("AccentPatternLabel");
            accentlessCheckbox = (CheckBox)FindName("AccentlessCheckbox");
            SetSliderDimensions(accentSlider, SliderConfig.ACCENT_SLIDER_MIN, SliderConfig.ACCENT_SLIDER_MAX);
            SetSliderDimensions(wordSlider, SliderConfig.WORD_SLIDER_MIN, SliderConfig.WORD_SLIDER_MAX);
            SetSliderDimensions(particleSlider, SliderConfig.PARTICLE_SLIDER_MIN, SliderConfig.PARTICLE_SLIDER_MAX);
        }

        private static void SetSliderDimensions(Slider slider, int minValue, int maxValue)
        {
            slider.Minimum = minValue;
            slider.Maximum = maxValue;
            slider.Width = SliderConfig.MORA_WIDTH * (maxValue - minValue);
            slider.Height = SliderConfig.SLIDER_HEIGHT;
        }

        private void WordMoraSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (wordSlider == null || accentSlider == null) return;
            if (wordSlider.Value < accentSlider.Value)
            {
                accentSlider.Value = wordSlider.Value;
            }
            RefreshPhonologicalWord();
        }

        private void ParticleMoraSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshPhonologicalWord();
        }

        private void AccentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {    
            if (wordSlider == null || accentSlider == null) return;
            if (accentSlider.Value > wordSlider.Value)
            {
                accentSlider.Value = wordSlider.Value;
            }
            RefreshPhonologicalWord();
        }

        private int GetAccentIndex()
        {
            if (accentSlider == null || accentlessCheckbox == null || accentlessCheckbox.IsChecked == true) 
                return ACCENTLESS_INDEX;            
            return (int)accentSlider.Value;
        }

        private void AccentlessCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (accentSlider == null) return;
            if (accentLabel == null) return;
            accentSlider.IsEnabled = false;
            accentLabel.Foreground = new SolidColorBrush(Colors.Gray);
            RefreshPhonologicalWord();
        }

        private void AccentlessCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (accentSlider == null) return;
            if (accentLabel == null) return;
            accentSlider.IsEnabled = true;
            accentLabel.Foreground = new SolidColorBrush(Colors.Black);
            RefreshPhonologicalWord();
        }

        private void RefreshPhonologicalWord()
        {
            if (wordSlider == null || particleSlider == null || accentPatternLabel == null || contourGrid == null) return;
            PhonologicalWord phonologicalWord = new(accentIndex: GetAccentIndex(),
                                        wordMoraCount: (int)wordSlider.Value,
                                        particleMoraCount: (int)particleSlider.Value);
            accentVisualisation?.ClearGrid();
            accentVisualisation = new AccentVisualisation(contourGrid, phonologicalWord, 
                accentPatternLabel);
            accentVisualisation.RenderVisualisation();
        }


        private void Button_MouseMove(object sender, MouseEventArgs e)
        {

            Button? target = sender as Button;
           
            if (target != null && e.LeftButton == MouseButtonState.Pressed)
            {
                    MessageBox.Show("Mouse moved in " + target.GetType());
                    DragDrop.DoDragDrop(target,
                           target.Background.ToString(),
                           DragDropEffects.Copy);  
            }   
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle? rectangle = sender as Rectangle;
            if (rectangle != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(rectangle,
                                     rectangle.Fill.ToString(),
                                     DragDropEffects.Copy);
            }
        }

        private Brush _previousFill = null;
        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                // Save the current Fill brush so that you can revert back to this value in DragLeave.
                _previousFill = stackPanel.Background;

                // If the DataObject contains string data, extract it.
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                    // If the string can be converted into a Brush, convert it.
                    BrushConverter converter = new BrushConverter();
                    if (converter.IsValid(dataString))
                    {
                        Brush newFill = (Brush)converter.ConvertFromString(dataString);
                        stackPanel.Background = newFill;
                    }
                }
            }
        }

        private void StackPanel_DragLeave(object sender, DragEventArgs e)
        {
            StackPanel? stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                stackPanel.Background = _previousFill;
            }
        }
    }
}