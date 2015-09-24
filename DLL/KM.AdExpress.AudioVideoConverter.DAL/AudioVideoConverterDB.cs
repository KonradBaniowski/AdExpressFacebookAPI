using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.DAL
{
 

  public class AudioVideoConverterDB : DbManager
  {
      public AudioVideoConverterDB(String connexionString, DataProviderBase provider)
          : base(provider, connexionString)
      {
      }
  }
}
