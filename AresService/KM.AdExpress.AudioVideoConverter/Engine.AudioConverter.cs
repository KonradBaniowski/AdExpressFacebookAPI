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
        private void DoAudioConversion(TaskExecution taskExecution, CancellationToken cancellationToken,
                                        ManualResetEventSlim pausEvent, string userToken)
        {
            var taskLogHeader = string.Format("The DoAudioConversion task '{0}' t=[{1}]", taskExecution.Name, userToken);
            string arguments = "  -vn -ar 44100 -ac 2 -ab 192 -f mp3 ";


            AdvancedTrace.TraceInformation(string.Format("{0} is running ...", taskLogHeader), ModuleName);
            try
            {
                if (UseImpersonate)
                {
                    OpenImpersonation();
                }
                const long ID_VEHICLE_RADIO = 2;
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

                    
                    var creatives = dal.GetCreative(db, dateCreationBeginning, dateCreationEnd, ID_VEHICLE_RADIO);
                    return;
                    ReportGenerator reportGenerator = new ReportGenerator(dateCreationBeginning, dateCreationEnd,  "RADIO");

                    string temPath = Path.Combine(Path.GetTempPath(), string.Format(@"AdExRadioCreative_{0}\", DateTime.Now.ToString("yyyyMMdd")));
                   

                    if (creatives.Any() && !Directory.Exists(temPath))
                    {
                        Directory.CreateDirectory(temPath);
                        AdvancedTrace.TraceInformation(string.Format("{0} Radio files to convert ...", creatives.Count()), ModuleName);
                    }

                    reportGenerator.NbAllFiles = creatives.Count();


                    List<string> notExistFilesSource = new List<string>();
                    List<string> filesNotConverted = new List<string>();
                    List<string> filesExisting = new List<string>();


                    creatives.ForEach(p =>
                    {
                        if (!string.IsNullOrEmpty(p.FileName) && p.IdMultimedia > 0)
                        {
                            AdvancedTrace.TraceInformation(string.Format("Debut  Traitement de la Version {0} et de son fichier source {1} ...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                            string cFullSourceFilename = string.Format("{0}.wma", Path.Combine(p.FilePath, p.FileName));
                            string cFullDestinationFileName = Path.Combine(RadioDestinationPath, string.Format(@"{0}\MP3\", p.MediaYear.ToString()));
                            if (!Directory.Exists(cFullDestinationFileName))
                            {
                                Directory.CreateDirectory(cFullDestinationFileName);
                            }
                            cFullDestinationFileName = string.Format("{0}.mp3", Path.Combine(cFullDestinationFileName, string.Format("2{0}", p.IdMultimedia.ToString())));

                            bool isFileSourceExist = true;

                            if (!File.Exists(cFullSourceFilename))
                            {
                                isFileSourceExist = false;
                                notExistFilesSource.Add(string.Format("{0};{1}", p.IdMultimedia, cFullSourceFilename));

                            };

                            bool isExistingDestFile = false;
                            if (File.Exists(cFullDestinationFileName))
                            {
                                isExistingDestFile = true;
                                filesExisting.Add(string.Format("{0};{1}", p.IdMultimedia, cFullSourceFilename));

                            };

                            if (isFileSourceExist && !isExistingDestFile)
                            {
                                string tempWmaFile = string.Format("{0}.wma", Path.Combine(temPath, p.FileName));
                                string tempMp3File = string.Format("{0}.mp3", Path.Combine(temPath, string.Format("2{0}", p.IdMultimedia.ToString())));

                                File.Copy(cFullSourceFilename, tempWmaFile, true);

                                var ffmpegLauncher = new FfmpegLauncher(arguments);
                                bool isConverted = ffmpegLauncher.Convert(tempWmaFile, tempMp3File);

                                if (isConverted)
                                {
                                    File.Copy(tempMp3File, cFullDestinationFileName, true);
                                    File.Delete(tempMp3File);
                                    File.Delete(tempWmaFile);
                                    reportGenerator.NbFileConverted++;
                                    AdvancedTrace.TraceInformation(string.Format("La Version {0} et  son fichier source {1}  converti en MP3 avec succès...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                }
                                else
                                {
                                    AdvancedTrace.TraceInformation(string.Format("Echec conversion duc fichier {1} de la Version {0}  en MP3 ...", p.IdMultimedia.ToString(), p.FileName), ModuleName);
                                    filesNotConverted.Add(string.Format("{0};{1};{2};{3}<br/>", p.IdMultimedia, p.MediaYear, p.FileName, p.FilePath));
                                }
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
                        var path = Path.Combine(dirNotExitFileSource, string.Format("RADIO_NotExitFileSource_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                        File.AppendAllLines(path, notExistFilesSource, Encoding.ASCII);
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
                        var path = Path.Combine(dirFilesNotConverted, string.Format("RADIO_FilesNotConverted_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                        File.AppendAllLines(path, filesNotConverted, Encoding.ASCII);
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
                        var path = Path.Combine(dirFilesExisting, string.Format("RADIO_FilesDestinationExisting_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                        File.AppendAllLines(path, filesExisting, Encoding.ASCII);
                        reportGenerator.NbExistingDestinationFiles = filesExisting.Count();
                    }    

                    if (Directory.Exists(temPath)) Directory.Delete(temPath, true);
                    filesNotConverted = null;
                    notExistFilesSource = null;
                    filesExisting = null;
                    AdvancedTrace.TraceInformation(" End of Radio treatment ...", ModuleName);
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
