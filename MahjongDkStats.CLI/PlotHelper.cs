using MahjongDkStatsCalculators;
using ScottPlot;

namespace MahjongDkStats.CLI
{
	internal class PlotHelper
	{
		private const string PlotBackgroundColorHex = "#F9F9F9";
		private const string PlotLineColorHex = "#9C0000";

		internal static Plot CreateRatingPlot(DateTimeChart ratingData, string title)
		{
			Plot plot = new();
			plot.Add.Palette = new MahjongDkPalette();
			plot.Add.ScatterLine(ratingData.X, ratingData.Y);
			plot.Title(title);
			plot.FigureBackground.Color = Color.FromHex(PlotBackgroundColorHex);
			plot.Axes.DateTimeTicksBottom();

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
