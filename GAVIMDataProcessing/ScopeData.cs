using System;
using System.IO;
using System.Linq;

namespace GAVIMDataProcessing;

internal class ScopeData
{
	private static int ArrayLength = 8986;

	private float[] Time = new float[ArrayLength];

	public float[] JunctionTemperature = new float[ArrayLength];

	public float[,] MultiCurve = new float[1000, ArrayLength];

	public float[] FitCurve = new float[ArrayLength];

	public int tauMin;

	public int RJC_SW;

	public int RJC_COND;

	public void GetAndConvertData(int currentFile, string PathToData, bool convertIGBT, SetupData _setupData, DataloggerData _dataloggerData)
	{
		GetScopeFromFile(currentFile, _setupData, PathToData);
		ConvertToJunctionToCase(convertIGBT, _setupData, _dataloggerData);
	}

	private void GetScopeFromFile(int currentFile, SetupData _setupData, string pathToData)
	{
		string currentFileString = Convert.ToString(currentFile);
		string currentFileFixedLength = currentFileString.PadLeft(2, '0');
		using StreamReader reader = new StreamReader(pathToData + "\\Scope\\GAVIM00" + currentFileFixedLength + ".csv");
		int counter = 0;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			string[] values = line.Split(',');
			counter++;
			int StartLine = 1040;
			int dataLine = counter - StartLine - 1;
			if (counter <= StartLine)
			{
				continue;
			}
			if (currentFile < 12)
			{
				if (currentFile % 2 == 0)
				{
					int ColumnOfInterest = currentFile / 2 + 1;
					JunctionTemperature[dataLine] = float.Parse(values[ColumnOfInterest]) * _setupData.Ib + _setupData.Ic;
				}
			}
			else if (currentFile % 2 != 0)
			{
				int ColumnOfInterest2 = (currentFile - 13) / 2 + 1;
				JunctionTemperature[dataLine] = float.Parse(values[ColumnOfInterest2]) * -1f * _setupData.Db + _setupData.Dc;
			}
			Time[dataLine] = Convert.ToSingle(2E-05 * (double)dataLine);
		}
	}

	private void ConvertToJunctionToCase(bool ConvertIGBT, SetupData _setupData, DataloggerData _dataloggerData)
	{
		int averagePoint = 100;
		int beginFinalAverageLine = ArrayLength - averagePoint;
		float[] finalAverage = new float[averagePoint];
		for (int i = beginFinalAverageLine; i < ArrayLength; i++)
		{
			finalAverage[i - beginFinalAverageLine] = JunctionTemperature[i];
		}
		float caseTemperature = finalAverage.Average();
		float maxJunctionTemperature = JunctionTemperature.Max();
		float junctionToCase = maxJunctionTemperature - caseTemperature;
		float powerloss = _setupData.DeviceCurrent * _dataloggerData.MaxVce;
		for (int j = 0; j < ArrayLength; j++)
		{
			JunctionTemperature[j] = (JunctionTemperature[j] - caseTemperature) / junctionToCase;
		}
		if (ConvertIGBT)
		{
			RJC_SW = Convert.ToInt16(Math.Floor(junctionToCase / powerloss * _setupData.Psw1kHzIGBT * 10f));
			RJC_COND = Convert.ToInt16(Math.Floor(junctionToCase / powerloss * _setupData.PcondMaxIGBT * 10f));
		}
		else
		{
			RJC_SW = Convert.ToInt16(Math.Floor(junctionToCase / powerloss * _setupData.Psw1kHzDiode * 10f));
			RJC_COND = Convert.ToInt16(Math.Floor(junctionToCase / powerloss * _setupData.PcondMaxDiode * 10f));
		}
	}

	public void CreateTheMultiCurve(int numberOfTests)
	{
		float timeConstantStep = float.Parse("0.00025");
		float timeincrament = Time[1] - Time[0];
		float[] tau = new float[numberOfTests];
		float previousPoint = 1f;
		for (int testNumber = 0; testNumber < numberOfTests; testNumber++)
		{
			tau[testNumber] = (float)(testNumber + 1) * timeConstantStep;
			float b1 = Convert.ToSingle(0.0 - Math.Exp((0f - timeincrament) / tau[testNumber]));
			for (int timeStep = 0; timeStep < ArrayLength; timeStep++)
			{
				MultiCurve[testNumber, timeStep] = 0f - b1 * previousPoint;
				previousPoint = MultiCurve[testNumber, timeStep];
			}
			previousPoint = 1f;
		}
	}

	public void PerformCurveFit(int numberOfTests)
	{
		float leastSquaresMin = 1E+09f;
		for (int tauLoop = 1; tauLoop < numberOfTests; tauLoop++)
		{
			float leastSquares = 0f;
			float[] currentFit = new float[ArrayLength];
			for (int i = 0; i < ArrayLength; i++)
			{
				float Testimated = MultiCurve[tauLoop, i];
				leastSquares += Convert.ToSingle(Math.Pow(JunctionTemperature[i] - Testimated, 2.0));
				currentFit[i] = Testimated;
			}
			if (leastSquares < leastSquaresMin)
			{
				tauMin = Convert.ToUInt16(65536f * float.Parse("0.00025") / ((float)(tauLoop + 1) * float.Parse("0.00025")));
				leastSquaresMin = leastSquares;
				FitCurve = currentFit;
			}
		}
	}
}
