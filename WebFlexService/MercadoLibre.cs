using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFlexService
{
    //Recepcion POST de Mensajes desde Mercado Libre
    public class MensajeMLA
    {
        public int user_id { get; set; }
        public string resource { get; set; }
        public string topic { get; set; }
        public string received { get; set; }
        public int application_id { get; set; }
        public string send { get; set; }
        public int attemps { get; set; }

    }
}