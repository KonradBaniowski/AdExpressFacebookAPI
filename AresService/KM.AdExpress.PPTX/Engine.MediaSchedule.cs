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
using System.Xml.Linq;

namespace KM.AdExpress.PPTX
{
  public partial class Engine
    {
      private void DoMediaSchedule(TaskExecution taskExecution, CancellationToken cancellationToken,
                                       ManualResetEventSlim pausEvent, string userToken)
      {
          var taskLogHeader = string.Format("The DoMediaSchedule task '{0}' t=[{1}]", taskExecution.Name, userToken);
          AdvancedTrace.TraceInformation(string.Format("{0} is running ...", taskLogHeader), ModuleName);

          try
          {
              if (UseImpersonate)
              {
                  OpenImpersonation();
              }
              float x = 10;
              float y = 50;
              string imgFileName = string.Empty;
              string pptxFileName = string.Empty;
              int slideIndex = 0;
              long idStaticNavSession;

              var xTask = XElement.Parse(taskExecution.XMLTask);
              if (xTask.Attributes().Any(p => p.Name == "parameter_id"))
              {
                  idStaticNavSession = (long)xTask.Attribute("parameter_id");
              }

              //Instantiate Prseetation class that represents the PPTX
              using (Presentation pres = new Presentation())
              {
                  string dataDir = AppDomain.CurrentDomain.BaseDirectory;
                  imgFileName = dataDir + "Photoshop_Image_of_the_horse_053857_.jpg";
                  pptxFileName = dataDir + "MediaSchedule.pptx";

                  AddImageToSlide(x, y, imgFileName, slideIndex, pres);
                
                  //Write the PPTX file to disk
                  pres.Save(pptxFileName, SaveFormat.Pptx);
              }

              ReleaseTask(taskExecution);
          }
          catch (Exception exception)
          {
              ReleaseTask(taskExecution,
                        new LogLine("An error occured during  Media Schedule powerpoint treatment.", exception,
                                    eLogCategories.Fatal));

          }
          finally
          {
              if (UseImpersonate && _impersonateInformation != null) CloseImpersonation();
              AdvancedTrace.TraceInformation(string.Format("{0} is done.", taskLogHeader),
                                           ModuleName);
          }
      }

      private static void AddImageToSlide(float x, float y, string imgFileName, int slideIndex, Presentation pres)
      {

          //Get the slide
          ISlide sld = pres.Slides[slideIndex];

          //Instantiate the ImageEx class
          System.Drawing.Image img = (System.Drawing.Image)new Bitmap(imgFileName);
          IPPImage imgx = pres.Images.AddImage(img);

          //Add Picture Frame with height and width equivalent of Picture
          IPictureFrame pf = sld.Shapes.AddPictureFrame(ShapeType.Rectangle, x, y, imgx.Width, imgx.Height, imgx);

          //Setting relative scale width and height
          float w = (float)pres.SlideSize.Size.Width / (float)imgx.Width;
          var coef = Math.Min((float)1.0, w);
          w = (float)pres.SlideSize.Size.Height / (float)imgx.Height;
          coef = Math.Min((float)1.0, w);
          pf.RelativeScaleHeight = coef;
          pf.RelativeScaleWidth = coef;
      }
    }
}
