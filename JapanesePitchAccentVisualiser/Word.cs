using System;

public class Word
{
	public enum MoraCategory
	{
		Regular,
		Defective,
		Empty // index 0
	}

	public enum PitchAccent
	{
		Flat,
		HeadHigh,
		MiddleHigh,
		TailHigh,
		None // particles
	}

	public MoraCategory MyMoraCategory;
	public PitchAccent MyPitchAccent;
	public int MoraCount;

	public Word()
	{
		this.MyMoraCategory = MoraCategory.Regular;
		this.MyPitchAccent = PitchAccent.Flat;
		this.MoraCount = 0;
	}


}
