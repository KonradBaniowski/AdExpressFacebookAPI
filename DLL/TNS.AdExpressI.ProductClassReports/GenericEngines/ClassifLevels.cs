#region Information
/*
 * Author : G Ragneau
 * Created on : 15/01/2009
 * Modified:
 *      Date - Author - Description
 * 
 * 
 * 
 * 
 * 
 * 
 * */
#endregion


using System;
using System.Collections;

using TNS.AdExpress.Web.Core.Exceptions;
using System.Collections.Generic;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines{
	/// <summary>
    /// 
	/// </summary>
	public class ClassifLevels<T>:Dictionary<Int64, ClassifLevels<T>>
    {

        #region Variables
        /// <summary>
        /// Current Level
        /// </summary>
        T _value;
        /// <summary>
        /// Sub levels of the current level
        /// </summary>
        ClassifLevels<T> _subLevels = null;
        #endregion

        #region Accessors
        public T Value
        {
            get { return _value; }
        }
        public ClassifLevels<T> SubLevels
        {
            get { return _subLevels; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public ClassifLevels(): base()
        {
        }
        /// <summary>
		/// Constructeur de base
		/// </summary>
		protected ClassifLevels(T value):base(){
            _value = value;
		}
		#endregion

        #region Add a level to the classification
        public void Add(List<Int64> keys, T value)
        {
            Add(keys, value, 0);
        }

        protected void Add(List<Int64> keys, T value, int index)
        {
            if (index == (keys.Count - 1))
            {
                this.Add(keys[index], new ClassifLevels<T>(value));
            }
            else
            {
                this[keys[index]].Add(keys, value, index + 1);
            }
        }
        #endregion

        #region Add a sub level to the classification
        public void AddSubLevel(List<Int64> mainKeys, List<Int64> subKeys, T value)
        {
            AddSubLevel(mainKeys, subKeys, value, 0);
        }

        protected void AddSubLevel(List<Int64> mainKeys, List<Int64> subKeys, T value, int index)
        {
            if (index == mainKeys.Count)
            {
                if (_subLevels == null)
                {
                    _subLevels = new ClassifLevels<T>();
                }
                _subLevels.Add(subKeys, value);
            }
            else
            {
                this[mainKeys[index]].AddSubLevel(mainKeys, subKeys, value, index + 1);
            }
        }
        #endregion

        #region MyContains
        public bool MyContains(List<Int64> keys)
        {
            return MyContains(keys, 0);
        }

        protected bool MyContains(List<Int64> keys, int index)
        {
            if (index == (keys.Count - 1))
            {
                return this.ContainsKey(keys[index]);
            }
            else
            {
                if (this.ContainsKey(keys[index]))
                {
                    return this[keys[index]].MyContains(keys, index + 1);
                }
                
            }
            return false;
        }
        #endregion

        #region Exist Sub Level
        public bool MyContainsSubLevel(List<Int64> mainKeys, List<Int64> subKeys)
        {

            return MyContainsSubLevel(mainKeys, subKeys, 0);

        }

        protected bool MyContainsSubLevel(List<Int64> mainKeys, List<Int64> subKeys, int index)
        {

            if (index == (mainKeys.Count - 1))
            {
                if (this.ContainsKey(mainKeys[index]))
                {
                    return this[mainKeys[index]]._subLevels != null && this[mainKeys[index]]._subLevels.MyContains(subKeys, 0);
                }
            }
            else
            {
                if (this.ContainsKey(mainKeys[index]))
                {
                    return this[mainKeys[index]].MyContainsSubLevel(mainKeys, subKeys, index + 1);
                }

            }

            return false;
        }
        #endregion

        #region Get Value
        public T GetValue(List<Int64> keys)
        {
            return GetValue(keys, 0);
        }

        protected T GetValue(List<Int64> keys, int index)
        {
            if (index == (keys.Count - 1))
            {
                if (this.ContainsKey(keys[index]))
                {
                    return this[keys[index]]._value;
                }
            }
            else
            {
                if (this.ContainsKey(keys[index]))
                {
                    return this[keys[index]].GetValue(keys, index + 1);
                }

            }
            return default(T);
        }
        #endregion

        #region Get Sub Value
        public T GetSubValue(List<Int64> mainKeys, List<Int64> subKeys)
        {

            return GetSubValue(mainKeys, subKeys, 0);

        }

        protected T GetSubValue(List<Int64> mainKeys, List<Int64> subKeys, int index)
        {

            if (index == (mainKeys.Count - 1))
            {
                if (this.ContainsKey(mainKeys[index]) && this[mainKeys[index]]._subLevels != null)
                {
                    return this[mainKeys[index]]._subLevels.GetValue(subKeys, 0);
                }
            }
            else
            {
                if (this.ContainsKey(mainKeys[index]))
                {
                    return this[mainKeys[index]].GetSubValue(mainKeys ,subKeys, index + 1);
                }

            }

            return default(T);
        }
        #endregion

    }

}
