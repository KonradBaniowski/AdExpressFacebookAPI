#region Info
/*
 * Author           : G RAGNEAU 
 * Date             : 09/08/2007
 * Modifications    :
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using System.Data;

using TNS.AdExpress.Web.Common.Results.Creatives.Comparers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI.TableControl;

namespace TNS.AdExpress.Web.Common.Results.Creatives {


	///<summary>
	/// CreativeItem provides information for the display of a creative
	/// </summary>
	///  <author>G Ragneau</author>
	///  <since>09/08/2007</since>
	///  <stereotype>entity</stereotype>
	public class CreativeItem : TableElement {

		#region Variables
		///<summary>Version Id</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		protected long _id;
		///<summary>Version path</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
        protected string _path = string.Empty;
		///<summary>Annonceur</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
        protected string _advertiser;
		/// <summary>Produit</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
        protected string _product;
		/// <summary>groupe de produit</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
        protected string _group;
		///<summary>
		///Creative budget
		///</summary>
		///<author>Guillaume.Ragneau</author>
		///<since>lundi 13 août 2007</since>		
        protected decimal _Budget;
		///<summary>
		///Number of active media
		///</summary>
		///<author>Guillaume.Ragneau</author>
		///<since>mardi 20 août 2007</since>		
        protected int _mediaNb = 0;
        /// <summary>
        /// Number of insertion of the creative
        /// </summary>
 		///<author>Guillaume.Ragneau</author>
		///<since>mardi 20 août 2007</since>		
        protected int _insertNb = 0;
        /// <summary>
        /// Adresse pour le GAD
        /// </summary>
        protected Int64 _adressId = -1;
        /// <summary>
        /// Web Session
        /// </summary>
        protected WebSession _session = null;
        #endregion

		#region Accessors
		///<summary>Get / Set Id Version</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		public long Id {
    		get {
        		return (_id);
    		}
		}

		///<summary>Get / Set Version Path</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		public string Path {
			get {
				return (_path);
			}
			set {
				_path = value;
			}
		}

		///<summary>Get / Set Annonceur</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		public string Advertiser {
			get {
				return (_advertiser);
			}
			set {
				_advertiser = value;
			}
		}

		///<summary>Get / Set Produit</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		public string Product {
			get {
				return (_product);
			}
			set {
				_product = value;
			}
		}

		///<summary>Get / Set groupe de produit</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		public string Group {
			get {
				return (_group);
			}
			set {
				_group = value;
			}
		}
		///<summary>
		///Get / Set Budget
		///</summary>
		///<author>Guillaume.Ragneau</author>
		///<since>lundi 13 août 2007</since>
		public decimal Budget {
    		get {
        		return (_Budget);
    		}
    		set {
        		_Budget = value;
    		}
		}
		///<summary>
		/// Get / Set Number of active media
		///</summary>
		///<author>Guillaume.Ragneau</author>
		///<since>mardi 20 août 2007</since>		
        public int MediaNb {
            get {
                return _mediaNb;
            }
            set {
                _mediaNb = value;
            }
        }
        /// <summary>
        /// Get / Set Number of insertion of the creative
        /// </summary>
 		///<author>Guillaume.Ragneau</author>
		///<since>mardi 20 août 2007</since>		
        public int InsertNb {
            get {
                return _insertNb;
            }
            set {
                _insertNb = value;
            }
        }
        /// <summary>
        /// Get / Set Web Session
        /// </summary>
        protected WebSession Session {
            get {
                return _session;
            }
            set {
                _session = value;
            }
        }

		#endregion
		
		#region Constructors
		///<summary>Constructor</summary>
		///<author>Guillaume Ragneau</author>
		///<since>jeudi 09 aout 2007</since>
		public CreativeItem( long id ) {
			_id = id;
            this.AddComparer(new AdvertiserComparer(857));
            this.AddComparer(new GroupComparer(1110));
            this.AddComparer(new ProductComparer(858));
		}
		#endregion

        #region TableElement Implementation
        /// <summary>
        /// Render Table Element
        /// </summary>
        /// <param name="output">Rendered code output</param>
        /// <author>Guillaume.Ragneau</author>
        /// <since>mardi 14 août 2007</since>
        public override void Render(System.Text.StringBuilder output) {
            output.AppendFormat("<table><td with=50%><tr><td>Advertiser</td><td>: {0}</td></tr><tr><td>Group</td><td>: {1}</td></tr><tr><td>Product</td><td>: {2}</td></tr></td><td with=50%><tr><td>Budget</td><td>: {3} €</td></tr></td>",
                _advertiser,
                _group,
                _product,
                _Budget);
        }
        /// <summary>
        /// Render Table Element
        /// </summary>
        /// <param name="output">Rendered code output</param>
        /// <param name="cssStyle">Css Style of the element</param>
        /// <author>Guillaume.Ragneau</author>
        /// <since>mardi 14 août 2007</since>
        public override void Render(System.Text.StringBuilder output, string cssStyle) {
            output.AppendFormat("<table class=\"{4}\"><td with=50%><tr><td>Advertiser</td><td>: {0}</td></tr><tr><td>Group</td><td>: {1}</td></tr><tr><td>Product</td><td>: {2}</td></tr></td><td with=50%><tr><td>Budget</td><td>: {3} €</td></tr></td>",
                _advertiser,
                _group,
                _product,
                _Budget,
                cssStyle);
        }
        #endregion

        #region GetInstance
        /// <summary>
        /// Build instanc of CreativeRadio using data containded in row
        /// </summary>
        /// <param name="row">Data container</param>
        /// <param name="session">Web Session</param>
        /// <returns>New CreativeItem Instance</returns>
        public virtual CreativeItem GetInstance(DataRow row, WebSession session) {

            long id = Convert.ToInt64(row["version"]);
            CreativeRadio item = new CreativeRadio(id);
            item.Session = session;
            FieldInstance(row, item);

            return item;

        }
        /// <summary>
        /// Fill instance with data
        /// </summary>
        /// <param name="row">Data Container</param>
        /// <param name="item">item to fill</param>
        protected virtual void FieldInstance(DataRow row, CreativeItem item) {
            item._advertiser = row["advertiser"].ToString();
            item._group = row["groupe"].ToString();
            item._path = row["visuel"].ToString();
            item._product = row["product"].ToString();
            if (row["id_address"].ToString().Length>0) item._adressId = Convert.ToInt64(row["id_address"]);
        }
        #endregion

    }
}
