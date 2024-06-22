using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using System.Windows.Xps.Packaging;

namespace Trax.kkrb
{

    internal class Program
    {

        string filePath = @"C:\scripts\";
        string inFile = "";
        string outFile = "";
        string script = "";
        DateTime logDate = DateTime.Now;
        List<string> Tracks = new List<string>();
        string tag2 = "The City of Sunshine's official radio station: Sunny 107; ";
        string tag1 = "Today's hits. Yesterday's favorites. Sunny 107; ";
        string tag3 = "Sunny 107: the Basin's soft rock station; ";

        [STAThread]
        private static void Main(string[] args)
        {
            Program VoiceTrack = new Program();
            VoiceTrack.RunMe();
        }

        private void RunMe()
        {
            bool keepTrying = true;
            string userResponse = "";
            string exportedText = "";

            while (keepTrying)
            {
                keepTrying = false;
                userResponse = GetLogDate();
                if (!DateTime.TryParse(userResponse, out logDate))
                {
                    keepTrying = true;
                    _ = GetLogDate();
                }
                else
                {
                    keepTrying = false;
                    inFile = filePath + "KKRBLog_" + logDate.ToString("M-d-yy") + ".xps";
                    outFile = filePath + "KKRBLog_" + logDate.ToString("M-d-yy") + ".txt";
                    script = filePath + "_ScriptLog_" + logDate.ToString("M-d-yy") + ".txt";
                }

            }
            exportedText = ConvertToText(inFile);
            // Console.WriteLine(exportedText);
            ProcessText(exportedText);
        }

        private string GetLogDate()
        {
            Console.WriteLine("Log date = ");
            return Console.ReadLine();
        }

        [STAThread]
        private string ConvertToText(string fileName)
        {
            string fullPageText = "";
            decimal originY = 0;
            try
            {
                XpsDocument xpsDocument = new XpsDocument(fileName, System.IO.FileAccess.Read);
                IXpsFixedDocumentSequenceReader fixedDocSeqReader = xpsDocument.FixedDocumentSequenceReader;
                IXpsFixedDocumentReader document = fixedDocSeqReader.FixedDocuments[0];
                FixedDocumentSequence sequence = xpsDocument.GetFixedDocumentSequence();

                for (int pageCount = 0; pageCount < sequence.DocumentPaginator.PageCount; ++pageCount)
                {
                    IXpsFixedPageReader _page = document.FixedPages[pageCount];
                    StringBuilder currentText = new StringBuilder();
                    System.Xml.XmlReader pageContentReader = _page.XmlReader;

                    if (pageContentReader != null)
                    {
                        while (pageContentReader.Read())
                        {
                            if (pageContentReader.Name == "Glyphs")
                            {
                                if (pageContentReader.HasAttributes)
                                {
                                    if (pageContentReader.GetAttribute("UnicodeString") != null &&
                                       !pageContentReader.GetAttribute("UnicodeString").Contains("Input File") &&
                                       !pageContentReader.GetAttribute("UnicodeString").Contains("KKRB FM") &&
                                       !pageContentReader.GetAttribute("UnicodeString").Contains("Legal ID"))
                                    {
                                        if (decimal.Parse(pageContentReader.GetAttribute("OriginY")) != originY)
                                        {
                                            currentText.AppendLine();
                                            originY = decimal.Parse(pageContentReader.GetAttribute("OriginY")); ;
                                        }
                                        currentText.Append(pageContentReader.GetAttribute("UnicodeString"));
                                        currentText.Append("|");
                                    }
                                }
                            }
                        }
                    }

                    fullPageText += currentText.ToString();
                }
                xpsDocument.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("something went wrong.");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            File.WriteAllText(outFile, fullPageText);
            return fullPageText;
        }

        public void ProcessText(string convertedText)
        {

            int iHour = 0;
            int iWheelPos = 0;
            int iMinutePos = 0;
            Random rando = new Random();
            string txtFile = outFile;
            StreamWriter writer = new StreamWriter(script, false);
            writer.WriteLine($"KKRB Script for {logDate.ToString("ddd")}, {logDate.ToString("M-d-yy")}");
            string whichTag = "";

            // th
            if (File.Exists(txtFile))
            {
                // Store each line in array of strings 
                string[] lines = File.ReadAllLines(txtFile);
                foreach (string line in lines)
                {
                    iHour = GetHour(line);
                    if ((iHour >= 6 && iHour < 18) &&
                        !line.Contains("SWEEPER") &&
                        !line.Contains("Artist") &&
                        !line.Contains("WX") &&
                        !line.Contains("|NEW|DA1000|"))
                    {
                        string[] fields = line.Split('|');
                        if (line.Contains("DA1000"))
                        {
                            Console.WriteLine($"");
                            Console.WriteLine($"***************** {fields[0]} HOUR *****************");
                        }
                        // get voice track (jv0608, jn...//
                        else if (line.ToLower().Contains("voice track") ||
                                 line.ToLower().Contains("voicetrack"))
                        {
                            iWheelPos = GetWheelPosition(fields[3]);
                            iMinutePos = GetMinutePosition(fields[3]);
                            Console.WriteLine($"");
                            Console.WriteLine($"{fields[3]}");
                            Console.WriteLine($"----------------");
                            if (iMinutePos != 20)
                            {
                                int randoNum = rando.Next();
                                if (randoNum % 3 == 0)
                                {
                                    // "Today's hits. Yesterday's favorites. Sunny 107; "
                                    whichTag = tag1;
                                }
                                else if (randoNum % 2 == 0)
                                {
                                    // "The City of Sunshine's official radio station: Sunny 107; "
                                    whichTag = tag2;
                                }
                                else
                                {
                                    // "Sunny 107: the Basin's soft rock station; "
                                    whichTag = tag3;
                                }
                                Console.WriteLine(whichTag);
                            }
                            else
                            {
                                Console.WriteLine(":20 BREAK TAGLINE HERE ");
                            }
                            Console.WriteLine();

                        } // line contains voicetrack or voice track
                        else
                        {
                            foreach (string field in fields)
                            {
                                Console.Write($"{field} ");
                            }
                            Console.WriteLine() ;
                        }
                    }
                }
                writer.Flush();
                writer.Close();
                writer.Dispose();
                // Console.ReadLine();
                /*
                            if (File.Exists(outFile))
                            {
                                File.Delete(outFile);
                            }
                */
                /*            if (File.Exists(inFile))
                            {
                                File.Delete(inFile);
                            }
                */
            }
        } // ProcessText

        private int GetMinutePosition(string vtfield)
        {
            int minutePos = 0;
            string subst = "";
            if (vtfield != null &&
                vtfield.Length > 0)
            {
                subst = vtfield.Substring(vtfield.Length - 2, 2);
            }

            if (int.TryParse(subst, out minutePos))
            {
                return minutePos;
            }
            return 0;
        } // GetMinutePosition

        private int GetWheelPosition(string vtfield)
        {
            int wheelPos = 0;
            string subst = "";
            if (vtfield != null &&
                vtfield.Length > 0)
            {
                subst = vtfield.Substring(vtfield.Length - 4, 4);
            }

            if (int.TryParse(subst, out wheelPos))
            {
                return wheelPos;
            }
            return 0;
        } // GetWheelPosition

        private int GetHour(string line)
        {
            int hour = 0;
            string subst = "";
            if (line != null &&
                line.Length > 0)
            {
                subst = line.Substring(0, 2);
            }

            if (int.TryParse(subst, out hour))
            {
                return hour;
            }
            return 0;
        } // GetHour
    } // class
} // namespace
