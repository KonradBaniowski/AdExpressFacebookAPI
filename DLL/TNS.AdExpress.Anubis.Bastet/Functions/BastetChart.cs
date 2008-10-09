#region Informations
// Auteur: D. V. Mussuma
// Date de création: 01/03/2006
// Date de modification:
#endregion

using System;
using Aspose.Cells;
using System.Drawing;

namespace TNS.AdExpress.Anubis.Bastet.Functions
{
	/// <summary>
	/// Description résumée de BastetChart.
	/// </summary>
	public class BastetChart
	{
		/// <summary>
		/// Construit un graphique de statistiques
		/// </summary>
		/// <param name="chart">Graphique</param>
		/// <param name="serial">serie de valeurs</param>
		/// <param name="category">libellés</param>
		/// <param name="name">nom de la série</param>
		/// <param name="titleText">titre du graphique</param>
		/// <param name="CategoryAxisTitleText">titre abscisse</param>
		/// <param name="valueAxisTitleText">titre ordonnées</param>
		public static void Build(Chart chart,string serial,string category,string name,string titleText,string CategoryAxisTitleText,string valueAxisTitleText) {
			//Adding NSeries (chart data source) to the chart ranging from "A1" cell to "B3"
			chart.NSeries.Add(serial,false);
			chart.NSeries.CategoryData=category;
			chart.NSeries[0].Name=name;

			//Setting the title of a chart
			chart.Title.Text = titleText;					 			

			//Setting the font color of the chart title to blue
			chart.Title.TextFont.Color = Color.Blue;


			//Setting the title of category axis of the chart
			chart.CategoryAxis.Title.Text = CategoryAxisTitleText;


			//Setting the title of value axis of the chart
			chart.ValueAxis.Title.Text = valueAxisTitleText;

			//Hiding the major gridlines
			chart.MajorGridLines.IsVisible = false;

			
		}
	}
}
