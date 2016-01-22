using Aspose.Slides;
using Aspose.Slides.Export;
using LinkSystem.LinkKernel.Core;
using LinkSystem.LinkKernel.CoreMonitor;
using LinkSystem.LinkKernel.Enums;
using LinkSystem.LinkMonitor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KM.AdExpress.PPTX
{
    public partial class Engine
    {
        private void DoGraphicKeyReports(TaskExecution taskExecution, CancellationToken cancellationToken,
                                         ManualResetEventSlim pausEvent, string userToken)
        {
            var taskLogHeader = string.Format("The DoGraphicKeyReports task '{0}' t=[{1}]", taskExecution.Name, userToken);
            AdvancedTrace.TraceInformation(string.Format("{0} is running ...", taskLogHeader), ModuleName);

            try
            {
                if (UseImpersonate)
                {
                    OpenImpersonation();
                }

                //Instantiate Prseetation class that represents the PPTX
                using (Presentation pres = new Presentation())
                {
                    string dataDir =  AppDomain.CurrentDomain.BaseDirectory;

                    //Get the first slide
                    ISlide sld = pres.Slides[0];

                    //Instantiate the ImageEx class
                    System.Drawing.Image img = (System.Drawing.Image)new Bitmap(dataDir + "tmpF095.tmp");
                    IPPImage imgx = pres.Images.AddImage(img);

                    //Add Picture Frame with height and width equivalent of Picture
                    sld.Shapes.AddPictureFrame(ShapeType.Rectangle, 50, 150, imgx.Width, imgx.Height, imgx);
                    
                    //Write the PPTX file to disk
                    pres.Save(dataDir + "RectPicFrame.pptx", SaveFormat.Pptx);
                }

                ReleaseTask(taskExecution);
            }
            catch (Exception exception)
            {
                ReleaseTask(taskExecution,
                          new LogLine("An error occured during  Graphic KeyReports powerpoint treatment.", exception,
                                      eLogCategories.Fatal));

            }
            finally
            {
                if (UseImpersonate && _impersonateInformation != null) CloseImpersonation();
                AdvancedTrace.TraceInformation(string.Format("{0} is done.", taskLogHeader),
                                             ModuleName);
            }

        }
    }
}
