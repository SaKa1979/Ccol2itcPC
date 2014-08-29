using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCOL2iTCPC
{
    class FasecyclusUitgang
    {
        public string id                { get; set; }
        public int    code              { get; set; }
        public faseType type            { get; set; }
        public int    index             { get; set; }
        public int    maxGeelTijd       { get; set; }

        ///////////////////////
        //onderstaande attributen zijn op volgorde van hoe het in het programma ITC-PC ingevuld staat
        ///////////////////////

        public string signalStart       { get; set; }
        public string signalControl     { get; set; }
        public string signalDisable     { get; set; }
        public string signalMgrp        { get; set; }
        public string masterGrp         { get; set; }
        public string mainRoad          { get; set; }
        public string minimize          { get; set; }
        public string red_Amber         { get; set; }
        public int    minGreen          { get; set; }
        public int    greenFlash        { get; set; }
        public int    minAmber          { get; set; }
        public int    varAmber          { get; set; }
        public int    minRed            { get; set; }
        public string varRedMode        { get; set; }
        public string varRed2           { get; set; }
        public string reservation       { get; set; }
        public string stageHoldTime     { get; set; }
        public string delayMode         { get; set; }
        public string requestPh         { get; set; }
        public string DIRinput          { get; set; }
        public string ontimeDIRinput    { get; set; }
        public string reqStartup        { get; set; }
        public string fixedControl      { get; set; }
        public string prio1_Mode1       { get; set; }
        public string prio1_Mode2       { get; set; }
        public string prio2_Mode1       { get; set; }
        public string prio2_Mode2       { get; set; }
        public string prio3_Inp         { get; set; }
        public string prio3_priority    { get; set; }
        public string prio3_Mode1       { get; set; }
        public string prio3_Mode2       { get; set; }
        public string stDelay_1         { get; set; }
        public string stDelay_2         { get; set; }
        public string stDelay_3         { get; set; }
        public string st_d_4Inp         { get; set; }
        public string relation          { get; set; }

        private bool tgglset =  false;

        public FasecyclusUitgang(string id)
        { this.id = id; }

        public bool TGGLset
        {
            get
            { return tgglset; }
            set
            { tgglset = value; }
        }
    }
}
