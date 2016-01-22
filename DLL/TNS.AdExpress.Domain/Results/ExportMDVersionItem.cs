#region Info
/* Author : Y. R'kaina
 * Creation : 20/06/2007
 *		G Ragneau - 30/04/2008 - Déplacer de TNS/.AdExpres.Web vers TNS.AdExpress.Domain
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Results {

    /// <summary>
    /// Item used in export of VMC version from Media Schedule
    /// </summary>
    public class ExportMDVersionItem : VersionItem {

        #region Variables
        /// <summary>
        /// poids d'une version
        /// </summary>
        private long _weight;
        /// <summary>
        /// Budget
        /// </summary>
        private long _expenditureEuro;
        /// <summary>
        /// Volume
        /// </summary>
        private double _volume;
        /// <summary>
        /// Format
        /// </summary>
        private string _format;
        /// <summary>
        /// Format du contenant
        /// </summary>
        private string _mailFormat;
        /// <summary>
        /// Type de document
        /// </summary>
        private string _mailType;
        /// <summary>
        /// Format
        /// </summary>
        private string _wpMailFormat;
        /// <summary>
        /// Nombre d'items
        /// </summary>
        private string _objectCount;
        /// <summary>
        /// Mail content
        /// </summary>
        private string _mailContent;
        /// <summary>
        /// Enveloppe gestion
        /// </summary>
        private string _mailingRapidity;
        /// <summary>Nombre de visuels</summary>
        private Int64 _nbVisuel;
        /// <summary>Id Media</summary>
        private Int64 _idMedia;
        #endregion

        #region Accessors
        ///<summary>Get / Set poids d'une version</summary>
        public long Weight {
            get {return (_weight);}
            set {_weight = value;}
        }

        ///<summary>Get / Set Budget</summary>
        public long ExpenditureEuro {
            get { return (_expenditureEuro); }
            set { _expenditureEuro = value; }
        }

        ///<summary>Get / Set Volume</summary>
        public double Volume {
            get { return (_volume); }
            set { _volume = value; }
        }

        ///<summary>Get / Set Format</summary>
        public string Format {
            get { return (_format); }
            set { _format = value; }
        }

        ///<summary>Get / Set Format du contenant</summary>
        public string MailFormat {
            get { return (_mailFormat); }
            set { _mailFormat = value; }
        }

        ///<summary>Get / Set Type de document</summary>
        public string MailType {
            get { return (_mailType); }
            set { _mailType = value; }
        }

        ///<summary>Get / Set Format</summary>
        public string WpMailFormat {
            get { return (_wpMailFormat); }
            set { _wpMailFormat = value; }
        }

        ///<summary>Get / Set Nombre d'items</summary>
        public string ObjectCount {
            get { return (_objectCount); }
            set { _objectCount = value; }
        }

        ///<summary>Get / Set Mail content</summary>
        public string MailContent {
            get { return (_mailContent); }
            set { _mailContent = value; }
        }

        ///<summary>Get / Set Enveloppe gestion</summary>
        public string MailingRapidity {
            get { return (_mailingRapidity); }
            set { _mailingRapidity = value; }
        }

        ///<summary>Get / Set Nombre de visuels</summary>
        public Int64 NbVisuel {
            get {return (_nbVisuel);}
            set {_nbVisuel = value;}
        }

        ///<summary>Get / Set Id media</summary>
        public Int64 IdMedia {
            get { return (_idMedia); }
            set { _idMedia = value; }
        }
        #endregion

        #region Constructors
		///<summary>Constructor</summary>
		public ExportMDVersionItem( Int64 id, string cssClass, string parution ):base( id, cssClass, parution ){
		}

		///<summary>Constructor</summary>
        public ExportMDVersionItem(Int64 id, string cssClass): base(id, cssClass){
		}
		#endregion


    }
}
