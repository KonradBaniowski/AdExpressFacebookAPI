using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KM.Framework.DataSource
{
    public class XmlReaderDataSource
    {

        #region Variables
        /// <summary>
        /// Source de données
        /// </summary>
        protected XmlTextReader _source;
        /// <summary>
        /// Type de la source de données
        /// </summary>
        //protected DataSource.Type _sourceType = DataSource.Type.xml;
        /// <summary>
        /// Repository location
        /// </summary>
        protected string _filePath = "";
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="source">Source de données</param>
        public XmlReaderDataSource(XmlTextReader source)
        {
            if (source == null) throw (new NullReferenceException("La source de données est NULL"));
            _source = source;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="filePath">Chemin de la source de données</param>
        public XmlReaderDataSource(string filePath)
        {
            if (filePath == null) throw (new NullReferenceException("Le chemin de la source de données est NULL"));
            if (filePath.Length == 0) throw (new ArgumentException("Le chemin de la source de données est vide"));
            if (!File.Exists(filePath)) throw (new FileNotFoundException("La source de donnes n'existe pas"));
            _filePath = filePath;
            _source = new XmlTextReader(filePath);
        }

        #endregion

        #region Méthodes Externes

        #region Repository
        /// <summary>
        /// Get Repository location
        /// </summary>
        /// <returns>Repository location</returns>
        public string GetRepository()
        {
            if (_filePath.Length == 0) throw (new System.ApplicationException("Repository is not define"));
            return (_filePath);
        }
        #endregion

        /// <summary>
        /// Obtient la source de données
        /// </summary>
        /// <returns>Source de données</returns>
        public object GetSource()
        {
            if (_source == null) throw (new NullReferenceException("La source de données est NULL"));
            return (_source);
        }

        /// <summary>
        /// Obtient le type de la source de données
        /// </summary>
        /// <returns>Type de la source de données</returns>
        //public DataSource.Type DataSourceType()
        //{
        //    return (_sourceType);
        //}

        /// <summary>
        /// Ouvre la source de données
        /// </summary>
        public void Open()
        {
            if (_source == null)
            {
                if (_filePath == null && _filePath.Length == 0) throw (new Exception("Invalid filePath to create stream"));
                if (!File.Exists(_filePath)) throw (new FileNotFoundException("Source does not exist"));
                _source = new XmlTextReader(_filePath);
            }
        }

        /// <summary>
        /// Ferme la source de données
        /// </summary>
        public void Close()
        {
            try
            {
                _source.Close();
                _source = null;
            }
            catch (System.Exception err)
            {
                throw (new XmlReaderDataSourceException("Impossible de fermer la source de données", err));
            }
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="command">Ligne de commande</param>
        /// <returns>Resultat</returns>
        //public DataSet Fill(string command)
        //{
        //    throw (new NotImplementedException("Non implémentée"));
        //}

        ///// <summary>
        ///// Modifie un élément
        ///// </summary>
        ///// <param name="command">Ligne de commande</param>
        //public void Update(string command)
        //{
        //    throw (new NotImplementedException("Not implemented"));
        //}
        ///// <summary>
        ///// Ajoute un élément
        ///// </summary>
        ///// <param name="command">Ligne de commande</param>
        //public void Insert(string command)
        //{
        //    throw (new NotImplementedException("Not implemented"));
        //}
        ///// <summary>
        ///// Supprime un élément
        ///// </summary>
        ///// <param name="command">Ligne de commande</param>
        //public void Delete(string command)
        //{
        //    throw (new NotImplementedException("Not implemented"));
        //}
        #endregion

    }

    public class XmlReaderDataSourceException : Exception
    {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public XmlReaderDataSourceException() : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public XmlReaderDataSourceException(string message) : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public XmlReaderDataSourceException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
        #endregion

    }
}
