using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFlexService
{
    public class ClientesMobiliza
    {

        /*
         * cli_razsoc     varchar(40) Razon Social
         * cli_direc      varchar(30) Direccion
         * cli_loc        varchar(25) Localidad
         * cliprv_codigo  varchar(3)  Codigo Provincia  Tabla Asociada prv
         * cli_codpos     varchar(8)  Codigo Postal
         * clisiv_cod     varchar(1)  Sit. IVA
         * clitdc_cod     smallint    Tipo Documento
         * cli_cuit       varchar(11) CUIT
         * clisib_cod     varchar(1)  Sit. IIBB
         * cli_nroib      varchar(15) Nro IIBB
         * clisig_cod     varchar(1)  Sit. Ganacias
         * clidc1_cod     varchar(4)  SI Cod. Unidad Negocio   Tabla defi1cli
         * clidc2_cod     varchar(4)  SI Cod. Canal            Tabla defi2cli
         * clidlp_cod     varchar(3)  SI Cod. Lista Precios    Tabla DefListP
         * clicvt_cod     varchar(3)  SI Cod. Cond. Venta      Tabla CondVta
         * cliven_Cod     varchar(4)  SI Cod. Vendedor         Tabla Vendedor
         * cli_habilitado bit         Habilitado (0-False 1-True)
         * clizon_Cod     varchar(4)  SI Cod. Zona             Tabla Zona
         * */

        public string cli_cod { get; set; }
        public string cli_razsoc { get; set; }
        public string cli_direc { get; set; }
        public string cli_loc { get; set; }
        public string cliprv_codigo { get; set; }
        public string cli_codpos { get; set; }
        public string clisiv_cod { get; set; }
        public int clitdc_cod { get; set; }
        public string cli_cuit { get; set; }
        public string clisib_cod { get; set; }
        public string cli_nroIB { get; set; }
        public string clisig_cod { get; set; }
        public string clidc1_cod { get; set; }
        public string clidc2_cod { get; set; }
        public string clidlp_cod { get; set; }
        public string clicvt_cod { get; set; }
        public bool cli_habilitado { get; set; }
        public string clizon_Cod { get; set; }
        public string cliven_Cod { get; set; }
        public string cli_Nota { get; set; }
        public string cli_Tel { get; set; }

    }
}