using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCOL2iTCPC
{
    class InterGreen
    {
        List<FasecyclusUitgang> faseLijst;
        StreamReader streamReader;

        IntergreenEntry[,] intergreen;
        int langsteTijd = 0;
        bool langsteTijdTGO = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="faseLijst"></param>
        /// <param name="locatie">locatie tab.c file</param>
        public InterGreen(List<FasecyclusUitgang> faseLijst, string locatie)
        { 
            this.faseLijst = faseLijst;
            streamReader = new StreamReader(locatie);

            intergreen = new IntergreenEntry[faseLijst.Count, faseLijst.Count];
            for(int i = 0; i < faseLijst.Count; i++)
                for (int j = 0; j < faseLijst.Count; j++)
                {
                    intergreen[i, j].entry = " . ";
                    intergreen[i, j].TGOset = false;
                }

            for (int i = 0; i < faseLijst.Count; i++)
                intergreen[i, i].entry = "X";

            makeIntergreen();
        }

        private void makeIntergreen()
        {
            string line = streamReader.ReadLine();
            do
            {

                int count = line.Count(f => f == ';');
                if ((line.Contains("TO_max") || line.Contains("TGO_max")) && count > 1)
                    splitLines(line);
                else
                {
                    if (line.Contains("TO_max"))
                        TO_max(line);
                    if (line.Contains("TGO_max"))
                        TGO_max(line);
                }

                if ((line.Contains("TO(") || line.Contains("To3")) && !line.Contains(Program.define))
                    CCOLv8ApplLine(line);

                line = streamReader.ReadLine();
            } while (line != null);
        }

        private void splitLines(string line)
        {
            List<string> singleLines = new List<string>();

            int indexStart = 0;
            int indexEnd = 0;
            while (indexEnd != -1)
            {
                indexStart = indexEnd + 1;
                indexEnd = line.IndexOf(';', indexStart);

                if (indexEnd != -1)
                    singleLines.Add(line.Substring(indexStart, indexEnd - indexStart));
            }

            foreach (string tempLine in singleLines)
            {
                if (tempLine.Contains("TO_max"))
                    TO_max(tempLine);
                if (tempLine.Contains("TGO_max"))
                    TGO_max(tempLine);
            }
        }

        private void TO_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = 0;
            int endIndex = 0;
            string FromID = String.Empty;
            string ToID = String.Empty;
            int FromIndex = 0;
            int ToIndex = 0;
            
            int intergreenTime = 0;

            startIndex = line.IndexOf("[") + 1;
            endIndex = line.IndexOf("]");
            FromID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("[", startIndex + 1) + 1;
            endIndex = line.IndexOf("]", endIndex+1);
            ToID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf('=');
            try
            { intergreenTime = int.Parse(line.Substring(startIndex + 1).Trim(trimChars)); }
            catch (FormatException)
            { return; }

            if (intergreenTime > langsteTijd && !langsteTijdTGO)
                langsteTijd = intergreenTime;

            foreach (FasecyclusUitgang uitgang in faseLijst)
            {
                if (uitgang.id.Equals(FromID))
                {
                    FromIndex = uitgang.index;
                    if (uitgang.type != faseType.Voetganger)
                        intergreenTime += uitgang.minAmber;
                }
                if (uitgang.id.Equals(ToID))
                    ToIndex = uitgang.index;
            }

            
            if (intergreen[FromIndex, ToIndex].TGOset == false)
                if (intergreenTime == 0)
                    intergreen[FromIndex, ToIndex].entry = "0.01";
                else
                {
                    IntergreenEntry entry = new IntergreenEntry();
                    double temp = intergreenTime / 10.0;
                    entry.entry = temp.ToString("F1");
                    intergreen[FromIndex, ToIndex] = entry;
                }
        }

        private void TGO_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = 0;
            int endIndex = 0;
            string FromID = String.Empty;
            string ToID = String.Empty;
            int FromIndex = 0;
            int ToIndex = 0;
            int intergreenTime = 0;

            startIndex = line.IndexOf("[") + 1;
            endIndex = line.IndexOf("]");
            FromID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("[", startIndex + 1) + 1;
            endIndex = line.IndexOf("]", endIndex + 1);
            ToID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf('=');
            try
            { intergreenTime = int.Parse(line.Substring(startIndex + 1).Trim(trimChars)); }
            catch (FormatException)
            { return; }

            langsteTijdTGO = true;
            if (intergreenTime > langsteTijd)
                langsteTijd = intergreenTime;

            foreach (FasecyclusUitgang uitgang in faseLijst)
            {
                if (uitgang.id.Equals(FromID))
                {
                    FromIndex = uitgang.index;
                    if (uitgang.type != faseType.Voetganger)
                        intergreenTime += uitgang.minAmber;
                }
                if (uitgang.id.Equals(ToID))
                    ToIndex = uitgang.index;
            }

            IntergreenEntry entry = new IntergreenEntry();
            if (intergreenTime == 0)
            {
                entry.entry = "0.01";
                entry.TGOset = true;
                intergreen[FromIndex, ToIndex] = entry;
            }
            else
            {
                double temp = intergreenTime / 10.0;
                entry.entry = temp.ToString("F1");
                entry.TGOset = true;
                intergreen[FromIndex, ToIndex] = entry;
            }
        }

        private void CCOLv8ApplLine(string line)
        {
            char[] trimChars = {' ', ','};
            if (line.Substring(0, 2).Equals("/*"))
                return;
            int intergreenTime = -1;

            int startIndex = line.IndexOf('(') + 1;
            int endIndex = line.IndexOf(',');
            string fromID = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
            int FromIndex = 0;

            startIndex = endIndex + 1;
            endIndex = line.IndexOf(',', startIndex);
            string toID = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
            int ToIndex = 0;

            startIndex = endIndex + 1;
            endIndex = line.IndexOf(',', startIndex);
            string OTstring = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
            if (OTstring != null)
            {
                if (OTstring.Equals("NK") || OTstring.Equals("FK"))
                    return;

                intergreenTime = int.Parse(OTstring);
                if (intergreenTime > langsteTijd && !langsteTijdTGO)
                    langsteTijd = intergreenTime;
            }

            startIndex = endIndex + 1;
            endIndex = line.IndexOf(')', startIndex);
            string GOTstring = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
            if (GOTstring != null)
            {
                if (GOTstring.Equals("NK") || GOTstring.Equals("FK"))
                    return;

                intergreenTime = int.Parse(GOTstring);
                langsteTijdTGO = true;
                if (intergreenTime > langsteTijd)
                    langsteTijd = intergreenTime;
            }

            foreach (FasecyclusUitgang uitgang in faseLijst)
            {
                if (uitgang.id.Equals(fromID))
                {
                    FromIndex = uitgang.index;
                    if(uitgang.type  != faseType.Voetganger)
                        intergreenTime += uitgang.minAmber;
                }
                if (uitgang.id.Equals(toID))
                    ToIndex = uitgang.index;
            }

            IntergreenEntry entry = new IntergreenEntry();
            if (intergreenTime == 0)
                entry.entry = "0.01";
            else
            {
                double temp = intergreenTime / 10.0;
                entry.entry = temp.ToString("F1");
            }

            if(GOTstring != null)
                entry.TGOset = true;
            if (GOTstring == null)
                entry.TGOset = false;

            if(intergreen[FromIndex, ToIndex].TGOset == false)
                intergreen[FromIndex, ToIndex] = entry;
            /////////
            //als de TGOset false is, dan is er nog helemaal niks geset, óf OT is geset. Als OT geset is, mag GOT eroverheen geset worden.
            //als er nog niks geset is, mag alles erover geset worden
            ////////
        }

        public IntergreenEntry[,] interGreen
        {
            get
            { return intergreen; }
        }

        public int LangsteOntruimingsTijd
        {
            get
            { return langsteTijd; }
        }
    }
}
