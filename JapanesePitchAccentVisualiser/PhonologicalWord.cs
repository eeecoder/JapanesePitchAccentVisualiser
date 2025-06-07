using System.Configuration;
using System.Diagnostics;
using System.Numerics;

namespace JapanesePitchAccentVisualiser
{

    public enum AccentPattern
    {
        平板型,
        頭高型,
        中高型,
        尾高型,
        なし
    }

    public enum AccentDisplaySymbol
    {
        Asterix = '*',
        ForwardSlash = '/',
        Caret = '^',
        RaisedDownArrow = 'ꜜ',
        ExtraHighToneBar = '˥'
    }

    public enum PitchLevel
    {
        Low,
        High
    }

    public static class MoraCategory
    {
        public static readonly string[] Particles = ["て", "に", "を", "は"];
        public static readonly string[] Letters = ["ULT", "PEN", "ANT"];
        public const string Default = "";
    }

    public abstract class Mora(PitchLevel pitchLevel)
    {
        public PitchLevel PitchLevel = pitchLevel;
        public int Index = -1;

        public override string ToString()
        {
            return $"Mora: [{PitchLevel}, {Index}]";
        }
    }

    public class HighToneMora : Mora
    {
        public HighToneMora() : base(PitchLevel.High) { }
    }

    public class LowToneMora : Mora
    {
        public LowToneMora() : base(PitchLevel.Low) { }
    }

    public struct LHSequence
    {
        public LowToneMora FirstMora = new();
        public HighToneMora SecondMora = new();

        public LHSequence() { }

        public readonly bool HasValue()
        {
            return FirstMora.Index != -1;
        }

        public override readonly string ToString()
        {
            return $"LHSequence: [{FirstMora}, {SecondMora}]";
        }
    }

    public struct HLSequence
    {
        public HighToneMora FirstMora = new();
        public LowToneMora SecondMora = new();

        public HLSequence() { }

        public readonly bool HasValue()
        {
            return FirstMora.Index != -1;
        }

        public override readonly string ToString()
        {
            return $"HLSequence: {FirstMora} {SecondMora}";
        }
    }


    public class PhonologicalWord
    {
        public AccentDisplaySymbol AccentSymbol = AccentDisplaySymbol.ExtraHighToneBar;
        public int AccentIndex = -1;
        public int WordMoraCount;
        public int ParticleMoraCount;
        public int PhonologicalWordMoraCount;
        public List<string> MoraCategories;
        public AccentPattern AccentPattern;
        public LHSequence LHSequence;
        public HLSequence HLSequence;


        public override string ToString()
        {
            string wordString = "-----------------------------------------------------------------\n";
            wordString += "Phonological Word\n";
            wordString += $"AccentSymbol: {AccentSymbol}\n";
            wordString += $"AccentIndex: {AccentIndex}\n";
            wordString += $"WordMoraCount: {WordMoraCount}\n";
            wordString += $"ParticleMoraCount: {ParticleMoraCount}\n";
            wordString += $"PhonologicalWordMoraCount: {PhonologicalWordMoraCount}\n";
            wordString += $"MoraCategories: {MoraCategories}\n";
            wordString += $"AccentPattern: {AccentPattern}\n";
            wordString += $"LHSequence: {LHSequence}\n";
            wordString += $"HLSequence: {HLSequence}\n";
            wordString += "-----------------------------------------------------------------";
            return wordString;
        }

        public PhonologicalWord(int accentIndex, int wordMoraCount, int particleMoraCount)
        {
            AccentIndex = accentIndex;
            WordMoraCount = wordMoraCount;
            ParticleMoraCount = particleMoraCount;
            PhonologicalWordMoraCount = WordMoraCount + ParticleMoraCount;
            MoraCategories = [];
            for (int i = 0; i < PhonologicalWordMoraCount; i++)
            {
                MoraCategories.Add(MoraCategory.Default);
            }
            LHSequence = new LHSequence();
            HLSequence = new HLSequence();
            SetContour();
        }

        public void SetContour()
        {
            SetAccentPattern();
            SetInitialRise();
            SetAccentDrop();
            SetAccentDrop();
            SetParticleCategories();
            //SetLetterCategories();
        }

        public void SetParticleCategories()
        {
            int j = 0;
            for (int i = WordMoraCount; i < PhonologicalWordMoraCount; i++)
            {
                MoraCategories[i] = MoraCategory.Particles[j];
                j++;
            }
        }

        public void SetLetterCategories()
        {
            int j = 0;
            for (int i = WordMoraCount - 1; i >= 0; i--)
            {
                if (j >= MoraCategory.Letters.Length) break;
                MoraCategories[i] = MoraCategory.Letters[j];
                j++;
            }
        }

        public void SetInitialRise()
        {
            if (AccentIndex == 1) return;
            LHSequence = new LHSequence
            {
                FirstMora = new LowToneMora { Index = 0 },
                SecondMora = new HighToneMora { Index = 1 }
            };
        }

        public void SetAccentDrop()
        {
            if (AccentIndex == 0) return;
            HLSequence = new HLSequence
            {
                FirstMora = new HighToneMora { Index = AccentIndex - 1 },
                SecondMora = new LowToneMora { Index = AccentIndex }
            };
        }

        public void SetAccentPattern()
        {
            if (AccentIndex == 0)
            {
                AccentPattern = AccentPattern.平板型;
            }
            else if (AccentIndex == 1)
            {
                AccentPattern = AccentPattern.頭高型;
            }
            else if (AccentIndex == WordMoraCount)
            {
                AccentPattern = AccentPattern.尾高型;
            }
            else
            {
                AccentPattern = AccentPattern.中高型;
            }
        }


    }
}
