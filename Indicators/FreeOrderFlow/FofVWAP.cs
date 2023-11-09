#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX.DirectWrite;
using NinjaTrader.Core;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators.FreeOrderFlow
{
	public class FofVWAP : Indicator
	{
		private Series<double> cumVol;
		private Series<double> cumPV;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Free Order Flow VWAP";
				Name										= "VWAP";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				double TextLabelWidth 				= 244;
				bool DrawTextLabel = false;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event.
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				AddPlot(Brushes.Orange, "VWAP");
			}
			else if (State == State.DataLoaded)
			{
				cumVol = new Series<double>(this);
				cumPV = new Series<double>(this);
			} else if (State == State.Historical) {
				// Displays a message if the bartype is not intraday
				if (!Bars.BarsType.IsIntraday)
				{
					Draw.TextFixed(this, "NinjaScriptInfo", "VwapAR Indicator only supports Intraday charts", TextPosition.BottomRight);
					Log("VwapAR only supports Intraday charts", LogLevel.Error);
				}
			}
		}

		protected override void OnBarUpdate()
		{
			if(Bars.IsFirstBarOfSession)
			{
				if(CurrentBar > 0) Values[0].Reset(1);
				cumVol[1] = 0;
				cumPV[1] = 0;
			}

			cumPV[0] = cumPV[1] + (Typical[0] * Volume[0]);
			cumVol[0] = cumVol[1] + Volume[0];

			// plot VWAP value
			Values[0][0] = cumPV[0] / (cumVol[0] == 0 ? 1 : cumVol[0]);
		}
		
		void DrawText(int index, string text, TextFormat textFormat, ChartControl chartControl, ChartScale chartScale) {
			
			
			Plot plot = Plots[index];
			double x = chartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex) -TextLabelWidth;
			double y = chartScale.GetYByValue(Values[index].GetValueAt(ChartBars.ToIndex-1));
			Point point = new Point(x, y);
			TextLayout textLayout = new TextLayout(Globals.DirectWriteFactory, text, textFormat, ChartPanel.W, textFormat.FontSize);
			RenderTarget.DrawTextLayout(point.ToVector2(), textLayout, plot.BrushDX);
		}
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			base.OnRender(chartControl, chartScale);
			
			TextFormat	textFormat	= chartControl.Properties.LabelFont.ToDirectWriteTextFormat();
			
			if (DrawTextLabel)
				DrawText(0, "VWAP", textFormat, chartControl, chartScale);

			textFormat.Dispose();
		}
		
		#region Properties
		[Display(ResourceType = typeof(Custom.Resource), Name = "TextLabelWidth", GroupName = "NinjaScriptParameters", Order = 1)]
		public double TextLabelWidth
		{ get; set; }
		[Display(ResourceType = typeof(Custom.Resource), Name = "DrawTextLabel", GroupName = "NinjaScriptParameters", Order = 2)]
		public bool DrawTextLabel
		{ get; set; }
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private FreeOrderFlow.FofVWAP[] cacheFofVWAP;
		public FreeOrderFlow.FofVWAP FofVWAP()
		{
			return FofVWAP(Input);
		}

		public FreeOrderFlow.FofVWAP FofVWAP(ISeries<double> input)
		{
			if (cacheFofVWAP != null)
				for (int idx = 0; idx < cacheFofVWAP.Length; idx++)
					if (cacheFofVWAP[idx] != null &&  cacheFofVWAP[idx].EqualsInput(input))
						return cacheFofVWAP[idx];
			return CacheIndicator<FreeOrderFlow.FofVWAP>(new FreeOrderFlow.FofVWAP(), input, ref cacheFofVWAP);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.FreeOrderFlow.FofVWAP FofVWAP()
		{
			return indicator.FofVWAP(Input);
		}

		public Indicators.FreeOrderFlow.FofVWAP FofVWAP(ISeries<double> input )
		{
			return indicator.FofVWAP(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.FreeOrderFlow.FofVWAP FofVWAP()
		{
			return indicator.FofVWAP(Input);
		}

		public Indicators.FreeOrderFlow.FofVWAP FofVWAP(ISeries<double> input )
		{
			return indicator.FofVWAP(input);
		}
	}
}

#endregion
