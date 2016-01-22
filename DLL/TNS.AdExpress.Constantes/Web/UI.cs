#region Information
/*
 * Author : G. Ragneau
 * Creation : 30/08/2005
 * Modifications :
 *		
 * */
#endregion

using System;
using System.Drawing;

namespace TNS.AdExpress.Constantes.Web.UI
{
	/// <summary>
	/// Constantes liées aux design
	/// </summary>
	public class UI
	{
		/// <summary>
		/// couleurs des tranches de camemberts
		/// </summary>
		public static Color[] pieColors = new Color[31]{
														   	//Couleurs mélangées des tons plus claires vers plus foncé
														   Color.FromArgb(100,72,131),
														   Color.FromArgb(177,163,193),
														   Color.FromArgb(208,200,218),
														   Color.FromArgb(255,153,204),
														   Color.FromArgb(204,255,255),
														   Color.FromArgb(204,204,255),
														   Color.FromArgb(255,204,255),
														   Color.FromArgb(255,255,204),
														   Color.FromArgb(204,255,204),
														   Color.FromArgb(255,204,204),
														   Color.FromArgb(204,102,153),
														   Color.FromArgb(153,255,255),
														   Color.FromArgb(153,153,255),
														   Color.FromArgb(255,153,255),
														   Color.FromArgb(255,255,153),
														   Color.FromArgb(153,255,153),
														   Color.FromArgb(255,153,153),
														   Color.FromArgb(153,51,102),
														   Color.FromArgb(102,255,255),
														   Color.FromArgb(102,102,255),
														   Color.FromArgb(255,102,255),
														   Color.FromArgb(255,255,102),
														   Color.FromArgb(102,255,102),
														   Color.FromArgb(255,102,102),
														   Color.FromArgb(102,0,51),
														   Color.FromArgb(51,255,255),
														   Color.FromArgb(51,51,255),
														   Color.FromArgb(255,51,255),
														   Color.FromArgb(255,255,51),
														   Color.FromArgb(51,255,51),
														   Color.FromArgb(255,51,51)
													   };
		/// <summary>
		/// Nouvelle palette pour les couleurs des tranches de camemberts
		/// </summary>
		public static Color[] newPieColors = new Color[32] {
														   Color.FromArgb(100,72,131),
														   Color.FromArgb(177,163,193),
														   Color.FromArgb(208,200,218),
														   Color.FromArgb(245,236,255),
										      			   Color.FromArgb(57,85,115),
														   Color.FromArgb(107,129,153),
														   Color.FromArgb(195,211,230),
														   Color.FromArgb(237,246,255),
														   Color.FromArgb(82,90,62),
														   Color.FromArgb(176,191,134),
														   Color.FromArgb(220,230,195),
														   Color.FromArgb(250,255,236),
														   Color.FromArgb(62,90,81),
														   Color.FromArgb(125,179,162),
														   Color.FromArgb(195,230,219),
														   Color.FromArgb(236,255,249),
														   Color.FromArgb(66,90,62),
														   Color.FromArgb(142,191,134),
														   Color.FromArgb(200,230,195),
														   Color.FromArgb(238,255,236),
														   Color.FromArgb(125,50,0),
														   Color.FromArgb(217,87,0),
														   Color.FromArgb(230,161,115),
														   Color.FromArgb(255,217,191),
														   Color.FromArgb(67,65,90),
														   Color.FromArgb(144,141,191),
														   Color.FromArgb(201,199,230),
														   Color.FromArgb(239,238,255),
														   Color.FromArgb(125,75,0),
														   Color.FromArgb(179,107,0),
														   Color.FromArgb(255,204,128),
														   Color.FromArgb(255,230,191)
													   };

		/// <summary>
		/// Couleurs des graphiques en barres
		/// </summary>
		public static Color[] barColors = new Color[2]{
									  Color.FromArgb(255,215,215),
									  Color.FromArgb(148,121,181)
								  };

	}
}