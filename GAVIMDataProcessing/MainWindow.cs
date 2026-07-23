using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using C1.Chart;
using C1.WPF.Chart;

namespace GAVIMDataProcessing;

public partial class MainWindow : Window, IComponentConnector
{
	private readonly BackgroundWorker _bWorker;

	private readonly DataloggerData _dataloggerData;

	private readonly ScopeData _scopeData;

	private readonly SetupData _setupData;

	private readonly OutputData _outputData;

	private readonly IPMData _IPMData;

	private PlotSeries _series1;

	private PlotSeries _series2;

	private string PathToData;

	private bool ConvertIPM;

	public MainWindow()
	{
		InitializeComponent();
		_bWorker = new BackgroundWorker();
		_bWorker.DoWork += ProcessData;
		_bWorker.WorkerSupportsCancellation = true;
		_bWorker.RunWorkerCompleted += BWorker_RunWorkerCompleted;
		_dataloggerData = new DataloggerData();
		_scopeData = new ScopeData();
		_setupData = new SetupData();
		_outputData = new OutputData();
		_IPMData = new IPMData();
		UserPlot.BeginUpdate();
		UserPlot.RenderMode = RenderMode.Direct2D;
		UserPlot.AxisX.Title = "Time (seconds)";
		Axis temperatureAxis = new Axis
		{
			Position = Position.Left,
			Title = "Temperature (oC)"
		};
		_series1 = new PlotSeries("Raw Data (oC)", LinePatternEnum.Solid, temperatureAxis, 0.05);
		_series2 = new PlotSeries("Curve Fit (oC)", LinePatternEnum.Solid, temperatureAxis, 0.05);
		UserPlot.Series.Add(_series1);
		UserPlot.Series.Add(_series2);
		UserPlot.EndUpdate();
	}

	private void ButtonTest_Click(object sender, RoutedEventArgs e)
	{
		EnableControls(Enable: false);
		PathToData = RawDataFolderSelected.Text;
		if (Convert.ToBoolean(IPMcheckBox.IsChecked))
		{
			ConvertIPM = true;
		}
		else
		{
			ConvertIPM = false;
		}
		_bWorker.RunWorkerAsync();
	}

	private void ProcessData(object sender, DoWorkEventArgs e)
	{
		bool testAborted = false;
		if (_setupData.PerformSetup(PathToData))
		{
			return;
		}
		int numberOfTests = 100;
		int numberOfOutputLinesMinusOne = 37;
		if (_outputData.CreateOutputFileWithHeadings(PathToData))
		{
			return;
		}
		if (ConvertIPM)
		{
			ProcessIPMData(numberOfTests, numberOfOutputLinesMinusOne);
		}
		else
		{
			int numberOfFiles = 26;
			int numberOfScopeTests = 250;
			for (int currentFile = 0; currentFile < numberOfFiles; currentFile++)
			{
				_dataloggerData.GetAndConvertData(currentFile, _setupData, PathToData);
				bool convertIGBT = true;
				bool performScopeCurveFit = false;
				if (currentFile < 12)
				{
					if (currentFile % 2 == 0)
					{
						_scopeData.GetAndConvertData(currentFile, PathToData, convertIGBT, _setupData, _dataloggerData);
						performScopeCurveFit = true;
					}
				}
				else if (currentFile < 24 && currentFile % 2 != 0)
				{
					convertIGBT = false;
					_scopeData.GetAndConvertData(currentFile, PathToData, convertIGBT, _setupData, _dataloggerData);
					performScopeCurveFit = true;
				}
				if (currentFile == 0)
				{
					_dataloggerData.CreateTheMultiCurves(numberOfTests);
					_scopeData.CreateTheMultiCurve(numberOfScopeTests);
					_outputData.WriteOtherCodetableData(PathToData, numberOfOutputLinesMinusOne, _setupData);
				}
				if (performScopeCurveFit)
				{
					_scopeData.PerformCurveFit(numberOfScopeTests);
					_outputData.WriteSelfImpeadance(PathToData, numberOfOutputLinesMinusOne, _scopeData);
				}
				_dataloggerData.FormatRthAndCurveFit(numberOfTests, numberOfFiles, _setupData, convertIGBT, numberOfOutputLinesMinusOne, PathToData, _dataloggerData, currentFile);
			}
		}
		System.Windows.MessageBox.Show("Done!");
	}

	private void ProcessIPMData(int numberOfTests, int OutputLinesMinusOne)
	{
		int numberOfFiles = 12;
		bool ConvertIGBT = true;
		for (int currentFile = 0; currentFile < numberOfFiles; currentFile++)
		{
			_IPMData.GetAndConvertData(currentFile, _setupData, PathToData);
			if (currentFile == 0)
			{
				_outputData.WriteOtherCodetableData(PathToData, OutputLinesMinusOne, _setupData);
				_IPMData.CreateTheMultiCurves(numberOfTests);
			}
			_IPMData.PerformAllCurveFits(numberOfTests, numberOfFiles);
			if (currentFile == 6)
			{
				ConvertIGBT = false;
			}
			_IPMData.AddRatioToThermalResistance(_setupData, ConvertIGBT);
			_outputData.WriteThermalImpeadanceValuesIPM(PathToData, OutputLinesMinusOne, _IPMData);
		}
	}

	private void BWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		EnableControls(Enable: true);
		UserPlot.BeginUpdate();
		_series1.PlotArray(_scopeData.JunctionTemperature);
		_series2.PlotArray(_scopeData.FitCurve);
		UserPlot.EndUpdate();
	}

	private void EnableControls(bool Enable)
	{
		ButtonTest.IsEnabled = Enable;
	}

	private void RawDataBrowse_Click(object sender, RoutedEventArgs e)
	{
		FolderBrowserDialog fbDialog = new FolderBrowserDialog();
		fbDialog.ShowDialog();
		RawDataFolderSelected.Text = fbDialog.SelectedPath;
	}

	private void AbortButtonClick(object sender, RoutedEventArgs e)
	{
		_bWorker.CancelAsync();
	}
}
