using System;
using System.Web.UI;
using System.Collections;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description résumée de CountItemsInClassificationWebControlBuilder.
	/// </summary>
	public class CountItemsInClassificationWebControlBuilder: ControlBuilder{
		/// <summary>
		/// Supports custom parsing of color items within 
		/// the HtmlColorDropDown control.
		/// </summary>
		public override Type GetChildControlType(string tagName,IDictionary attribs) {
			if (string.Compare(tagName,"classificationdescription", true) == 0) {
				return typeof(ClassificationDescriptionHelper);
			}
			return base.GetChildControlType (tagName, attribs);
		}
	}
}
