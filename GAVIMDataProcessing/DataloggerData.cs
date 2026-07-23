using System;
using System.IO;
using System.Linq;

namespace GAVIMDataProcessing;

public class DataloggerData
{
	private const int ArrayLength = 20000;

	private int FileLength;

	private float[] Time = new float[20000];

	private float[] UU = new float[20000];

	private float[] UL = new float[20000];

	private float[] VU = new float[20000];

	private float[] VL = new float[20000];

	private float[] WU = new float[20000];

	private float[] WL = new float[20000];

	public float[] Therm = new float[20000];

	public float[] Amb = new float[20000];

	public float[] UUjth = new float[20000];

	public float[] ULjth = new float[20000];

	public float[] VUjth = new float[20000];

	public float[] VLjth = new float[20000];

	public float[] WUjth = new float[20000];

	public float[] WLjth = new float[20000];

	private float[] UUjamb = new float[20000];

	private float[] ULjamb = new float[20000];

	private float[] VUjamb = new float[20000];

	private float[] VLjamb = new float[20000];

	private float[] WUjamb = new float[20000];

	private float[] WLjamb = new float[20000];

	private float[,] MultiCurve1 = new float[100, 20000];

	private float[,] MultiCurve2 = new float[100, 20000];

	public float[] FitCurve = new float[20000];

	public float tau1minUU;

	public float tau1minUL;

	public float tau1minVU;

	public float tau1minVL;

	public float tau1minWU;

	public float tau1minWL;

	public float tau2minUU;

	public float tau2minUL;

	public float tau2minVU;

	public float tau2minVL;

	public float tau2minWU;

	public float tau2minWL;

	public int tauRatioMinUU;

	public int tauRatioMinUL;

	public int tauRatioMinVU;

	public int tauRatioMinVL;

	public int tauRatioMinWU;

	public int tauRatioMinWL;

	public float MaxVce;

	public float RUUJth;

	public float RULJth;

	public float RVUJth;

	public float RVLJth;

	public float RWUJth;

	public float RWLJth;

	public int RUUJthSW;

	public int RULJthSW;

	public int RVUJthSW;

	public int RVLJthSW;

	public int RWUJthSW;

	public int RWLJthSW;

	public int RUUJthCond;

	public int RULJthCond;

	public int RVUJthCond;

	public int RVLJthCond;

	public int RWUJthCond;

	public int RWLJthCond;

	public float RUUJamb;

	public float RULJamb;

	public float RVUJamb;

	public float RVLJamb;

	public float RWUJamb;

	public float RWLJamb;

	public int RUUJambSW;

	public int RULJambSW;

	public int RVUJambSW;

	public int RVLJambSW;

	public int RWUJambSW;

	public int RWLJambSW;

	public int RUUJambCond;

	public int RULJambCond;

	public int RVUJambCond;

	public int RVLJambCond;

	public int RWUJambCond;

	public int RWLJambCond;

	public void GetAndConvertData(int currentFile, SetupData _setupData, string PathToData)
	{
		GetDataloggerDataFromFile(currentFile, _setupData, PathToData);
		CreateJunctionToThermistorCurves(_setupData, currentFile);
		CreateJunctionToAmbientCurves(_setupData, currentFile);
	}

	public void GetDataloggerDataFromFile(int currentFile, SetupData _setupData, string pathToData)
	{
		FileLength = 0;
		string currentFileString = Convert.ToString(currentFile);
		string currentFileFixedLength = currentFileString.PadLeft(2, '0');
		using StreamReader reader = new StreamReader(pathToData + "\\Datalogger\\GAVIML" + currentFileFixedLength + ".csv");
		int counter = 0;
		float[] uu = new float[20000];
		float[] ul = new float[20000];
		float[] vu = new float[20000];
		float[] vl = new float[20000];
		float[] wu = new float[20000];
		float[] wl = new float[20000];
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			string[] values = line.Split(',');
			counter++;
			int StartLine = 12199;
			int sampleDelay = 100;
			int dataLine = counter - StartLine + sampleDelay - 1;
			if (counter <= StartLine - sampleDelay)
			{
				continue;
			}
			if (currentFile % 2 == 0)
			{
				uu[dataLine] = float.Parse(values[1]);
				ul[dataLine] = float.Parse(values[2]);
				vu[dataLine] = float.Parse(values[3]);
				vl[dataLine] = float.Parse(values[4]);
				wu[dataLine] = float.Parse(values[5]);
				wl[dataLine] = float.Parse(values[6]);
				if (counter > StartLine)
				{
					UU[dataLine - sampleDelay] = uu[dataLine] * _setupData.Ib + _setupData.Ic;
					UL[dataLine - sampleDelay] = ul[dataLine] * _setupData.Ib + _setupData.Ic;
					VU[dataLine - sampleDelay] = vu[dataLine] * _setupData.Ib + _setupData.Ic;
					VL[dataLine - sampleDelay] = vl[dataLine] * _setupData.Ib + _setupData.Ic;
					WU[dataLine - sampleDelay] = wu[dataLine] * _setupData.Ib + _setupData.Ic;
					WL[dataLine - sampleDelay] = wl[dataLine] * _setupData.Ib + _setupData.Ic;
				}
			}
			else
			{
				uu[dataLine] = float.Parse(values[1]) * -1f;
				ul[dataLine] = float.Parse(values[2]) * -1f;
				vu[dataLine] = float.Parse(values[3]) * -1f;
				vl[dataLine] = float.Parse(values[4]) * -1f;
				wu[dataLine] = float.Parse(values[5]) * -1f;
				wl[dataLine] = float.Parse(values[6]) * -1f;
				if (counter > StartLine)
				{
					UU[dataLine - sampleDelay] = uu[dataLine] * _setupData.Db + _setupData.Dc;
					UL[dataLine - sampleDelay] = ul[dataLine] * _setupData.Db + _setupData.Dc;
					VU[dataLine - sampleDelay] = vu[dataLine] * _setupData.Db + _setupData.Dc;
					VL[dataLine - sampleDelay] = vl[dataLine] * _setupData.Db + _setupData.Dc;
					WU[dataLine - sampleDelay] = wu[dataLine] * _setupData.Db + _setupData.Dc;
					WL[dataLine - sampleDelay] = wl[dataLine] * _setupData.Db + _setupData.Dc;
				}
			}
			MaxVce = new float[6]
			{
				uu.Max(),
				ul.Max(),
				vu.Max(),
				vl.Max(),
				wu.Max(),
				wl.Max()
			}.Max();
			if (counter > StartLine)
			{
				Time[dataLine - sampleDelay] = float.Parse(values[0]);
				float thermistorVoltage = float.Parse(values[7]);
				float thermistorResistance = thermistorVoltage / ((_setupData.Vref - thermistorVoltage) / _setupData.Pullup);
				float BconstBit = Convert.ToSingle(Math.Log(thermistorResistance / _setupData.Rref)) / _setupData.Bconstant;
				Therm[dataLine - sampleDelay] = Convert.ToSingle(1.0 / ((double)BconstBit + 0.0033540164346805303) - 273.15);
				Amb[dataLine - sampleDelay] = float.Parse(values[8]);
				FileLength++;
			}
		}
	}

	public void CreateJunctionToThermistorCurves(SetupData _setupData, int currentFile)
	{
		for (int i = 0; i < FileLength; i++)
		{
			UUjth[i] = UU[i] - Therm[i];
			ULjth[i] = UL[i] - Therm[i];
			VUjth[i] = VU[i] - Therm[i];
			VLjth[i] = VL[i] - Therm[i];
			WUjth[i] = WU[i] - Therm[i];
			WLjth[i] = WL[i] - Therm[i];
		}
		float[] invert = new float[6];
		for (int j = 0; j < 6; j++)
		{
			invert[j] = 1f;
		}
		if (UUjth[0] < 0f)
		{
			for (int k = 0; k < FileLength; k++)
			{
				UUjth[k] *= -1f;
			}
			invert[0] = -1f;
		}
		if (ULjth[0] < 0f)
		{
			for (int l = 0; l < FileLength; l++)
			{
				ULjth[l] *= -1f;
			}
			invert[1] = -1f;
		}
		if (VUjth[0] < 0f)
		{
			for (int m = 0; m < FileLength; m++)
			{
				VUjth[m] *= -1f;
			}
			invert[2] = -1f;
		}
		if (VLjth[0] < 0f)
		{
			for (int n = 0; n < FileLength; n++)
			{
				VLjth[n] *= -1f;
			}
			invert[3] = -1f;
		}
		if (WUjth[0] < 0f)
		{
			for (int num = 0; num < FileLength; num++)
			{
				WUjth[num] *= -1f;
			}
			invert[4] = -1f;
		}
		if (WLjth[0] < 0f)
		{
			for (int num2 = 0; num2 < FileLength; num2++)
			{
				WLjth[num2] *= -1f;
			}
			invert[5] = -1f;
		}
		float uuMin = UUjth.Min();
		float ulMin = ULjth.Min();
		float vuMin = VUjth.Min();
		float vlMin = VLjth.Min();
		float wuMin = WUjth.Min();
		float wlMin = WLjth.Min();
		float maxUUJth = UUjth.Max();
		float maxULJth = ULjth.Max();
		float maxVUJth = VUjth.Max();
		float maxVLJth = VLjth.Max();
		float maxWUJth = WUjth.Max();
		float maxWLJth = WLjth.Max();
		float powerLoss = MaxVce * _setupData.DeviceCurrent;
		if (currentFile < 24)
		{
			RUUJth = maxUUJth * 10f * invert[0] / powerLoss;
			RULJth = maxULJth * 10f * invert[1] / powerLoss;
			RVUJth = maxVUJth * 10f * invert[2] / powerLoss;
			RVLJth = maxVLJth * 10f * invert[3] / powerLoss;
			RWUJth = maxWUJth * 10f * invert[4] / powerLoss;
			RWLJth = maxWLJth * 10f * invert[5] / powerLoss;
		}
		else
		{
			RUUJth = maxUUJth * 10f * invert[0];
			RULJth = maxULJth * 10f * invert[1];
			RVUJth = maxVUJth * 10f * invert[2];
			RVLJth = maxVLJth * 10f * invert[3];
			RWUJth = maxWUJth * 10f * invert[4];
			RWLJth = maxWLJth * 10f * invert[5];
		}
		for (int num3 = 0; num3 < FileLength; num3++)
		{
			UUjth[num3] = (UUjth[num3] - uuMin) / (maxUUJth - uuMin);
			ULjth[num3] = (ULjth[num3] - ulMin) / (maxULJth - ulMin);
			VUjth[num3] = (VUjth[num3] - vuMin) / (maxVUJth - vuMin);
			VLjth[num3] = (VLjth[num3] - vlMin) / (maxVLJth - vlMin);
			WUjth[num3] = (WUjth[num3] - wuMin) / (maxWUJth - wuMin);
			WLjth[num3] = (WLjth[num3] - wlMin) / (maxWLJth - wlMin);
		}
	}

	public void CreateJunctionToAmbientCurves(SetupData _setupData, int currentFile)
	{
		for (int i = 0; i < 10000; i++)
		{
			UUjamb[i] = UU[i] - Amb[i];
			ULjamb[i] = UL[i] - Amb[i];
			VUjamb[i] = VU[i] - Amb[i];
			VLjamb[i] = VL[i] - Amb[i];
			WUjamb[i] = WU[i] - Amb[i];
			WLjamb[i] = WL[i] - Amb[i];
		}
		int averagePoints = 100;
		int beginFinalAverageLine = FileLength - averagePoints;
		float[] finalAverageUU = new float[averagePoints];
		float[] finalAverageUL = new float[averagePoints];
		float[] finalAverageVU = new float[averagePoints];
		float[] finalAverageVL = new float[averagePoints];
		float[] finalAverageWU = new float[averagePoints];
		float[] finalAverageWL = new float[averagePoints];
		for (int j = beginFinalAverageLine; j < FileLength; j++)
		{
			finalAverageUU[j - beginFinalAverageLine] = UUjamb[j];
			finalAverageUL[j - beginFinalAverageLine] = ULjamb[j];
			finalAverageVU[j - beginFinalAverageLine] = VUjamb[j];
			finalAverageVL[j - beginFinalAverageLine] = VLjamb[j];
			finalAverageWU[j - beginFinalAverageLine] = WUjamb[j];
			finalAverageWL[j - beginFinalAverageLine] = WLjamb[j];
		}
		float uuMin = finalAverageUU.Average();
		float ulMin = finalAverageUL.Average();
		float vuMin = finalAverageVU.Average();
		float vlMin = finalAverageVL.Average();
		float wuMin = finalAverageWU.Average();
		float wlMin = finalAverageWL.Average();
		float maxUUJamb = UUjamb.Max();
		float maxULJamb = ULjamb.Max();
		float maxVUJamb = VUjamb.Max();
		float maxVLJamb = VLjamb.Max();
		float maxWUJamb = WUjamb.Max();
		float maxWLJamb = WLjamb.Max();
		float powerLoss = MaxVce * _setupData.DeviceCurrent;
		if (currentFile < 24)
		{
			RUUJamb = maxUUJamb * 10f / powerLoss;
			RULJamb = maxULJamb * 10f / powerLoss;
			RVUJamb = maxVUJamb * 10f / powerLoss;
			RVLJamb = maxVLJamb * 10f / powerLoss;
			RWUJamb = maxWUJamb * 10f / powerLoss;
			RWLJamb = maxWLJamb * 10f / powerLoss;
		}
		else
		{
			RUUJamb = maxUUJamb * 10f;
			RULJamb = maxULJamb * 10f;
			RVUJamb = maxVUJamb * 10f;
			RVLJamb = maxVLJamb * 10f;
			RWUJamb = maxWUJamb * 10f;
			RWLJamb = maxWLJamb * 10f;
		}
		for (int k = 0; k < FileLength; k++)
		{
			UUjamb[k] = (UUjamb[k] - uuMin) / (maxUUJamb - uuMin);
			ULjamb[k] = (ULjamb[k] - ulMin) / (maxULJamb - ulMin);
			VUjamb[k] = (VUjamb[k] - vuMin) / (maxVUJamb - vuMin);
			VLjamb[k] = (VLjamb[k] - vlMin) / (maxVLJamb - vlMin);
			WUjamb[k] = (WUjamb[k] - wuMin) / (maxWUJamb - wuMin);
			WLjamb[k] = (WLjamb[k] - wlMin) / (maxWLJamb - wlMin);
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

	public void FormatRthAndCurveFit(int numberOfTests, int numberOfFiles, SetupData _setupData, bool convertIGBT, int numberOfOutputLines, string PathToData, DataloggerData _dataloggerData, int currentFile)
	{
		bool convertToThermistor = true;
		OutputData _outputData = new OutputData();
		PerformCurveFit(numberOfTests, numberOfFiles);
		AddRatioToThermalResistance(_setupData, convertIGBT, convertToThermistor, currentFile);
		_outputData.WriteJthValues(PathToData, numberOfOutputLines, _dataloggerData);
		PerformCurveFitAmbient(numberOfTests, numberOfFiles);
		convertToThermistor = false;
		AddRatioToThermalResistance(_setupData, convertIGBT, convertToThermistor, currentFile);
		_outputData.WriteJambValues(PathToData, numberOfOutputLines, _dataloggerData);
	}

	public void PerformCurveFit(int numberOfTests, int numberOfFiles)
	{
		float leastSquaresMinUU = 1E+09f;
		float leastSquaresMinUL = 1E+09f;
		float leastSquaresMinVU = 1E+09f;
		float leastSquaresMinVL = 1E+09f;
		float leastSquaresMinWU = 1E+09f;
		float leastSquaresMinWL = 1E+09f;
		float Ts = float.Parse("0.001") * (float)(6 * numberOfFiles) / 10f;
		float numberOfLevels = 15f;
		for (int tauOneLoop = 0; tauOneLoop < numberOfTests; tauOneLoop++)
		{
			for (int tauTwoLoop = 0; tauTwoLoop < numberOfTests; tauTwoLoop++)
			{
				for (int tauRatio = 0; (float)tauRatio <= numberOfLevels; tauRatio++)
				{
					float TsteadyState1 = Convert.ToSingle(tauRatio) / numberOfLevels;
					float TsteadyState2 = (numberOfLevels - Convert.ToSingle(tauRatio)) / numberOfLevels;
					float leastSquaresUU = 0f;
					float leastSquaresUL = 0f;
					float leastSquaresVU = 0f;
					float leastSquaresVL = 0f;
					float leastSquaresWU = 0f;
					float leastSquaresWL = 0f;
					float[] currentFit = new float[20000];
					for (int i = 0; i < FileLength; i++)
					{
						float Testimated = TsteadyState1 * MultiCurve1[tauOneLoop, i] + TsteadyState2 * MultiCurve2[tauTwoLoop, i];
						leastSquaresUU += Convert.ToSingle(Math.Pow(UUjth[i] - Testimated, 2.0));
						leastSquaresUL += Convert.ToSingle(Math.Pow(ULjth[i] - Testimated, 2.0));
						leastSquaresVU += Convert.ToSingle(Math.Pow(VUjth[i] - Testimated, 2.0));
						leastSquaresVL += Convert.ToSingle(Math.Pow(VLjth[i] - Testimated, 2.0));
						leastSquaresWU += Convert.ToSingle(Math.Pow(WUjth[i] - Testimated, 2.0));
						leastSquaresWL += Convert.ToSingle(Math.Pow(WLjth[i] - Testimated, 2.0));
						currentFit[i] = Testimated;
					}
					if (leastSquaresUU < leastSquaresMinUU)
					{
						tau1minUU = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minUU = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinUU = tauRatio;
						leastSquaresMinUU = leastSquaresUU;
						FitCurve = currentFit;
					}
					if (leastSquaresUL < leastSquaresMinUL)
					{
						tau1minUL = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minUL = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinUL = tauRatio;
						leastSquaresMinUL = leastSquaresUL;
					}
					if (leastSquaresVU < leastSquaresMinVU)
					{
						tau1minVU = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minVU = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinVU = tauRatio;
						leastSquaresMinVU = leastSquaresVU;
					}
					if (leastSquaresVL < leastSquaresMinVL)
					{
						tau1minVL = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minVL = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinVL = tauRatio;
						leastSquaresMinVL = leastSquaresVL;
					}
					if (leastSquaresWU < leastSquaresMinWU)
					{
						tau1minWU = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minWU = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinWU = tauRatio;
						leastSquaresMinWU = leastSquaresWU;
					}
					if (leastSquaresWL < leastSquaresMinWL)
					{
						tau1minWL = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minWL = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinWL = tauRatio;
						leastSquaresMinWL = leastSquaresWL;
					}
				}
			}
		}
	}

	public void PerformCurveFitAmbient(int numberOfTests, int numberOfFiles)
	{
		float leastSquaresMinUU = 1E+09f;
		float leastSquaresMinUL = 1E+09f;
		float leastSquaresMinVU = 1E+09f;
		float leastSquaresMinVL = 1E+09f;
		float leastSquaresMinWU = 1E+09f;
		float leastSquaresMinWL = 1E+09f;
		float Ts = float.Parse("0.001") * (float)(numberOfFiles * 6) / 10f;
		float numberOfLevels = 15f;
		for (int tauOneLoop = 0; tauOneLoop < numberOfTests; tauOneLoop++)
		{
			for (int tauTwoLoop = 0; tauTwoLoop < numberOfTests; tauTwoLoop++)
			{
				for (int tauRatio = 0; (float)tauRatio < numberOfLevels; tauRatio++)
				{
					float TsteadyState1 = Convert.ToSingle(tauRatio) / numberOfLevels;
					float TsteadyState2 = (numberOfLevels - Convert.ToSingle(tauRatio)) / numberOfLevels;
					float leastSquaresUU = 0f;
					float leastSquaresUL = 0f;
					float leastSquaresVU = 0f;
					float leastSquaresVL = 0f;
					float leastSquaresWU = 0f;
					float leastSquaresWL = 0f;
					float[] currentFit = new float[20000];
					for (int i = 0; i < FileLength; i++)
					{
						float Testimated = TsteadyState1 * MultiCurve1[tauOneLoop, i] + TsteadyState2 * MultiCurve2[tauTwoLoop, i];
						leastSquaresUU += Convert.ToSingle(Math.Pow(UUjamb[i] - Testimated, 2.0));
						leastSquaresUL += Convert.ToSingle(Math.Pow(ULjamb[i] - Testimated, 2.0));
						leastSquaresVU += Convert.ToSingle(Math.Pow(VUjamb[i] - Testimated, 2.0));
						leastSquaresVL += Convert.ToSingle(Math.Pow(VLjamb[i] - Testimated, 2.0));
						leastSquaresWU += Convert.ToSingle(Math.Pow(WUjamb[i] - Testimated, 2.0));
						leastSquaresWL += Convert.ToSingle(Math.Pow(WLjamb[i] - Testimated, 2.0));
						currentFit[i] = Testimated;
					}
					if (leastSquaresUU < leastSquaresMinUU)
					{
						tau1minUU = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minUU = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinUU = tauRatio;
						leastSquaresMinUU = leastSquaresUU;
						FitCurve = currentFit;
					}
					if (leastSquaresUL < leastSquaresMinUL)
					{
						tau1minUL = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minUL = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinUL = tauRatio;
						leastSquaresMinUL = leastSquaresUL;
					}
					if (leastSquaresVU < leastSquaresMinVU)
					{
						tau1minVU = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minVU = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinVU = tauRatio;
						leastSquaresMinVU = leastSquaresVU;
					}
					if (leastSquaresVL < leastSquaresMinVL)
					{
						tau1minVL = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minVL = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinVL = tauRatio;
						leastSquaresMinVL = leastSquaresVL;
					}
					if (leastSquaresWU < leastSquaresMinWU)
					{
						tau1minWU = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minWU = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinWU = tauRatio;
						leastSquaresMinWU = leastSquaresWU;
					}
					if (leastSquaresWL < leastSquaresMinWL)
					{
						tau1minWL = (int)Convert.ToUInt16(65536f * Ts / ((float)(tauOneLoop + 1) * float.Parse("0.1")));
						tau2minWL = (int)Convert.ToUInt16(65536f * Ts / (float)(tauTwoLoop + 1));
						tauRatioMinWL = tauRatio;
						leastSquaresMinWL = leastSquaresWL;
					}
				}
			}
		}
	}

	public void AddRatioToThermalResistance(SetupData _setupData, bool convertIGBT, bool convertToThermistor, int currentFile)
	{
		int tauRatioMinUU1 = tauRatioMinUU & 3;
		int tauRatioMinUU2 = (tauRatioMinUU >> 2) & 3;
		int tauRatioMinUL1 = tauRatioMinUL & 3;
		int tauRatioMinUL2 = (tauRatioMinUL >> 2) & 3;
		int tauRatioMinVU1 = tauRatioMinVU & 3;
		int tauRatioMinVU2 = (tauRatioMinVU >> 2) & 3;
		int tauRatioMinVL1 = tauRatioMinVL & 3;
		int tauRatioMinVL2 = (tauRatioMinVL >> 2) & 3;
		int tauRatioMinWU1 = tauRatioMinWU & 3;
		int tauRatioMinWU2 = (tauRatioMinWU >> 2) & 3;
		int tauRatioMinWL1 = tauRatioMinWL & 3;
		int tauRatioMinWL2 = (tauRatioMinWL >> 2) & 3;
		float Psw1KHzIGBTLocal = _setupData.Psw1kHzIGBT;
		float PcondMaxIGBTLocal = _setupData.PcondMaxIGBT;
		float Psw1KHzDiodeLocal = _setupData.Psw1kHzDiode;
		float PcondMaxDiodeLocal = _setupData.PcondMaxDiode;
		if (currentFile > 23)
		{
			Psw1KHzIGBTLocal = 1f;
			PcondMaxIGBTLocal = 1f;
			Psw1KHzDiodeLocal = 1f;
			PcondMaxDiodeLocal = 1f;
		}
		if (convertToThermistor)
		{
			if (convertIGBT)
			{
				RUUJthSW = (Convert.ToInt16(RUUJth * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinUU1 << 14);
				RULJthSW = (Convert.ToInt16(RULJth * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinUL1 << 14);
				RVUJthSW = (Convert.ToInt16(RVUJth * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinVU1 << 14);
				RVLJthSW = (Convert.ToInt16(RVLJth * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinVL1 << 14);
				RWUJthSW = (Convert.ToInt16(RWUJth * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinWU1 << 14);
				RWLJthSW = (Convert.ToInt16(RWLJth * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinWL1 << 14);
				RUUJthCond = (Convert.ToInt16(RUUJth * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinUU2 << 14);
				RULJthCond = (Convert.ToInt16(RULJth * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinUL2 << 14);
				RVUJthCond = (Convert.ToInt16(RVUJth * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinVU2 << 14);
				RVLJthCond = (Convert.ToInt16(RVLJth * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinVL2 << 14);
				RWUJthCond = (Convert.ToInt16(RWUJth * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinWU2 << 14);
				RWLJthCond = (Convert.ToInt16(RWLJth * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinWL2 << 14);
			}
			else
			{
				RUUJthSW = (Convert.ToInt16(RUUJth * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinUU1 << 14);
				RULJthSW = (Convert.ToInt16(RULJth * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinUL1 << 14);
				RVUJthSW = (Convert.ToInt16(RVUJth * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinVU1 << 14);
				RVLJthSW = (Convert.ToInt16(RVLJth * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinVL1 << 14);
				RWUJthSW = (Convert.ToInt16(RWUJth * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinWU1 << 14);
				RWLJthSW = (Convert.ToInt16(RWLJth * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinWL1 << 14);
				RUUJthCond = (Convert.ToInt16(RUUJth * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinUU2 << 14);
				RULJthCond = (Convert.ToInt16(RULJth * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinUL2 << 14);
				RVUJthCond = (Convert.ToInt16(RVUJth * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinVU2 << 14);
				RVLJthCond = (Convert.ToInt16(RVLJth * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinVL2 << 14);
				RWUJthCond = (Convert.ToInt16(RWUJth * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinWU2 << 14);
				RWLJthCond = (Convert.ToInt16(RWLJth * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinWL2 << 14);
			}
		}
		else if (convertIGBT)
		{
			RUUJambSW = (Convert.ToInt16(RUUJamb * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinUU1 << 14);
			RULJambSW = (Convert.ToInt16(RULJamb * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinUL1 << 14);
			RVUJambSW = (Convert.ToInt16(RVUJamb * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinVU1 << 14);
			RVLJambSW = (Convert.ToInt16(RVLJamb * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinVL1 << 14);
			RWUJambSW = (Convert.ToInt16(RWUJamb * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinWU1 << 14);
			RWLJambSW = (Convert.ToInt16(RWLJamb * Psw1KHzIGBTLocal) & 0x3FFF) | (tauRatioMinWL1 << 14);
			RUUJambCond = (Convert.ToInt16(RUUJamb * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinUU2 << 14);
			RULJambCond = (Convert.ToInt16(RULJamb * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinUL2 << 14);
			RVUJambCond = (Convert.ToInt16(RVUJamb * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinVU2 << 14);
			RVLJambCond = (Convert.ToInt16(RVLJamb * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinVL2 << 14);
			RWUJambCond = (Convert.ToInt16(RWUJamb * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinWU2 << 14);
			RWLJambCond = (Convert.ToInt16(RWLJamb * PcondMaxIGBTLocal) & 0x3FFF) | (tauRatioMinWL2 << 14);
		}
		else
		{
			RUUJambSW = (Convert.ToInt16(RUUJamb * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinUU1 << 14);
			RULJambSW = (Convert.ToInt16(RULJamb * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinUL1 << 14);
			RVUJambSW = (Convert.ToInt16(RVUJamb * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinVU1 << 14);
			RVLJambSW = (Convert.ToInt16(RVLJamb * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinVL1 << 14);
			RWUJambSW = (Convert.ToInt16(RWUJamb * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinWU1 << 14);
			RWLJambSW = (Convert.ToInt16(RWLJamb * Psw1KHzDiodeLocal) & 0x3FFF) | (tauRatioMinWL1 << 14);
			RUUJambCond = (Convert.ToInt16(RUUJamb * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinUU2 << 14);
			RULJambCond = (Convert.ToInt16(RULJamb * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinUL2 << 14);
			RVUJambCond = (Convert.ToInt16(RVUJamb * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinVU2 << 14);
			RVLJambCond = (Convert.ToInt16(RVLJamb * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinVL2 << 14);
			RWUJambCond = (Convert.ToInt16(RWUJamb * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinWU2 << 14);
			RWLJambCond = (Convert.ToInt16(RWLJamb * PcondMaxDiodeLocal) & 0x3FFF) | (tauRatioMinWL2 << 14);
		}
	}
}
