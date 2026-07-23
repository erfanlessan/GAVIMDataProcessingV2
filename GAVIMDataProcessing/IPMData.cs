using System;
using System.IO;
using System.Linq;

namespace GAVIMDataProcessing;

internal class IPMData
{
	private enum DeviceNames
	{
		Iuu,
		Iul,
		Ivu,
		Ivl,
		Iwu,
		Iwl,
		Duu,
		Dul,
		Dvu,
		Dvl,
		Dwu,
		Dwl
	}

	private const int ArrayLength = 20000;

	private int FileLength;

	public static int NoOfDevices = 12;

	private float[,] MultiCurve1 = new float[100, 20000];

	private float[,] MultiCurve2 = new float[100, 20000];

	public float[] FitCurve = new float[20000];

	public float[] FitCurveAmb = new float[20000];

	public float[,] CaseTemperatures = new float[NoOfDevices, 20000];

	public float[] Time = new float[20000];

	public float[] Therm = new float[20000];

	public float[] Amb = new float[20000];

	public float[,] CaseToThermistorNormalised = new float[NoOfDevices, 20000];

	public float[,] CaseToAmbientNormalised = new float[NoOfDevices, 20000];

	public float[] RthJth = new float[NoOfDevices];

	public float[] RthJamb = new float[NoOfDevices];

	public int[] RthJthSW = new int[NoOfDevices];

	public int[] RthJthCond = new int[NoOfDevices];

	public int[] RthJambSW = new int[NoOfDevices];

	public int[] RthJambCond = new int[NoOfDevices];

	public int[] Tau1Min = new int[NoOfDevices];

	public int[] Tau2Min = new int[NoOfDevices];

	public int[] TauRatioMin = new int[NoOfDevices];

	public int[] Tau1MinAmb = new int[NoOfDevices];

	public int[] Tau2MinAmb = new int[NoOfDevices];

	public int[] TauRatioMinAmb = new int[NoOfDevices];

	public float MaxVce;

	public void GetAndConvertData(int currentFile, SetupData _setupData, string PathToData)
	{
		GetDataFromFile(currentFile, _setupData, PathToData);
		CreateJunctionToThermistorCurves(_setupData, currentFile);
		CreateJunctionToAmbientCurves(_setupData, currentFile);
	}

	public void PerformAllCurveFits(int numberOfTests, int numberOfFiles)
	{
		PerformCurveFit(numberOfTests, numberOfFiles);
		PerformCurveFitAmbient(numberOfTests, numberOfFiles);
	}

	public void GetDataFromFile(int currentFile, SetupData _setupData, string pathToData)
	{
		FileLength = 0;
		string currentFileString = Convert.ToString(currentFile);
		string currentFileFixedLength = currentFileString.PadLeft(2, '0');
		float[] DeviceVce = new float[100];
		using (StreamReader reader = new StreamReader(pathToData + "\\Datalogger\\GAVIML" + currentFileFixedLength + ".csv"))
		{
			int counter = 0;
			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				string[] values = line.Split(',');
				counter++;
				int StartLine = 6111;
				int sampleDelay = 100;
				int dataLine = counter - StartLine + sampleDelay - 1;
				if (counter <= StartLine - sampleDelay)
				{
					continue;
				}
				if (counter < StartLine)
				{
					if (currentFile < 6)
					{
						if (counter < StartLine)
						{
							DeviceVce[dataLine] = float.Parse(values[currentFile + 1]);
						}
					}
					else
					{
						DeviceVce[dataLine] = float.Parse(values[currentFile - 6 + 1]) * -1f;
					}
					continue;
				}
				for (int i = 0; i < 12; i++)
				{
					CaseTemperatures[i, dataLine - sampleDelay + 1] = float.Parse(values[i + 9]);
				}
				Time[dataLine - sampleDelay + 1] = float.Parse(values[0]);
				float thermistorVoltage = float.Parse(values[7]);
				float thermistorResistance = thermistorVoltage / ((_setupData.Vref - thermistorVoltage) / _setupData.Pullup);
				float BconstBit = Convert.ToSingle(Math.Log(thermistorResistance / _setupData.Rref)) / _setupData.Bconstant;
				Therm[dataLine - sampleDelay + 1] = Convert.ToSingle(1.0 / ((double)BconstBit + 0.0033540164346805303) - 273.15);
				Amb[dataLine - sampleDelay + 1] = float.Parse(values[8]);
				FileLength++;
			}
		}
		MaxVce = DeviceVce.Average();
	}

	public void CreateJunctionToThermistorCurves(SetupData _setupData, int currentFile)
	{
		float[,] CaseToThermistorTemperatures = new float[12, FileLength];
		for (int i = 0; i < FileLength; i++)
		{
			for (int Device = 0; Device < 12; Device++)
			{
				CaseToThermistorTemperatures[Device, i] = CaseTemperatures[Device, i] - Therm[i];
			}
		}
		float[] invert = new float[12];
		for (int j = 0; j < 12; j++)
		{
			invert[j] = 1f;
		}
		for (int k = 0; k < 12; k++)
		{
			if (CaseToThermistorTemperatures[k, 0] < 0f)
			{
				for (int l = 0; l < FileLength; l++)
				{
					CaseToThermistorTemperatures[k, l] *= -1f;
				}
				invert[k] = -1f;
			}
		}
		float[] minTemperatures = new float[12];
		float[] maxTemperatures = new float[12];
		for (int m = 0; m < 12; m++)
		{
			minTemperatures[m] = 10000000f;
			maxTemperatures[m] = 0f;
		}
		for (int device = 0; device < 12; device++)
		{
			for (int n = 0; n < FileLength; n++)
			{
				if (CaseToThermistorTemperatures[device, n] < minTemperatures[device])
				{
					minTemperatures[device] = CaseToThermistorTemperatures[device, n];
				}
				if (CaseToThermistorTemperatures[device, n] > maxTemperatures[device])
				{
					maxTemperatures[device] = CaseToThermistorTemperatures[device, n];
				}
			}
		}
		float powerLoss = MaxVce * _setupData.DeviceCurrent;
		for (int num = 0; num < 12; num++)
		{
			RthJth[num] = maxTemperatures[num] * 10f * invert[0] / powerLoss;
		}
		for (int num2 = 0; num2 < 12; num2++)
		{
			for (int num3 = 0; num3 < FileLength; num3++)
			{
				CaseToThermistorNormalised[num2, num3] = (CaseToThermistorTemperatures[num2, num3] - minTemperatures[num2]) / (maxTemperatures[num2] - minTemperatures[num2]);
			}
		}
	}

	public void CreateJunctionToAmbientCurves(SetupData _setupData, int currentFile)
	{
		float[,] CaseToAmbientTemperatures = new float[12, FileLength];
		for (int i = 0; i < FileLength; i++)
		{
			for (int Device = 0; Device < 12; Device++)
			{
				CaseToAmbientTemperatures[Device, i] = CaseTemperatures[Device, i] - Amb[i];
			}
		}
		float[] minTemperatures = new float[12];
		float[] maxTemperatures = new float[12];
		for (int j = 0; j < 12; j++)
		{
			minTemperatures[j] = 10000000f;
			maxTemperatures[j] = 0f;
		}
		for (int device = 0; device < 12; device++)
		{
			for (int k = 0; k < FileLength; k++)
			{
				if (CaseToAmbientTemperatures[device, k] < minTemperatures[device])
				{
					minTemperatures[device] = CaseToAmbientTemperatures[device, k];
				}
				if (CaseToAmbientTemperatures[device, k] > maxTemperatures[device])
				{
					maxTemperatures[device] = CaseToAmbientTemperatures[device, k];
				}
			}
		}
		float powerLoss = MaxVce * _setupData.DeviceCurrent;
		for (int l = 0; l < 12; l++)
		{
			RthJamb[l] = maxTemperatures[l] * 10f / powerLoss;
		}
		for (int m = 0; m < 12; m++)
		{
			for (int n = 0; n < FileLength; n++)
			{
				CaseToAmbientNormalised[m, n] = (CaseToAmbientTemperatures[m, n] - minTemperatures[m]) / (maxTemperatures[m] - minTemperatures[m]);
			}
		}
	}

	public void CreateTheMultiCurves(int numberOfTests)
	{
		int numberOfSamples = FileLength;
		float sampleTime = Time[1] - Time[0];
		float timeConstantStep1 = float.Parse("0.1");
		float timeConstantStep2 = 1f;
		float timeincrament = sampleTime;
		float[] tau1 = new float[numberOfTests];
		float[] tau2 = new float[numberOfTests];
		float previousPoint1 = 1f;
		float previousPoint2 = 1f;
		for (int testNumber = 0; testNumber < numberOfTests; testNumber++)
		{
			tau1[testNumber] = (float)(testNumber + 1) * timeConstantStep1;
			tau2[testNumber] = (float)(testNumber + 1) * timeConstantStep2;
			float b1 = Convert.ToSingle(0.0 - Math.Exp((0f - timeincrament) / tau1[testNumber]));
			float b2 = Convert.ToSingle(0.0 - Math.Exp((0f - timeincrament) / tau2[testNumber]));
			for (int timeStep = 0; timeStep < numberOfSamples; timeStep++)
			{
				MultiCurve1[testNumber, timeStep] = 0f - b1 * previousPoint1;
				MultiCurve2[testNumber, timeStep] = 0f - b2 * previousPoint2;
				previousPoint1 = MultiCurve1[testNumber, timeStep];
				previousPoint2 = MultiCurve2[testNumber, timeStep];
			}
			previousPoint1 = 1f;
			previousPoint2 = 1f;
		}
	}

	public void PerformCurveFit(int numberOfTests, int numberOfFiles)
	{
		float[] leastSquaresMin = new float[NoOfDevices];
		for (int device = 0; device < NoOfDevices; device++)
		{
			leastSquaresMin[device] = 1E+09f;
		}
		float Ts = float.Parse("0.001") * (float)(NoOfDevices * numberOfFiles) / 10f;
		float numberOfLevels = 15f;
		for (int tauOneLoop = 0; tauOneLoop < numberOfTests; tauOneLoop++)
		{
			for (int tauTwoLoop = 0; tauTwoLoop < numberOfTests; tauTwoLoop++)
			{
				for (int tauRatio = 0; (float)tauRatio <= numberOfLevels; tauRatio++)
				{
					float TsteadyState1 = Convert.ToSingle(tauRatio) / numberOfLevels;
					float TsteadyState2 = (numberOfLevels - Convert.ToSingle(tauRatio)) / numberOfLevels;
					float[] leastSquares = new float[NoOfDevices];
					for (int i = 0; i < NoOfDevices; i++)
					{
						leastSquares[i] = 0f;
					}
					float[] currentFit = new float[20000];
					for (int j = 0; j < FileLength; j++)
					{
						float Testimated = TsteadyState1 * MultiCurve1[tauOneLoop, j] + TsteadyState2 * MultiCurve2[tauTwoLoop, j];
						for (int k = 0; k < NoOfDevices; k++)
						{
							leastSquares[k] += Convert.ToSingle(Math.Pow(CaseToThermistorNormalised[k, j] - Testimated, 2.0));
						}
						currentFit[j] = Testimated;
					}
					for (int l = 0; l < NoOfDevices; l++)
					{
						if (leastSquares[l] < leastSquaresMin[l])
						{
							Tau1Min[l] = Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
							Tau2Min[l] = Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
							TauRatioMin[l] = tauRatio;
							leastSquaresMin[l] = leastSquares[l];
							FitCurve = currentFit;
						}
					}
				}
			}
		}
	}

	public void PerformCurveFitAmbient(int numberOfTests, int numberOfFiles)
	{
		float[] leastSquaresMin = new float[NoOfDevices];
		for (int device = 0; device < NoOfDevices; device++)
		{
			leastSquaresMin[device] = 1E+09f;
		}
		float Ts = float.Parse("0.001") * (float)(NoOfDevices * numberOfFiles) / 10f;
		float numberOfLevels = 15f;
		for (int tauOneLoop = 0; tauOneLoop < numberOfTests; tauOneLoop++)
		{
			for (int tauTwoLoop = 0; tauTwoLoop < numberOfTests; tauTwoLoop++)
			{
				for (int tauRatio = 0; (float)tauRatio <= numberOfLevels; tauRatio++)
				{
					float TsteadyState1 = Convert.ToSingle(tauRatio) / numberOfLevels;
					float TsteadyState2 = (numberOfLevels - Convert.ToSingle(tauRatio)) / numberOfLevels;
					float[] leastSquares = new float[NoOfDevices];
					for (int i = 0; i < NoOfDevices; i++)
					{
						leastSquares[i] = 0f;
					}
					float[] currentFit = new float[20000];
					for (int j = 0; j < FileLength; j++)
					{
						float Testimated = TsteadyState1 * MultiCurve1[tauOneLoop, j] + TsteadyState2 * MultiCurve2[tauTwoLoop, j];
						for (int k = 0; k < NoOfDevices; k++)
						{
							leastSquares[k] += Convert.ToSingle(Math.Pow(CaseToAmbientNormalised[k, j] - Testimated, 2.0));
						}
						currentFit[j] = Testimated;
					}
					for (int l = 0; l < NoOfDevices; l++)
					{
						if (leastSquares[l] < leastSquaresMin[l])
						{
							Tau1MinAmb[l] = Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
							Tau2MinAmb[l] = Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
							TauRatioMinAmb[l] = tauRatio;
							leastSquaresMin[l] = leastSquares[l];
							FitCurveAmb = currentFit;
						}
					}
				}
			}
		}
	}

	public void AddRatioToThermalResistance(SetupData _setupData, bool convertIGBT)
	{
		int[] tauRatioMin1 = new int[NoOfDevices];
		int[] tauRatioMin2 = new int[NoOfDevices];
		int[] tauRatioMinAmb1 = new int[NoOfDevices];
		int[] tauRatioMinAmb2 = new int[NoOfDevices];
		for (int device = 0; device < NoOfDevices; device++)
		{
			tauRatioMin1[device] = TauRatioMin[device] & 3;
			tauRatioMin2[device] = (TauRatioMin[device] >> 2) & 3;
			tauRatioMinAmb1[device] = TauRatioMinAmb[device] & 3;
			tauRatioMinAmb2[device] = (TauRatioMinAmb[device] >> 2) & 3;
		}
		if (convertIGBT)
		{
			for (int i = 0; i < NoOfDevices; i++)
			{
				RthJthSW[i] = (Convert.ToInt16(RthJth[i] * _setupData.Psw1kHzIGBT) & 0x3FFF) | (tauRatioMin1[i] << 14);
				RthJthCond[i] = (Convert.ToInt16(RthJth[i] * _setupData.PcondMaxIGBT) & 0x3FFF) | (tauRatioMin2[i] << 14);
			}
		}
		else
		{
			for (int j = 0; j < NoOfDevices; j++)
			{
				RthJthSW[j] = (Convert.ToInt16(RthJth[j] * _setupData.Psw1kHzDiode) & 0x3FFF) | (tauRatioMin1[j] << 14);
				RthJthCond[j] = (Convert.ToInt16(RthJth[j] * _setupData.PcondMaxDiode) & 0x3FFF) | (tauRatioMin2[j] << 14);
			}
		}
		if (convertIGBT)
		{
			for (int k = 0; k < NoOfDevices; k++)
			{
				RthJambSW[k] = (Convert.ToInt16(RthJamb[k] * _setupData.Psw1kHzIGBT) & 0x3FFF) | (tauRatioMinAmb1[k] << 14);
				RthJambCond[k] = (Convert.ToInt16(RthJamb[k] * _setupData.PcondMaxIGBT) & 0x3FFF) | (tauRatioMinAmb2[k] << 14);
			}
		}
		else
		{
			for (int l = 0; l < NoOfDevices; l++)
			{
				RthJambSW[l] = (Convert.ToInt16(RthJamb[l] * _setupData.Psw1kHzDiode) & 0x3FFF) | (tauRatioMinAmb1[l] << 14);
				RthJambCond[l] = (Convert.ToInt16(RthJamb[l] * _setupData.PcondMaxDiode) & 0x3FFF) | (tauRatioMinAmb2[l] << 14);
			}
		}
	}
}
