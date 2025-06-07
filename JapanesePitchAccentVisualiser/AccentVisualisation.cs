using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows;
using System.Linq.Expressions;


namespace JapanesePitchAccentVisualiser
{
    public class AccentVisualisation(Grid contourGrid, PhonologicalWord phonologicalWord,
        Label accentPatternLabel)
    {
        public Grid ContourGrid = contourGrid;
        public Label AccentPatternLabel = accentPatternLabel;

        public static class Dimension
        {
            public const int SIZE = 50;
            public const int LINE_HIGH_HEIGHT = 100;
            public const int LINE_LOW_HEIGHT = LINE_HIGH_HEIGHT + SIZE;
            public const int TEXT_H1 = 0;
            public const int TEXT_H2 = 20;
            public const int TEXT_X_OFFSET = 0;
        }

        public readonly struct ContourColor
        {
            public static readonly SolidColorBrush initialRise = new(Colors.Yellow);
            public static readonly SolidColorBrush toneInterpolation = new(Colors.Purple);
            public static readonly SolidColorBrush accentDrop = new(Colors.Yellow);
        }
;

        public void ClearGrid()
        {
            ContourGrid.Children.Clear();
        }

        public void RenderVisualisation()
        {
            DrawAccentContour();
            UpdateLabels();
        }

        public void DrawAccentContour()
		{
            DrawLowToneInterpolation();
            DrawHighToneInterpolation();
            DrawInitialRise();
            DrawAccentDrop();

        }

        public void UpdateLabels()
        {
            UpdateAccentPatternLabel();
        }


        public void UpdateAccentPatternLabel()
        {
            AccentPatternLabel.Content = phonologicalWord.AccentPattern;
            AccentPatternLabel.Foreground = AccentPatternColors.accentPatternToColors[phonologicalWord.AccentPattern];
        }

        public void DrawVisual(int index, int x1, int x2, int y1, int y2, SolidColorBrush brush, bool isAccent = false)
        {
            DrawLine(x1, x2, y1, y2, brush);
            DrawTextH1(index, x1, isAccent);
            DrawTextH2(index, x1, isAccent);
        }

        private void DrawLine(int x1, int x2, int y1, int y2, SolidColorBrush brush)
        {
            Line line = new()
            {
                Visibility = Visibility.Visible,
                StrokeThickness = 20,
                Stroke = brush,
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Triangle,
                //StrokeDashArray = new DoubleCollection() {1}
            };
            ContourGrid.Children.Add(line);
        }     

        public static class AccentPatternColors
        {
            public static readonly SolidColorBrush noneColor = new(Colors.Black);
            public static readonly SolidColorBrush flatColor = new(Colors.Pink);
            public static readonly SolidColorBrush headHighColor = new(Colors.LightGreen);
            public static readonly SolidColorBrush middleHighColor = new(Colors.LightBlue);
            public static readonly SolidColorBrush tailHighColor = new(Colors.Orange);

            public static readonly Dictionary<AccentPattern, SolidColorBrush> accentPatternToColors = new()
            {
                { AccentPattern.頭高型, headHighColor },
                { AccentPattern.中高型, middleHighColor },
                { AccentPattern.尾高型, tailHighColor },
                { AccentPattern.平板型, flatColor },
                { AccentPattern.なし, noneColor }
            };
        }

        public static class FontConfig
        {
            public const int HEADING_FONT_SIZE = 18;
           


            public static FontWeight GetFontWeight(bool isAccent)
            {
                return isAccent == true ? FontWeights.Bold : FontWeights.Normal;
            }

            public static FontStyle GetFontStyle(PhonologicalWord word, int index)
            {
                return index >= word.WordMoraCount ? FontStyles.Italic : FontStyles.Normal;
            }

            public static SolidColorBrush GetFontColor(PhonologicalWord word, int index)
            {
                if (index == 0)
                {
                    return AccentPatternColors.accentPatternToColors[AccentPattern.頭高型];
                }
                if (index >= word.WordMoraCount)
                {
                    return AccentPatternColors.accentPatternToColors[AccentPattern.なし];
                }
                if (index == word.WordMoraCount - 1)
                {
                    return AccentPatternColors.accentPatternToColors[AccentPattern.尾高型];
                }
                return AccentPatternColors.accentPatternToColors[AccentPattern.中高型];
            }
        }

        public void DrawTextH1(int index, int left, bool isAccent = false)
        {
            string displayIndex = (index + 1).ToString();
            DrawText(
                text: displayIndex,
                left: left + Dimension.TEXT_X_OFFSET,
                top: Dimension.TEXT_H1,
                fontFamily: new FontFamily("Gothic"),
                fontSize: FontConfig.HEADING_FONT_SIZE,
                fontStretch: FontStretches.Condensed,
                fontStyle: FontConfig.GetFontStyle(phonologicalWord, index),
                fontWeight: FontConfig.GetFontWeight(isAccent),
                foreground: FontConfig.GetFontColor(phonologicalWord, index));
        }

        public void DrawTextH2(int index, int left, bool isAccent = false)
        {
            try
            {
                string moraCategory = phonologicalWord.MoraCategories[index];
                DrawText(
                    text: moraCategory.ToString(), 
                    left: left + Dimension.TEXT_X_OFFSET, 
                    top: Dimension.TEXT_H2,
                    fontFamily: new FontFamily("Comic Sans MS"),
                    fontSize: 18,
                    fontStretch: FontStretches.Normal,
                    fontStyle: FontConfig.GetFontStyle(phonologicalWord, index),
                    fontWeight: FontConfig.GetFontWeight(isAccent),
                    foreground: FontConfig.GetFontColor(phonologicalWord, index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void DrawText(string text, int left, int top, 
            FontFamily fontFamily, int fontSize, FontStretch fontStretch, 
            FontStyle fontStyle, FontWeight fontWeight, SolidColorBrush foreground)
        {
            TextBlock myTextBlock = new()
            {
                Text = text,
                Margin = new Thickness(left, top, 0, 0),
                FontFamily = fontFamily,
                FontSize = fontSize,
                FontStretch = fontStretch,
                FontStyle = fontStyle,
                FontWeight = fontWeight,
                Foreground = foreground
            };
            ContourGrid.Children.Add(myTextBlock);
       
        }

        public void DrawHorizontalLine(int index, int y1, int y2, SolidColorBrush color, bool isAccent = false)
        {
            int x1 = Dimension.SIZE * index;
            int x2 = Dimension.SIZE * (index + 1);
            DrawVisual(index, x1, x2, y1, y2, color, isAccent);
        }

        public void DrawVerticalLine(int index, int y1, int y2, SolidColorBrush color)
        {
            int x1 = Dimension.SIZE * index;
            DrawVisual(index, x1, x1, y1, y2, color);
        }


        public void DrawLL(int index, SolidColorBrush color)
        {
            DrawHorizontalLine(index, Dimension.LINE_LOW_HEIGHT, Dimension.LINE_LOW_HEIGHT, color);
        }

        public void DrawHH(int index, SolidColorBrush color, bool isAccent = false)
        {
            DrawHorizontalLine(index, Dimension.LINE_HIGH_HEIGHT, Dimension.LINE_HIGH_HEIGHT, color, isAccent);
        }

        public void DrawLH(int index, SolidColorBrush color)
        {
            DrawVerticalLine(index, Dimension.LINE_LOW_HEIGHT, Dimension.LINE_HIGH_HEIGHT, color);
        }

        public void DrawHL(int index, SolidColorBrush color)
        {
            DrawVerticalLine(index, Dimension.LINE_HIGH_HEIGHT, Dimension.LINE_LOW_HEIGHT, color);
        }

        private void DrawInitialRise()
        {
            if (!phonologicalWord.LHSequence.HasValue()) return;
            DrawLL(phonologicalWord.LHSequence.FirstMora.Index, ContourColor.initialRise);
            DrawLH(phonologicalWord.LHSequence.SecondMora.Index, ContourColor.initialRise);
            DrawHH(phonologicalWord.LHSequence.SecondMora.Index, ContourColor.initialRise);
        }

        private void DrawAccentDrop()
        {
            if (!phonologicalWord.HLSequence.HasValue()) return;
            DrawHH(phonologicalWord.HLSequence.FirstMora.Index, ContourColor.accentDrop, isAccent: true);
            DrawHL(phonologicalWord.HLSequence.SecondMora.Index, ContourColor.accentDrop);
            DrawLL(phonologicalWord.HLSequence.SecondMora.Index, ContourColor.accentDrop);
        }

        private void DrawLowToneInterpolation()
        {
            if (!phonologicalWord.HLSequence.HasValue()) return;
            int start = phonologicalWord.HLSequence.SecondMora.Index + 1;
            int end = phonologicalWord.PhonologicalWordMoraCount;
            for (int i = start; i < end; i++)
            {
                DrawLL(i, ContourColor.toneInterpolation);
            }
        }

        private void DrawHighToneInterpolation()
        {
            if (!phonologicalWord.LHSequence.HasValue()) return;
            int end = phonologicalWord.HLSequence.HasValue() ? 
                phonologicalWord.HLSequence.FirstMora.Index : phonologicalWord.PhonologicalWordMoraCount;
            int start = phonologicalWord.LHSequence.SecondMora.Index + 1;
            for (int i = start; i < end; i++)
            {
                DrawHH(i, ContourColor.toneInterpolation);
            }            
        }

    }
}