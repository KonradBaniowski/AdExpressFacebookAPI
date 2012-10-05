using System;
using System.Collections.Generic;

namespace TNS.AdExpress.Rolex.Loader.Domain
{
    public class RolexDetail
    {
        #region Variables
        /// <summary>
        /// ID site
        /// </summary>
        private long _idSite;

        /// <summary>
        /// ID location
        /// </summary>
        private long _idLocation;


        /// <summary>
        /// ID Type presences 
        /// </summary>
        private List<long> _idTypePresences;

        /// <summary>
        /// Date begin
        /// </summary>
        private DateTime _dateBegin;

        /// <summary>
        /// Date End
        /// </summary>
        private DateTime _dateEnd;

     
             
        /// <summary>
        /// promotion visual List
        /// </summary>
        private List<string> _visuals;

        /// <summary>
        /// Commentary
        /// </summary>
        private readonly string _commentary;

        #endregion


        public RolexDetail(long idSite, long idLocation, List<long> idTypePresences, DateTime dateBegin, DateTime dateEnd, List<string> visuals, string commentary)
        {
            if (idSite < 1) throw new ArgumentException(" Parameter idSite  is invalid");
            _idSite = idSite;
            if (idLocation < 1) throw new ArgumentException(" Parameter idLocation  is invalid");
            _idLocation = idLocation;
            _dateBegin = dateBegin;
            _dateEnd = dateEnd;
            if (idTypePresences == null || idTypePresences.Count == 0) throw new ArgumentException(" Parameter idTypePresences is invalid");
            _idTypePresences = idTypePresences;
           
            if (visuals == null || visuals.Count == 0) throw new ArgumentNullException("Parameter visuals cannot be null or empty");
            _visuals = visuals;
            _commentary = commentary;
        }

        /// <summary>
        /// Commentary
        /// </summary>
        public string Commentary
        {
            get { return _commentary; }
        }

        /// <summary>
        /// ID media
        /// </summary>
        public long IdSite
        {
            get { return _idSite; }
        }

        /// <summary>
        /// ID location
        /// </summary>
        public long IdLocation
        {
            get { return _idLocation; }
        }

        /// <summary>
        /// ID Type presences
        /// </summary>
        public List<long> IdTypePresences
        {
            get { return _idTypePresences; }
        }

        /// <summary>
        /// Date begin
        /// </summary>
        public DateTime DateBegin
        {
            get { return _dateBegin; }
        }

        /// <summary>
        /// Date End
        /// </summary>
        public DateTime DateEnd
        {
            get { return _dateEnd; }
        }

      

        /// <summary>
        /// promotion visual List
        /// </summary>
        public List<string> Visuals
        {
            get { return _visuals; }
        }
    }
}
