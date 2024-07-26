using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using System.Windows.Xps.Packaging;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

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
        string tag4 = "Today's hits; Timeless favorites. Sunny 107;";

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
            string rawText = "";

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
                    inFile = filePath  + logDate.ToString("yyMMdd") + ".pdf";
                    outFile = filePath + "KKRBLog_" + logDate.ToString("M-d-yy") + ".txt";
                    script = filePath + "_ScriptLog_" + logDate.ToString("M-d-yy") + ".txt";
                }

            }
            rawText = GetTextFromPDF();
            //Console.WriteLine(rawText);
            ProcessText();
        }

        private string GetLogDate()
        {
            Console.WriteLine("Log date = ");
            return Console.ReadLine();
        }

        private string GetTextFromPDF()
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(inFile))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }
            File.WriteAllText(outFile, text.ToString());
            return text.ToString();
        }

        public void ProcessText()
        {
            Random rando = new Random();
            int iHour = 0;
            string txtFile = outFile;
            StreamWriter writer = new StreamWriter(script, false);
            writer.WriteLine("------------------------------------------------------------");
            writer.WriteLine($"KKRB Script for {logDate.ToString("ddd")}, {logDate.ToString("M-d-yy")}");
            writer.WriteLine("------------------------------------------------------------");
            writer.WriteLine();
            string whichTag;
            string prevLine = "";
            string thisLine = "";

            if (File.Exists(outFile))
            {
                string[] lines = File.ReadAllLines(outFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(' ');
                    iHour = GetHour(lines[i]);
                    if ((iHour >= 6 && iHour < 18) &&
                        !lines[i].Contains("SWEEPER") &&
                        !lines[i].Contains("Artist") &&
                        !lines[i].Contains("WX") &&
                        !lines[i].Contains("Stock Market") &&
                        fields[2] != "NEW")
                    {
                        thisLine = fields[2];
                        if (lines[i].Contains("VOICETRACK") ||
                            lines[i].Contains("Voice Track"))
                        {
                            whichTag = SelectTag(rando.Next(7, 10000));
                            writer.WriteLine();
                            writer.WriteLine($"{fields[3]}");
                            writer.WriteLine("----------------");
                            writer.WriteLine($"{whichTag}");
                            writer.WriteLine();
                        }
                        else
                        {
                            if (lines[i].Contains("STOPSET"))
                            {
                                writer.WriteLine($"{fields[5]} {fields[6]}");
                            }

                            else if (prevLine == "CM" &&
                                     thisLine != prevLine)
                            {
                                writer.WriteLine();
                            }
                            if (!lines[i].Contains("STOPSET"))
                            {
                                writer.WriteLine($"{lines[i]}");
                            }
                        }
                    }
                    prevLine = thisLine;                    
                }
            }
            writer.Flush();
            writer.Close();
            writer.Dispose();
            Console.ReadLine();
        } // ProcessText

        private string SelectTag(int randomTag)
        {
            
            bool lIsPrime = IsPrime(randomTag);
            if (lIsPrime)
            {
                return tag4;
            }
            if (randomTag % 2 == 0)
            {
                return tag2;
            }
            if (randomTag % 3 == 0)
            {
                return tag3;
            }
            return tag1;
        }


        private bool IsPrime(int whatNumber)
        {
            for (uint i = 8; i < whatNumber; i++)
                if (whatNumber % i == 0) return false;
            return true;
        }

        private int GetMinutePosition(string vtword)
        {
            int minutePos = 0;
            string subst = "";
            if (vtword != null &&
                vtword.Length > 0)
            {
                subst = vtword.Substring(vtword.Length - 2, 2);
            }

            if (int.TryParse(subst, out minutePos))
            {
                return minutePos;
            }
            return 0;
        } // GetMinutePosition

        private int GetWheelPosition(string vtword)
        {
            int wheelPos = 0;
            string subst = "";
            if (vtword != null &&
                vtword.Length > 0)
            {
                subst = vtword.Substring(vtword.Length - 4, 4);
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
