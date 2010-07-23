using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Web.Navigation
{
    public class RightMenuLinks
    {

        /// <summary>
        /// RightMenuLinksList
        /// </summary>
        protected List<RightMenuLinksItem> _rightMenuLinksList = new List<RightMenuLinksItem>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        public RightMenuLinks(IDataSource source)
        {
            RightMenuLinksXL.LoadRightMenuLinks(source, _rightMenuLinksList);
        }

        /// <summary>
        /// Get RightMenuLinksList
        /// </summary>
        public List<RightMenuLinksItem> RightMenuLinksList
        {
            get { return _rightMenuLinksList; }
        }

    }
}
