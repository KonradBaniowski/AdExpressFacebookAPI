using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.DAL
{
  public  interface IAudioVideoConvertDbConnectionFactory
    {
      AudioVideoConverterDB CreateDbManager();
    }
}
