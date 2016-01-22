using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using KM.AdExpress.BlurPressCreative.Domain.Types;
using KM.AdExpress.BlurPressCreative.Domain.XmlLoaders;
using NUnit.Framework;

namespace KM.AdExpress.BlurPressCreative.Test
{
    public class BlurPressCreativeTest
    {
        private const string MediaInformationFileName = "MediasInformation.xml";
        private List<MediaInformation> _mediaInformations;
        private const string ConfigurationDirectoryName = "Configuration";
        readonly string _configurationDirectoryRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationDirectoryName);
        private const string _sourceDirectory = @"F:\SCANS";
        private const string _destinationDirectory = @"F:\ResBlur";
        private const string ThumbnailDirectoryName = @"imagette";
        string _nConveterProcessPath = @"C:\XnView\nconvert.exe";


        [SetUp]
        public void Setup()
        {

            _mediaInformations = MediaInformationLoader.Load(Path.Combine(_configurationDirectoryRoot, MediaInformationFileName));
        }

        [Test]
        public void BlurCreativeTest()
        {
            long idMedia = 1300;
            var date = new DateTime(2015, 01, 01);

            string outputFiles = string.Format("{0}\\{1}\\{2}\\blur\\%.jpg", _destinationDirectory, idMedia.ToString(), date.ToString("yyyyMMdd"));
            string inputFiles = string.Format("{0}\\{1}\\{2}\\*.jpg", _sourceDirectory, idMedia.ToString(), date.ToString("yyyyMMdd"));
            string outputFilesThumbnails = string.Format("{0}\\{1}\\{2}\\{3}\\blur\\%.jpg", _destinationDirectory, idMedia.ToString(), date.ToString("yyyyMMdd"), ThumbnailDirectoryName);
            string inputFilesThumbnails = string.Format("{0}\\{1}\\{2}\\{3}\\*.jpg", _sourceDirectory, idMedia.ToString(), date.ToString("yyyyMMdd"), ThumbnailDirectoryName);


            string cParams = string.Format(" -average 13 -text_pos 50 50 -text_font arial 26 -text_color 255 255 255 -text_back 255 0 0 -text \"TEXTE NON LISIBLE\" -o \"{0}\"  \"{1}\" ", outputFiles, inputFiles);
            string cParamsThumbnails = string.Format(" -average 13 -text_pos 30 30 -text_font arial 16 -text_color 255 255 255 -text_back 255 0 0 -text \"TEXTE NON LISIBLE\" -o \"{0}\"  \"{1}\" ", outputFilesThumbnails, inputFilesThumbnails);

            //Buring press media files
            StartBluringProcess(_nConveterProcessPath, cParams);

            //Buring press media thumbnail
            StartBluringProcess(_nConveterProcessPath, cParamsThumbnails);
        }

        [Test]
        public void GetPressDirectoryToBlurTest()
        {
            _mediaInformations.ForEach(m =>
                {
                    string intputDir = string.Format("{0}\\{1}", _sourceDirectory, m.Id.ToString());

                    var inputDatesPathes = Directory.EnumerateDirectories(intputDir).Select(Path.GetFileName).Select(p => Convert.ToInt64(p)).Where( p => p >  Convert.ToInt64(m.LatestBlurDate)).ToList();
                    Assert.True(inputDatesPathes.Any());
                });
        }

        [Test]
        public void BlurAllPressDirectoryTest()
        {
            _mediaInformations.ForEach(m =>
            {
                string intputDir = string.Format("{0}\\{1}", _sourceDirectory, m.Id.ToString());

                var inputDatesPathes = Directory.EnumerateDirectories(intputDir).Select(Path.GetFileName).Select(p => Convert.ToInt64(p)).Where(p => p > Convert.ToInt64(m.LatestBlurDate)).ToList();
                inputDatesPathes.ForEach( p =>
                    {

                        string outputFiles = string.Format("{0}\\{1}\\{2}\\blur\\%.jpg", 
                            _destinationDirectory, m.Id.ToString(), p);
                        string inputFiles = string.Format("{0}\\{1}\\{2}\\*.jpg",
                            _sourceDirectory, m.Id.ToString(), p);
                        string outputFilesThumbnails = string.Format("{0}\\{1}\\{2}\\{3}\\blur\\%.jpg",
                            _destinationDirectory, m.Id.ToString(), p, ThumbnailDirectoryName);
                        string inputFilesThumbnails = string.Format("{0}\\{1}\\{2}\\{3}\\*.jpg",
                            _sourceDirectory, m.Id.ToString(), p, ThumbnailDirectoryName);

                        string cParams = string.Format(" -average 13 -text_pos 50 50 -text_font arial 26 -text_color 255 255 255 -text_back 255 0 0 -text \"TEXTE NON LISIBLE\" -o \"{0}\"  \"{1}\" "
                            , outputFiles, inputFiles);
                        string cParamsThumbnails = string.Format(" -average 13 -text_pos 30 30 -text_font arial 16 -text_color 255 255 255 -text_back 255 0 0 -text \"TEXTE NON LISIBLE\" -o \"{0}\"  \"{1}\" "
                            , outputFilesThumbnails, inputFilesThumbnails);

                        //Buring press media pages
                        StartBluringProcess(_nConveterProcessPath, cParams);

                        //Buring press media thumbnail
                        StartBluringProcess(_nConveterProcessPath, cParamsThumbnails);

                        //Update latest blur date in XML file
                        XDocument xdoc = XDocument.Load(Path.Combine(_configurationDirectoryRoot, MediaInformationFileName));
                        UpdateLatestBlurDate(xdoc, m.Id.ToString(), p.ToString());
                       
                        
                    });
            });
        }
       
        [Test]
        public void UpdateLatestBlurDateTest()
        {
               XDocument xdoc = XDocument.Load(Path.Combine(_configurationDirectoryRoot, MediaInformationFileName));
            UpdateLatestBlurDate(xdoc, "1300", "20150201");
        }
        private void UpdateLatestBlurDate(XDocument xdoc, string id, string value)
        {
            var mediaInfo = xdoc.Descendants("media").SingleOrDefault(x => x.Attribute("id").Value == id);
            if (mediaInfo != null)
            {
                mediaInfo.SetAttributeValue("latestBlurDate", value);
                xdoc.Save(Path.Combine(_configurationDirectoryRoot, MediaInformationFileName));
            }
        }

        private static void StartBluringProcess(string nConveterProcessPath, string cParams)
        {
            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = nConveterProcessPath;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = cParams;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
