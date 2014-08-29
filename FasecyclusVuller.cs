using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCOL2iTCPC
{
    class FasecyclusVuller
    {
        List<FasecyclusUitgang> faseCyclusUitgangLijst;
        StreamReader fileReader;

        public FasecyclusVuller(string location)
        { fileReader = new StreamReader(location); }

        public void vulAttributen(List<FasecyclusUitgang> lijst)
        {
            faseCyclusUitgangLijst = lijst;

            string line = String.Empty;
            while (line != null)
            {
                if (line.Contains("#else"))
                {
                    do
                    { line = fileReader.ReadLine(); }
                    while (!line.Contains("#endif"));

                    line = fileReader.ReadLine();
                    continue;
                }
                if (line.Contains("#ifdef"))
                {
                    line = fileReader.ReadLine();
                    continue;
                }

                if (line.Contains("FC_code"))
                    FC_code(line);

                int countDotComma = line.Count(f => f == ';');
                int countComma = line.Count(g => g == ',');
                if ((line.Contains("TRG_max") || line.Contains("TGG_max") || line.Contains("TGGL_max") || line.Contains("TGL_max") || line.Contains("TMGL_max")) && countDotComma > 1)
                    splitTijdLine(line);
                else
                {
                    if (line.Contains("TRG_max"))
                        TRG_max(line);
                    if (line.Contains("TGG_max"))
                        TGG_max(line);
                    if (line.Contains("TGGL_max"))
                        TGGL_max(line);
                    if (line.Contains("TGL_max"))
                        TGL_max(line);
                    if (line.Contains("TMGL_max"))
                        TMGL_max(line);
                }

                if (countComma > 1 && countDotComma == 1)   //CCOL versie 8.0
                {
                    if (line.Contains("FC_us2"))
                        readFasecycliV8CCOL(line);
                    else
                        if (line.Contains("FC"))
                            readFasecycliV8CCOLappltab(line);
                }

                line = fileReader.ReadLine();
            }
            signalControl();
            reqStartup();
            signalStart();
            variabelGeel();
        }

        private void variabelGeel()
        {
            maxGeelTijdenVoorNietIngevulde();

            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.type != faseType.Voetganger)
                    uitgang.varAmber = uitgang.maxGeelTijd - uitgang.minAmber;
        }

        private void maxGeelTijdenVoorNietIngevulde()
        {
            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.maxGeelTijd == 0 && uitgang.type != faseType.Voetganger)
                    uitgang.maxGeelTijd = 60;
        }

        private void reqStartup()
        {
            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                uitgang.reqStartup = "1";
        }

        private void signalStart()
        {
            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.type == faseType.Voetganger)
                    uitgang.signalStart = "5-5";
                else
                    uitgang.signalStart = "2-2";
        }

        private void splitTijdLine(string line)
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
                if (tempLine.Contains("TRG_max"))
                    TRG_max(tempLine);
                if (tempLine.Contains("TGG_max"))
                    TGG_max(tempLine);
                if (tempLine.Contains("TGGL_max"))
                    TGGL_max(tempLine);
                if (tempLine.Contains("TGL_max"))
                    TGL_max(tempLine);
                if (tempLine.Contains("TMGL_max"))
                    TMGL_max(line);
            }
        }

        private void TRG_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            int isIndex = line.IndexOf('=') + 1;
            string TRG = line.Substring(isIndex).Trim(trimChars);
            if (TRG.Equals("NG"))
                return;

            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.id.Equals(ID))
                    uitgang.minRed = int.Parse(TRG);
        }

        private void TGG_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            int isIndex = line.IndexOf('=') + 1;
            string TGG = line.Substring(isIndex).Trim(trimChars);
            if (TGG.Equals("NG"))
                return;

            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.id.Equals(ID))
                    uitgang.minGreen = int.Parse(TGG);
        }

        private void TGGL_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            int isIndex = line.IndexOf('=') + 1;
            string TGGL = line.Substring(isIndex).Trim(trimChars);
            if (TGGL.Equals("NG"))
                return;

            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
            {
                if (uitgang.id.Equals(ID) && uitgang.type == faseType.Voetganger)
                {
                    uitgang.greenFlash = int.Parse(TGGL);
                    uitgang.TGGLset = true;
                    break;
                }
                if (uitgang.id.Equals(ID) && uitgang.type != faseType.Voetganger)
                {
                    uitgang.minAmber = int.Parse(TGGL);
                    uitgang.TGGLset = true;
                    break;
                }
            }
        }

        private void TGL_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            int isIndex = line.IndexOf('=') + 1;
            string TGL = line.Substring(isIndex).Trim(trimChars);
            if (TGL.Equals("NG"))
                return;

            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.id.Equals(ID) && uitgang.TGGLset == false)
                {
                    if (uitgang.type == faseType.Voetganger)
                        uitgang.greenFlash = int.Parse(TGL);
                    if (uitgang.type != faseType.Voetganger)
                        uitgang.minAmber = int.Parse(TGL);
                }
        }

        private void TMGL_max(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            int isIndex = line.IndexOf('=') + 1;
            string TMGL = line.Substring(isIndex).Trim(trimChars);
            if (TMGL.Equals("NG"))
                return;

            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
                if (uitgang.id.Equals(ID) && uitgang.type != faseType.Voetganger)
                    uitgang.maxGeelTijd = int.Parse(TMGL);
        }

        private void signalControl()
        {
            foreach (FasecyclusUitgang uitgang in faseCyclusUitgangLijst)
            {
                if (uitgang.type == faseType.Auto)
                    uitgang.signalControl = "01";

                if (uitgang.type == faseType.Fiets)
                    uitgang.signalControl = "03";

                if (uitgang.type == faseType.Voetganger)
                    uitgang.signalControl = "09";

                if(uitgang.type == faseType.OpenbaarVervoer)
                    uitgang.signalControl = "01";
            }
        }

        private void FC_code(string line)
        {
            char[] trimChars = { ' ', ';' };
            int startIndex = line.IndexOf("[") + 1;
            int endIndex = line.IndexOf("]");
            string ID = line.Substring(startIndex, endIndex - startIndex);

            startIndex = line.IndexOf("\"") + 1;
            endIndex = line.IndexOf("\"", startIndex);
            if (endIndex == -1)
                return;
            string code = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

            for (int i = 0; i < faseCyclusUitgangLijst.Count; i++)
                if (faseCyclusUitgangLijst[i].id.Equals(ID))
                {
                    int temp = 0;
                    if (int.TryParse(code, out temp))
                    {
                        faseCyclusUitgangLijst[i].code = temp;
                        faseCyclusUitgangLijst[i] = FC_type(faseCyclusUitgangLijst[i]);
                    }
                    break;
                }
        }

        private FasecyclusUitgang FC_type(FasecyclusUitgang uitgang)
        {
            if (uitgang.code <= 20 || uitgang.code > 60 && uitgang.code <= 80)
                uitgang.type = faseType.Auto;

            if (uitgang.code > 40 && uitgang.code <= 60)
                uitgang.type = faseType.OpenbaarVervoer;

            if (uitgang.code > 20 && uitgang.code <= 30 || uitgang.code > 80 && uitgang.code <= 90)
                uitgang.type = faseType.Fiets;

            if (uitgang.code > 30 && uitgang.code <= 40 || uitgang.code > 90 && uitgang.code < 100)
                uitgang.type = faseType.Voetganger;

            return uitgang;
        }

        private void readFasecycliV8CCOL(string line)
        {
            char[] trimChars = { ' ', ',', '"' };

            int startIndex = line.IndexOf('(') + 1;
            int endIndex = line.IndexOf(',');
            string naam = String.Empty;
            string trg  = String.Empty;
            string tfg  = String.Empty;
            string tvg  = String.Empty;
            string tgg  = String.Empty;
            string tgl  = String.Empty;
            string tggl = String.Empty;
            string ID = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

            for (int i = 0; i < 7; i++)
            {
                bool stop = false;
                startIndex = endIndex + 1;
                endIndex = line.IndexOf(',', startIndex);
                if (endIndex == -1)
                {
                    endIndex = line.IndexOf(')');
                    stop = true;
                }

                if (i == 0)
                    naam = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 1)
                    trg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 2)
                    tfg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 3)
                    tvg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 4)
                    tgg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 5)
                    tgl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 6)
                    tggl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

                if (stop)
                    break;
            }

            for(int i = 0; i < faseCyclusUitgangLijst.Count; i++)
                if (faseCyclusUitgangLijst[i].id.Equals(ID))
                {
                    int temp = 0;
                    if (faseCyclusUitgangLijst[i].code == 0)
                        if (int.TryParse(naam, out temp))
                        {
                            faseCyclusUitgangLijst[i].code = temp;
                            faseCyclusUitgangLijst[i] = FC_type(faseCyclusUitgangLijst[i]);
                        }
                    if (int.TryParse(trg, out temp))
                        faseCyclusUitgangLijst[i].minRed = temp;
                    if (int.TryParse(tgg, out temp))
                        faseCyclusUitgangLijst[i].minGreen = temp;
                    if (int.TryParse(tggl, out temp))
                    {
                        if (faseCyclusUitgangLijst[i].type == faseType.Voetganger)
                        {
                            faseCyclusUitgangLijst[i].greenFlash = temp;
                            faseCyclusUitgangLijst[i].TGGLset = true;
                        }
                        else
                        {
                            faseCyclusUitgangLijst[i].minAmber = temp;
                            faseCyclusUitgangLijst[i].TGGLset = true;
                        }
                    }
                    if (int.TryParse(tgl, out temp))
                    {
                        if (faseCyclusUitgangLijst[i].type == faseType.Voetganger && faseCyclusUitgangLijst[i].TGGLset == false)
                            faseCyclusUitgangLijst[i].greenFlash = temp;
                        if (faseCyclusUitgangLijst[i].type != faseType.Voetganger && faseCyclusUitgangLijst[i].TGGLset == false)
                            faseCyclusUitgangLijst[i].minAmber = temp;
                    }
                }
        }

        private void readFasecycliV8CCOLappltab(string line)
        {
            char[] trimChars = { ' ', ',', '"' };

            int startIndex = line.IndexOf('(') + 1;
            int endIndex = line.IndexOf(',');
            string naam = String.Empty;
            string trg = String.Empty;
            string tgg = String.Empty;
            string tfg = String.Empty;
            string tggl = String.Empty;
            string tgl = String.Empty;
            string tmgl = String.Empty;
            string ID = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

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
                    naam = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 1)
                    trg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 2)
                    tgg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 3)
                    tfg = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 4)
                    tggl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 5)
                    tgl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);
                if (i == 6)
                    tmgl = line.Substring(startIndex, endIndex - startIndex).Trim(trimChars);

                if (stop)
                    break;
            }

            for(int i = 0; i < faseCyclusUitgangLijst.Count; i++)
                if (faseCyclusUitgangLijst[i].id.Equals(ID))
                {
                    int temp = 0;
                    if (faseCyclusUitgangLijst[i].code == 0)
                        if (int.TryParse(naam, out temp))
                        {
                            faseCyclusUitgangLijst[i].code = temp;
                            faseCyclusUitgangLijst[i] = FC_type(faseCyclusUitgangLijst[i]);
                        }
                    if (int.TryParse(trg, out temp))
                        faseCyclusUitgangLijst[i].minRed = temp;
                    if (int.TryParse(tgg, out temp))
                        faseCyclusUitgangLijst[i].minGreen = temp;
                    if (int.TryParse(tggl, out temp))
                    {
                        if (faseCyclusUitgangLijst[i].type == faseType.Voetganger)
                        {
                            faseCyclusUitgangLijst[i].greenFlash = temp;
                            faseCyclusUitgangLijst[i].TGGLset = true;
                        }
                        else
                        {
                            faseCyclusUitgangLijst[i].minAmber = temp;
                            faseCyclusUitgangLijst[i].TGGLset = true;
                        }
                    }
                    if (int.TryParse(tgl, out temp))
                    {
                        if (faseCyclusUitgangLijst[i].type == faseType.Voetganger && faseCyclusUitgangLijst[i].TGGLset == false)
                            faseCyclusUitgangLijst[i].greenFlash = temp;
                        if (faseCyclusUitgangLijst[i].type != faseType.Voetganger && faseCyclusUitgangLijst[i].TGGLset == false)
                            faseCyclusUitgangLijst[i].minAmber = temp;
                    }
                    if (int.TryParse(tmgl, out temp) && faseCyclusUitgangLijst[i].type != faseType.Voetganger)
                        faseCyclusUitgangLijst[i].maxGeelTijd = temp;

                    break;
                }
        }

        public List<FasecyclusUitgang> FaseCyclusUitgangLijst
        {
            get
            { return faseCyclusUitgangLijst.OrderBy(o => o.index).ToList(); }
        }
    }
}
