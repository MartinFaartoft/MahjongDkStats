using MahjongDkStatsCalculators;
using ScottPlot;

namespace MahjongDkStats.CLI
{
	internal class PlotHelper
	{
		private const string PlotBackgroundColorHex = "#F9F9F9";
		private const string PlotLineColorHex = "#9C0000";

		internal static Plot CreateDateTimePlot(DateTimeChart data, string title)
		{
			Plot plot = new();

			if (data.X.Length == 0)
			{
				return plot;
			}

			plot.Add.Palette = new MahjongDkPalette();
			plot.Add.ScatterLine(data.X, data.Y);
			plot.Title(title);
			plot.FigureBackground.Color = Color.FromHex(PlotBackgroundColorHex);
			plot.Axes.DateTimeTicksBottom();

			return plot;
		}

		internal static Plot CreatingInvertedYDateTimePlot(DateTimeChart data, string title)
		{
			var plot = CreateDateTimePlot(data, title);
			//plot.Axes.SetLimitsY(1, 50); // TODO fiddle with this some more to find a good balance between detail and overview
			plot.Axes.InvertY();
			
			return plot;
		}

		private class MahjongDkPalette : IPalette
		{
			public Color[] Colors => [Color.FromHex(PlotLineColorHex)];

			public string Name => "MahjongDK";

			public string Description => "";

			public Color GetColor(int index)
			{
				return Colors[0];
			}
		}
	}
}
