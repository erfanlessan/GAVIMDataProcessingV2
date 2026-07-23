using System.IO;
using System.Windows;

namespace GAVIMDataProcessing;

internal class OutputData
{
	public bool CreateOutputFileWithHeadings(string pathToData)
	{
		if (File.Exists(pathToData + "\\Output.csv"))
		{
			MessageBox.Show("An output file already exists, Please remove it and start again.");
			return true;
		}
		using (StreamWriter sw = File.CreateText(pathToData + "\\Output.csv"))
		{
			sw.WriteLine("Rth_Cond");
			sw.WriteLine("Rth_SW");
			sw.WriteLine("Tauth1");
			sw.WriteLine("Tauth2");
			sw.WriteLine("Ramb_Cond");
			sw.WriteLine("Ramb_SW");
			sw.WriteLine("Tauamb1");
			sw.WriteLine("Tauamb2");
			sw.WriteLine("RJC_Cond");
			sw.WriteLine("RJC_SW");
			sw.WriteLine("TauJC");
			sw.WriteLine("Kc");
			sw.WriteLine("A_SW_IGBT");
			sw.WriteLine("B_SW_IGBT");
			sw.WriteLine("C_SW_IGBT");
			sw.WriteLine("A_SW_DIODE");
			sw.WriteLine("B_SW_DIODE");
			sw.WriteLine("C_SW_DIODE");
			sw.WriteLine("VDC_MAX");
			sw.WriteLine("VDC_NOM");
			sw.WriteLine("SW_RATIO_IGBT");
			sw.WriteLine("SW_RATIO_DIODE");
			sw.WriteLine("A_COND_IGBT");
			sw.WriteLine("B_COND_IGBT");
			sw.WriteLine("C_COND_IGBT");
			sw.WriteLine("A_COND_DIODE");
			sw.WriteLine("B_COND_DIODE");
			sw.WriteLine("C_COND_DIODE");
			sw.WriteLine("VDC_TEST_RECT");
			sw.WriteLine("TJMAX");
			sw.WriteLine("DELTA_TJ_SWITCH_DOWN");
			sw.WriteLine("DELTA_TJ_SWITCH_UP");
			sw.WriteLine("PSW_MAX_IGBT");
			sw.WriteLine("PCOND_MAX_IGBT");
			sw.WriteLine("PSW_MAX_DIODE");
			sw.WriteLine("PCOND_MAX_DIODE");
			sw.WriteLine("TS_FAST");
			sw.WriteLine("TS_SLOW");
		}
		return false;
	}

	public void WriteJthValues(string pathToData, int linesInOutputFile, DataloggerData _dataloggerData)
	{
		string[] lines = File.ReadAllLines(pathToData + "\\Output.csv");
		using StreamWriter sw = File.CreateText(pathToData + "\\Output.csv");
		for (int currentLine = 0; currentLine <= linesInOutputFile; currentLine++)
		{
			switch (currentLine)
			{
			case 0:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.RUUJthCond + ",");
				sw.Write(_dataloggerData.RULJthCond + ",");
				sw.Write(_dataloggerData.RVUJthCond + ",");
				sw.Write(_dataloggerData.RVLJthCond + ",");
				sw.Write(_dataloggerData.RWUJthCond + ",");
				sw.Write(_dataloggerData.RWLJthCond);
				sw.Write("\r\n");
				break;
			case 1:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.RUUJthSW + ",");
				sw.Write(_dataloggerData.RULJthSW + ",");
				sw.Write(_dataloggerData.RVUJthSW + ",");
				sw.Write(_dataloggerData.RVLJthSW + ",");
				sw.Write(_dataloggerData.RWUJthSW + ",");
				sw.Write(_dataloggerData.RWLJthSW);
				sw.Write("\r\n");
				break;
			case 2:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.tau1minUU + ",");
				sw.Write(_dataloggerData.tau1minUL + ",");
				sw.Write(_dataloggerData.tau1minVU + ",");
				sw.Write(_dataloggerData.tau1minVL + ",");
				sw.Write(_dataloggerData.tau1minWU + ",");
				sw.Write(_dataloggerData.tau1minWL);
				sw.Write("\r\n");
				break;
			case 3:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.tau2minUU + ",");
				sw.Write(_dataloggerData.tau2minUL + ",");
				sw.Write(_dataloggerData.tau2minVU + ",");
				sw.Write(_dataloggerData.tau2minVL + ",");
				sw.Write(_dataloggerData.tau2minWU + ",");
				sw.Write(_dataloggerData.tau2minWL);
				sw.Write("\r\n");
				break;
			default:
				sw.WriteLine(lines[currentLine]);
				break;
			}
		}
	}

	public void WriteJambValues(string pathToData, int linesInOutputFile, DataloggerData _dataloggerData)
	{
		string[] lines = File.ReadAllLines(pathToData + "\\Output.csv");
		using StreamWriter sw = File.CreateText(pathToData + "\\Output.csv");
		for (int currentLine = 0; currentLine <= linesInOutputFile; currentLine++)
		{
			switch (currentLine)
			{
			case 4:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.RUUJambCond + ",");
				sw.Write(_dataloggerData.RULJambCond + ",");
				sw.Write(_dataloggerData.RVUJambCond + ",");
				sw.Write(_dataloggerData.RVLJambCond + ",");
				sw.Write(_dataloggerData.RWUJambCond + ",");
				sw.Write(_dataloggerData.RWLJambCond);
				sw.Write("\r\n");
				break;
			case 5:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.RUUJambSW + ",");
				sw.Write(_dataloggerData.RULJambSW + ",");
				sw.Write(_dataloggerData.RVUJambSW + ",");
				sw.Write(_dataloggerData.RVLJambSW + ",");
				sw.Write(_dataloggerData.RWUJambSW + ",");
				sw.Write(_dataloggerData.RWLJambSW);
				sw.Write("\r\n");
				break;
			case 6:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.tau1minUU + ",");
				sw.Write(_dataloggerData.tau1minUL + ",");
				sw.Write(_dataloggerData.tau1minVU + ",");
				sw.Write(_dataloggerData.tau1minVL + ",");
				sw.Write(_dataloggerData.tau1minWU + ",");
				sw.Write(_dataloggerData.tau1minWL);
				sw.Write("\r\n");
				break;
			case 7:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_dataloggerData.tau2minUU + ",");
				sw.Write(_dataloggerData.tau2minUL + ",");
				sw.Write(_dataloggerData.tau2minVU + ",");
				sw.Write(_dataloggerData.tau2minVL + ",");
				sw.Write(_dataloggerData.tau2minWU + ",");
				sw.Write(_dataloggerData.tau2minWL);
				sw.Write("\r\n");
				break;
			default:
				sw.WriteLine(lines[currentLine]);
				break;
			}
		}
	}

	public void WriteSelfImpeadance(string pathToData, int linesInOutputFile, ScopeData _scopeData)
	{
		string[] lines = File.ReadAllLines(pathToData + "\\Output.csv");
		using StreamWriter sw = File.CreateText(pathToData + "\\Output.csv");
		for (int currentLine = 0; currentLine <= linesInOutputFile; currentLine++)
		{
			switch (currentLine)
			{
			case 8:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_scopeData.RJC_COND);
				sw.Write("\r\n");
				break;
			case 9:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_scopeData.RJC_SW);
				sw.Write("\r\n");
				break;
			case 10:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_scopeData.tauMin);
				sw.Write("\r\n");
				break;
			default:
				sw.WriteLine(lines[currentLine]);
				break;
			}
		}
	}

	public void WriteOtherCodetableData(string pathToData, int linesInOutputFile, SetupData _setupData)
	{
		string[] lines = File.ReadAllLines(pathToData + "\\Output.csv");
		using StreamWriter sw = File.CreateText(pathToData + "\\Output.csv");
		for (int currentLine = 0; currentLine <= linesInOutputFile; currentLine++)
		{
			switch (currentLine)
			{
			case 11:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.Kc);
				sw.Write("\r\n");
				break;
			case 12:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.A_SW_IGBT);
				sw.Write("\r\n");
				break;
			case 13:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.B_SW_IGBT);
				sw.Write("\r\n");
				break;
			case 14:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.C_SW_IGBT);
				sw.Write("\r\n");
				break;
			case 15:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.A_SW_Diode);
				sw.Write("\r\n");
				break;
			case 16:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.B_SW_Diode);
				sw.Write("\r\n");
				break;
			case 17:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.C_SW_Diode);
				sw.Write("\r\n");
				break;
			case 18:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.VdcMax);
				sw.Write("\r\n");
				break;
			case 19:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.VdcNom);
				sw.Write("\r\n");
				break;
			case 20:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.EratioIGBT);
				sw.Write("\r\n");
				break;
			case 21:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.EratioDiode);
				sw.Write("\r\n");
				break;
			case 22:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.A_COND_IGBT);
				sw.Write("\r\n");
				break;
			case 23:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.B_COND_IGBT);
				sw.Write("\r\n");
				break;
			case 24:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.C_COND_IGBT);
				sw.Write("\r\n");
				break;
			case 25:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.A_COND_Diode);
				sw.Write("\r\n");
				break;
			case 26:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.B_COND_Diode);
				sw.Write("\r\n");
				break;
			case 27:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.C_COND_Diode);
				sw.Write("\r\n");
				break;
			case 28:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.VDC_TEST_RECT);
				sw.Write("\r\n");
				break;
			case 29:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.TjMax);
				sw.Write("\r\n");
				break;
			case 30:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.DELTA_TJ_SWITCH_DOWN);
				sw.Write("\r\n");
				break;
			case 31:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.DELTA_TJ_SWITCH_UP);
				sw.Write("\r\n");
				break;
			case 32:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.Psw1kHzIGBT);
				sw.Write("\r\n");
				break;
			case 33:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.PcondMaxIGBT);
				sw.Write("\r\n");
				break;
			case 34:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.Psw1kHzDiode);
				sw.Write("\r\n");
				break;
			case 35:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.PcondMaxDiode);
				sw.Write("\r\n");
				break;
			case 36:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.TS_FAST);
				sw.Write("\r\n");
				break;
			case 37:
				sw.Write(lines[currentLine] + ",");
				sw.Write(_setupData.TS_SLOW);
				sw.Write("\r\n");
				break;
			default:
				sw.WriteLine(lines[currentLine]);
				break;
			}
		}
	}

	public void WriteThermalImpeadanceValuesIPM(string pathToData, int OutputLinesMinusOne, IPMData _IPMData)
	{
		string[] lines = File.ReadAllLines(pathToData + "\\Output.csv");
		using StreamWriter sw = File.CreateText(pathToData + "\\Output.csv");
		for (int currentLine = 0; currentLine <= OutputLinesMinusOne; currentLine++)
		{
			switch (currentLine)
			{
			case 0:
			{
				sw.Write(lines[currentLine]);
				for (int l = 0; l < 12; l++)
				{
					sw.Write("," + _IPMData.RthJthCond[l]);
				}
				sw.Write("\r\n");
				break;
			}
			case 1:
			{
				sw.Write(lines[currentLine]);
				for (int i = 0; i < 12; i++)
				{
					sw.Write("," + _IPMData.RthJthSW[i]);
				}
				sw.Write("\r\n");
				break;
			}
			case 2:
			{
				sw.Write(lines[currentLine]);
				for (int m = 0; m < 12; m++)
				{
					sw.Write("," + _IPMData.Tau1Min[m]);
				}
				sw.Write("\r\n");
				break;
			}
			case 3:
			{
				sw.Write(lines[currentLine]);
				for (int num = 0; num < 12; num++)
				{
					sw.Write("," + _IPMData.Tau2Min[num]);
				}
				sw.Write("\r\n");
				break;
			}
			case 4:
			{
				sw.Write(lines[currentLine]);
				for (int j = 0; j < 12; j++)
				{
					sw.Write("," + _IPMData.RthJambCond[j]);
				}
				sw.Write("\r\n");
				break;
			}
			case 5:
			{
				sw.Write(lines[currentLine]);
				for (int n = 0; n < 12; n++)
				{
					sw.Write("," + _IPMData.RthJambSW[n]);
				}
				sw.Write("\r\n");
				break;
			}
			case 6:
			{
				sw.Write(lines[currentLine]);
				for (int k = 0; k < 12; k++)
				{
					sw.Write("," + _IPMData.Tau1MinAmb[k]);
				}
				sw.Write("\r\n");
				break;
			}
			case 7:
			{
				sw.Write(lines[currentLine]);
				for (int device = 0; device < 12; device++)
				{
					sw.Write("," + _IPMData.Tau2MinAmb[device]);
				}
				sw.Write("\r\n");
				break;
			}
			default:
				sw.WriteLine(lines[currentLine]);
				break;
			}
		}
	}
}
