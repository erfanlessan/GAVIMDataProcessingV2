using System;
using System.Collections.Generic;
using System.Windows;
using C1.Chart;
using C1.WPF.Chart;

namespace GAVIMDataProcessing;

internal class PlotSeries : Series
{
	private readonly double _ts;

	public PlotSeries(string seriesName, LinePatternEnum linePattern, Axis yAxis, double ts)
	{
		base.BindingX = "X";
		base.Binding = "Y";
		base.ChartType = ChartType.Line;
		base.SeriesName = seriesName;
		base.Style = new ChartStyle
		{
			StrokeThickness = 1.0,
			LinePattern = linePattern
		};
		base.AxisY = yAxis;
		_ts = ts;
	}

	public void ClearPlot()
	{
		base.ItemsSource = null;
	}

	public void PlotArray(float[] sourceData, double decimationRadius = 0.0, int rounding = -1)
	{
		Point[] points = new Point[sourceData.Length];
		for (int p = 0; p < points.Length; p++)
		{
			double time = _ts * (double)p;
			double data = sourceData[p];
			if (rounding >= 0)
			{
				data = Math.Round(data, rounding);
			}
			points[p] = new Point(time, data);
		}
		if (decimationRadius != 0.0)
		{
			points = DecimatePoints(points, decimationRadius);
		}
		base.ItemsSource = points;
	}

	public void PlotArray(int[] sourceData, double decimationRadius)
	{
		Point[] points = new Point[sourceData.Length];
		for (int p = 0; p < points.Length; p++)
		{
			double time = _ts * (double)p;
			double data = sourceData[p];
			points[p] = new Point(time, data);
		}
		if (decimationRadius != 0.0)
		{
			points = DecimatePoints(points, decimationRadius);
		}
		base.ItemsSource = points;
	}

	public void PlotLine(double constantYValue, int numOfSamples)
	{
		double endTime = _ts * (double)numOfSamples;
		Point[] startAndEndPoints = new Point[2]
		{
			new Point(0.0, constantYValue),
			new Point(endTime, constantYValue)
		};
		base.ItemsSource = startAndEndPoints;
	}

	private Point[] DecimatePoints(Point[] originalPoints, double decimationRadius)
	{
		int pointsLength = originalPoints.Length;
		if (pointsLength == 0)
		{
			return originalPoints;
		}
		double xmin = double.NaN;
		double xmax = double.NaN;
		double ymin = double.NaN;
		double ymax = double.NaN;
		for (int p = 0; p < pointsLength; p++)
		{
			if (double.IsNaN(xmin) || originalPoints[p].X < xmin)
			{
				xmin = originalPoints[p].X;
			}
			if (double.IsNaN(xmax) || originalPoints[p].X > xmax)
			{
				xmax = originalPoints[p].X;
			}
			if (double.IsNaN(ymin) || originalPoints[p].Y < ymin)
			{
				ymin = originalPoints[p].Y;
			}
			if (double.IsNaN(ymax) || originalPoints[p].Y > ymax)
			{
				ymax = originalPoints[p].Y;
			}
		}
		double xrad = (xmax - xmin) * decimationRadius;
		double yrad = (ymax - ymin) * decimationRadius;
		List<Point> newPointsList = new List<Point>();
		double xlast = originalPoints[0].X;
		double ylast = originalPoints[0].Y;
		newPointsList.Add(new Point(xlast, ylast));
		for (int i = 0; i < pointsLength; i++)
		{
			if (Math.Abs(xlast - originalPoints[i].X) > xrad || Math.Abs(ylast - originalPoints[i].Y) > yrad || i == pointsLength - 1)
			{
				xlast = originalPoints[i].X;
				ylast = originalPoints[i].Y;
				newPointsList.Add(new Point(xlast, ylast));
			}
		}
		return newPointsList.ToArray();
	}
}
