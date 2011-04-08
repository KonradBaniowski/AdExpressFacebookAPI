using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TNS.AdExpress.Constantes.Web;
using System.IO;
using TNS.AdExpress.WebService.Domain;
using TNS.AdExpress.WebService.Domain.Configuration;
using System.Drawing;

namespace WebServiceCreativeView {
    /// <summary>
    /// Description résumée de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    // [System.Web.Script.Services.ScriptService]
    public class CreativeView : System.Web.Services.WebService {

        #region Get Binaries
        /// <summary>
        /// Get Binaries of the creative
        /// </summary>
        /// <param name="relativePath">Relative path file</param>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="isBlur">Return creative real (if false) or blur (if true)</param>
        /// <returns>Binaries of the creative</returns>
        [WebMethod]
        public byte[] GetBinaries(string relativePath, Int64 idVehicle, bool isBlur) {
            VehicleCreativesInformation vehicleCreativesInformation = VehiclesCreativesInformation.GetVehicleCreativesInformation(idVehicle);
            if (vehicleCreativesInformation != null) {
                vehicleCreativesInformation.Open();
                try {
                    if (File.Exists(Path.Combine(vehicleCreativesInformation.CreativeInfo.Path, relativePath))) {
                        byte[] imageBytes = null;
                        string pathFile = Path.Combine(CreationServerPathes.LOCAL_PATH_IMAGE, relativePath);
                        if (isBlur) {
                            MemoryStream fs = null;
                            BinaryReader br = null;
                            try {
                                Bitmap bitmap = new Bitmap(pathFile);
                                Bitmap bitmapBlur = Utilities.Media.Image.Image.GaussianBlur(bitmap, 3);

                                fs = new MemoryStream();
                                bitmapBlur.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                                imageBytes = fs.ToArray();
                            }
                            finally {
                                if (br != null) br.Close();
                                if (fs != null) fs.Close();
                            }
                        }
                        else {
                            imageBytes = File.ReadAllBytes(pathFile);
                        }
                        return imageBytes;

                    }
                    else
                        return null;
                }
                finally {
                    vehicleCreativesInformation.Close();
                }
            }
            else
                return null;
        }
        #endregion
    }
}
