using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CCOL2iTCPC
{
    class SysFileHandler
    {
        StreamReader sysReader;
        string location;

        /// <summary>
        /// Handles all actions regarding the reading out of the sys.h file
        /// </summary>
        /// <param name="location">The location of the sys.h file</param>
        public SysFileHandler(string location)
        {
            sysReader = new StreamReader(location);
            this.location = location;
        }

        public List<FasecyclusUitgang> getFaseCyclusIDs()
        {
            List<FasecyclusUitgang> faseCyclusLijst = new List<FasecyclusUitgang>();

            StreamReader sysReader = new StreamReader(location);

            List<string> previousLines = new List<string>();

            string endOfDefinition = "FCMAX";   // this is the end of defining the detectors in the sys.h file
            string line = String.Empty;
            do
            {
                line = sysReader.ReadLine();
                previousLines.Add(line);
            }
            while (!line.Contains(endOfDefinition));

            int faseCyclusLinesStart = 0;
            for (int i = previousLines.Count - 1; i > 0; i--)
            {
                int index = getIndex(previousLines[i]);
                if (index == 0)
                {
                    faseCyclusLinesStart = i;
                    break;
                }
            }

            for (int lineNumber = faseCyclusLinesStart; lineNumber < previousLines.Count; lineNumber++)
            {
                line = previousLines[lineNumber];
                if (line.Contains("#ifdef") || line.Contains("#endif") || line.Contains(endOfDefinition) || line.Equals("") || line.StartsWith("/*") ||
                    line.Contains("SYSTEM"))
                    continue;

                string tempString = line.Substring(line.IndexOf(Program.define) + Program.define.Length);

                int commentIndex = tempString.IndexOf("/*");
                if (commentIndex != -1)
                    tempString = tempString.Substring(0, commentIndex);

                int startIndex_FaseCyclusName = tempString.IndexOf('f');
                if (startIndex_FaseCyclusName == -1)
                    continue;
                int endIndex_FaseCyclusName = tempString.IndexOf(' ', startIndex_FaseCyclusName);

                string IDstring = tempString.Substring(startIndex_FaseCyclusName, endIndex_FaseCyclusName - startIndex_FaseCyclusName);

                string indexString = tempString.Substring(endIndex_FaseCyclusName).Trim();

                int index = int.Parse(indexString);

                FasecyclusUitgang faseCyclusUitgang = new FasecyclusUitgang(IDstring);
                faseCyclusUitgang.index = index;

                faseCyclusLijst.Add(faseCyclusUitgang);
            }
            return faseCyclusLijst.OrderBy(o => o.index).ToList();
        }

        public List<DetectorLogic> getDetectieLogicIDs()
        {
            List<DetectorLogic> detectieIngangLijst = new List<DetectorLogic>();

            Console.WriteLine("Selected Sys file is: " + location);
            StreamReader sysReader = new StreamReader(location);

            List<string> previousLines = new List<string>();

            string endOfDefinition = "DPMAX";   // this is the end of defining the detectors in the sys.h file
            string line = String.Empty;
            do
            {
                line = sysReader.ReadLine();
                previousLines.Add(line);
            }
            while (!line.Contains(endOfDefinition));

            int detectionLinesStart = 0;
            for (int i = previousLines.Count - 1; i > 0; i--)
            {
                int index = getIndex(previousLines[i]);
                if (index == 0)
                {
                    detectionLinesStart = i;
                    break;
                }
            }

            for (int lineNumber = detectionLinesStart; lineNumber < previousLines.Count; lineNumber++)
            {
                line = previousLines[lineNumber];
                if (line.Contains("#ifdef") || line.Contains("#endif") || line.Contains(endOfDefinition) || line.Equals("") || line.StartsWith("/*") ||
                    line.Contains("SYSTEM"))
                    continue;

                string tempString = line.Substring(line.IndexOf(Program.define) + Program.define.Length);

                int commentIndex = tempString.IndexOf("/*");
                if (commentIndex != -1)
                    tempString = tempString.Substring(0, commentIndex);

                int startIndexDetectorName = tempString.IndexOf('d');
                if (startIndexDetectorName == -1)
                    continue;
                int endIndexDetectorName = tempString.IndexOf(' ', startIndexDetectorName);

                string IDstring = tempString.Substring(startIndexDetectorName, endIndexDetectorName - startIndexDetectorName);

                string indexString = tempString.Substring(endIndexDetectorName).Trim();

                int index = int.Parse(indexString);

                DetectorLogic detectieIngang = new DetectorLogic(IDstring);
                detectieIngang.index = index;

                detectieIngangLijst.Add(detectieIngang);
            }
            return detectieIngangLijst.OrderBy(o => o.index).ToList();
        }

        private int getIndex(string tempString)
        {
            tempString = tempString.Substring(tempString.IndexOf(Program.define) + 1);

            tempString = tempString.Substring(tempString.IndexOf(" ") + 1);
            tempString = tempString.Substring(tempString.IndexOf(" ") + 1).Trim();

            int tempIndex = tempString.IndexOf("/*");
            if (tempIndex == -1)
                tempIndex = tempString.IndexOf("//");

            if (tempIndex != -1)
                tempString = tempString.Substring(0, tempIndex).Trim();

            int returnInt = 0;
            bool parsing = int.TryParse(tempString, out returnInt);

            if (parsing)
                return returnInt;
            else
                return -1;
        }
    }
}
