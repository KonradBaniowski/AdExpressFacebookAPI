using KM.AdExpress.AudioVideoConverter.DAL;
using KM.AdExpress.AudioVideoConverter.Tools;
using LinkSystem.LinkKernel.Core;
using LinkSystem.LinkKernel.CoreMonitor;
using LinkSystem.LinkKernel.Enums;
using LinkSystem.LinkMonitor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace KM.AdExpress.AudioVideoConverter
{
   public partial class Engine
    {
       private void DoVideoConversion(TaskExecution taskExecution, CancellationToken cancellationToken,
                                       ManualResetEventSlim pausEvent, string userToken)
       {
           var taskLogHeader = string.Format("The DoVideoConversion task '{0}' t=[{1}]", taskExecution.Name, userToken);
           string arguments = "  -acodec libfaac -aq 200 -vcodec libx264 -s {0} -aspect 16:9 ";


           AdvancedTrace.TraceInformation(string.Format("{0} is running ...", taskLogHeader), ModuleName);
           try
           {
               if (UseImpersonate)
               {
                   OpenImpersonation();
               }

               const long ID_VEHICLE_TV = 3;
               var xTask = XElement.Parse(taskExecution.XMLTask);
               var dal = new CreativeDAL(_connectionString, _providerDataAccess);
               using (var db = dal.AudioVideoConverterDB.CreateDbManager())
               {
                   DateTime dateCreationBeginning = DateTime.Now.AddDays(-1);
                   if (xTask.Attributes().Any(p => p.Name == "DATE_CREATION_BEGIN"))
                   {
                       dateCreationBeginning = DateTimeHelpers.YYYYMMDDToDateTime((string)xTask.Attribute("DATE_CREATION_BEGIN"));
                   }

                   DateTime dateCreationEnd = DateTime.Now;
                   if (xTask.Attributes().Any(p => p.Name == "DATE_CREATION_END"))
                   {
                       dateCreationEnd = DateTimeHelpers.YYYYMMDDToDateTime((string)xTask.Attribute("DATE_CREATION_END"));
                   }



                   var creatives = dal.GetCreative(db, dateCreationBeginning, dateCreationEnd, ID_VEHICLE_TV);
                  

                   string temPath = Path.Combine(Path.GetTempPath(), string.Format(@"AdExTvCreative_{0}\", DateTime.Now.ToString("yyyyMMdd")));

                   if (creatives.Any() && !Directory.Exists(temPath))
                   {
                       Directory.CreateDirectory(temPath);
                       AdvancedTrace.TraceInformation(string.Format("{0} television files to convert ...", creatives.Count()), ModuleName);
                   }
                   ReportGenerator reportGenerator = new ReportGenerator(dateCreationBeginning, dateCreationEnd,  "TELEVISION");
                   reportGenerator.NbAllFiles = creatives.Count();

                   List<string> notExistFilesSource = new List<string>();
                   List<string> filesNotConverted = new List<string>();
                   List<string> filesExisting = new List<string>();

                   creatives.ForEach(p =>
                   {
                       if (!string.IsNullOrEmpty(p.FileName) && p.IdMultimedia > 0)
                       {
                           AdvancedTrace.TraceInformation(string.Format("Debut  Traitement de la Version {0} et de son fichier source {1} ...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                           string cFullSourceFilename = string.Format("{0}.avi", Path.Combine(p.FilePath, p.FileName));
                           string cFullDestinationFileName = Path.Combine(TvDestinationPath, string.Format(@"{0}\", p.MediaYear.ToString()));

                           bool isFileSourceExist = true;

                           if (!File.Exists(cFullSourceFilename))
                           {
                               isFileSourceExist = false;
                               notExistFilesSource.Add(string.Format("{0};{1}", p.IdMultimedia, cFullSourceFilename));

                           };

                           if (!Directory.Exists(cFullDestinationFileName))
                               Directory.CreateDirectory(cFullDestinationFileName);
                           
                           if (!Directory.Exists(string.Format(@"{0}\360", cFullDestinationFileName)))                          
                               Directory.CreateDirectory(string.Format(@"{0}\360", cFullDestinationFileName));
                          
                           if (!Directory.Exists(string.Format(@"{0}\240", cFullDestinationFileName)))                          
                               Directory.CreateDirectory(string.Format(@"{0}\240", cFullDestinationFileName));
                           
                           string cHrFullDestinationFileName = string.Format("{0}.mp4", Path.Combine(cFullDestinationFileName, string.Format("3{0}",p.IdMultimedia.ToString())));                         
                           string cMrFullDestinationFileName = string.Format("{0}.mp4", Path.Combine(cFullDestinationFileName, string.Format(@"360\3{0}", p.IdMultimedia.ToString())));
                           string cBrFullDestinationFileName = string.Format("{0}.mp4", Path.Combine(cFullDestinationFileName, string.Format(@"240\3{0}", p.IdMultimedia.ToString())));

                           bool isExistingDestFile = false;
                           if (File.Exists(cHrFullDestinationFileName))
                           {
                               isExistingDestFile = true;
                               filesExisting.Add(string.Format("{0};{1}", p.IdMultimedia, cHrFullDestinationFileName));

                           };

                           if (isFileSourceExist && !isExistingDestFile)
                           {
                               string tempAviFile = string.Format("{0}.avi", Path.Combine(temPath, p.FileName));
                               string tempMp4File = string.Format("{0}.mp4", Path.Combine(temPath, string.Format("3{0}",p.IdMultimedia.ToString())));

                               File.Copy(cFullSourceFilename, tempAviFile, true);

                               //HR conversion
                               var ffmpegLauncher = new FfmpegLauncher( string.Format(arguments,VideoHrQuality));
                               bool isConverted = ffmpegLauncher.Convert(tempAviFile, tempMp4File);

                               if (isConverted)
                               {
                                   File.Copy(tempMp4File, cHrFullDestinationFileName, true);
                                   File.Delete(tempMp4File);                                 
                                   reportGenerator.NbFileConverted++;
                                   AdvancedTrace.TraceInformation(string.Format("La Version {0} et  son fichier source {1}  converti en MP4 et Haute resolution avec succès...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                               }
                               else
                               {
                                   AdvancedTrace.TraceInformation(string.Format("Echec conversion duc fichier {1} de la Version {0}  en MP4 et Haute resolution ...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                   filesNotConverted.Add(string.Format("{0};{1};{2};{3}<br/>", p.IdMultimedia, p.MediaYear, p.FileName, p.FilePath));
                               }

                               //MR conversion
                                ffmpegLauncher = new FfmpegLauncher(string.Format(arguments, VideoMrQuality));
                                isConverted = ffmpegLauncher.Convert(tempAviFile, tempMp4File);
                                if (isConverted)
                                {
                                    File.Copy(tempMp4File, cMrFullDestinationFileName, true);
                                    File.Delete(tempMp4File);
                                    reportGenerator.NbFileConverted++;
                                    AdvancedTrace.TraceInformation(string.Format("La Version {0} et  son fichier source {1}  converti en MP4 et Moyenne resolution avec succès...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                }
                                else
                                {
                                    AdvancedTrace.TraceInformation(string.Format("Echec conversion duc fichier {1} de la Version {0}  en MP4 et Moyenne resolution ...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                    filesNotConverted.Add(string.Format("{0};{1};{2};{3}<br/>", p.IdMultimedia, p.MediaYear, p.FileName, p.FilePath));
                                }

                                //BR conversion
                                ffmpegLauncher = new FfmpegLauncher(string.Format(arguments, VideoBrQuality));
                                isConverted = ffmpegLauncher.Convert(tempAviFile, tempMp4File);
                                if (isConverted)
                                {
                                    File.Copy(tempMp4File, cBrFullDestinationFileName, true);
                                    File.Delete(tempMp4File);
                                    reportGenerator.NbFileConverted++;
                                    AdvancedTrace.TraceInformation(string.Format("La Version {0} et  son fichier source {1}  converti en MP4 et Basse resolution avec succès...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                }
                                else
                                {
                                    AdvancedTrace.TraceInformation(string.Format("Echec conversion duc fichier {1} de la Version {0}  en MP4 et Basse resolution ...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                    filesNotConverted.Add(string.Format("{0};{1};{2};{3}<br/>", p.IdMultimedia, p.MediaYear, p.FileName, p.FilePath));
                                }

                               if(File.Exists(tempAviFile)) File.Delete(tempAviFile);
                              
                           }
                       }

                   });

                   //Logging Source files not  existing
                   if (notExistFilesSource.Any())
                   {
                       var dirNotExitFileSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\");
                       if (!Directory.Exists(dirNotExitFileSource))
                       {
                           Directory.CreateDirectory(dirNotExitFileSource);
                       }
                       var notExitFileSourcePath = Path.Combine(dirNotExitFileSource, string.Format("TV_NotExitFileSource_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                       File.AppendAllLines(notExitFileSourcePath, notExistFilesSource, Encoding.ASCII);
                       reportGenerator.NbFilesSourceNotExisting = notExistFilesSource.Count();
                   }


                   //Logging Nb files not converted
                   if (filesNotConverted.Any())
                   {
                       var dirFilesNotConverted = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\");
                       if (!Directory.Exists(dirFilesNotConverted))
                       {
                           Directory.CreateDirectory(dirFilesNotConverted);
                       }
                       var filesNotConvertedPath = Path.Combine(dirFilesNotConverted, string.Format("TV_FilesNotConverted_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                       File.AppendAllLines(filesNotConvertedPath, filesNotConverted, Encoding.ASCII);
                       reportGenerator.NbFileConverted = filesNotConverted.Count();
                   }

                   //Logging Nb existing destination file
                   if (filesExisting.Any())
                   {
                       var dirFilesExisting = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\");
                       if (!Directory.Exists(dirFilesExisting))
                       {
                           Directory.CreateDirectory(dirFilesExisting);
                       }
                       var path = Path.Combine(dirFilesExisting, string.Format("TV_FilesDestinationExisting_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                       File.AppendAllLines(path, filesExisting, Encoding.ASCII);
                       reportGenerator.NbExistingDestinationFiles = filesExisting.Count();
                   }   

                   if (Directory.Exists(temPath)) Directory.Delete(temPath, true);
                   filesNotConverted = null;
                   notExistFilesSource = null;
                   AdvancedTrace.TraceInformation(" End of Video treatment ...", ModuleName);
                   SendEmail(reportGenerator.GetHtml());
               }
               ReleaseTask(taskExecution);
           }
           catch (Exception exception)
           {
               ReleaseTask(taskExecution,
                         new LogLine("An error occured during Audio Conversion treatment.", exception,
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
