using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCOL2iTCPC
{
    class DetectieLogicVuller
    {
        List<DetectorLogic> detectieLogicLijst;
        StreamReader tabReader;
        public DetectieLogicVuller(string location)
        { tabReader = new StreamReader(location); }

        public void vulAttributen(List<DetectorLogic> lijst)
        {
            detectieLogicLijst = lijst;

            string line = String.Empty;
            while (line != null)
            {
                if (line.Contains("#else"))
                {
                    do
                    { line = tabReader.ReadLine(); }
                    while (!line.Contains("#endif"));

                    line = tabReader.ReadLine();
                    continue;
                }
                if (line.Contains("#ifdef"))
                {
                    line = tabReader.ReadLine();
                    continue;
                }

                int countDotComma = line.Count(f => f == ';');
                int countComma = line.Count(g => g == ',');
                if (countDotComma > 1 && countComma < 1)
                    splitString(line);
                else
                {
                    if (line.Contains("D_code"))
                        D_code(line);
                    if (line.Contains("DE_type"))
                        D_type(line);
                    if (line.Contains("TBG_max"))
                        TBG_max(line);
                    if (line.Contains("TOG_max"))
                        TOG_max(line);
                }

                if (countDotComma == 1 && countComma > 1 && (line.Contains("DP") && !line.Contains(Program.define) || line.Contains("DET_is")))
                    readDetectorV8CCOL(line);

                line = tabReader.ReadLine();
            }

            setInputForDetectorLogic();

            tabReader.BaseStream.Position = 0;
            tabReader.DiscardBufferedData();
        }

        private void readDetectorV8CCOL(string line)
        {
            char[] trimChars = { ' ', ',', '"' };

            int startIndex = line.IndexOf('(') + 1;
            int endIndex = line.IndexOf(',');
            string ID = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
            string name = String.Empty;
            string tdb = String.Empty;  //bezettijd, hoeft niet
            string tdh = String.Empty;  //hiattijd, hoeft ook  niet
            string tbg = String.Empty;  //ontime
            string tog = String.Empty;  //offtime
            string tfl = String.Empty;  //wordt niet gebruikt
            string cfl = String.Empty;  //wordt niet gebruikt

            for (int i = 0; i < 7; i++)
            {
                bool stop = false;
                startIndex = endIndex + 1;
                endIndex = line.IndexOf(',', startIndex);
                if (endIndex == -1) {
                    endIndex = line.IndexOf(')');
                    stop = true;
                }

                if (i == 0)
                    name= line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if(i == 1)
                    tdb = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if(i == 2)
                    tdh = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if(i == 3)
                    tbg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if(i == 4)
                    tog = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if(i == 5)
                    tfl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if(i == 6)
                    cfl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

                if (stop)
                    break;
            }

            foreach(DetectorLogic logic in detectieLogicLijst)
                if (logic.id.Equals(ID))
                {
                    int temp = 0;
                    bool succeeded = int.TryParse(tog, out temp);
                    if (succeeded)
                        logic.offTime = temp;

                    succeeded = int.TryParse(tbg, out temp);
                    if (succeeded)
                        logic.onTime = temp;

                    if (logic.code != null)
                        logic.code = name;

                    logic.errorMode = "002";
                }
        }

        private void splitString(string line)
        {
            int startIndex = 0;
            int endIndex = 0;

            int count = line.Count(f => f == ';');
            for(int i = 0; i < count; i++)
            {
                startIndex = endIndex + 1;
                endIndex = line.IndexOf(';', startIndex);

                string temp = line.Substring(startIndex, endIndex + 1 - startIndex);

                if (temp.Contains("D_code"))
                    D_code(temp);
                if (temp.Contains("DE_type"))
                    D_type(temp);
                if (temp.Contains("TBG_max"))
                    TBG_max(temp);
                if (temp.Contains("TOG_max"))
                    TOG_max(temp);
            }
            
        }

        private void D_code(string line)
        {
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("\"") + 1;
            endIndex = line.IndexOf("\"", startIndex);
            if (endIndex == -1)
                return;
            string code = line.Substring(startIndex, endIndex - startIndex);

            for (int i = 0; i < detectieLogicLijst.Count; i++)
                if (detectieLogicLijst[i].id.Equals(ID))
                {
                    detectieLogicLijst[i].code = code;
                    break;
                }
        }

        private void D_type(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("=", endIndex) + 1;
            endIndex = line.IndexOf(";", startIndex);
            string type = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

            for (int i = 0; i < detectieLogicLijst.Count; i++)
                if (detectieLogicLijst[i].id.Equals(ID))
                {
                    detectieLogicLijst[i].type = type;
                    break;
                }
        }

        private void TBG_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int index = 0;
            int index2 = 0;
            string onTimeID = String.Empty;
            int onTime = 0;

            index = line.IndexOf("[") + 1;
            index2 = line.IndexOf("]");
            onTimeID = line.Substring(index, index2 - index);

            index = line.IndexOf("=") + 1;
            if (!int.TryParse(line.Substring(index).Trim(trimChars), out onTime))
                onTime = 0;

            foreach (DetectorLogic ingang in detectieLogicLijst)
                if (onTimeID.Equals(ingang.id))
                {
                    ingang.onTime = onTime;
                    if (ingang.errorMode == null)
                        ingang.errorMode = "002";
                }
        }

        private void TOG_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int offTime = 0;
            int index = 0;
            int index2 = 0;
            string offTimeID = String.Empty;

            index = line.IndexOf(";") + 1;

            index = line.IndexOf("[") + 1;
            index2 = line.IndexOf("]");
            offTimeID = line.Substring(index, index2 - index);

            index = line.IndexOf("=") + 1;
            string subLine = line.Substring(index).Trim(trimChars);
            if (subLine.Equals("NG"))
                return;
            offTime = int.Parse(subLine) / 60;

            foreach (DetectorLogic ingang in detectieLogicLijst)
                if (offTimeID.Equals(ingang.id))
                {
                    ingang.offTime = offTime;
                    if (ingang.errorMode == null)
                        ingang.errorMode = "002";
                }
        }

        private void setInputForDetectorLogic()
        {
            int detector = 0;
            int drukknop = 0;
            foreach (DetectorLogic logic in detectieLogicLijst)
            {
                if (logic.id.Contains('k')) // || logic.id.Contains('r') toevoegen om ook de "dr" detectors een drukknop te maken
                {
                    drukknop++;
                    #region omrekeningen
                    if (drukknop == 9)
                        drukknop = 17;
                    if (drukknop == 25)
                        drukknop = 33;
                    if (drukknop == 41)
                        drukknop = 49;
                    if (drukknop == 57)
                        drukknop = 65;
                    if (drukknop == 73)
                        drukknop = 81;
                    #endregion

                    if (drukknop < 10)
                        logic.input = "01-002-10" + drukknop;
                    else
                        if (drukknop < 100)
                            logic.input = "01-002-1" + drukknop;
                }
                else
                {
                    detector++;
                    if (detector < 10)
                        logic.input = "01-002-00" + detector;
                    else
                        if (detector < 100)
                            logic.input = "01-002-0" + detector;
                }
            }
        }

        public List<DetectorLogic> DetectieLogicLijst
        {
            get
            { return detectieLogicLijst.OrderBy(o => o.index).ToList(); }
        }
    }
}
