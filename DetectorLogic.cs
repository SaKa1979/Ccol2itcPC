using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCOL2iTCPC
{
    class DetectorLogic
    {
        public int      index               { get; set; }
        public string   code                { get; set; }
        public string   type                { get; set; }

        ///////////////////////
        //onderstaande attributen zijn op volgorde van hoe het in het programma ITC-PC ingevuld staat
        ///////////////////////

        public string   id                  { get; set; }
        public string   phaseNo             { get; set; }
        public string   input               { get; set; }
        public string   enableInp           { get; set; }
        public string   reqMode             { get; set; }
        public string   reqCancel           { get; set; }
        public string   reqDelay            { get; set; }
        public string   delayNewReq         { get; set; }
        public string   returnToGreen       { get; set; }
        public string   reserveIntv         { get; set; }
        public string   minGrnMode1         { get; set; }
        public string   minGrnMode2         { get; set; }
        public string   reextIntvMode       { get; set; }
        public string   maxGrnIntv1         { get; set; }
        public string   maxGrnIntv2         { get; set; }
        public string   reductIntv          { get; set; }
        public string   delayedReduct       { get; set; }
        public string   PEGintv             { get; set; }
        public string   disableTime         { get; set; }
        public string   amberIntv           { get; set; }
        public string   redIntv             { get; set; }
        public string   output              { get; set; }
        public string   trafficCounting     { get; set; }
        public int      onTime              { get; set; }
        public int      offTime             { get; set; }
        public string   errorMode           { get; set; }
        public string   intsecNo            { get; set; }
        public string   option              { get; set; }
        public string   auxPar              { get; set; }

        public DetectorLogic(string id)
        { this.id = id; }

        
    }
}