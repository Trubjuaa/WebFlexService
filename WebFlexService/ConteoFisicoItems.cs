using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFlexService
{
    public class ConteoFisicoItems

    {

        public string DobleUnidad { get; set; }
        public string LLevaPartida { get; set; }
        public Int64 RowID { get; set; }
        public string art_DescGen { get; set; }
        public string cfs_CodDep { get; set; }
                
        public float cfs_RecuentoUM1 { get; set; }
        public float cfs_RecuentoUM2 { get; set; }
        public string cfsart_CodEle1 { get; set; }
        public string cfsart_CodEle2 { get; set; }
        public string cfsart_CodEle3 { get; set; }
        
        public string cfsart_CodGen {get;set;}
        public Int64 cfsccs_Id { get; set; }
        
        public string cfsstp_Partida {get;set;}
        
        //public string art_CodBarras { get; set; }
    }
}