using System;
using System.IO;
using System.Windows;

namespace GAVIMDataProcessing;

public class SetupData
{
	public int TS_FAST = 250;

	public int TS_SLOW = 10;

	public float Ia { get; private set; }

	public float Ib { get; private set; }

	public float Ic { get; private set; }

	public float Da { get; private set; }

	public float Db { get; private set; }

	public float Dc { get; private set; }

	public float Bconstant { get; private set; }

	public float Rref { get; private set; }

	public float Vref { get; private set; }

	public float Pullup { get; private set; }

	public float DeviceCurrent { get; private set; }

	public float OIAClevel { get; private set; }

	public int TjMax { get; private set; }

	public float VdcMax { get; private set; }

	public int VdcNom { get; private set; }

	public float AswIGBT { get; private set; }

	public float BswIGBT { get; private set; }

	public float CswIGBT { get; private set; }

	public int EratioIGBT { get; private set; }

	public float AcondIGBT { get; private set; }

	public float BcondIGBT { get; private set; }

	public float CcondIGBT { get; private set; }

	public float AswDiode { get; private set; }

	public float BswDiode { get; private set; }

	public float CswDiode { get; private set; }

	public int EratioDiode { get; private set; }

	public float AcondDiode { get; private set; }

	public float BcondDiode { get; private set; }

	public float CcondDiode { get; private set; }

	public float FswMax { get; private set; }

	public int Kc { get; private set; }

	public float Imin { get; private set; }

	public int DELTA_TJ_SWITCH_DOWN { get; private set; }

	public int DELTA_TJ_SWITCH_UP { get; private set; }

	public int VDC_TEST_RECT { get; private set; }

	public float Psw1kHzIGBT { get; set; }

	public float PcondMaxIGBT { get; private set; }

	public float Psw1kHzDiode { get; private set; }

	public float PcondMaxDiode { get; private set; }

	public int A_SW_IGBT { get; private set; }

	public int B_SW_IGBT { get; private set; }

	public int C_SW_IGBT { get; private set; }

	public int A_COND_IGBT { get; private set; }

	public int B_COND_IGBT { get; private set; }

	public int C_COND_IGBT { get; private set; }

	public int A_SW_Diode { get; private set; }

	public int B_SW_Diode { get; private set; }

	public int C_SW_Diode { get; private set; }

	public int A_COND_Diode { get; private set; }

	public int B_COND_Diode { get; private set; }

	public int C_COND_Diode { get; private set; }

	public bool PerformSetup(string PathToData)
	{
		if (!File.Exists(PathToData + "\\SetupFile.csv"))
		{
			MessageBox.Show("Could not find Setup.csv\nCheck the file path");
			return true;
		}
		GetSetupDataFromFile(PathToData);
		CalculatePowerlossCoefficients();
		return false;
	}

	public void GetSetupDataFromFile(string PathToData)
	{
		using StreamReader reader = new StreamReader(PathToData + "\\SetupFile.csv");
		int counter = 0;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			string[] values = line.Split(',');
			switch (counter)
			{
			case 1:
				Ia = float.Parse(values[1]);
				Da = float.Parse(values[2]);
				break;
			case 2:
				Ib = float.Parse(values[1]);
				Db = float.Parse(values[2]);
				break;
			case 3:
				Ic = float.Parse(values[1]);
				Dc = float.Parse(values[2]);
				break;
			case 5:
				Bconstant = float.Parse(values[1]);
				break;
			case 6:
				Rref = float.Parse(values[1]);
				break;
			case 7:
				Vref = float.Parse(values[1]);
				break;
			case 8:
				Pullup = float.Parse(values[1]);
				break;
			case 10:
				DeviceCurrent = float.Parse(values[1]);
				break;
			case 12:
				OIAClevel = float.Parse(values[1]);
				break;
			case 13:
				TjMax = int.Parse(values[1]) * 10;
				break;
			case 14:
				VdcMax = float.Parse(values[1]);
				break;
			case 15:
				VdcNom = Convert.ToInt16(float.Parse(values[1]) / VdcMax * 4095f);
				break;
			case 16:
				AswIGBT = float.Parse(values[1]);
				break;
			case 17:
				BswIGBT = float.Parse(values[1]);
				break;
			case 18:
				CswIGBT = float.Parse(values[1]);
				break;
			case 19:
				EratioIGBT = Convert.ToUInt16(Math.Floor(float.Parse(values[1]) * 65535f));
				break;
			case 20:
				AcondIGBT = float.Parse(values[1]);
				break;
			case 21:
				BcondIGBT = float.Parse(values[1]);
				break;
			case 22:
				CcondIGBT = float.Parse(values[1]);
				break;
			case 23:
				AswDiode = float.Parse(values[1]);
				break;
			case 24:
				BswDiode = float.Parse(values[1]);
				break;
			case 25:
				CswDiode = float.Parse(values[1]);
				break;
			case 26:
				EratioDiode = Convert.ToUInt16(Math.Floor(float.Parse(values[1]) * 65535f));
				break;
			case 27:
				AcondDiode = float.Parse(values[1]);
				break;
			case 28:
				BcondDiode = float.Parse(values[1]);
				break;
			case 29:
				CcondDiode = float.Parse(values[1]);
				break;
			case 30:
				FswMax = float.Parse(values[1]);
				break;
			case 31:
				Kc = Convert.ToInt16(float.Parse(values[1]) * 100f);
				break;
			case 32:
				Imin = float.Parse(values[1]);
				break;
			case 33:
				DELTA_TJ_SWITCH_DOWN = int.Parse(values[1]) * 10;
				break;
			case 34:
				DELTA_TJ_SWITCH_UP = int.Parse(values[1]) * 10;
				break;
			case 35:
				VDC_TEST_RECT = int.Parse(values[1]);
				break;
			}
			counter++;
		}
	}

	public void CalculatePowerlossCoefficients()
	{
		float Root2 = Convert.ToSingle(Math.Sqrt(2.0));
		float kc = Convert.ToSingle(Kc) / 100f;
		float KcSquared = kc * kc;
		Psw1kHzIGBT = (2f * AswIGBT * KcSquared + Root2 * BswIGBT * kc + CswIGBT) * 1000f;
		Psw1kHzDiode = (2f * AswDiode * KcSquared + Root2 * BswDiode * kc + CswDiode) * 1000f;
		PcondMaxIGBT = 2f * AcondIGBT * KcSquared + Root2 * BcondIGBT * kc + CcondIGBT;
		PcondMaxDiode = 2f * AcondDiode * KcSquared + Root2 * BcondDiode * kc + CcondDiode;
		A_SW_IGBT = Convert.ToInt16(Math.Floor(AswIGBT * 1000f * (2f * KcSquared) * (16383f / Psw1kHzIGBT)));
		A_SW_Diode = Convert.ToInt16(Math.Floor(AswDiode * 1000f * (2f * KcSquared) * (16383f / Psw1kHzDiode)));
		B_SW_IGBT = Convert.ToInt16(Math.Floor(BswIGBT * 1000f * (Root2 * kc) * (16383f / Psw1kHzIGBT)));
		B_SW_Diode = Convert.ToInt16(Math.Floor(BswDiode * 1000f * (Root2 * kc) * (16383f / Psw1kHzDiode)));
		C_SW_IGBT = Convert.ToInt16(Math.Floor(CswIGBT * 1000f * (16383f / Psw1kHzIGBT)));
		C_SW_Diode = Convert.ToInt16(Math.Floor(CswDiode * 1000f * (16383f / Psw1kHzDiode)));
		A_COND_IGBT = Convert.ToInt16(Math.Floor(AcondIGBT * (2f * KcSquared) * (16383f / PcondMaxIGBT)));
		A_COND_Diode = Convert.ToInt16(Math.Floor(AcondDiode * (2f * KcSquared) * (16383f / PcondMaxDiode)));
		B_COND_IGBT = Convert.ToInt16(Math.Floor(BcondIGBT * Root2 * kc * (16383f / PcondMaxIGBT)));
		B_COND_Diode = Convert.ToInt16(Math.Floor(BcondDiode * Root2 * kc * (16383f / PcondMaxDiode)));
		C_COND_IGBT = Convert.ToInt16(Math.Floor(CcondIGBT * (16383f / PcondMaxIGBT)));
		C_COND_Diode = Convert.ToInt16(Math.Floor(CcondDiode * (16383f / PcondMaxDiode)));
	}
}
