using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCOL2iTCPC
{
    public class MainStart
    {
        string sysLocation = String.Empty;
        string tabLocation = String.Empty;
        string templateFile = String.Empty;
        string outputFile = String.Empty;

        List<DetectorLogic> detectieLogicLijst  = new List<DetectorLogic>();
        List<FasecyclusUitgang> faseCyclusLijst = new List<FasecyclusUitgang>();
        SysFileHandler sysFileHandler;
        DetectieLogicVuller detectieLogicVuller;
        FasecyclusVuller fasecyclusVuller;
        IntergreenEntry[,] intergreen;

        int langsteOntruimingsTijd = 0;

        public MainStart()
        {}

        public void getInputFolder(out string sysString, out string tabString)
        {
            OpenFileDialog sysBrowserDialog = new OpenFileDialog();
            OpenFileDialog tabBrowserDialog = new OpenFileDialog();
            sysBrowserDialog.Title = "Selecteer het SYS.H bestand of het equivalent hiervan";
            tabBrowserDialog.Title = "Selecteer het TAB.C bestand of het equivalent hiervan";
            sysBrowserDialog.Filter = "SYS.H file|*sys.h|All Files|*.*";
            tabBrowserDialog.Filter = "TAB.C file|*tab.c|All Files|*.*";

            string sysResult = "";
            string tabResult = "";

            sysBrowserDialog.FileName = Application.StartupPath;
            DialogResult sysBrowseResult = sysBrowserDialog.ShowDialog();

            if (sysBrowseResult == DialogResult.OK)
                sysResult = sysBrowserDialog.FileName;

            DialogResult tabBrowseResult = tabBrowserDialog.ShowDialog();
            if (tabBrowseResult == DialogResult.OK)
                tabResult = tabBrowserDialog.FileName;

            sysString = sysResult;
            tabString = tabResult;
        }

        public string getTemplateFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "PTC2 file (.PTC2)|*.PTC2|All Files|*.*";
            fileDialog.Title = "Selecteer de PTC2-template  file";
            fileDialog.InitialDirectory = Application.StartupPath;
            DialogResult result = fileDialog.ShowDialog();

            if (result == DialogResult.OK)
                templateFile = fileDialog.FileName;

            return templateFile;
        }

        public string getOutputFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "PTC2 file (.PTC2)|*.PTC2|All Files|*.*";
            fileDialog.Title = "Selecteer de PTC2-output  file";
            fileDialog.InitialDirectory = Application.StartupPath;
            DialogResult result = fileDialog.ShowDialog();

            if (result == DialogResult.OK)
                outputFile = fileDialog.FileName;

            return outputFile;
        }

        public void readFiles(string sysFile, string tabFile, string template, string output)
        {
            templateFile = template;
            outputFile = output;

            sysLocation = sysFile;
            tabLocation = tabFile;

            ////    initialisatie
            sysFileHandler = new SysFileHandler(sysLocation);
            detectieLogicVuller = new DetectieLogicVuller(tabLocation);
            fasecyclusVuller = new FasecyclusVuller(tabLocation);

            ////    vullen van detectielogic lijst
            detectieLogicLijst = sysFileHandler.getDetectieLogicIDs();
            detectieLogicVuller.vulAttributen(detectieLogicLijst);
            detectieLogicLijst = detectieLogicVuller.DetectieLogicLijst;

            ////    vullen van fasecyclus lijst
            faseCyclusLijst = sysFileHandler.getFaseCyclusIDs();
            fasecyclusVuller.vulAttributen(faseCyclusLijst);
            faseCyclusLijst = fasecyclusVuller.FaseCyclusUitgangLijst;

            ////    maken van de intergreen
            InterGreen interGreen = new InterGreen(faseCyclusLijst, tabLocation);
            intergreen = interGreen.interGreen;
            langsteOntruimingsTijd = interGreen.LangsteOntruimingsTijd;
        }

        public void writeFiles()
        {
            List<string> file  = new List<string>();
            
            using (StreamReader reader = new StreamReader(templateFile))
            {
                string line = String.Empty;
                while (line != null)
                {
                    line = reader.ReadLine();
                    file.Add(line);
                }
            }

            file = makeWork000(file);
            file = makeWork004(file);
            file = makeWork005(file);
            file = makeWork006(file);
            file = makeWork007(file);
            file = makeWork008(file);
            file = makeWork009(file);
            file = makeWork010(file);
            file = makeWork012(file);
            file = makeWork992(file);
            file = makeWork997(file);
            file = makeWork998(file);
            file = makeWork999(file);

            using (StreamWriter writer = new StreamWriter(outputFile))
                foreach (string line in file)
                    writer.WriteLine(line);
        }

        private List<string> makeWork000(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;
            StringBuilder stringBuilder = new StringBuilder(file[1]);
            int startIndex = 0;
            int endIndex = 0;
            int aantalFysiekeDetectoren = 0;
            foreach(DetectorLogic logic in detectieLogicLijst)
                if(!logic.id.Contains('k'))
                    aantalFysiekeDetectoren++;

            endIndex = stringBuilder.ToString().IndexOf(';');
            if (endIndex != 0)
                stringBuilder.Remove(0, endIndex);
            stringBuilder.Insert(0, faseCyclusLijst.Count);
            endIndex = 0;
            for (int i = 0; i < 2; i++)
            {
                startIndex = endIndex + 1;
                endIndex = stringBuilder.ToString().IndexOf(';', startIndex);

                if (i == 1)
                {
                    if (startIndex != endIndex)
                        stringBuilder.Remove(startIndex, endIndex - startIndex);

                    stringBuilder.Insert(startIndex, aantalFysiekeDetectoren);
                }
            }

            startIndex = 0;
            endIndex   = 0;
            for (int i = 0; i < 4; i++)
            {
                startIndex = endIndex + 1;
                endIndex = stringBuilder.ToString().IndexOf(';', startIndex);

                if (detectieLogicLijst != null && i == 3)
                {
                    if (startIndex != endIndex)
                        stringBuilder.Remove(startIndex, endIndex - startIndex);

                    stringBuilder.Insert(startIndex, detectieLogicLijst.Count);
                    file[1] = stringBuilder.ToString();
                }
            }

            startIndex = 0;
            endIndex = 0;
            for (int i = 0; i < 10; i++)
            {
                startIndex = endIndex + 1;
                endIndex = stringBuilder.ToString().IndexOf(';', startIndex);

                if (i == 9)
                {
                    if (startIndex != endIndex)
                        stringBuilder.Remove(startIndex, endIndex - startIndex);

                    stringBuilder.Insert(startIndex, langsteOntruimingsTijd);
                    file[1] = stringBuilder.ToString();
                }
            }

            return file;
        }

        private List<string> makeWork004(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;
            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.004"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            foreach(FasecyclusUitgang uitgang in faseCyclusLijst)
                file.Add(makeSingleWork004Line(uitgang));

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork005(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.005"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            StringBuilder tempString = new StringBuilder();
            foreach (FasecyclusUitgang uitgang in faseCyclusLijst)
            {
                if (uitgang.type != faseType.Voetganger)
                {
                    #region als type geen voetganger is
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append("1;");
                    tempString.Append("5;");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append("1;");
                    tempString.Append("8;");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append("1;");
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append("001;");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append("100;");
                    tempString.Append("100;");
                    tempString.Append("100;");
                    #endregion
                }
                else
                {
                    #region als type wel voetganger is
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append("1;");
                    tempString.Append("5;");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append("1;");
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append("001;");
                    tempString.Append(";");
                    tempString.Append("100;");
                    tempString.Append(";");
                    tempString.Append("100;");
                    #endregion
                }

                file.Add(tempString.ToString());
                tempString.Clear();
            }

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork006(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.006"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder("");

                for (int j = 0; j < faseCyclusLijst.Count; j++)
                    if(intergreen[j,i].entry.Contains('X') || intergreen[j,i].entry.Equals(" . "))
                        tempLine.Append(intergreen[j, i].entry + ";");
                    else
                        tempLine.Append("00-" + intergreen[j, i].entry + ";");

                file.Add(tempLine.ToString());
            }

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork007(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.007"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Contains("Work.008"))
                {
                    linesToDelete--;
                    break;
                }
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            //////////////////
            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder();
                tempLine.Append(";");
                tempLine.Append("060;");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append("1;");
                tempLine.Append("1;");
                tempLine.Append("1;");
                
                file.Add(tempLine.ToString());
            }
            file.Add("NeXt");
            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder();
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append("1;");
                tempLine.Append("1;");
                tempLine.Append("1;");

                file.Add(tempLine.ToString());
            }
            file.Add("NeXt");
            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder();
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append("1;");
                tempLine.Append("2;");
                tempLine.Append("2;");                

                file.Add(tempLine.ToString());
            }
            file.Add("NeXt");
            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder();
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append(";");
                tempLine.Append("1;");
                tempLine.Append("2;");
                tempLine.Append("2;");

                file.Add(tempLine.ToString());
            }
            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork008(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.008"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Contains("Work.009"))
                {
                    linesToDelete--;
                    break;
                }
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            //////////////////
            for (int i = 0; i < faseCyclusLijst.Count; i++)
                file.Add("1;;;;;;;;;;;;;;;;;;;;;;");
            file.Add("NeXt");
            for (int i = 0; i < faseCyclusLijst.Count; i++)
                file.Add("1;;;;;;;;;;;;;;;;;;;;;;");

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork009(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.009"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Contains("Work.010"))
                {
                    linesToDelete--;
                    break;
                }
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            //////////////////////////////////////////////////////
                                                                //
            for (int i = 0; i < faseCyclusLijst.Count; i++)     //
                file.Add(";;;1;");                              //
            file.Add("NeXt");                                   //
            for (int i = 0; i < faseCyclusLijst.Count; i++)     //
                file.Add(";;;1;");                              //
                                                                //
            //////////////////////////////////////////////////////

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork010(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;
            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.010"))
                    break;
            }
            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);
            file.RemoveRange(workPosition, file.Count - workPosition);

            foreach (DetectorLogic logic in detectieLogicLijst)
                if (!logic.id.Contains('k'))
                    file.Add("2;2;6;120;04000;;;;;002-0;;");

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork012(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;
            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.012"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            foreach (DetectorLogic ingang in detectieLogicLijst)
                file.Add(makeSingleWork012Line(ingang));

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork992(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.992"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder("");

                for (int j = 0; j < faseCyclusLijst.Count; j++)
                    tempLine.Append(intergreen[j, i].entry + ";");
                file.Add(tempLine.ToString());
            }

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork997(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;

            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.997"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            for (int i = 0; i < faseCyclusLijst.Count; i++)
            {
                StringBuilder tempLine = new StringBuilder("");

                for (int j = 0; j < faseCyclusLijst.Count; j++)
                    tempLine.Append(intergreen[j,i].entry + ";");
                file.Add(tempLine.ToString());
            }

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork998(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;
            string[] file2;
            int workPosition = 0;
            int linesToDelete = 0;

            foreach (string line in file)
            {
                workPosition++;
                if (line.Contains("Work.998"))
                    break;
            }

            for (int i = workPosition; i < file.Count; i++)
            {
                string line = file[i];
                if (line.Equals("NeXt"))
                    break;
                else
                    linesToDelete++;
            }
            if (linesToDelete != 0)
                file.RemoveRange(workPosition, linesToDelete);

            file2 = new string[file.Count - workPosition];
            file.CopyTo(workPosition, file2, 0, file.Count - workPosition);

            file.RemoveRange(workPosition, file.Count - workPosition);

            foreach (FasecyclusUitgang uitgang in faseCyclusLijst)
                file.Add(makeSingleWork998Line(uitgang));

            if (!file2[0].Equals("NeXt"))
                file.Add("NeXt");

            file.AddRange(file2);

            return file;
        }

        private List<string> makeWork999(List<string> fileToWrite)
        {
            List<string> file = fileToWrite;
            int aantalFysiekeDetectoren = 0;
            foreach (DetectorLogic logic in detectieLogicLijst)
                if (!logic.id.Contains('k'))
                    aantalFysiekeDetectoren++;

            if (detectieLogicLijst != null)
                for (int i = 00; i < file.Count; i++)
                {
                    string line = file[i];
                    if (line.Contains("Work.999"))
                    {
                        #region Number of plans & sequence logics & stage logics
                        int beginIndex = 0;
                        int endIndex = 0;
                        StringBuilder stringBuilder = new StringBuilder(file[i + 1]);
                        for (int j = 0; j < 5; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 4)
                            {
                                if (beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, "04");
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        beginIndex = 0;
                        endIndex = 0;
                        for (int j = 0; j < 8; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 7)
                            {
                                if (beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, "02");
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        beginIndex = 0;
                        endIndex = 0;
                        for (int j = 0; j < 9; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 8)
                            {
                                if (beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, "02");
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        #endregion
                        #region Aantal fysieke detectoren
                        beginIndex = 0;
                        endIndex = 0;
                        for (int j = 0; j < 11; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 10)
                            {
                                if(beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, aantalFysiekeDetectoren);
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        #endregion
                        #region Aantal detectielogic
                        beginIndex = 0;
                        endIndex = 0;
                        for (int j = 0; j < 13; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 12)
                            {
                                if (beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, detectieLogicLijst.Count);
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        #endregion
                        #region Aantal fasen
                        beginIndex = 0;
                        endIndex = 0;
                        for (int j = 0; j < 7; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 6)
                            {
                                if (beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, faseCyclusLijst.Count);
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        #endregion
                        #region Aantal conflicten
                        beginIndex = 0;
                        endIndex = 0;
                        for (int j = 0; j < 10; j++)
                        {
                            beginIndex = endIndex + 1;
                            endIndex = stringBuilder.ToString().IndexOf(';', endIndex + 1);

                            if (j == 9)
                            {
                                if (beginIndex != endIndex)
                                    stringBuilder.Remove(beginIndex, endIndex - beginIndex);

                                stringBuilder.Insert(beginIndex, "20");
                                file[i + 1] = stringBuilder.ToString();
                            }
                        }
                        #endregion

                        break;
                    }
                }
            return file;
        }

        private string makeSingleWork004Line(FasecyclusUitgang uitgang)
        {
            StringBuilder line = new StringBuilder();

            line.Append(uitgang.signalStart + ";");
            line.Append(uitgang.signalControl + ";");
            line.Append(uitgang.signalDisable + ";");
            line.Append(uitgang.signalMgrp + ";");
            line.Append(uitgang.masterGrp + ";");
            line.Append(uitgang.mainRoad + ";");
            line.Append(uitgang.minimize + ";");
            line.Append(uitgang.red_Amber + ";");
            #region min green
            if (uitgang.minGreen == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.minGreen / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + ";");
            }
            #endregion
            #region green flash
            if (uitgang.greenFlash == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.greenFlash / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + ";");
            }
            #endregion
            #region min amber
            if (uitgang.minAmber == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.minAmber / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + ";");
            }
            #endregion
            #region var amber
            if (uitgang.varAmber == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.varAmber / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + ";");
            }
            #endregion
            #region min red
            if (uitgang.minRed == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.minRed / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + ";");
            }
            #endregion
            line.Append(uitgang.varRedMode + ";");
            line.Append(uitgang.varRed2 + ";");
            line.Append(uitgang.reservation + ";");
            line.Append(uitgang.stageHoldTime + ";");
            line.Append(uitgang.delayMode + ";");
            line.Append(uitgang.requestPh + ";");
            line.Append(uitgang.DIRinput + ";");
            line.Append(uitgang.ontimeDIRinput + ";");
            line.Append(uitgang.reqStartup + ";");
            line.Append(uitgang.fixedControl + ";");
            line.Append(uitgang.prio1_Mode1 + ";");
            line.Append(uitgang.prio1_Mode2 + ";");
            line.Append(uitgang.prio2_Mode1 + ";");
            line.Append(uitgang.prio2_Mode2 + ";");
            line.Append(uitgang.prio3_Inp + ";");
            line.Append(uitgang.prio3_priority + ";");
            line.Append(uitgang.prio3_Mode1 + ";");
            line.Append(uitgang.prio3_Mode2 + ";");
            line.Append(uitgang.stDelay_1 + ";");
            line.Append(uitgang.stDelay_2 + ";");
            line.Append(uitgang.stDelay_3 + ";");
            line.Append(uitgang.st_d_4Inp + ";");
            line.Append(uitgang.relation + ";");

            return line.ToString();
        }
        
        private string makeSingleWork012Line(DetectorLogic ingang)
        {
            StringBuilder line = new StringBuilder();

            line.Append(ingang.id);
            for (int i = 0; i < 20 - ingang.id.Length; i++)
                line.Append(" ");
            line.Append(";");
            line.Append(ingang.phaseNo          + ";");
            line.Append(ingang.input            + ";");
            line.Append(ingang.enableInp        + ";");
            line.Append(ingang.reqMode          + ";");
            line.Append(ingang.reqCancel        + ";");
            line.Append(ingang.reqDelay         + ";");
            line.Append(ingang.delayNewReq      + ";");
            line.Append(ingang.returnToGreen    + ";");
            line.Append(ingang.reserveIntv      + ";");
            line.Append(ingang.minGrnMode1      + ";");
            line.Append(ingang.minGrnMode2      + ";");
            line.Append(ingang.reextIntvMode    + ";");
            line.Append(ingang.maxGrnIntv1      + ";");
            line.Append(ingang.maxGrnIntv2      + ";");
            line.Append(ingang.reductIntv       + ";");
            line.Append(ingang.delayedReduct    + ";");
            line.Append(ingang.PEGintv          + ";");
            line.Append(ingang.disableTime      + ";");
            line.Append(ingang.amberIntv        + ";");
            line.Append(ingang.redIntv          + ";");
            line.Append(ingang.output           + ";");
            line.Append(ingang.trafficCounting  + ";");
            #region On-time
            if (ingang.onTime >= 100)
                line.Append(ingang.onTime       + ";");
            else
                if (ingang.onTime >= 10)
                    line.Append("0" + ingang.onTime + ";");
                else
                    line.Append("00" + ingang.onTime + ";");
            #endregion
            #region Off-time
            if (ingang.offTime >= 100)
                line.Append(ingang.offTime      + ";");
            else
                if (ingang.offTime >= 10)
                    line.Append("0" + ingang.offTime + ";");
                else
                    line.Append("00" + ingang.offTime + ";");
            #endregion
            line.Append(ingang.errorMode        + ";");
            line.Append(ingang.intsecNo         + ";");
            line.Append(ingang.option           + ";");
            line.Append(ingang.auxPar           + ";");

            return line.ToString();
        }

        private string makeSingleWork998Line(FasecyclusUitgang uitgang)
        {
            StringBuilder line = new StringBuilder();

            line.Append(uitgang.id);
            for (int i = 0; i < 10 - uitgang.id.Length; i++)
                line.Append(" ");
            line.Append(";1;1;");       //Intsec no & Rack no
            line.Append(uitgang.index+1 + ";");
            line.Append(uitgang.signalControl + "-000;");
            line.Append(";;;1;");       //Option & Option 2 & Intergreen Conflict & Red when Conflict
            line.Append("02.0-00.0;");  //MinR lim
            line.Append(";");           //RY limit
            #region MinG lim
            if (uitgang.minGreen == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.minGreen / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + "-00.0;");
            }
            #endregion
            line.Append(";");       //Max Green
            #region FlaG lim
            if (uitgang.greenFlash == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.greenFlash / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + "-00.0;");
            }
            #endregion
            #region MinA lim
            if (uitgang.minAmber == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.minAmber / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + "-00.0;");
            }
            #endregion
            #region Max Amber
            if (uitgang.maxGeelTijd == 0)
                line.Append(";");
            else
            {
                double temp = uitgang.maxGeelTijd / 10.0;
                string write = temp.ToString("F1");
                line.Append(write + ";");
            }
            #endregion

            return line.ToString();
        }
    }
}