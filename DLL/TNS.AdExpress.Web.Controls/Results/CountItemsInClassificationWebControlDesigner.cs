using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Drawing;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description résumée de CountItemsInClassificationWebControlDesigner.
	/// </summary>
	public class CountItemsInClassificationWebControlDesigner: ControlDesigner{
		/// <summary>
		/// Provides custom HTML for persisting the UserDefinedColors 
		/// collection as inner content for the HtmlColorDropDown
		/// control tag.
		/// </summary>
		public override string GetPersistInnerHtml() {
			StringWriter sw = new StringWriter();
			HtmlTextWriter html = new HtmlTextWriter(sw);

			CountItemsInClassificationWebControl dd = this.Component as CountItemsInClassificationWebControl;
			if (dd != null) {
				// for each color in the collection, output its
				// html known name (if it is a known color)
				// or its html hex string representation
				foreach(ClassificationDescription c in dd.Levels) {
					string labelTextId = c.labelTextId.ToString();
					string LevelType = c.LevelType.GetHashCode().ToString();

					html.WriteBeginTag("classificationdescription");
					html.WriteAttribute("labelTextId", labelTextId);
					html.WriteAttribute("LevelType", LevelType);
					html.WriteLine(HtmlTextWriter.SelfClosingTagEnd);
				}

			}
			
			return sw.ToString();

		}
	}
}
