using MahjongDkStatsCalculators;
using ScottPlot;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MahjongDkStats.CLI
{
    internal partial class PlotHelper
	{
		private const string PlotBackgroundColorHex = "#FFFFFF";
		private const string PlotColorRedHex = "#9C0000";
		private const string PlotColorGreenHex = "#2CA02C";


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

		private static readonly double[] RatingPositionTicks = [1, 10, 20, 30, 40, 50];

		internal static Plot CreateInvertedYDateTimePlot(DateTimeChart data, string title)
		{
			var plot = CreateDateTimePlot(data, title);
			if (data.X.Length == 0)
			{
				return plot;
			}
			//plot.Axes.Left.SetTicks(RatingPositionTicks, RatingPositionTicks.Select(v => v.ToString()).ToArray());
			plot.Axes.SetLimitsY(0, data.Y.Max()); // TODO fiddle with this some more to find a good balance between detail and overview
			plot.Axes.InvertY();

			return plot;
		}

		internal static Plot CreateGamesPerYearByRulesetPlot(YearStatistics[] stats)
		{
			var positions = Enumerable.Range(1, 100).Where(x => x % 3 != 0).Chunk(2).ToArray();

			Plot plot = new();
			plot.Title("Games played per year");
			var x = stats.Select((s, i) => new Bar[] {
				new Bar() { Position = positions[i][0], Value = s.McrGames, FillColor = Color.FromHex(PlotColorRedHex) },
				new Bar() { Position = positions[i][1], Value = s.RiichiGames, FillColor = Color.FromHex(PlotColorGreenHex) }});

            var bars = x.SelectMany(x => x).ToArray();
			
			plot.Add.Bars(bars);

			// build the legend manually
			plot.Legend.IsVisible = true;
			plot.Legend.Alignment = Alignment.UpperRight;
			plot.Legend.ManualItems.Add(new() { LabelText = "MCR", FillColor = Color.FromHex(PlotColorRedHex) });
			plot.Legend.ManualItems.Add(new() { LabelText = "Riichi", FillColor = Color.FromHex(PlotColorGreenHex) });

			// show group labels on the bottom axis
			Tick[] ticks = stats.Select((s, i) => new Tick(positions[i][0] + .5, s.Year.ToString())).ToArray();
			plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
			plot.Axes.Bottom.MajorTickStyle.Length = 0;
			plot.HideGrid();

			plot.Axes.Margins(bottom: 0);

			return plot;			
		}

        internal static Plot CreateActivePlayersPerYearByRulesetPlot(YearStatistics[] stats)
        {
            var positions = Enumerable.Range(1, 100).Where(x => x % 3 != 0).Chunk(2).ToArray();

            Plot plot = new();
            plot.Title("Active players per year");
            var x = stats.Select((s, i) => new Bar[] {
                new Bar() { Position = positions[i][0], Value = s.McrActivePlayers, FillColor = Color.FromHex(PlotColorRedHex) },
                new Bar() { Position = positions[i][1], Value = s.RiichiActivePlayers, FillColor = Color.FromHex(PlotColorGreenHex) }});

            var bars = x.SelectMany(x => x).ToArray();

            plot.Add.Bars(bars);

            // build the legend manually
            plot.Legend.IsVisible = true;
            plot.Legend.Alignment = Alignment.UpperRight;
            plot.Legend.ManualItems.Add(new() { LabelText = "MCR", FillColor = Color.FromHex(PlotColorRedHex) });
            plot.Legend.ManualItems.Add(new() { LabelText = "Riichi", FillColor = Color.FromHex(PlotColorGreenHex) });

            // show group labels on the bottom axis
            Tick[] ticks = stats.Select((s, i) => new Tick(positions[i][0] + .5, s.Year.ToString())).ToArray();
            plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            plot.Axes.Bottom.MajorTickStyle.Length = 0;
            plot.HideGrid();

            plot.Axes.Margins(bottom: 0);

            return plot;
        }

        private class MahjongDkPalette : IPalette
		{
			public Color[] Colors => [Color.FromHex(PlotColorRedHex)];

			public string Name => "MahjongDK";

			public string Description => "";

			public Color GetColor(int index)
			{
				return Colors[0];
			}
		}
	}
}
