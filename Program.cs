using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Documents;
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
            Random rando = new Random();
            string txtFile = outFile;
            StreamWriter writer = new StreamWriter(script, false);
            writer.WriteLine($"KKRB Script for {logDate.ToString("ddd")}, {logDate.ToString("M-d-yy")}");
            string whichTag = "";
            
            // To read a text file line by line 
            if (File.Exists(txtFile))
            {
                // Store each line in array of strings 
                string[] lines = File.ReadAllLines(txtFile);
                foreach (string line in lines)
                {
                    string[] fields = line.Split('|');
                    if (fields.Length > 2 &&
                        !line.Contains("SWEEPER") &&
                        !line.Contains("Artist") &&
                        !line.Contains("WX") &&
                        fields[2] != "NEW" &&
                        (line.StartsWith("06:") ||
                        line.StartsWith("07:") ||
                        line.StartsWith("08:") ||
                        line.StartsWith("09:") ||
                        line.StartsWith("10:") ||
                        line.StartsWith("11:") ||
                        line.StartsWith("12:") ||
                        line.StartsWith("13:") ||
                        line.StartsWith("14:") ||
                        line.StartsWith("15:") ||
                        line.StartsWith("16:") ||
                        line.StartsWith("17:")))
                    {
                        if (line.Contains("DA1000"))
                        {
                            writer.WriteLine($"");
                            writer.WriteLine($"***************** {fields[0]} HOUR *****************");
                        }
                        else if (line.Contains("JV06") ||
                                 line.Contains("JV07") ||
                                 line.Contains("JV08") ||
                                 line.Contains("JV09") ||
                                 line.Contains("JV10") ||
                                 line.Contains("JV11") ||
                                 line.Contains("JV12") ||
                                 line.Contains("JV13") ||
                                 line.Contains("JV14") ||
                                 line.Contains("JV15") ||
                                 line.Contains("JV16") ||
                                 line.Contains("JV17") ||
                                 line.Contains("JN06") ||
                                 line.Contains("JN07") ||
                                 line.Contains("JN08") ||
                                 line.Contains("JN09") ||
                                 line.Contains("JN10") ||
                                 line.Contains("JN11") ||
                                 line.Contains("JN12") ||
                                 line.Contains("JN13") ||
                                 line.Contains("JN14") ||
                                 line.Contains("JN15") ||
                                 line.Contains("JN16") ||
                                 line.Contains("JN17") ||
                                 line.Contains("JNTIME"))
                        {
                            writer.WriteLine($"");
                            writer.WriteLine($"{fields[3]}");
                            if (!line.Contains("JV0620") &&
                                !line.Contains("JV0720") &&
                                !line.Contains("JV0820") &&
                                !line.Contains("JV0920") &&
                                !line.Contains("JV1020") &&
                                !line.Contains("JV1120") &&
                                !line.Contains("JV1220") &&
                                !line.Contains("JV1320") &&
                                !line.Contains("JV1420") &&
                                !line.Contains("JV1520") &&
                                !line.Contains("JV1620") &&
                                !line.Contains("JV1720"))
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
                                writer.WriteLine($"----------------");
                                writer.WriteLine(whichTag);
                                writer.WriteLine();
                            }
                            else if (line.Contains("JV0620") ||
                                     line.Contains("JV0720") ||
                                     line.Contains("JV0820") ||
                                     line.Contains("JV0920") ||
                                     line.Contains("JV1020") ||
                                     line.Contains("JV1120") ||
                                     line.Contains("JV1220") ||
                                     line.Contains("JV1320") ||
                                     line.Contains("JV1420") ||
                                     line.Contains("JV1520") ||
                                     line.Contains("JV1620") ||
                                     line.Contains("JV1720"))
                            {
                                writer.WriteLine("------------");
                                writer.WriteLine(":20 BREAK TAGLINE HERE ");
                                writer.WriteLine();
                            }
                        }
                        else if (line.Contains("STOPSET"))
                        {
                            writer.WriteLine($"STOPSET");
                        }
                        else if (fields[2] == "CM" ||
                            fields[3] == "LIVE")
                        {
                            if (fields[3].StartsWith("DA"))
                            {
                                writer.WriteLine($"{fields[1]} {fields[2]} {fields[4]} {fields[5]} {fields[6]}");
                            }
                            else
                            {
                                writer.WriteLine($"{fields[1]} {fields[2]} {fields[3]} {fields[4]} {fields[5]} {fields[6]}");
                            }
                        }
                        else
                        {
                            if (fields.Length == 6)
                            {
                                writer.WriteLine($"66666666");
                                writer.WriteLine($"{fields[0]} -- {fields[1]} -- {fields[2]} -- {fields[3]} -- {fields[4]} -- {fields[5]}");
                            }
                            if (fields.Length == 7)
                            {
                                writer.WriteLine($"77777777");
                                writer.WriteLine($"{fields[0]} -- {fields[1]} -- {fields[2]} -- {fields[3]} -- {fields[4]} -- {fields[5]} -- {fields[6]}");
                            }
                            if (fields.Length == 8)
                            {
                                writer.WriteLine($"{fields[0]} -- {fields[1]} -- {fields[2]} -- {fields[3]} -- {fields[4]} -- {fields[5]} -- {fields[6]} -- {fields[7]}");
                            }
                            if (fields.Length == 9)
                            {
                                writer.WriteLine($"{fields[0]} -- {fields[1]} -- {fields[6]} -- {fields[7]} -- {fields[8]}");
                            }
                        }
                    }
                
                }
            }
            writer.WriteLine("");
            writer.WriteLine("*******************************************************");
            writer.WriteLine("**************KATIE: TURN ON YOUR FRIDGE!**************");
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
            if (File.Exists(inFile))
            {
                File.Delete(inFile);
            }

        }
    } // class
} // namespace
