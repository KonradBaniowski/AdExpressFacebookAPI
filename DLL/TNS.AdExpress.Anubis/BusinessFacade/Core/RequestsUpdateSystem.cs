#region Information
//Author : G. RAGNEAU
//Creation : 05/08/2005
/*
 * Modifications : 
 *	G Ragneau - 19/12/2005 - Limitation d'Anubis au traitement de certains types de requ�tes pr�sents en BD (PDF ChronoPresse, Excel BasTet)
 * 
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using System.Timers;
using System.Threading;

using TNS.AdExpress.Anubis.Common.Configuration;
using TNS.AdExpress.Anubis.DataAccess;
using TNS.AdExpress.Anubis.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.BusinessFacade.Core
{
	/// <summary>
	/// Keep list of works to run up to date
	/// </summary>
	public class RequestsUpdateSystem{

		#region Ev�nements
		/// <summary>
		/// Le datatable contenant les demandes a �t� recalcul�
		/// </summary>
		public delegate void NewLines();
		/// <summary>
		/// Le datatable contenant les demandes a �t� recalcul�
		/// </summary>
		public event NewLines OnNewLines;
		/// <summary>
		/// An error occured
		/// </summary>
		public delegate void Error(string message, System.Exception exc);
		/// <summary>
		/// Thrown when an error occured
		/// </summary>
		public event Error OnError;
		#endregion

		#region Variables
		/// <summary>
		/// Thread qui traite l'alerte
		/// </summary>
		private Thread _myThread;
		/// <summary>
		/// The thread must stop  if true
		/// </summary>
		private bool _stop = false;
		/// <summary>
		/// Data Source relative to configuration
		/// </summary>
		private AnubisConfig _config;
		/// <summary>
		/// Data Source relative to data access
		///	</summary>
		private IDataSource _dataSource;
		/// <summary>
		/// DataTable to keep up to date
		/// </summary>
		private DataTable _requestsDt;
		/// <summary>
		/// Nombre d'update effectu�.
		/// </summary>
		private int _runNb = 0;
		/// <summary>
		/// Listes des requ�tes (plug ins ) autoris�es
		/// </summary>
		private Hashtable _plgLst = null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="config">Configuration</param>
		/// <param name="requestsDt">Donn�es</param>
		/// <param name="dataSource">Source de donn�es</param>
		/// <param name="plgLst">Listes des ty�s de requ�tes / Plugins autorios�es, si null, pas de limitation</param>
		public RequestsUpdateSystem(AnubisConfig config,DataTable requestsDt, IDataSource dataSource, Hashtable plgLst){
			if (config==null)throw (new ArgumentNullException("Parameter config can not be null."));
			if(requestsDt==null)throw (new ArgumentNullException("Parameter requestsDt can not be null."));
			_config = config;
			_dataSource=dataSource;
			_requestsDt=requestsDt;
			_plgLst = plgLst;

		}
		#endregion

		#region Lancement du thread
		/// <summary>
		/// Lance le traitement de l'alerte
		/// </summary>
		/// <remarks>
		/// Le traitement se charge de charger les donn�es n�cessaires � son execution et de lancer les alertes
		/// </remarks>
		public void Treatement(){
			try{
				ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
				_myThread=new Thread(myThreadStart);
				_myThread.Name="RequestsUpdateSystem";
				_myThread.Start();				
			}
			catch(System.Exception exc){
				//this.OnError("Unable to launch the server :<br>"+exc.Message+" - "+exc.StackTrace);
				this.OnError("Unable to launch the server",exc);
				throw(new RequestsUpdateSystemException("Unable to launch request update server",exc));
			}
		}
		#endregion

		#region Fermeture du thread
		/// <summary>
		/// Arr�t du serveur
		/// </summary>
		public void StopServer(){
			_stop = true;
			if(_myThread.ThreadState == ThreadState.WaitSleepJoin)
				_myThread.Interrupt();
		}
		#endregion

		#region Traitment
		/// <summary>
		/// Partie utile du serveur : mis � jour de la liste de jobs � effectuer.
		/// Si ComputeTreatement en est � sa premi�re ex�cution (donc qu'il n'y a pas eu d'erreur), elle se lance imm�diatement.
		/// Sinon, elle attend 
		/// </summary>
		private void ComputeTreatement(){
			try{
				DataTable dt=null;
				DataRow dr=null;
				bool newLines=false;
				try{
					if(_runNb>0)
						_myThread.Join(_config.UpdateErrorDelay);
				}
				catch(ThreadInterruptedException){}
				while(_myThread.IsAlive && !_stop){
					_runNb++;
					lock(_requestsDt){
						string notIn="";
						foreach(DataRow r in _requestsDt.Rows){
							notIn+=","+r["ID_STATIC_NAV_SESSION"].ToString();
						}
						if(notIn.Length>0)notIn=notIn.Remove(0,1);
						dt=RequestsDataAccess.Get(_dataSource,notIn,_plgLst);
						//_requestsDt.Rows.Clear();
						foreach(DataRow r in dt.Rows){
							newLines=true;
							dr=_requestsDt.NewRow();
							foreach(DataColumn currentColumn in dt.Columns){
								dr[currentColumn.ColumnName]=r[currentColumn.ColumnName];
							}
							_requestsDt.Rows.Add(dr);
						}
					}
					if(newLines)this.OnNewLines();
					try{
						if(!_stop)
							_myThread.Join(_config.JobInterval);
					}
					catch(ThreadInterruptedException){}
				}
			}
			catch(System.Exception ex){
				//this.OnError("Request update server failed : <br>"+ex.Message+" - "+ex.StackTrace);
				this.OnError("Request update server failed", ex);
				throw(new RequestsUpdateSystemException("Request update server failed",ex));
			}
		}
		#endregion

	}
}
