using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
// agregar esta linea para enviar los encabezados SOAP
// dando seguridad al sitio
using System.Web.Services.Protocols;
////
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using SB.NET.eFlex.SDKLib;
using SB.NET.eFlex.SDKLib.Objs;
using System.Diagnostics;


namespace WebFlexService
{
    /// <summary>
    /// Descripción breve de WebService1
    /// </summary>
    ///         
    //   

    

    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "http://juan/WebServiceFlex/")]    
  //  [WebService(Namespace = "http://casesa.serveftp.com:8024/WebServiceFlex/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    
    //public class WebService1 : System.Web.Services.WebService
    public class eFlexwareWebService : System.Web.Services.WebService

       
        
    {
        // Aqui instancio el Proxy del WEB Service para agregar la seguridad        
        // WebService1 service = new WebService1(); 

        // defino el usuario para autenticacion SOAP	     
        public UserDetails UserDetails { get; set; }
        public UserDetails User;       
        // hasta aqui definicion         

               
        [WebMethod]
        // agrego el SoapHeader para dar seguridad	
        [SoapHeader("User", Required = true)]
        public DataSet getUsuario()
        {

            if (ControlaAcceso())
            {                
                    string sql = "SELECT [usu_codigo],[usu_nombre],[usu_fallos] FROM usu";
                    string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                    SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                    string miDatoSource = ConfigurationManager.AppSettings["DataSource"];
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return ds;
                
            }
            else
            {
                DataSet ds = new DataSet();
                return ds;
            }                
        }        

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string getUsuarioJSON()
        {
            if (ControlaAcceso())
            {
                string sql = "SELECT [usu_codigo],[usu_nombre],[usu_fallos] FROM usu";
                string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(dt);

                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return serializer.Serialize(rows);

            }
            else
            {
                string json = "[{\"ACCESO\":\"DEBE INDICAR USUARIO Y CONTRASEÑA\"}]";
                return json;
            }   
        }    

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string IngresarComprobanteVentasJSON(string json)
        {
            string IPClient = HttpContext.Current.Request.UserHostAddress;


            if (ControlaAcceso())
            {
                
                bool lOK = true;
                string resultado;
                resultado = "";
                int t = 0;

                // JAT -09-11-2018 Agrego este control antes del envio al SDK. Si existe en comprobante lo verifico por fuera del SDK y devuelvo un OK.
                bool lNoExiste = true;
                // Inicio la coneccion                                     
                string vconectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                SqlConnection vconexion = new SqlConnection(vconectionstring);

                // Recorro la lista recibida para hacer los cambios en la base
                //                         for (int i = 0; i < items.Count; i++)
                //                         {
                //abro la conexion para actualizar los datos.
                /*
                if (HabilitarLog)
                {
                    EventLog.WriteEntry(sSource, "Inicio la grabacion del Log", EventLogEntryType.Information, 233);
                }               
                */

                vconexion.Open();
                EFlexSDK_ComprobanteVentas ComprobanteaValidar = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);
                string ClaveaValidar = ComprobanteaValidar.Comprobante_Tipo.PadRight(3, ' ') + ComprobanteaValidar.Comprobante_Letra.PadRight(1, ' ') + ComprobanteaValidar.Comprobante_PtoVenta.PadRight(4, ' ') + ComprobanteaValidar.Comprobante_Numero + ComprobanteaValidar.Cliente_Codigo.PadRight(6, ' ') + ComprobanteaValidar.Comprobante_FechaEmision.Year.ToString().Trim() + ComprobanteaValidar.Comprobante_FechaEmision.Month.ToString().Trim().PadLeft(2, '0') + ComprobanteaValidar.Comprobante_FechaEmision.Day.ToString().Trim().PadLeft(2, '0');

                // Si el SDK me informó alta OK verifico que exista el comprobante en Flex sino devuelvo un error
                string vtipoComprobante = ClaveaValidar.Substring(0, 3);
                string vLetra = ClaveaValidar.Substring(3, 1);
                string vPuntoVenta = ClaveaValidar.Substring(4, 4);
                string vNroComprobante = ClaveaValidar.Substring(8, 8);
                string vCliente = ClaveaValidar.Substring(16, 6);
                string vFechaEmision = ClaveaValidar.Substring(22, 8);

                string vsql = "SELECT * FROM WEB_ExisteComprobante Where Comprobante='" + vtipoComprobante + "' and Letra='" + vLetra + "' and PtoVta='" + vPuntoVenta + "' and numero='" + vNroComprobante + "' and cli_cod='" + vCliente + "' and fechaemision='" + vFechaEmision + "'";

                SqlDataAdapter vda = new SqlDataAdapter(vsql, vconectionstring);
                DataSet vds = new DataSet();
                DataTable vdt = new DataTable();
                vda.Fill(vdt);
                Int32 vcuenta = vdt.Rows.Count;

                if (vcuenta > 0)
                {
                    // Como existe, salteo el alta por el SDK asi es mas rapido
                    lNoExiste = false;
                    resultado = "OK:" + vtipoComprobante+vLetra+vPuntoVenta+vNroComprobante+vCliente+vFechaEmision+" Comprobante ya ingresado a Flex."  ;
                }
                else
                {
                    // Como no existe lo dejo true para que lo haga el alta
                    lNoExiste = true;
                }
                // Como ya habia sido ingresado no dejo log en WEBServiceIntegra

                if ( lNoExiste )
                {
                    // Abro la conexion a la base de Flex
                    EFlexSDK_TokenValidacion pToken = null;

                    //EFlexSDK_TokenValidacion pToken = null;
                    string xCodigoEmpresa = ConfigurationManager.AppSettings["CodEmpresa"];
                    string xCodigoPuesto = ConfigurationManager.AppSettings["CodPuesto"];
                    // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                    //string xCodigoUsuario = ConfigurationManager.AppSettings["CodUsuario"];
                    string xCodigoUsuario = "CASE";
                    // Diferencia Maxima de Ajuste
                    Double xAjustarDiferenciaMax = Convert.ToDouble(ConfigurationManager.AppSettings["AjustarDiferenciaMax"]);

                    string xClaveUsuario = "Case2273";

                    string sql = "";
                    try
                    {

                        pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);

                        if (pToken == null)
                        {
                            lOK = false;
                            resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();

                            /*
                            //Se escribe un error
                             if (HabilitarLog)
                             {
                                 EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Error, 231);
                             }
                             if (HabilitarFileLog)
                             {                                 
                                 log.WriteLine(resultado);
                                 log.Flush();
                                 log.Close();
                             }     
                             */

                        }
                        else
                        // Abro la empresa de Flex
                        {

                            if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                            {
                                lOK = false;
                                resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                                /*
                                if (HabilitarLog)
                                {
                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Error, 231);
                                }
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine(resultado);
                                    log.Flush();
                                    log.Close();
                                } 
                                 */
                            }

                        }
                        if (lOK)
                        {
                            try
                            {
                                EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                                // Esta linea habilita el Ajuste por redondeo
                                ventas.HabilitarAjusteComprobantes(xAjustarDiferenciaMax);
                                //EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();

                                //       IEFlexSDK_ComprobanteVentas xCompNew = ventas.IngresarComprobanteJSON(json, "N", "R", pToken);


                                EFlexSDK_ComprobanteVentas xCompV = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);
                                // Utilizo la opcion "S" y "E" para indicar que es un nuevo comprobante en Flex.
                                /*
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Intento Ingresar el comprobante ");
                                    log.Flush();
                                    //log.Close();
                                } 
                                 */

                                IEFlexSDK_ComprobanteVentas xCompNew = ventas.IngresarComprobante(xCompV, "N", "R", pToken);
                                //anulo la linea de arriba
                                //EFlexSDK_ComprobanteVentas xCompNew = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);


                                //ventas.IngresarComprobante(xCompV, "N", "R", pToken);      
                                /*
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Ya ingresé el comprobante");
                                    log.Flush();
                                    //log.Close();
                                }     
                                                                
                 
                                if (HabilitarLog)
                                {
                                    EventLog.WriteEntry(sSource, "Ya ingreso el comprobante y ahora obtendo el resultado", EventLogEntryType.Information, 233);
                                }

                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Intento obtener los errores");
                                    log.Flush();
                                    //log.Close();
                                } 

                                 */
                                resultado = ventas.ObtenerListaErrores();

                                /*
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Obtuve el error");
                                    log.Flush();                    
                                    log.WriteLine(resultado);
                                    log.Flush();
                                    log.Close();
                                } 
                   
                                if (HabilitarLog)
                                {
                                   EventLog.WriteEntry(sSource, "Ya obtuve el resultado, va a continuacion", EventLogEntryType.Information, 233);
                                   EventLog.WriteEntry(sSource, resultado , EventLogEntryType.Information, 233);
                                   EventLog.WriteEntry(sSource, "Te mostre el resultado", EventLogEntryType.Information, 233);                 
                                }                                               
                                */

                                bool AltaOK = resultado.Contains("El comprobante se importó correctamente");

                                // Inicializo variables
                                string clave = "";
                                string ComprobanteNuevo = "";
                                Int64 IdFlex = 0;

                                clave = xCompV.Comprobante_Tipo.PadRight(3, ' ') + xCompV.Comprobante_Letra.PadRight(1, ' ') + xCompV.Comprobante_PtoVenta.PadRight(4, ' ') + xCompV.Comprobante_Numero + xCompV.Cliente_Codigo.PadLeft(6, '0') + xCompV.Comprobante_FechaEmision.Year.ToString().Trim() + xCompV.Comprobante_FechaEmision.Month.ToString().Trim().PadLeft(2, '0') + xCompV.Comprobante_FechaEmision.Day.ToString().Trim().PadLeft(2, '0');

                                // Cambios 10/05/2018 en libreria WebService, ahora si el comprobante se importa OK no devuelve nada en resultado
                                if (xCompNew != null)
                                {
                                    AltaOK = true;

                                    // el xCompNew me devuelve casi todos los valores Null, entonces le coloco los valores pasado porque es registracion
                                    // ComprobanteNuevo = xCompNew.Comprobante_Tipo.PadRight(3, ' ') + xCompNew.Comprobante_Letra + xCompNew.Comprobante_PtoVenta + xCompNew.Comprobante_Numero + xCompNew.Cliente_Codigo.PadRight(6, ' '); // xCompV.Comprobante_FechaEmision;
                                    ComprobanteNuevo = clave;
                                    IdFlex = xCompNew.Comprobante_ID;
                                }
                                else
                                {
                                    AltaOK = false;
                                    if (AltaOK)
                                    {
                                        // Si es registracion tambien devuelve null
                                        ComprobanteNuevo = clave;
                                    }
                                    ComprobanteNuevo = "Se Produjo un error en la importacion";
                                }

                                //////
                                try
                                {
                                    // Inicio la coneccion                                     
                                    string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                                    SqlConnection conexion = new SqlConnection(conectionstring);

                                    // Recorro la lista recibida para hacer los cambios en la base
                                    //                         for (int i = 0; i < items.Count; i++)
                                    //                         {
                                    //abro la conexion para actualizar los datos.
                                    /*
                                    if (HabilitarLog)
                                    {
                                        EventLog.WriteEntry(sSource, "Inicio la grabacion del Log", EventLogEntryType.Information, 233);
                                    }               
                                    */

                                    conexion.Open();

                                    //if (AltaOK)
                                    //{
                                        // Si el SDK me informó alta OK verifico que exista el comprobante en Flex sino devuelvo un error
                                        string tipoComprobante = clave.Substring(0, 3);
                                        string Letra = clave.Substring(3, 1);
                                        string PuntoVenta = clave.Substring(4, 4);
                                        string NroComprobante = clave.Substring(8, 8);
                                        string Cliente = clave.Substring(16, 6);
                                        string FechaEmision = clave.Substring(22, 8);

                                        sql = "SELECT * FROM WEB_ExisteComprobante Where Comprobante='" + tipoComprobante + "' and Letra='" + Letra + "' and PtoVta='" + PuntoVenta + "' and numero='" + NroComprobante + "' and cli_cod='" + Cliente + "' and fechaemision='" + FechaEmision + "'";

                                        SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                                        DataSet ds = new DataSet();
                                        DataTable dt = new DataTable();
                                        da.Fill(dt);
                                        Int32 cuenta = dt.Rows.Count;

                                        if (cuenta > 0)
                                        {
                                            AltaOK = true;
                                        }
                                        else
                                        {
                                            AltaOK = false;
                                        }
                                    //}

                                    // Cambio JAT 05/11/2018 Verifico si ingresó sino le coloco AltaOK=False
                                    //if (xCompNew != null || AltaOK)
                                    if (AltaOK)
                                    {
                                        // Dio de Alta OK el Comprobante
                                        sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave,OK,Mensaje,IdFlex,ComprobanteFlex) Values (@ComprobanteJSON,@Clave,1,@Mensaje,@IdFlex,@ComprobanteFlex)";
                                    }
                                    else
                                    {
                                        // Hubo un error en el alta
                                        sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave,OK,Mensaje,IdFlex,ComprobanteFlex) Values (@ComprobanteJSON,@Clave,0,@Mensaje,@IdFlex,@ComprobanteFlex)";
                                    }

                                    SqlCommand sqlcom = new SqlCommand(sql, conexion);
                                    using (sqlcom)
                                    {
                                        // Cargo los valores de los parametros     
                                        // La Fecha la coloco con un trigger
                                        sqlcom.Parameters.Add(new SqlParameter("@ComprobanteJSON", json));
                                        sqlcom.Parameters.Add(new SqlParameter("@Clave", clave));
                                        sqlcom.Parameters.Add(new SqlParameter("@Mensaje", resultado));
                                        sqlcom.Parameters.Add(new SqlParameter("@IdFlex", IdFlex));
                                        sqlcom.Parameters.Add(new SqlParameter("@ComprobanteFlex", ComprobanteNuevo));
                                        try
                                        {
                                            // Ejecuto y contabilizo los registros afectados
                                            t = t + sqlcom.ExecuteNonQuery();
                                            if (t > 0)
                                            {
                                                t = 1;
                                            }
                                            conexion.Close();

                                            //if (xCompNew != null)
                                            if (AltaOK)
                                            {
                                                resultado = "OK:" + ComprobanteNuevo;
                                                /*                             
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                } 
                                                 */
                                            }
                                            else
                                            {
                                                resultado = "ERROR:" + resultado;
                                                /*
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                } 
                                                 **/

                                            }
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);

                                        }
                                        catch
                                        {
                                            // Se produjo un error
                                            //if (xCompNew != null)
                                            if (AltaOK)
                                            {
                                                resultado = "OK:" + ComprobanteNuevo;
                                                /*
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                }               
                                */
                                            }
                                            else
                                            {
                                                resultado = "ERROR:" + resultado;
                                                /*
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                } 
                                                 */
                                            }
                                            conexion.Close();
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                    EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                                    resultado = "ERROR: " + ex.ToString();
                                    /*
                                      if (HabilitarLog)
                                      {
                                          EventLog.WriteEntry(sSource, "Hubo un error en la grabacion sali por catch", EventLogEntryType.Information, 233);
                                      }               
                                     */
                                }
                            }
                            catch (Exception ex)
                            {
                                resultado = "ERROR: "+ex.ToString();
                                /*
                                if (HabilitarLog)
                                {
                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                } 
                                 */
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado = "ERROR: "+ex.ToString();
                        /*
                        if (HabilitarLog)
                        {
                            EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                        } 
                         */
                    }
                    /*
                    if (HabilitarLog)
                    {
                        EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                    } 
                     */
                }
                
                return resultado + IPClient; 
            }
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
             
                return resultado;
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string IngresarCobranzaJSON(string json)
        {
            string IPClient = HttpContext.Current.Request.UserHostAddress;


            if (ControlaAcceso())
            {

                bool lOK = true;
                string resultado;
                resultado = "";
                int t = 0;

                // JAT -09-11-2018 Agrego este control antes del envio al SDK. Si existe en comprobante lo verifico por fuera del SDK y devuelvo un OK.
                bool lNoExiste = true;
                // Inicio la coneccion                                     
                string vconectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                SqlConnection vconexion = new SqlConnection(vconectionstring);

                // Recorro la lista recibida para hacer los cambios en la base
                //                         for (int i = 0; i < items.Count; i++)
                //                         {
                //abro la conexion para actualizar los datos.
                /*
                if (HabilitarLog)
                {
                    EventLog.WriteEntry(sSource, "Inicio la grabacion del Log", EventLogEntryType.Information, 233);
                }               
                */

                vconexion.Open();
                EFlexSDK_ComprobanteVentas ComprobanteaValidar = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);
                string ClaveaValidar = ComprobanteaValidar.Comprobante_Tipo.PadRight(3, ' ') + ComprobanteaValidar.Comprobante_Letra.PadRight(1, ' ') + ComprobanteaValidar.Comprobante_PtoVenta.PadRight(4, ' ') + ComprobanteaValidar.Comprobante_Numero + ComprobanteaValidar.Cliente_Codigo.PadRight(6, ' ') + ComprobanteaValidar.Comprobante_FechaEmision.Year.ToString().Trim() + ComprobanteaValidar.Comprobante_FechaEmision.Month.ToString().Trim().PadLeft(2, '0') + ComprobanteaValidar.Comprobante_FechaEmision.Day.ToString().Trim().PadLeft(2, '0');

                // Si el SDK me informó alta OK verifico que exista el comprobante en Flex sino devuelvo un error
                string vtipoComprobante = ClaveaValidar.Substring(0, 3);
                string vLetra = ClaveaValidar.Substring(3, 1);
                string vPuntoVenta = ClaveaValidar.Substring(4, 4);
                string vNroComprobante = ClaveaValidar.Substring(8, 8);
                string vCliente = ClaveaValidar.Substring(16, 6);
                string vFechaEmision = ClaveaValidar.Substring(22, 8);

                string vsql = "SELECT * FROM WEB_ExisteRecibo Where Comprobante='" + vtipoComprobante + "' and Letra='" + vLetra + "' and PtoVta='" + vPuntoVenta + "' and numero='" + vNroComprobante + "' and cli_cod='" + vCliente + "' and fechaemision='" + vFechaEmision + "'";

                SqlDataAdapter vda = new SqlDataAdapter(vsql, vconectionstring);
                DataSet vds = new DataSet();
                DataTable vdt = new DataTable();
                vda.Fill(vdt);
                Int32 vcuenta = vdt.Rows.Count;

                if (vcuenta > 0)
                {
                    // Como existe, salteo el alta por el SDK asi es mas rapido
                    lNoExiste = false;
                    resultado = "OK:" + vtipoComprobante + vLetra + vPuntoVenta + vNroComprobante + vCliente + vFechaEmision + " Comprobante ya ingresado a Flex.";
                }
                else
                {
                    // Como no existe lo dejo true para que lo haga el alta
                    lNoExiste = true;
                }
                // Como ya habia sido ingresado no dejo log en WEBServiceIntegra

                if (lNoExiste)
                {
                    // Abro la conexion a la base de Flex
                    EFlexSDK_TokenValidacion pToken = null;

                    //EFlexSDK_TokenValidacion pToken = null;
                    string xCodigoEmpresa = ConfigurationManager.AppSettings["CodEmpresa"];
                    string xCodigoPuesto = ConfigurationManager.AppSettings["CodPuesto"];
                    // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                    //string xCodigoUsuario = ConfigurationManager.AppSettings["CodUsuario"];
                    string xCodigoUsuario = "CASE";
                    // Diferencia Maxima de Ajuste
                    Double xAjustarDiferenciaMax = Convert.ToDouble(ConfigurationManager.AppSettings["AjustarDiferenciaMax"]);

                    string xClaveUsuario = "Case2273";

                    string sql = "";
                    try
                    {

                        pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);

                        if (pToken == null)
                        {
                            lOK = false;
                            resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();

                            /*
                            //Se escribe un error
                             if (HabilitarLog)
                             {
                                 EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Error, 231);
                             }
                             if (HabilitarFileLog)
                             {                                 
                                 log.WriteLine(resultado);
                                 log.Flush();
                                 log.Close();
                             }     
                             */

                        }
                        else
                        // Abro la empresa de Flex
                        {

                            if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                            {
                                lOK = false;
                                resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                                /*
                                if (HabilitarLog)
                                {
                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Error, 231);
                                }
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine(resultado);
                                    log.Flush();
                                    log.Close();
                                } 
                                 */
                            }

                        }
                        if (lOK)
                        {
                            try
                            {
                                EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                                // Esta linea habilita el Ajuste por redondeo
                                ventas.HabilitarAjusteComprobantes(xAjustarDiferenciaMax);
                                //EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();

                                //       IEFlexSDK_ComprobanteVentas xCompNew = ventas.IngresarComprobanteJSON(json, "N", "R", pToken);


                                EFlexSDK_ComprobanteVentas xCompV = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);
                                // Utilizo la opcion "S" y "E" para indicar que es un nuevo comprobante en Flex.
                                /*
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Intento Ingresar el comprobante ");
                                    log.Flush();
                                    //log.Close();
                                } 
                                 */

                                IEFlexSDK_ComprobanteVentas xCompNew = ventas.IngresarComprobante(xCompV, "N", "R", pToken);
                                //anulo la linea de arriba
                                //EFlexSDK_ComprobanteVentas xCompNew = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);


                                //ventas.IngresarComprobante(xCompV, "N", "R", pToken);      
                                /*
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Ya ingresé el comprobante");
                                    log.Flush();
                                    //log.Close();
                                }     
                                                                
                 
                                if (HabilitarLog)
                                {
                                    EventLog.WriteEntry(sSource, "Ya ingreso el comprobante y ahora obtendo el resultado", EventLogEntryType.Information, 233);
                                }

                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Intento obtener los errores");
                                    log.Flush();
                                    //log.Close();
                                } 

                                 */
                                resultado = ventas.ObtenerListaErrores();

                                /*
                                if (HabilitarFileLog)
                                {
                                    log.WriteLine("Obtuve el error");
                                    log.Flush();                    
                                    log.WriteLine(resultado);
                                    log.Flush();
                                    log.Close();
                                } 
                   
                                if (HabilitarLog)
                                {
                                   EventLog.WriteEntry(sSource, "Ya obtuve el resultado, va a continuacion", EventLogEntryType.Information, 233);
                                   EventLog.WriteEntry(sSource, resultado , EventLogEntryType.Information, 233);
                                   EventLog.WriteEntry(sSource, "Te mostre el resultado", EventLogEntryType.Information, 233);                 
                                }                                               
                                */

                                bool AltaOK = resultado.Contains("El comprobante se importó correctamente");

                                // Inicializo variables
                                string clave = "";
                                string ComprobanteNuevo = "";
                                Int64 IdFlex = 0;

                                clave = xCompV.Comprobante_Tipo.PadRight(3, ' ') + xCompV.Comprobante_Letra.PadRight(1, ' ') + xCompV.Comprobante_PtoVenta.PadRight(4, ' ') + xCompV.Comprobante_Numero + xCompV.Cliente_Codigo.PadLeft(6, '0') + xCompV.Comprobante_FechaEmision.Year.ToString().Trim() + xCompV.Comprobante_FechaEmision.Month.ToString().Trim().PadLeft(2, '0') + xCompV.Comprobante_FechaEmision.Day.ToString().Trim().PadLeft(2, '0');

                                // Cambios 10/05/2018 en libreria WebService, ahora si el comprobante se importa OK no devuelve nada en resultado
                                if (xCompNew != null)
                                {
                                    AltaOK = true;

                                    // el xCompNew me devuelve casi todos los valores Null, entonces le coloco los valores pasado porque es registracion
                                    // ComprobanteNuevo = xCompNew.Comprobante_Tipo.PadRight(3, ' ') + xCompNew.Comprobante_Letra + xCompNew.Comprobante_PtoVenta + xCompNew.Comprobante_Numero + xCompNew.Cliente_Codigo.PadRight(6, ' '); // xCompV.Comprobante_FechaEmision;
                                    ComprobanteNuevo = clave;
                                    IdFlex = xCompNew.Comprobante_ID;
                                }
                                else
                                {
                                    AltaOK = false;
                                    if (AltaOK)
                                    {
                                        // Si es registracion tambien devuelve null
                                        ComprobanteNuevo = clave;
                                    }
                                    ComprobanteNuevo = "Se Produjo un error en la importacion";
                                }

                                //////
                                try
                                {
                                    // Inicio la coneccion                                     
                                    string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                                    SqlConnection conexion = new SqlConnection(conectionstring);

                                    // Recorro la lista recibida para hacer los cambios en la base
                                    //                         for (int i = 0; i < items.Count; i++)
                                    //                         {
                                    //abro la conexion para actualizar los datos.
                                    /*
                                    if (HabilitarLog)
                                    {
                                        EventLog.WriteEntry(sSource, "Inicio la grabacion del Log", EventLogEntryType.Information, 233);
                                    }               
                                    */

                                    conexion.Open();

                                    //if (AltaOK)
                                    //{
                                    // Si el SDK me informó alta OK verifico que exista el comprobante en Flex sino devuelvo un error
                                    string tipoComprobante = clave.Substring(0, 3);
                                    string Letra = clave.Substring(3, 1);
                                    string PuntoVenta = clave.Substring(4, 4);
                                    string NroComprobante = clave.Substring(8, 8);
                                    string Cliente = clave.Substring(16, 6);
                                    string FechaEmision = clave.Substring(22, 8);

                                    sql = "SELECT * FROM WEB_ExisteRecibo Where Comprobante='" + tipoComprobante + "' and Letra='" + Letra + "' and PtoVta='" + PuntoVenta + "' and numero='" + NroComprobante + "' and cli_cod='" + Cliente + "' and fechaemision='" + FechaEmision + "'";

                                    SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable();
                                    da.Fill(dt);
                                    Int32 cuenta = dt.Rows.Count;

                                    if (cuenta > 0)
                                    {
                                        AltaOK = true;
                                    }
                                    else
                                    {
                                        AltaOK = false;
                                    }
                                    //}

                                    // Cambio JAT 05/11/2018 Verifico si ingresó sino le coloco AltaOK=False
                                    //if (xCompNew != null || AltaOK)
                                    if (AltaOK)
                                    {
                                        // Dio de Alta OK el Comprobante
                                        sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave,OK,Mensaje,IdFlex,ComprobanteFlex) Values (@ComprobanteJSON,@Clave,1,@Mensaje,@IdFlex,@ComprobanteFlex)";
                                    }
                                    else
                                    {
                                        // Hubo un error en el alta
                                        sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave,OK,Mensaje,IdFlex,ComprobanteFlex) Values (@ComprobanteJSON,@Clave,0,@Mensaje,@IdFlex,@ComprobanteFlex)";
                                    }

                                    SqlCommand sqlcom = new SqlCommand(sql, conexion);
                                    using (sqlcom)
                                    {
                                        // Cargo los valores de los parametros     
                                        // La Fecha la coloco con un trigger
                                        sqlcom.Parameters.Add(new SqlParameter("@ComprobanteJSON", json));
                                        sqlcom.Parameters.Add(new SqlParameter("@Clave", clave));
                                        sqlcom.Parameters.Add(new SqlParameter("@Mensaje", resultado));
                                        sqlcom.Parameters.Add(new SqlParameter("@IdFlex", IdFlex));
                                        sqlcom.Parameters.Add(new SqlParameter("@ComprobanteFlex", ComprobanteNuevo));
                                        try
                                        {
                                            // Ejecuto y contabilizo los registros afectados
                                            t = t + sqlcom.ExecuteNonQuery();
                                            if (t > 0)
                                            {
                                                t = 1;
                                            }
                                            conexion.Close();

                                            //if (xCompNew != null)
                                            if (AltaOK)
                                            {
                                                resultado = "OK:" + ComprobanteNuevo;
                                                /*                             
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                } 
                                                 */
                                            }
                                            else
                                            {
                                                resultado = "ERROR:" + resultado;
                                                /*
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                } 
                                                 **/

                                            }
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);

                                        }
                                        catch
                                        {
                                            // Se produjo un error
                                            //if (xCompNew != null)
                                            if (AltaOK)
                                            {
                                                resultado = "OK:" + ComprobanteNuevo;
                                                /*
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                }               
                                */
                                            }
                                            else
                                            {
                                                resultado = "ERROR:" + resultado;
                                                /*
                                                if (HabilitarLog)
                                                {
                                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                                } 
                                                 */
                                            }
                                            conexion.Close();
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                            EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                    EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                                    resultado = "ERROR: " + ex.ToString();
                                    /*
                                      if (HabilitarLog)
                                      {
                                          EventLog.WriteEntry(sSource, "Hubo un error en la grabacion sali por catch", EventLogEntryType.Information, 233);
                                      }               
                                     */
                                }
                            }
                            catch (Exception ex)
                            {
                                resultado = "ERROR: " + ex.ToString();
                                /*
                                if (HabilitarLog)
                                {
                                    EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                                } 
                                 */
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado = "ERROR: " + ex.ToString();
                        /*
                        if (HabilitarLog)
                        {
                            EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                        } 
                         */
                    }
                    /*
                    if (HabilitarLog)
                    {
                        EventLog.WriteEntry(sSource, resultado, EventLogEntryType.Information, 233);
                    } 
                     */
                }

                return resultado + IPClient;
            }
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";

                return resultado;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string IngresarComprobanteFranquiciaJSON(string json)
        {
            string IPClient = HttpContext.Current.Request.UserHostAddress;
            if (ControlaAcceso())
            {
                bool lOK = true;
                string resultado;
                resultado = "";
                int t = 0;

                // Abro la conexion a la base de Flex
                EFlexSDK_TokenValidacion pToken = null;

                //EFlexSDK_TokenValidacion pToken = null;
                string xCodigoEmpresa = ConfigurationManager.AppSettings["CodFranquicia"];
                string xCodigoPuesto = ConfigurationManager.AppSettings["CodPuesto"];
                // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                //string xCodigoUsuario = ConfigurationManager.AppSettings["CodUsuario"];
                string xCodigoUsuario = "CASE";
                string xClaveUsuario = "Case2273";

                Double xAjustarDiferenciaMax = Convert.ToDouble(ConfigurationManager.AppSettings["AjustarDiferenciaMax"]);

                string sql = "";
                try
                {

                    pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);

                    if (pToken == null)
                    {
                        lOK = false;
                        resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                    }
                    else
                    // Abro la empresa de Flex
                    {


                        if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                        {
                            lOK = false;
                            resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                        }

                    }
                    if (lOK)
                    {
                        try
                        {
                            EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                            //EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();


                            ventas.HabilitarAjusteComprobantes(xAjustarDiferenciaMax);

                            EFlexSDK_ComprobanteVentas xCompV = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);
                            // Utilizo la opcion "S" y "E" para indicar que es un nuevo comprobante en Flex.

                            IEFlexSDK_ComprobanteVentas xCompNew = ventas.IngresarComprobante(xCompV, "N", "R", pToken);

                            resultado = ventas.ObtenerListaErrores();

                            bool AltaOK = resultado.Contains("El comprobante se importó correctamente");
                            //////
                            try
                            {
                                string clave = "";
                                string ComprobanteNuevo = "";
                                Int32 IdFlex = 0;
                                //EFlexSDK_ComprobanteVentas xCompV = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);

                                clave = xCompV.Comprobante_Tipo.PadRight(3, ' ') + xCompV.Comprobante_Letra + xCompV.Comprobante_PtoVenta + xCompV.Comprobante_Numero + xCompV.Cliente_Codigo.PadRight(6, ' ') + xCompV.Comprobante_FechaEmision;

                                if (xCompNew != null)
                                {
                                    ComprobanteNuevo = xCompNew.Comprobante_Tipo.PadRight(3, ' ') + xCompNew.Comprobante_Letra + xCompNew.Comprobante_PtoVenta + xCompNew.Comprobante_Numero + xCompNew.Cliente_Codigo.PadRight(6, ' ') + xCompV.Comprobante_FechaEmision;
                                    IdFlex = xCompNew.Comprobante_ID;
                                }
                                else
                                {
                                    if (AltaOK)
                                    {
                                        // Si es registracion tambien devuelve null
                                        ComprobanteNuevo = clave;
                                    }
                                    ComprobanteNuevo = "Se Produjo un error en la importacion";
                                }

                                // Inicio la coneccion                                     
                                string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                                SqlConnection conexion = new SqlConnection(conectionstring);

                                // Recorro la lista recibida para hacer los cambios en la base
                                //                         for (int i = 0; i < items.Count; i++)
                                //                         {
                                //abro la conexion para actualizar los datos.
                                conexion.Open();

                                if (xCompNew != null || AltaOK)
                                {
                                    // Dio de Alta OK el Comprobante
                                    sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave,OK,Mensaje,IdFlex,ComprobanteFlex) Values (@ComprobanteJSON,@Clave,1,@Mensaje,@IdFlex,@ComprobanteFlex)";
                                }
                                else
                                {
                                    // Hubo un error en el alta
                                    sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave,OK,Mensaje,IdFlex,ComprobanteFlex) Values (@ComprobanteJSON,@Clave,0,@Mensaje,@IdFlex,@ComprobanteFlex)";
                                }

                                SqlCommand sqlcom = new SqlCommand(sql, conexion);
                                using (sqlcom)
                                {
                                    // Cargo los valores de los parametros     
                                    // La Fecha la coloco con un trigger
                                    sqlcom.Parameters.Add(new SqlParameter("@ComprobanteJSON", json));
                                    sqlcom.Parameters.Add(new SqlParameter("@Clave", clave));
                                    sqlcom.Parameters.Add(new SqlParameter("@Mensaje", resultado));
                                    sqlcom.Parameters.Add(new SqlParameter("@IdFlex", IdFlex));
                                    sqlcom.Parameters.Add(new SqlParameter("@ComprobanteFlex", ComprobanteNuevo));
                                    try
                                    {
                                        // Ejecuto y contabilizo los registros afectados
                                        t = t + sqlcom.ExecuteNonQuery();
                                        if (t > 0)
                                        {
                                            t = 1;
                                        }
                                        conexion.Close();
                                        if (xCompNew != null)
                                        {
                                            resultado = "OK:" + ComprobanteNuevo;
                                        }
                                        else
                                        {
                                            resultado = "ERROR:" + resultado;
                                        }
                                        EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                        EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);

                                    }
                                    catch
                                    {
                                        // Se produjo un error
                                        if (xCompNew != null)
                                        {
                                            resultado = "OK:" + ComprobanteNuevo;
                                        }
                                        else
                                        {
                                            resultado = "ERROR:" + resultado;
                                        }
                                        conexion.Close();
                                        EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                        EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                                    }
                                }
                            }
                            catch
                            {
                                EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                                EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                                resultado = "ERROR:";
                            }
                        }
                        catch (Exception ex)
                        {
                            resultado = ex.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultado = ex.ToString();
                }
                return resultado + IPClient;
            }
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                return resultado;
            }
           
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string ConsultarComprobanteVentasJSON(string TipoComprobante, string Letra,string PuntoVenta, string NroComprobante)
        {            
            string IPClient = HttpContext.Current.Request.UserHostAddress;

            if (ControlaAcceso())
            {
                bool lOK = true;
                string resultado;
                resultado = " No se obtuvieron resultados para la consulta";
                string xCodigoEmpresa = ConfigurationManager.AppSettings["CodEmpresa"];
                string xCodigoPuesto = ConfigurationManager.AppSettings["CodPuesto"];
                // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                //string xCodigoUsuario = ConfigurationManager.AppSettings["CodUsuario"];
                string xCodigoUsuario = "CASE";
                string xClaveUsuario = "Case2273";

                EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();

                // Abro la conexion a la base de Flex
                EFlexSDK_TokenValidacion pToken = null;

                //EFlexSDK_TokenValidacion pToken = null;                    

                pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);
                if (pToken == null)
                {
                    lOK = false;
                    resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                }
                else
                // Abro la empresa de Flex
                {
                    if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                    {
                        lOK = false;
                        resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                    }
                }
                if (lOK)
                {

                    //EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();
                    EFlexSDK_ItemVentas ItemVentas = new EFlexSDK_ItemVentas();

                    List<EFlexSDK_FiltroComprobantes> xFiltros = new List<EFlexSDK_FiltroComprobantes>();
                    EFlexSDK_FiltroComprobantes MyFiltro = new EFlexSDK_FiltroComprobantes();
                    EFlexSDK_FiltroComprobantes MyFiltroLetra = new EFlexSDK_FiltroComprobantes();
                    EFlexSDK_FiltroComprobantes MyFiltroNumero = new EFlexSDK_FiltroComprobantes();

                    // Aplicar Filtros
                    // Operacion = “AND”, “OR”, o vacio – Establece la relación con la estructura anterior (Obviamente la primera tendrá operación en vacio)
                    // Campo = Nombre del Campo de la estructura del comprobante – Ejemplo “Comprobante_Tipo”
                    // Accion = Modo de comparación del campo con el valor – Es un conjunto acotado "IGUAL","DISTINTO","MAYOR","MAYOR O IGUAL","MENOR","MENOR O IGUAL","CONTIENE","NO CONTIENE"
                    // Valor = Valor contra el que se debe comparar.
                    // Se aplica como si fuera el WHERE de la consulta a cabventa/cabcompra 

                    bool lAnd = false;
                    // Agrego el primer filtro                        
                    if (TipoComprobante.Trim() != "")
                    {
                        MyFiltro.Operacion = "";
                        MyFiltro.Accion = "IGUAL";
                        MyFiltro.Campo = "Comprobante_Tipo";
                        MyFiltro.Valor = TipoComprobante.Trim();
                        //MyFiltro.Valor = tbx_TipoComprobane.Text;
                        xFiltros.Add(MyFiltro);
                        lAnd = true;
                    }

                    //Agrego el segundo filtro por Letra
                    if (Letra.Trim() != "")
                    {
                        if (lAnd)
                        {
                            MyFiltroLetra.Operacion = "AND";
                        }
                        else
                        {
                            MyFiltroLetra.Operacion = "";
                        }
                        MyFiltroLetra.Accion = "CONTIENE";
                        MyFiltroLetra.Campo = "Comprobante_Letra";
                        MyFiltroLetra.Valor = Letra.Trim();
                        xFiltros.Add(MyFiltroLetra);
                    }

                    //Agrego el Tercer filtro por Punto Venta
                    if (PuntoVenta.Trim() != "" && PuntoVenta != "0000")
                    {
                        if (lAnd)
                        {
                            MyFiltroNumero.Operacion = "AND";
                        }
                        else
                        {
                            MyFiltroNumero.Operacion = "";
                        }
                        MyFiltroNumero.Accion = "IGUAL";
                        MyFiltroNumero.Campo = "Comprobante_PtoVenta";
                        MyFiltroNumero.Valor = PuntoVenta.Trim().PadLeft(4, '0');
                        xFiltros.Add(MyFiltroNumero);
                    }


                    //Agrego el Cuarto filtro por Numero
                    if (NroComprobante.Trim() != "" && NroComprobante != "00000000")
                    {
                        if (lAnd)
                        {
                            MyFiltroNumero.Operacion = "AND";
                        }
                        else
                        {
                            MyFiltroNumero.Operacion = "";
                        }
                        MyFiltroNumero.Accion = "IGUAL";
                        MyFiltroNumero.Campo = "Comprobante_Numero";
                        MyFiltroNumero.Valor = NroComprobante.Trim().PadLeft(8, '0');
                        xFiltros.Add(MyFiltroNumero);
                    }                    

             
                    resultado = JsonConvert.DeserializeObject(ventas.ListarComprobantes_json(xFiltros, pToken)).ToString();
                    resultado = ventas.ObtenerListaErrores();
                    EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                    EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);

                }
                return resultado;
            }            
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                return resultado;       
            }              
         }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string ExisteComprobanteVentaenFlex(string TipoComprobante, string Letra, string PuntoVenta, string NroComprobante, string Cliente , string FechaEmision)
        {

            // Parametros
            // Formato FechaEmision AAAAMMDD


            string IPClient = HttpContext.Current.Request.UserHostAddress;

            if (ControlaAcceso())
            {
                try
                {
                    string sql = "SELECT * FROM WEB_ExisteComprobante Where Comprobante='"+TipoComprobante+"' and Letra='"+Letra+"' and PtoVta='"+PuntoVenta+"' and numero='"+NroComprobante+"' and cli_cod='"+Cliente+"' and fechaemision='"+FechaEmision+"'";

                    string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                    SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Int32 cuenta = dt.Rows.Count;
                    string resultado = "";

                    if (cuenta>0)
                    {
                        resultado = "SI";                        
                    }
                    else
                    {
                        resultado = "NO";                        
                    }
                    return resultado;
                }
                catch
                {
                    string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                    return resultado;
                }
            }
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                return resultado;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string WSTest(string Parametro)
        {

            // Parametros
            // Formato FechaEmision AAAAMMDD
            
            string IPClient = HttpContext.Current.Request.UserHostAddress;

            if (ControlaAcceso())
            {
                try
                {
                    string sql = "SELECT Resultado FROM WSIntegracion Where CodSucursal=" + Parametro.Trim() ;

                    string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                                        

                    SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    // Si no existe devuelvo un 0 
                    if (rows.Count==0)
                    {
                        row = new Dictionary<string, object>();
                        row.Add("Resultado", 0);
                        rows.Add(row);
                    }                   

                    return serializer.Serialize(rows);                      
                }
                catch
                {
                    string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                    return resultado;
                }
            }
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                return resultado;
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [SoapHeader("User", Required = true)]
        public string ConsultarDetalleVentasJSON(string TipoComprobante, string Letra, string PuntoVenta, string NroComprobante)
        {
            
            string IPClient = HttpContext.Current.Request.UserHostAddress;

            if (ControlaAcceso())
            {
                bool lOK = true;
                string resultado;
                resultado = " No se obtuvieron resultados para la consulta";
                string xCodigoEmpresa = ConfigurationManager.AppSettings["CodEmpresa"];
                string xCodigoPuesto = ConfigurationManager.AppSettings["CodPuesto"];
                // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                //string xCodigoUsuario = ConfigurationManager.AppSettings["CodUsuario"];
                string xCodigoUsuario = "CASE";
                string xClaveUsuario = "Case2273";

                EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();

                // Abro la conexion a la base de Flex
                EFlexSDK_TokenValidacion pToken = null;

                pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);
                if (pToken == null)
                {
                    lOK = false;
                    resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                }
                else
                // Abro la empresa de Flex
                {
                    if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                    {
                        lOK = false;
                        resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                    }
                }
                if (lOK)
                {

                    EFlexSDK_ItemVentas ItemVentas = new EFlexSDK_ItemVentas();

                    List<EFlexSDK_FiltroComprobantes> xFiltros = new List<EFlexSDK_FiltroComprobantes>();
                    EFlexSDK_FiltroComprobantes MyFiltro = new EFlexSDK_FiltroComprobantes();
                    EFlexSDK_FiltroComprobantes MyFiltroLetra = new EFlexSDK_FiltroComprobantes();
                    EFlexSDK_FiltroComprobantes MyFiltroNumero = new EFlexSDK_FiltroComprobantes();

                    // Aplicar Filtros
                    // Operacion = “AND”, “OR”, o vacio – Establece la relación con la estructura anterior (Obviamente la primera tendrá operación en vacio)
                    // Campo = Nombre del Campo de la estructura del comprobante – Ejemplo “Comprobante_Tipo”
                    // Accion = Modo de comparación del campo con el valor – Es un conjunto acotado "IGUAL","DISTINTO","MAYOR","MAYOR O IGUAL","MENOR","MENOR O IGUAL","CONTIENE","NO CONTIENE"
                    // Valor = Valor contra el que se debe comparar.
                    // Se aplica como si fuera el WHERE de la consulta a cabventa/cabcompra 

                    bool lAnd = false;
                    // Agrego el primer filtro                        
                    if (TipoComprobante.Trim() != "")
                    {
                        MyFiltro.Operacion = "";
                        MyFiltro.Accion = "IGUAL";
                        MyFiltro.Campo = "Comprobante_Tipo";
                        MyFiltro.Valor = TipoComprobante.Trim();
                        //MyFiltro.Valor = tbx_TipoComprobane.Text;
                        xFiltros.Add(MyFiltro);
                        lAnd = true;
                    }

                    //Agrego el segundo filtro por Letra
                    if (Letra.Trim() != "")
                    {
                        if (lAnd)
                        {
                            MyFiltroLetra.Operacion = "AND";
                        }
                        else
                        {
                            MyFiltroLetra.Operacion = "";
                        }
                        MyFiltroLetra.Accion = "CONTIENE";
                        MyFiltroLetra.Campo = "Comprobante_Letra";
                        MyFiltroLetra.Valor = Letra.Trim();
                        xFiltros.Add(MyFiltroLetra);
                    }

                    //Agrego el Tercer filtro por Punto Venta
                    if (PuntoVenta.Trim() != "" && PuntoVenta != "0000")
                    {
                        if (lAnd)
                        {
                            MyFiltroNumero.Operacion = "AND";
                        }
                        else
                        {
                            MyFiltroNumero.Operacion = "";
                        }
                        MyFiltroNumero.Accion = "IGUAL";
                        MyFiltroNumero.Campo = "Comprobante_PtoVenta";
                        MyFiltroNumero.Valor = PuntoVenta.Trim().PadLeft(4, '0');
                        xFiltros.Add(MyFiltroNumero);
                    }

                    //Agrego el Cuarto filtro por Numero
                    if (NroComprobante.Trim() != "" && NroComprobante != "00000000")
                    {
                        if (lAnd)
                        {
                            MyFiltroNumero.Operacion = "AND";
                        }
                        else
                        {
                            MyFiltroNumero.Operacion = "";
                        }
                        MyFiltroNumero.Accion = "IGUAL";
                        MyFiltroNumero.Campo = "Comprobante_Numero";
                        MyFiltroNumero.Valor = NroComprobante.Trim().PadLeft(8, '0');
                        xFiltros.Add(MyFiltroNumero);
                    }

                    resultado = JsonConvert.DeserializeObject(ventas.DetalleComprobante_json(xFiltros, pToken)).ToString();
                    EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                    EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);

                }
                return resultado;
            }          
            else
            {
                string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                return resultado;             
            }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string ConsultarItemsVentasJSON(string NroComprobante)
         {

             if (ControlaAcceso())
             {
                 bool lOK = true;
                 string resultado;
                 resultado = " No se obtuvieron resultados para la consulta";
                 EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                 EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();

                 // Abro la conexion a la base de Flex
                 EFlexSDK_TokenValidacion pToken = null;

                 //EFlexSDK_TokenValidacion pToken = null;
                 // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                 string xCodigoUsuario = "CASE";
                 string xClaveUsuario = "Case2273";

                 pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);
                 if (pToken == null)
                 {
                     lOK = false;
                     resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                 }
                 else
                 // Abro la empresa de Flex
                 {
                     string xCodigoEmpresa = "MODE";
                     string xCodigoPuesto = "1";
                     if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                     {
                         lOK = false;
                         resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                     }
                 }
                 if (lOK)
                 {

                     //EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();
                     EFlexSDK_ItemVentas ItemVentas = new EFlexSDK_ItemVentas();

                     List<EFlexSDK_FiltroComprobantes> xFiltros = new List<EFlexSDK_FiltroComprobantes>();
                     EFlexSDK_FiltroComprobantes MyFiltro = new EFlexSDK_FiltroComprobantes();
                     EFlexSDK_FiltroComprobantes MyFiltroLetra = new EFlexSDK_FiltroComprobantes();
                     EFlexSDK_FiltroComprobantes MyFiltroNumero = new EFlexSDK_FiltroComprobantes();

                     // Aplicar Filtros
                     // Operacion = “AND”, “OR”, o vacio – Establece la relación con la estructura anterior (Obviamente la primera tendrá operación en vacio)
                     // Campo = Nombre del Campo de la estructura del comprobante – Ejemplo “Comprobante_Tipo”
                     // Accion = Modo de comparación del campo con el valor – Es un conjunto acotado "IGUAL","DISTINTO","MAYOR","MAYOR O IGUAL","MENOR","MENOR O IGUAL","CONTIENE","NO CONTIENE"
                     // Valor = Valor contra el que se debe comparar.
                     // Se aplica como si fuera el WHERE de la consulta a cabventa/cabcompra 

                     // Agrego el primer filtro
                     MyFiltro.Operacion = "";
                     MyFiltro.Accion = "IGUAL";
                     MyFiltro.Campo = "Comprobante_Tipo";
                     MyFiltro.Valor = "FC";
                     //MyFiltro.Valor = tbx_TipoComprobane.Text;
                     xFiltros.Add(MyFiltro);

                     //Agrego el segundo filtro por Letra

                     MyFiltroLetra.Operacion = "AND";
                     MyFiltroLetra.Accion = "CONTIENE";
                     MyFiltroLetra.Campo = "Comprobante_Letra";
                     MyFiltroLetra.Valor = "A";
                     xFiltros.Add(MyFiltroLetra);


                     //Agrego el tercer filtro por Numero
                     if (NroComprobante.Trim() != "" && NroComprobante != "00000000")
                     {
                         MyFiltroNumero.Operacion = "AND";
                         MyFiltroNumero.Accion = "IGUAL";
                         MyFiltroNumero.Campo = "Comprobante_Numero";
                         MyFiltroNumero.Valor = NroComprobante.Trim().PadLeft(8, '0');
                         xFiltros.Add(MyFiltroNumero);
                     }

                     resultado = JsonConvert.DeserializeObject(ventas.DetalleComprobante_json(xFiltros, pToken)).ToString();

                     EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                     EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);

                 }
                 return resultado;
             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
             }               
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string ConsultarComprobanteIVAJSON(string NroComprobante)
         {
             if (ControlaAcceso())
             {
                 bool lOK = true;
                 string resultado;
                 resultado = " No se obtuvieron resultados para la consulta";
                 EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();
                 EFlexSDK_ComprobanteVentas xCompV = new EFlexSDK_ComprobanteVentas();

                 // Abro la conexion a la base de Flex
                 EFlexSDK_TokenValidacion pToken = null;

                 //EFlexSDK_TokenValidacion pToken = null;
                 // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                 string xCodigoUsuario = "CASE";
                 string xClaveUsuario = "Case2273";

                 pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);
                 if (pToken == null)
                 {
                     lOK = false;
                     resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                 }
                 else
                 // Abro la empresa de Flex
                 {
                     string xCodigoEmpresa = "MODE";
                     string xCodigoPuesto = "1";
                     if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                     {
                         lOK = false;
                         resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                     }
                 }
                 if (lOK)
                 {

                     EFlexSDK_ItemVentas ItemVentas = new EFlexSDK_ItemVentas();

                     List<EFlexSDK_FiltroComprobantes> xFiltros = new List<EFlexSDK_FiltroComprobantes>();
                     EFlexSDK_FiltroComprobantes MyFiltro = new EFlexSDK_FiltroComprobantes();
                     EFlexSDK_FiltroComprobantes MyFiltroLetra = new EFlexSDK_FiltroComprobantes();
                     EFlexSDK_FiltroComprobantes MyFiltroNumero = new EFlexSDK_FiltroComprobantes();

                     // Aplicar Filtros
                     // Operacion = “AND”, “OR”, o vacio – Establece la relación con la estructura anterior (Obviamente la primera tendrá operación en vacio)
                     // Campo = Nombre del Campo de la estructura del comprobante – Ejemplo “Comprobante_Tipo”
                     // Accion = Modo de comparación del campo con el valor – Es un conjunto acotado "IGUAL","DISTINTO","MAYOR","MAYOR O IGUAL","MENOR","MENOR O IGUAL","CONTIENE","NO CONTIENE"
                     // Valor = Valor contra el que se debe comparar.
                     // Se aplica como si fuera el WHERE de la consulta a cabventa/cabcompra 

                     // Agrego el primer filtro
                     MyFiltro.Operacion = "";
                     MyFiltro.Accion = "IGUAL";
                     MyFiltro.Campo = "Comprobante_Tipo";
                     MyFiltro.Valor = "FC";
                     //MyFiltro.Valor = tbx_TipoComprobane.Text;
                     xFiltros.Add(MyFiltro);

                     //Agrego el segundo filtro por Letra

                     MyFiltroLetra.Operacion = "AND";
                     MyFiltroLetra.Accion = "CONTIENE";
                     MyFiltroLetra.Campo = "Comprobante_Letra";
                     MyFiltroLetra.Valor = "A";
                     xFiltros.Add(MyFiltroLetra);


                     //Agrego el tercer filtro por Numero
                     if (NroComprobante.Trim() != "" && NroComprobante != "00000000")
                     {
                         MyFiltroNumero.Operacion = "AND";
                         MyFiltroNumero.Accion = "IGUAL";
                         MyFiltroNumero.Campo = "Comprobante_Numero";
                         MyFiltroNumero.Valor = NroComprobante.Trim().PadLeft(8, '0');
                         xFiltros.Add(MyFiltroNumero);
                     }

                     // Recuperar los comprobantes en formato json                         
                     //resultado = JsonConvert.DeserializeObject(ventas.DetalleComprobanteIVA_json(xFiltros, pToken)).ToString();
                     resultado = "";

                     EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                     EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                 }
                 return resultado;
             }            
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                 
             }               
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string ConsultarCondicionVentaJSON(string CondVenta)
         {
             if (ControlaAcceso())
             {
                 bool lOK = true;
                 string resultado;
                 resultado = " No se obtuvieron resultados para la consulta";
                 EFlexSDK_Ventas ventas = new EFlexSDK_Ventas();

                 // Abro la conexion a la base de Flex
                 EFlexSDK_TokenValidacion pToken = null;

                 //EFlexSDK_TokenValidacion pToken = null;
                 // JAT - 01/11/2018 Junto con Sebas decidimos utilizar el usuario CASE
                 string xCodigoUsuario = "CASE";
                 string xClaveUsuario = "Case2273";

                 pToken = EFlexSDK_Procesos.ProcesosFlex.IniciarProcesoFlex(xCodigoUsuario, xClaveUsuario);
                 if (pToken == null)
                 {
                     lOK = false;
                     resultado = "Error al Conectar a Flex :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                 }
                 else
                 // Abro la empresa de Flex
                 {
                     string xCodigoEmpresa = "MODE";
                     string xCodigoPuesto = "1";
                     if (!EFlexSDK_Procesos.ProcesosFlex.AbrirEmpresaFlex(xCodigoEmpresa, xCodigoPuesto, pToken))
                     {
                         lOK = false;
                         resultado = "Error al Abrir la Empresa :" + EFlexSDK_Procesos.ProcesosFlex.ObtenerListaErrores();
                     }
                 }
                 if (lOK)
                 {

                     resultado = "CLASE NO IMPLEMENTADA";

                     EFlexSDK_Procesos.ProcesosFlex.CerrarProcesoFlex(pToken);
                     EFlexSDK_Procesos.ProcesosFlex.CerrarEmpresaFlex(pToken);
                 }
                 return resultado; 
             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
             }  
             
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getArticulosJSON(string CodigoGenerico,string FechaModificacion)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     bool lAnd = false;
                     string sql = "SELECT * FROM WEB_Articulos";
                     if (CodigoGenerico.Trim() != "")
                     {
                         sql += " Where art_codgen like '%" + CodigoGenerico + "%'";
                         lAnd = true;
                     }
                     if (FechaModificacion.Trim() != "")
                     {
                         if (lAnd)
                         {
                             sql += " And CONVERT(DATETIME,CONVERT (char(10), art_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                         }
                         else
                         {
                             sql += " Where CONVERT(DATETIME,CONVERT (char(10), art_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                             lAnd = true;
                         }
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
             }  
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getKitsJSON(string CodigoGenerico)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_kits";
                     if (CodigoGenerico.Trim() != "")
                     {
                         sql += " Where Kit_codgen like '%" + CodigoGenerico + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);  
                 }
                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }

             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                 
             }       
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getKitsIntegracionJSON(string CodigoGenerico)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_kitsIntegracion";
                     if (CodigoGenerico.Trim() != "")
                     {
                         sql += " Where Kit_codgen like '%" + CodigoGenerico + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);
                 }

                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }

             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getListaPreciosJSON(string CodigoGenerico, string CanalCod, string Grupo, string FechaModificacion)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_ListaPrecios";
                     bool lwhere = false;
                     if (CodigoGenerico.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where lprart_codgen like '%" + CodigoGenerico + "%'";
                             lwhere = true;
                         }

                     }
                     if (CanalCod.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where CanalCod in ('"+CanalCod+"')";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " and CanalCod in ('" + CanalCod + "')";
                         }

                     }
                     if (Grupo.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where Grupo in ('" + Grupo + "')";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " and Grupo in ('" + Grupo + "')";
                         }
                     }
                     if (FechaModificacion.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where CONVERT(DATETIME,CONVERT (char(10), lpr_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";

                         }
                         else
                         {
                             sql += " And CONVERT(DATETIME,CONVERT (char(10), lpr_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                         }
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);   
                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }            
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;         
             }       
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getSaldoClientesJSON(string CodigoCliente)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_SaldoCliente";
                     bool lwhere = false;
                     if (CodigoCliente.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where cli_Cod like '%" + CodigoCliente + "%'";
                             lwhere = true;
                         }
                     }                     

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getRegimenes()
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_RegEsp";
                     
                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
             }
         }
         

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getClientesRegimenesJSON(string CodigoCliente)
         {
             
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_ClientesRegEsp";
                     if (CodigoCliente.Trim() != "")
                     {
                         sql += " Where reccli_Cod like '%" + CodigoCliente + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);    
                 }
                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                
             }  
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string getVendedoresJSON(string CodigoVendedor)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_Vendedor";
                     if (CodigoVendedor.Trim() != "")
                     {
                         sql += " Where ven_cod like '%" + CodigoVendedor + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);  
                 }
                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }             
             else
             {
                 string json = "[{\"ACCESO\":\"DENEGADO\"}]";                     
                 return json;
             }           
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string getBancosJSON()
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_Bancos";
                     //if (CodigoVendedor.Trim() != "")
                     //{
                     //    sql += " Where ven_cod like '%" + CodigoVendedor + "%'";
                     //}

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);
                 }

                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string json = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return json;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getPromocionesJSON()
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_PromocionesPV";

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);  

                 }
                  catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }            
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                
             }          
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public int RecibirUsuarioJSON(string json)
         {
              int t = 0;
              if (ControlaAcceso())
              {
                  // Convierto el JSON en lista de objetos
                  List<Usuario> usuarios = JsonConvert.DeserializeObject<List<Usuario>>(json) as List<Usuario>;

                  // Inicio la coneccion 
                  string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=Manager; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                  SqlConnection conexion = new SqlConnection(conectionstring);

                  // Recorro la lista recibida para hacer los cambios en la base
                  for (int i = 0; i < usuarios.Count; i++)
                  {
                      //abro la conexion para actualizar los datos.
                      conexion.Open();
                      string sql = "update usu set usu_fallos= @status, usu_fultcam=@fecha where usu_fallos <> @Status and usu_codigo=@usu_codigo";
                      SqlCommand sqlcom = new SqlCommand(sql, conexion);
                      using (sqlcom)
                      {
                          // Cargo los valores de los parametros
                          sqlcom.Parameters.Add(new SqlParameter("@status", usuarios[i].usu_fallos));
                          sqlcom.Parameters.Add(new SqlParameter("@usu_codigo", usuarios[i].usu_codigo));
                          sqlcom.Parameters.Add(new SqlParameter("@fecha", DateTime.Now));
                          try
                          {
                              // Ejecuto y contabilizo los registros afectados
                              t = t + sqlcom.ExecuteNonQuery();
                              conexion.Close();
                          }
                          catch
                          {
                              // Se produjo un error
                              conexion.Close();
                          }
                      }

                  }
                  // Retorno el total de registros afectados.
                  return t;   
              }             
             else
             {                 
                 return t;
             }   
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getCabCFStockJSON()
         {

              if (ControlaAcceso())
              {
                  try
                  {
                      string sql = "SELECT [cfsccs_Id],[dcf_Desc] FROM WEB_ConteoFisico group by [cfsccs_Id],[dcf_Desc]";

                      string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                      SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                      DataSet ds = new DataSet();
                      DataTable dt = new DataTable();
                      da.Fill(dt);

                      System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                      List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                      Dictionary<string, object> row;
                      foreach (DataRow dr in dt.Rows)
                      {
                          row = new Dictionary<string, object>();
                          foreach (DataColumn col in dt.Columns)
                          {
                              row.Add(col.ColumnName, dr[col]);
                          }
                          rows.Add(row);
                      }
                      return serializer.Serialize(rows); 
                  }                  
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
              }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                 
             }

         }
         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getDetCFStockJSON(int Id)
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT cfsccs_Id, cfsart_CodGen, cfsart_CodEle1, cfsart_CodEle2, cfsart_CodEle3, cfs_CodDep, cfs_RecuentoUM1, cfs_RecuentoUM2, cfsstp_Partida,art_DescGen, DobleUnidad, LLevaPartida,art_CodBarras FROM WEB_ConteoFisico";
                     if (Id > 0)
                     {
                         sql += " Where cfsccs_Id = " + Id.ToString();
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows); 
                 }                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                          
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public int RecibirConteoFisicoJSON(string json)
         {
             int t = 0;
             if (ControlaAcceso())
             {
                 try
                 {
                     // Convierto el JSON en lista de objetos
                     List<ConteoFisicoItems> items = JsonConvert.DeserializeObject<List<ConteoFisicoItems>>(json) as List<ConteoFisicoItems>;

                     // Inicio la coneccion 
                     //string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=SBDAGBAR; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                     SqlConnection conexion = new SqlConnection(conectionstring);

                     // Recorro la lista recibida para hacer los cambios en la base
                     for (int i = 0; i < items.Count; i++)
                     {
                         //abro la conexion para actualizar los datos.
                         conexion.Open();
                         string sql = "update cfstock set cfs_RecuentoUM1= @cfs_RecuentoUM1 where cfsccs_Id=@cfsccs_Id and cfsart_CodGen=@cfsart_CodGen and cfsart_CodEle1=@cfsart_CodEle1 and cfsart_CodEle2=@cfsart_CodEle2 and cfsart_CodEle3=@cfsart_CodEle3 and cfs_CodDep=@cfs_CodDep";
                         SqlCommand sqlcom = new SqlCommand(sql, conexion);
                         using (sqlcom)
                         {
                             // Cargo los valores de los parametros
                             sqlcom.Parameters.Add(new SqlParameter("@cfs_RecuentoUM1", Convert.ToDecimal(items[i].cfs_RecuentoUM1)));
                             sqlcom.Parameters.Add(new SqlParameter("@cfsccs_Id", items[i].cfsccs_Id));
                             sqlcom.Parameters.Add(new SqlParameter("@cfsart_CodGen", items[i].cfsart_CodGen));
                             sqlcom.Parameters.Add(new SqlParameter("@cfsart_CodEle1", items[i].cfsart_CodEle1));
                             sqlcom.Parameters.Add(new SqlParameter("@cfsart_CodEle2", items[i].cfsart_CodEle2));
                             sqlcom.Parameters.Add(new SqlParameter("@cfsart_CodEle3", items[i].cfsart_CodEle3));
                             sqlcom.Parameters.Add(new SqlParameter("@cfs_CodDep", items[i].cfs_CodDep));
                             try
                             {
                                 // Ejecuto y contabilizo los registros afectados
                                 t = t + sqlcom.ExecuteNonQuery();
                                 conexion.Close();
                             }
                             catch
                             {
                                 // Se produjo un error
                                 conexion.Close();
                             }
                         }

                     }
                 }
                 catch
                 {
                     t = -1;
                 }
                 // Retorno el total de registros afectados.
                 return t; 
             }                             
             else
             {        
                 return t;
            }
         }
        
         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getConceptosJSON(string CodConcepto)
         {
              if (ControlaAcceso())
              {
                 try
                 {
                     string sql = "SELECT con_Cod, con_Desc, con_DescAdic, con_Tipo, conmon_Cod, mon_descrip, mon_simbolo, conmtca_Cod, con_TipoTasaVta, contiv_CodVta, tiv_Insc FROM WEB_Conceptos";
                     if (CodConcepto != "")
                     {
                         sql += " Where con_Cod = " + CodConcepto;
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows); 
                 }
                  
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
              }            
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;              
             }
         }
        
         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]

         public string getProveedoresJSON(string CodProveedor, string FechaModificacion,string InformaPV, string InformaFran)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_Proveedores";
                     bool lwhere = false;
                     if (CodProveedor != "")
                     {
                         sql += " Where pro_Cod = '" + CodProveedor + "'";
                         lwhere = true;
                     }
                     if (FechaModificacion.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where CONVERT(DATETIME,CONVERT (char(10), pro_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " And CONVERT(DATETIME,CONVERT (char(10), pro_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                         }

                     }
                     if (InformaPV.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where InformaPV ='" + InformaPV.Trim().ToUpper() + "'";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " And InformaPV ='" + InformaPV.Trim().ToUpper() + "'";
                         }
                     }
                     if (InformaFran.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where InformaFran ='" + InformaFran.Trim().ToUpper() + "'";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " And InformaFran ='" + InformaFran.Trim().ToUpper() + "'";
                         }
                     }
                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);  
                 }                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }            
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]

         public string getTarjetasJSON(string FechaModificacion)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_Tarjetas";
                     if (FechaModificacion != "")
                     {
                         sql += " Where CONVERT(DATETIME,CONVERT (char(10), FechaMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows); 
                 }                 
                 catch (Exception ex)
                 {
                     string resultado = "[{\"ERROR\":\""+ex+"\"}]";
                     return resultado;
                 }
             }           
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                  
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getClientesJSON(string CodigoCliente, string FechaModificacion, string InformaPV, string InformaFran)
         {
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "SELECT * FROM WEB_Clientes";
                     bool lwhere = false;
                     if (CodigoCliente.Trim() != "")
                     {
                         sql += " Where cli_cod like '%" + CodigoCliente + "%'";
                         lwhere = true;
                     }
                     if (FechaModificacion.Trim() != "")
                     {
                         if (!lwhere)
                         {

                             sql += " Where CONVERT(DATETIME,CONVERT (char(10), cli_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " And CONVERT(DATETIME,CONVERT (char(10), cli_fecMod, 103), 103) >= CONVERT(DATETIME,'" + FechaModificacion + "', 103)";
                         }

                     }
                     if (InformaPV.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where InformaPV ='" + InformaPV.Trim().ToUpper() + "'";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " And InformaPV ='" + InformaPV.Trim().ToUpper() + "'";
                         }
                     }
                     if (InformaFran.Trim() != "")
                     {
                         if (!lwhere)
                         {
                             sql += " Where InformaFran ='" + InformaFran.Trim().ToUpper() + "'";
                             lwhere = true;
                         }
                         else
                         {
                             sql += " And InformaFran ='" + InformaFran.Trim().ToUpper() + "'";
                         }
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);


                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                 }
                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }             
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;                
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public int RecibirComprobanteJSON(string json)
         {
             int t = 0;

              if (ControlaAcceso())
              {
                  try
                  {
                      string clave = "";

                      EFlexSDK_ComprobanteVentas xCompV = JsonConvert.DeserializeObject<EFlexSDK_ComprobanteVentas>(json);
                      clave = xCompV.Comprobante_Tipo.PadRight(3, ' ') + xCompV.Comprobante_Letra + xCompV.Comprobante_PtoVenta + xCompV.Comprobante_Numero + xCompV.Cliente_Codigo.PadRight(6, ' ') + xCompV.Comprobante_FechaEmision;


                      string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                      SqlConnection conexion = new SqlConnection(conectionstring);

                      //abro la conexion para actualizar los datos.
                      conexion.Open();
                      string sql = "Insert Into WEBServiceIntegra (ComprobanteJSON,Clave) Values (@ComprobanteJSON,@Clave)";
                      SqlCommand sqlcom = new SqlCommand(sql, conexion);
                      using (sqlcom)
                      {
                          // Cargo los valores de los parametros     
                          // La Fecha la coloco con un trigger
                          sqlcom.Parameters.Add(new SqlParameter("@ComprobanteJSON", json));
                          sqlcom.Parameters.Add(new SqlParameter("@Clave", clave));
                          try
                          {
                              // Ejecuto y contabilizo los registros afectados
                              t = t + sqlcom.ExecuteNonQuery();
                              if (t > 0)
                              {
                                  t = 1;
                              }
                              conexion.Close();
                          }
                          catch
                          {
                              // Se produjo un error
                              conexion.Close();
                          }

                      }
                  }
                  catch
                  {
                      t = -1;
                  }
                  // Retorno el total de registros afectados.
                  return t;  
              }            
             else
             {                 
                 return t;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getLogImportacionJSON(string Comprobante,string Estado, string Fecha)
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: FC B00010000010700000119/06/2017
         //
         // Estado - OK sin errores/ER con Errores/ALL o en blanco(todos)
         // Fecha  - Fecha de Procesamiento
         {
              bool lAnd = false;
              if (ControlaAcceso())
              {
                  try
                  {
                      string sql = "SELECT * FROM WEBServiceIntegra ";
                      if (Comprobante.Trim() != "")
                      {
                          lAnd = true;
                          sql += " Where Left(clave,34) like '%" + Comprobante + "%'";
                      }
                      if (Estado.Trim() != "" && Estado.Trim() != "ALL")
                      {
                          if (lAnd)
                          {
                              if (Estado.Trim().ToUpper() == "OK")
                              {
                                  sql += " And OK = 1";
                              }
                              else
                              {
                                  if (Estado.Trim().ToUpper().Substring(0, 2) == "ER")
                                  {
                                      sql += " And OK = 0";
                                  }
                              }
                          }
                          else
                          {
                              if (Estado.Trim().ToUpper() == "OK")
                              {
                                  sql += " Where OK = 1";
                              }
                              else
                              {
                                  if (Estado.Trim().ToUpper().Substring(0, 2) == "ER")
                                  {
                                      sql += " Where OK = 0";
                                  }

                              }
                              lAnd = true;
                          }
                      }
                      if (Fecha.Trim() != "")
                      {
                          if (lAnd)
                          {
                              sql += " And CONVERT (char(10), fecha, 103) = '" + Fecha + "'";
                          }
                          else
                          {
                              sql += " Where CONVERT (char(10), fecha, 103) = '" + Fecha + "'";
                              lAnd = true;
                          }

                      }

                      string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                      SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                      DataSet ds = new DataSet();
                      DataTable dt = new DataTable();
                      da.Fill(dt);

                      System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                      List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                      Dictionary<string, object> row;
                      foreach (DataRow dr in dt.Rows)
                      {
                          row = new Dictionary<string, object>();
                          foreach (DataColumn col in dt.Columns)
                          {
                              row.Add(col.ColumnName, dr[col]);
                          }
                          rows.Add(row);
                      }
                      serializer.MaxJsonLength = Int32.MaxValue;
                      return serializer.Serialize(rows);
                      //   return "Usuario Validado";
                  }

                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
              }            
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string getDescuentosJSON(string Tipo)
         {

              if (ControlaAcceso())
              {
                 try
                 {
                     string sql = "SELECT * FROM WEB_Descuentos";
                     if (Tipo.Trim() != "")
                     {
                         sql += " Where Tipo like '%" + Tipo + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     return serializer.Serialize(rows);   
                 }
                  
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
              }             
             else
             {
                 string json = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return json;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getEstadoPedidosJSON(string Comprobante, string Fecha)
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: FC B00010000010700000119/06/2017
         //       
         // Fecha  - Fecha de Procesamiento
         {            

             bool lAnd = false;

              if (ControlaAcceso())
              {
                  try
                  {
                      string sql = "SELECT * FROM WEB_EstadoPedidos ";
                      if (Comprobante.Trim() != "")
                      {
                          lAnd = true;
                          sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente like '%" + Comprobante + "%'";
                      }
                      if (Fecha.Trim() != "")
                      {
                          if (lAnd)
                          {
                              sql += " And CONVERT (char(10), Doc1_FEmision, 103) = '" + Fecha + "'";
                          }
                          else
                          {
                              sql += " Where CONVERT (char(10), Doc1_FEmision, 103) = '" + Fecha + "'";
                              lAnd = true;
                          }

                      }

                      string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                      SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                      da.SelectCommand.CommandTimeout = 180;
                      DataSet ds = new DataSet();
                      DataTable dt = new DataTable();
                      da.Fill(dt);

                      System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                      List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                      Dictionary<string, object> row;
                      foreach (DataRow dr in dt.Rows)
                      {
                          row = new Dictionary<string, object>();
                          foreach (DataColumn col in dt.Columns)
                          {
                              row.Add(col.ColumnName, dr[col]);
                          }
                          rows.Add(row);
                      }
                      serializer.MaxJsonLength = Int32.MaxValue;
                      return serializer.Serialize(rows);
                      //   return "Usuario Validado";

                  }

                  catch (Exception ex)
                 {
                     string resultado = "[{\"ERROR\":\"ERROR TIPO "+ex.GetType().ToString().Trim()+" - "+ex.Message+"\"}]";
                     return resultado;
                 }
              }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string PV_AltaClienteJSON(string json)
         {
             string ret = "000000";             
             

             if (ControlaAcceso())
             {
                 //var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                 var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                 // Obtengo el valor desde <appSettings> Key Conexion                            
                 string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                 SqlConnection connection = new SqlConnection(conectionstring);
                 //abro la conexion para actualizar los datos.                            
                 connection.Open();

                 SqlCommand command = connection.CreateCommand();
                 SqlTransaction transaction;

                 // Begin Transaction 
                 transaction = connection.BeginTransaction("GrabaCliente");

                 // Must assign both transaction object and connection
                 // to Command object for a pending local transaction
                 command.Connection = connection;
                 command.Transaction = transaction;

                 try
                 {
                     // Recupero el JSON con los datos del cliente 
                     Clientes Cliente = JsonConvert.DeserializeObject<Clientes>(json) as Clientes;

                     // Obtengo el proximo numero de cliente                                                
                     command.CommandText = "select right('000000'+ltrim(Str(IsNull(max(cli_cod ),0)+1)),6) from clientes";
                     string Cli_CodNuevoCli = (string)command.ExecuteScalar();

                     // Obtengo el proximo numero de clienteCRM                                                
                     command.CommandText = "select right('000000'+ltrim(Str(IsNull(max(clr_cod ),0)+1)),6) from clientesCRM";
                     string Cli_CodNuevoCRM = (string)command.ExecuteScalar();

                     // Asigno como Numero de Cliente el mayor entre Clientes y ClienteCRM
                     string Cli_CodNuevo = Math.Max(Convert.ToInt64(Cli_CodNuevoCli), Convert.ToInt64(Cli_CodNuevoCRM)).ToString().Trim().PadLeft(6, '0');

                     // Doy de Alta en la tabla de Clientes                        
                     command.CommandText = "insert into clientes (cli_cod, cli_razsoc,cli_direc,cli_loc,cliprv_codigo,clisiv_cod,clitdc_cod,cli_cuit,clisib_cod,clisig_cod,Cli_NomFantasia,cli_CodPos,cli_Tel,cli_Fax,cli_EMail,cli_Modem,cli_NroIB,cli_Contacto,cli_RespPago,cli_LugarPago,cli_HorarioPago,cli_PagWeb,cli_Password,cli_activo,cli_FecMod,cliusu_Codigo,cli_fotocopiaCUIT,clicvt_cod,cli_ControlaCredAutoriz,cli_eSales,Clidc1_cod,Clidc2_cod,cli_habilitado) Values ('" + Cli_CodNuevo + "', '" + Cliente.cli_razsoc + "','" + Cliente.cli_direc + "','" + Cliente.cli_loc + "','" + Cliente.cliprv_codigo + "','" + Cliente.clisiv_cod + "'," + Cliente.clitdc_cod + ",'" + Cliente.cli_cuit + "','" + Cliente.clisib_cod + "','" + Cliente.clisig_cod + "',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','N',GetDate(),'WEBSERVICE',0,'CON','N','N','CONF','CONF',1)";
                     command.ExecuteNonQuery();

                     // Doy de Alta en la tabla de ClientesCRM
                     command.CommandText = "insert into clientesCRM (clr_cod, clr_razsoc,clr_direc,clr_loc,clrprv_codigo,clrsiv_cod,clrtdc_cod,clr_cuit,clrsib_cod,clrsig_cod,Clr_NomFantasia,clr_CodPos,clr_Tel,clr_Fax,clr_EMail,clr_Modem,clr_NroIB,clr_Contacto,clr_RespPago,clr_LugarPago,clr_HorarioPago,clr_PagWeb,clr_Password,clr_activo,clr_FecMod,clrusu_Codigo,clr_fotocopiaCUIT,clrcvt_cod,clr_ControlaCred,clr_potencial,clrdr1_cod,clrdr2_cod,clr_habilitado) Values ('" + Cli_CodNuevo + "', '" + Cliente.cli_razsoc + "','" + Cliente.cli_direc + "','" + Cliente.cli_loc + "','" + Cliente.cliprv_codigo + "','" + Cliente.clisiv_cod + "'," + Cliente.clitdc_cod + ",'" + Cliente.cli_cuit + "','" + Cliente.clisib_cod + "','" + Cliente.clisig_cod + "',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','N',GetDate(),'WEBSERVICE',0,'CON',0,'N','CONF','CONF',1)";
                     command.ExecuteNonQuery();

                     // Doy de Alta en la tabla ClientesEMP
                     command.CommandText = "insert into ClientesEMP (clecli_cod,cleemp_codigo) values ('" + Cli_CodNuevo + "',' ')";
                     command.ExecuteNonQuery();

                     // Doy de Alta en la tabla SitFCieraC
                     command.CommandText = "insert into SitFcieraC (sfccli_cod,sfc_fsitfinan,sfcmon_Codigo,sfcmtca_Codigo,sfcusu_Codigo,sfc_FecMod,sfcmon_CodigoOrig,sfcmtca_CodigoOrig) values ('" + Cli_CodNuevo + "',GetDate(),'1','UNI','WEBSERVICE',GetDate(),'1','UNI')";
                     command.ExecuteNonQuery();

                     // Doy de Alta los datos adicionales del cliente para informarlo al punto de venta, sino cuando sincronizan no lo ven
                     // Solicitado Gustavo 11/10/2017
                     command.CommandText = "insert into DTSClientes (cli_cod,dcli_informaAPV,dcli_InformaAFRAN) values ('" + Cli_CodNuevo + "','S','N')";
                     command.ExecuteNonQuery();

                     // Attempt to commit the transaction.
                     transaction.Commit();

                     // Todo estuvo Correcto, Cierro la conexion
                     connection.Close();
                     // Devuelvo el Numero de Cliente Asignado
                     ret = Cli_CodNuevo;

                 }
                 catch (Exception ex)
                 {
                     // Attempt to roll back the transaction.
                     try
                     {
                         transaction.Rollback();
                         connection.Close();
                     }
                     catch (Exception ex2)
                     {
                         // This catch block will handle any errors that may have occurred
                         // on the server that would cause the rollback to fail, such as
                         // a closed connection.
                         //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                         //Console.WriteLine("  Message: {0}", ex2.Message);                         
                         connection.Close();
                     }
                 }

                 return ret;
             }          
             else
             {
                 return ret;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string Mobiliza_AltaClienteJSON(string json)
         {
             string ret = "000000";
             string CodPais = "";
             string CodEmpresa = "";
             string Cli_Fax = "";


             if (ControlaAcceso())
             {
                 try
                 {
                     // Recupero el JSON con los datos del cliente 
                     ClientesMobiliza Cliente = JsonConvert.DeserializeObject<ClientesMobiliza>(json) as ClientesMobiliza;
                     CodPais = ConfigurationManager.AppSettings["CodigoPais"];
                     CodEmpresa = ConfigurationManager.AppSettings["CodEmpresa"];
                     Cli_Fax = (Cliente.clidc2_cod=="IMAG") ? "99" : ""; //Si es Imagen coloco 99 en Cli_Fax

                         //var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                     var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                     // Obtengo el valor desde <appSettings> Key Conexion                            
                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";
                     
                     SqlConnection connection = new SqlConnection(conectionstring);
                     //abro la conexion para actualizar los datos.                            
                     connection.Open();

                     SqlCommand command = connection.CreateCommand();

                     // Verifico si existe el CUIT del Cliente en la base                                           
                     command.CommandText = "select cli_cod from clientes where rtrim(ltrim(cli_cuit))='" + Cliente.cli_cuit.Trim() + "'";
                     string Cli_CodNuevoCli = (string)command.ExecuteScalar();

                     if (Cli_CodNuevoCli==null)
                     {
                         // Si no existe el CUIT del cliente en la base lo doy de alta.
                         SqlTransaction transaction;

                         // Begin Transaction 
                         transaction = connection.BeginTransaction("GrabaCliente");

                         // Must assign both transaction object and connection
                         // to Command object for a pending local transaction
                         command.Connection = connection;
                         command.Transaction = transaction;

                         try
                         {
                             // Obtengo el proximo numero de cliente                                                
                             command.CommandText = "select right('000000'+ltrim(Str(IsNull(max(cli_cod ),0)+1)),6) from clientes";
                             Cli_CodNuevoCli = (string)command.ExecuteScalar();

                             // Obtengo el proximo numero de clienteCRM                                                
                             command.CommandText = "select right('000000'+ltrim(Str(IsNull(max(clr_cod ),0)+1)),6) from clientesCRM";
                             string Cli_CodNuevoCRM = (string)command.ExecuteScalar();

                             // Asigno como Numero de Cliente el mayor entre Clientes y ClienteCRM
                             string Cli_CodNuevo = Math.Max(Convert.ToInt64(Cli_CodNuevoCli), Convert.ToInt64(Cli_CodNuevoCRM)).ToString().Trim().PadLeft(6, '0');

                             // Doy de Alta en la tabla de Clientes                        
                             command.CommandText = "insert into clientes (cli_cod, cli_razsoc,cli_direc,cli_loc,cliprv_codigo,clisiv_cod,clitdc_cod,cli_cuit,clisib_cod,clisig_cod,Cli_NomFantasia,cli_CodPos,cli_Tel,cli_Fax,cli_EMail,cli_Modem,cli_NroIB,cli_Contacto,cli_RespPago,cli_LugarPago,cli_HorarioPago,cli_PagWeb,cli_Password,cli_activo,cli_FecMod,cliusu_Codigo,cli_fotocopiaCUIT,clicvt_cod,cli_ControlaCredAutoriz,cli_eSales,Clidc1_cod,Clidc2_cod,cli_habilitado,cliven_cod,clizon_cod,clipai_Cod,clitic_Cod,cli_Nota,clidlp_cod) Values ('" +
                                 Cli_CodNuevo + "', '" + Cliente.cli_razsoc + "','" + Cliente.cli_direc + "','" + Cliente.cli_loc + "','" + Cliente.cliprv_codigo + "','" + Cliente.clisiv_cod + "'," + Cliente.clitdc_cod + ",'" + Cliente.cli_cuit + "','" + Cliente.clisib_cod + "','" + Cliente.clisig_cod + "',' ','" + Cliente.cli_codpos + "','"+Cliente.cli_Tel+"','"+Cli_Fax+"',' ',' ','" + Cliente.cli_nroIB + "',' ',' ',' ',' ',' ',' ','N',GetDate(),'WEBSERVICEMOB',0,'" + Cliente.clicvt_cod + "','N','N','" + Cliente.clidc1_cod + "','" + Cliente.clidc2_cod + "'," + (Cliente.cli_habilitado ? "1" : "0") + ",'" + Cliente.cliven_Cod + "','" + Cliente.clizon_Cod + "','"+CodPais+"','MAYO','"+Cliente.cli_Nota+"','"+Cliente.clidlp_cod+"')";

                             command.ExecuteNonQuery();

                             // Doy de Alta en la tabla de ClientesCRM
                             command.CommandText = "insert into clientesCRM (clr_cod, clr_razsoc,clr_direc,clr_loc,clrprv_codigo,clrsiv_cod,clrtdc_cod,clr_cuit,clrsib_cod,clrsig_cod,Clr_NomFantasia,clr_CodPos,clr_Tel,clr_Fax,clr_EMail,clr_Modem,clr_NroIB,clr_Contacto,clr_RespPago,clr_LugarPago,clr_HorarioPago,clr_PagWeb,clr_Password,clr_activo,clr_FecMod,clrusu_Codigo,clr_fotocopiaCUIT,clrcvt_cod,clr_ControlaCred,clr_potencial,clrdr1_cod,clrdr2_cod,clr_habilitado,clrven_cod,clrzon_cod,clrpai_Cod,clrtic_Cod,clr_Nota,clrdlp_cod) Values ('" +
                                  Cli_CodNuevo + "', '" + Cliente.cli_razsoc + "','" + Cliente.cli_direc + "','" + Cliente.cli_loc + "','" + Cliente.cliprv_codigo + "','" + Cliente.clisiv_cod + "'," + Cliente.clitdc_cod + ",'" + Cliente.cli_cuit + "','" + Cliente.clisib_cod + "','" + Cliente.clisig_cod + "',' ','" + Cliente.cli_codpos + "','"+Cliente.cli_Tel+"','"+Cli_Fax+"',' ',' ','" + Cliente.cli_nroIB + "',' ',' ',' ',' ',' ',' ','N',GetDate(),'WEBSERVICEMOB',0,'" + Cliente.clicvt_cod + "',0,'N','" + Cliente.clidc1_cod + "','" + Cliente.clidc2_cod + "'," +  (Cliente.cli_habilitado ? "1" : "0" )+ ",'" + Cliente.cliven_Cod + "','" + Cliente.clizon_Cod + "','"+CodPais+"','MAYO','"+Cliente.cli_Nota+"','"+Cliente.clidlp_cod+"')";
                             command.ExecuteNonQuery();

                             // Doy de Alta en la tabla ClientesEMP
                             command.CommandText = "insert into ClientesEMP (clecli_cod,cleemp_codigo) values ('" + Cli_CodNuevo + "',' ')";
                             command.ExecuteNonQuery();

                             // Doy de Alta en la tabla SitFCieraC
                             command.CommandText = "insert into SitFcieraC (sfccli_cod,sfc_fsitfinan,sfcmon_Codigo,sfcmtca_Codigo,sfcusu_Codigo,sfc_FecMod,sfcmon_CodigoOrig,sfcmtca_CodigoOrig) values ('" + Cli_CodNuevo + "',GetDate(),'1','UNI','WEBSERVICEMOB',GetDate(),'1','UNI')";
                             command.ExecuteNonQuery();

                             // Doy de Alta los datos adicionales del cliente para informarlo al punto de venta, sino cuando sincronizan no lo ven
                             // Solicitado Gustavo 11/10/2017
                             command.CommandText = "insert into DTSClientes (cli_cod,dcli_informaAPV,dcli_InformaAFRAN) values ('" + Cli_CodNuevo + "','S','N')";
                             command.ExecuteNonQuery();

                             // Doy de Alta el Lugar de Entrega por defecto
                             // len_ID, lencli_Cod, len_Cod, len_Desc, len_Lugar, len_Loc, lenprv_Codigo, len_CodPos, len_Horario, len_GuiaCalles, len_EsDefault, lenemp_Codigo,lensuc_Cod, lenpai_Cod
                             // Solicitado Alejandro 21/05/2018 para alta desde Mobiliza

                             command.CommandText = "insert into LugarEnt (len_ID, lencli_Cod, len_Cod, len_Desc, len_Lugar, len_Loc, lenprv_Codigo, len_CodPos, len_Horario, len_GuiaCalles, len_EsDefault, lenemp_Codigo,lensuc_Cod, lenpai_Cod) values ('" +
                                     Cli_CodNuevo + "001" + "','" + Cli_CodNuevo + "','001','" + Cliente.cli_direc.Substring(0, Math.Min(Cliente.cli_direc.Length, 25)) + "','" + Cliente.cli_direc.Substring(0, Math.Min(Cliente.cli_direc.Length, 25)) + "','" + Cliente.cli_loc.Substring(0, Math.Min(Cliente.cli_loc.Length, 15)) + "','" + Cliente.cliprv_codigo + "','" + Cliente.cli_codpos + "',' ',' ',1,'" + CodEmpresa + "',' ','" + CodPais + "')";
                             command.ExecuteNonQuery();

                             // Attempt to commit the transaction.
                             transaction.Commit();

                             // Todo estuvo Correcto, Cierro la conexion
                             connection.Close();
                             // Devuelvo el Numero de Cliente Asignado
                             ret = Cli_CodNuevo;

                         }
                         catch (Exception ex)
                         {
                             // Attempt to roll back the transaction.
                             try
                             {
                                 transaction.Rollback();
                                 connection.Close();
                             }
                             catch (Exception ex2)
                             {
                                 // This catch block will handle any errors that may have occurred
                                 // on the server that would cause the rollback to fail, such as
                                 // a closed connection.
                                 //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                                 //Console.WriteLine("  Message: {0}", ex2.Message);                         
                                 connection.Close();
                             }
                         }                         
                     }
                     else
                     {
                         // El CUIT ya existe, devuelvo el código del cliente en Flex para no darlo de alta nuevamente
                         ret = Cli_CodNuevoCli;
                         connection.Close();
                     }
                 }
                 catch (Exception ex)
                 {
                     // Attempt to roll back the transaction.  
                                
                 }

                 return ret;

             }
             else
             {
                 return ret;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string PV_InformarRecepcionPedido(string Comprobante)
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: FC B00010000010700000119/06/2017
         {
             string ret = "ERROR";

             if (ControlaAcceso())
             {
                 //var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                 var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                 // Obtengo el valor desde <appSettings> Key Conexion                            
                 string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                 SqlConnection connection = new SqlConnection(conectionstring);
                 //abro la conexion para actualizar los datos.                            
                 connection.Open();

                 SqlCommand command = connection.CreateCommand();
                 SqlTransaction transaction;

                 // Begin Transaction 
                 transaction = connection.BeginTransaction("InformaRecepcion");

                 // Must assign both transaction object and connection
                 // to Command object for a pending local transaction
                 command.Connection = connection;
                 command.Transaction = transaction;

                 try
                 {

                     // Verifico que existe el comprobante solicitado
                     command.CommandText = "Select Count(1) from segtiposv where Left(spvtco_cod+'   ',3)+Left(spv_letra+' ',1)+Left(spv_codpvt+'    ',4)+spv_nro+(select scvcli_cod from segcabv where scv_id=SegTiposV.spvscv_id)+(select dbo.Dtoc(scv_femision) from segcabv where scv_id=SegTiposV.spvscv_id) = '" + Comprobante + "'";
                     Int16 QtyRegistros = (Int16)Convert.ToInt16(command.ExecuteScalar());

                     if (QtyRegistros > 0)
                     {
                         // Obtengo el ID de la tabla SegCabV del Comprobante Solicitado
                         command.CommandText = "Select spvscv_id from segtiposv where Left(spvtco_cod+'   ',3)+Left(spv_letra+' ',1)+Left(spv_codpvt+'    ',4)+spv_nro+(select scvcli_cod from segcabv where scv_id=SegTiposV.spvscv_id)+(select dbo.Dtoc(scv_femision) from segcabv where scv_id=SegTiposV.spvscv_id) = '" + Comprobante + "'";
                         Int64 ID_SegCabV = (Int64)Convert.ToInt64(command.ExecuteScalar());

                         // Obtengo el ID de la tabla SegCabV del Comprobante Solicitado
                         command.CommandText = "Select Count(1) from dtssegcabv where scv_id=" + ID_SegCabV.ToString().Trim();
                         QtyRegistros = (Int16)Convert.ToInt64(command.ExecuteScalar());

                         if (QtyRegistros > 0)
                         {
                             // Debo hacer un Update del Registro DTSSegCabV
                             command.CommandText = "Update DTSSegCabV Set dscv_fecharecepcion=GetDate() where scv_id=" + ID_SegCabV.ToString().Trim() + " And dscv_fecharecepcion is Null";
                             command.ExecuteNonQuery();
                         }
                         else
                         {
                             
                             // Debo Insertar el registro en DTSSegCabV                                     
                             command.CommandText = "insert into DTSSegCabV (scvemp_codigo,scvsuc_cod,scv_id,dscv_fecharecepcion) Values ('" + ConfigurationManager.AppSettings["CodEmpresa"] + "',' '," + ID_SegCabV.ToString().Trim() + ", GetDate())";
                             command.ExecuteNonQuery();
                         }

                         // Attempt to commit the transaction.
                         transaction.Commit();

                         // Todo estuvo Correcto, Cierro la conexion
                         connection.Close();
                         // Devuelvo el Numero de Cliente Asignado
                         ret = "OK";
                     }

                 }
                 catch (Exception ex)
                 {
                     // Attempt to roll back the transaction.
                     try
                     {
                         transaction.Rollback();
                         connection.Close();
                     }
                     catch (Exception ex2)
                     {
                         // This catch block will handle any errors that may have occurred
                         // on the server that would cause the rollback to fail, such as
                         // a closed connection.
                         //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                         //Console.WriteLine("  Message: {0}", ex2.Message);                         
                         connection.Close();
                     }
                 }

                 return ret;
             }             
             else
             {
                 return ret;
             }
         }

         

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         // agrego el SoapHeader para dar seguridad	
         [SoapHeader("User", Required = true)]
         public string PV_InformarRecepcionItemPedido(string Comprobante,Int64 IdItem)
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: FC B00010000010700000119/06/2017
         // ID SegDetV 
         {
             string ret = "ERROR";

             if (ControlaAcceso())
             {
                 
                 if ((Comprobante.Substring(0, Math.Min(1, Comprobante.Length))!=" ") && (IdItem>0))
                 // Solo ejecuto si recibo ambos parametros
                 {
                     //var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                     var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                     // Obtengo el valor desde <appSettings> Key Conexion                            
                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlConnection connection = new SqlConnection(conectionstring);
                     //abro la conexion para actualizar los datos.                            
                     connection.Open();

                     SqlCommand command = connection.CreateCommand();
                     SqlTransaction transaction;

                     // Begin Transaction 
                     transaction = connection.BeginTransaction("InformaRecepcionItem");

                     // Must assign both transaction object and connection
                     // to Command object for a pending local transaction
                     command.Connection = connection;
                     command.Transaction = transaction;

                     try
                     {

                         // Verifico que existe el comprobante solicitado
                         command.CommandText = "Select Count(1) from segtiposv where Left(spvtco_cod+'   ',3)+Left(spv_letra+' ',1)+Left(spv_codpvt+'    ',4)+spv_nro+(select scvcli_cod from segcabv where scv_id=SegTiposV.spvscv_id)+(select dbo.Dtoc(scv_femision) from segcabv where scv_id=SegTiposV.spvscv_id) = '" + Comprobante + "'";
                         Int16 QtyRegistros = (Int16)Convert.ToInt16(command.ExecuteScalar());

                         // Solo actualizo si existe el comprobante
                         if (QtyRegistros > 0)
                         {
                             // Obtengo el ID de la tabla SegCabV del Comprobante Solicitado
                             command.CommandText = "Select spvscv_id from segtiposv where Left(spvtco_cod+'   ',3)+Left(spv_letra+' ',1)+Left(spv_codpvt+'    ',4)+spv_nro+(select scvcli_cod from segcabv where scv_id=SegTiposV.spvscv_id)+(select dbo.Dtoc(scv_femision) from segcabv where scv_id=SegTiposV.spvscv_id) = '" + Comprobante + "'";
                             Int64 ID_SegCabV = (Int64)Convert.ToInt64(command.ExecuteScalar());

                             // Controlo que el ID de la tabla SegCabV del Comprobante Solicitado tengo un Item con el IDItem enviado
                             command.CommandText = "Select Count(1) from Segdetv where sdvscv_id="+ID_SegCabV.ToString().Trim()+" and sdv_id=" + IdItem.ToString().Trim();
                             QtyRegistros = (Int16)Convert.ToInt64(command.ExecuteScalar());

                             if (QtyRegistros > 0)
                             {
                                 //Si el IdItem corresponde al comprobante ingresado                                 

                                 // Verifico si existen registros para los datos adicionales
                                 command.CommandText = "Select Count(1) from dtssegcabv where scv_id=" + ID_SegCabV.ToString().Trim();
                                 QtyRegistros = (Int16)Convert.ToInt64(command.ExecuteScalar());

                                 if (QtyRegistros > 0)
                                 {
                                     // Debo hacer un Update del Registro DTSSegCabV solo si la fecha de recepcion es Null sino queda la que ya habia ingresado
                                     command.CommandText = "Update DTSSegCabV Set dscv_fecharecepcion=GetDate() where scv_id=" + ID_SegCabV.ToString().Trim() + " And dscv_fecharecepcion is Null";
                                     command.ExecuteNonQuery();
                                 }
                                 else
                                 {
                                     // Debo Insertar el registro en DTSSegCabV                                     
                                     command.CommandText = "insert into DTSSegCabV (scvemp_codigo,scvsuc_cod,scv_id,dscv_fecharecepcion) Values ('" + ConfigurationManager.AppSettings["CodEmpresa"] + "',' '," + ID_SegCabV.ToString().Trim() + ", GetDate())";
                                     command.ExecuteNonQuery();
                                 }

                                 // Verifico si ya hay un registro para los datos adicionales 
                                 command.CommandText = "Select Count(1) from dtssegdetv where sdv_id=" + IdItem.ToString().Trim();
                                 QtyRegistros = (Int16)Convert.ToInt64(command.ExecuteScalar());

                                 if (QtyRegistros > 0)
                                 {
                                     // Debo hacer un Update del Registro DTSSegCabV
                                     command.CommandText = "Update DTSSegDetV Set dsdv_fecharecepcion=GetDate() where sdv_id=" + IdItem.ToString().Trim() + " And dsdv_fecharecepcion is Null";
                                     command.ExecuteNonQuery();
                                     // Devuelvo el Numero de Cliente Asignado
                                     ret = "OK";
                                 }
                                 else
                                 {
                                     // Debo Insertar el registro en DTSSegCabV                                     
                                     command.CommandText = "insert into DTSSegDetV (sdvemp_codigo,sdvsuc_cod,sdv_id,dsdv_fecharecepcion) Values ('" + ConfigurationManager.AppSettings["CodEmpresa"] + "',' '," + IdItem.ToString().Trim() + ", GetDate())";
                                     command.ExecuteNonQuery();
                                     // Devuelvo el Numero de Cliente Asignado
                                     ret = "OK";
                                 }
                             }

                             // Attempt to commit the transaction.
                             transaction.Commit();

                             // Todo estuvo Correcto, Cierro la conexion
                             connection.Close();
                             
                         }

                     }
                     catch (Exception ex)
                     {
                         // Attempt to roll back the transaction.
                         try
                         {
                             transaction.Rollback();
                             connection.Close();
                         }
                         catch (Exception ex2)
                         {
                             // This catch block will handle any errors that may have occurred
                             // on the server that would cause the rollback to fail, such as
                             // a closed connection.
                             //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                             //Console.WriteLine("  Message: {0}", ex2.Message);                         
                             connection.Close();
                         }
                     }

                 }
                 
                 return ret;
             }
             else
             {
                 return ret;
             }
         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string PV_PedidosPendientesRecepcionJSON(string Comprobante)
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: PFRS00010000010700000119/06/2017        
         {
             
             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_PedidosPendienteRecepcion ";
                     if (Comprobante.Trim() != "")
                     {
                         sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";
                 }
                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string PV_ItemsPedidosPendientesRecepcionJSON(string Comprobante)
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: PFRS00010000010700000119/06/2017        
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_ItemPedidoPendienteRecepcion ";
                     if (Comprobante.Trim() != "")
                     {
                         sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";
                 }

                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

        
         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string getFinanciacionJSON()
         //Parametros
         // Comprobante que debe estar formado por 
         // Tipo Comprobante (Char 3)
         // Letra (Char 1)
         // Punto de Venta (Char 4)
         // Nro Comprobante (Char 8)
         // Codigo Cliente (Char 6)
         // Fecha Emision (Char 10)   
         // Ejemplo: PFRS00010000010700000119/06/2017        
         {
            // bool NoValidarUsuario;
            // bool OkUsuario;

             if (ControlaAcceso())
             {
                     try
                     {
                         string sql = "select * from WEB_Financiacion where Habilitado=1";

                         //    if (Comprobante.Trim() != "")
                         //    {
                         //        sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                         //    }

                         string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                         SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                         DataSet ds = new DataSet();
                         DataTable dt = new DataTable();
                         da.Fill(dt);

                         System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                         List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                         Dictionary<string, object> row;
                         foreach (DataRow dr in dt.Rows)
                         {
                             row = new Dictionary<string, object>();
                             foreach (DataColumn col in dt.Columns)
                             {
                                 row.Add(col.ColumnName, dr[col]);
                             }
                             rows.Add(row);
                         }
                         serializer.MaxJsonLength = Int32.MaxValue;
                         return serializer.Serialize(rows);
                         //   return "Usuario Validado";
                     }
                    
                     catch
                     {
                         string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                         return resultado;
                     }
                 }
                 else
                 {
                     string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                     return resultado;
                     //  return Error de Acceso            
                 }
         }

         // Desde aqui Bacosoft

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetParcelasJSON()         
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_Parcelas ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";                
                 
                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetExistenciaInsumosFincasJSON()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from web_ExistenciaInsumosFincas ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";                

                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }


         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetTratamientosJSON()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_Tratamientos ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";                
                 
                 }
                 catch
                 {

                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }

             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetMaquinasJSON()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_Maquinas ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";
                 }                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetOperariosJSON()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_Operarios ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";
                 }                 
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetCentrosdeCosto()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_CentroCosto ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";                

                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }


         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetCuadrillasJSON()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_Cuadrillas ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";
                 }
                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
                 
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetMateriaPrimaJSON()
         {

             if (ControlaAcceso())
             {
                 try
                 {
                     string sql = "select * from WEB_MateriaPrima ";
                     //if (Comprobante.Trim() != "")
                     //{
                     //    sql += " Where Left(doc1+'   ',3)+Left(doc1_letra+' ',1)+Left(Doc1_PtoVta+'    ',4)+Doc1_Nro+CodCliente+(select dbo.Dtoc(Doc1_femision)) like '%" + Comprobante + "%'";
                     // }

                     string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                     SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                     // Setting command timeout to 2 minutes
                     da.SelectCommand.CommandTimeout = 180;
                     DataSet ds = new DataSet();
                     DataTable dt = new DataTable();
                     da.Fill(dt);

                     System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                     Dictionary<string, object> row;
                     foreach (DataRow dr in dt.Rows)
                     {
                         row = new Dictionary<string, object>();
                         foreach (DataColumn col in dt.Columns)
                         {
                             row.Add(col.ColumnName, dr[col]);
                         }
                         rows.Add(row);
                     }
                     serializer.MaxJsonLength = Int32.MaxValue;
                     return serializer.Serialize(rows);
                     //   return "Usuario Validado";
                 }                

                 catch
                 {
                     string resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                     return resultado;
                 }
             }
             else
             {
                 string resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

        /*
         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_GetPartesJSON(string id_UnicoTablet)
         {
             string resultado = ""; 
             if (ControlaAcceso())
             {
                 if (id_UnicoTablet=="")
                 {
                     resultado = "[{\"ATENCION\":\"DEBE INGRESAR EL ID DEL PARTE\"}]";
                     return resultado;
                 }
                 else
                 {
                     try
                     {
                         List<PartesFinca> estospartes = new List<PartesFinca>();
                         PartesFinca esteparte = new PartesFinca();
                         int Id_Parte = 0;                         

                         string sql = "select * from CASE_Partes Where id_UnicoTablet = '" + id_UnicoTablet.Trim()+"'";

                         string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                         SqlDataAdapter da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         DataSet ds = new DataSet();
                         DataTable dt = new DataTable();
                         da.Fill(dt);
                         
                         foreach (DataRow dr in dt.Rows)
                         {
                             
                             esteparte.id = Convert.ToInt16(dr[0]);
                             esteparte.idUnico = Convert.ToInt16(dr[1].ToString());
                             esteparte.fechaDeCargar = Convert.ToDateTime(dr[2]).ToString();
                             esteparte.fechaActual = Convert.ToDateTime(dr[3]).ToString();
                             esteparte.descripcion = dr[4].ToString();
                            // esteparte.InsertDate = Convert.ToDateTime(dr[5]);
                            // esteparte.ProcessDate = Convert.ToDateTime(dr[6]);
                            // esteparte.Estado = Convert.ToInt16(dr[7]);
                       //      esteparte.Observaciones = dr[8].ToString();
                            // esteparte.Autorizado = Convert.ToBoolean(dr[9]);
                           //  esteparte.LastUpdate = Convert.ToDateTime(dr[10]);
                             Id_Parte = esteparte.id;
                             estospartes.Add(esteparte);
                         }

                         //Con la tarea ahora busco el/los tratamientos
                         List<Tratamiento> estostratamientos = new List<Tratamiento>();
                         Tratamiento estetratamiento = new Tratamiento();

                         sql = "select * from CASE_Partes_Tratamientos Where id_Parte = " + Id_Parte ;

                         //string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bacosoft; Password=bs;";

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             estetratamiento.id = Convert.ToInt16(dr[0]);
                             estetratamiento.id_parte = Convert.ToInt16(dr[1]);
                             estetratamiento.OperCod = dr[2].ToString();
                             estostratamientos.Add(estetratamiento);
                         }

                         esteparte.Tratamientos = estostratamientos;

                         //Con la tarea ahora busco las parcelas
                         List<Parcela> estasparcelas = new List<Parcela>();                         

                         sql = "select * from CASE_Partes_Parcelas Where id_Parte = " + Id_Parte;

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             Parcela estaparcela = new Parcela();
                             estaparcela.id = Convert.ToInt16(dr[0]);
                             estaparcela.id_parte = Convert.ToInt16(dr[1]);
                             estaparcela.CuadDes = dr[2].ToString();
                             estaparcela.VitiNro = Convert.ToInt16(dr[3]);
                             estaparcela.VitSedeTim = dr[4].ToString();
                             estaparcela.Hileras = float.Parse(dr[5].ToString());
                             estasparcelas.Add(estaparcela);                              
                         }

                         esteparte.Parcelas = estasparcelas;

                         //Con la tarea ahora busco los operarios
                         List<Operario> estosoperarios = new List<Operario>();

                         sql = "select * from CASE_Partes_Operarios Where id_Parte = " + Id_Parte;

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             Operario esteoperario = new Operario();
                             esteoperario.id = Convert.ToInt16(dr[0]);
                             esteoperario.id_parte = Convert.ToInt16(dr[1]);
                             esteoperario.horas = float.Parse(dr[2].ToString());
                             esteoperario.PersCod = Convert.ToInt16(dr[3]);                             
                             estosoperarios.Add(esteoperario);
                         }

                         esteparte.Operarios = estosoperarios;

                         //Con la tarea ahora busco los Materiales
                         List<Material> estosmateriales = new List<Material>();

                         sql = "select * from CASE_Partes_Materiales Where id_Parte = " + Id_Parte;

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             Material estematerial = new Material();
                             estematerial.id = Convert.ToInt16(dr[0]);
                             estematerial.id_parte = Convert.ToInt16(dr[1]);
                             estematerial.ElabNro = dr[2].ToString();
                             estematerial.Unidades = float.Parse(dr[3].ToString());
                             estosmateriales.Add(estematerial);

                         }

                         esteparte.Materiales = estosmateriales;

                         //Con la tarea ahora busco la Maquinaria
                         List<Maquina> estasmaquinas = new List<Maquina>();

                         sql = "select * from CASE_Partes_Maquinas Where id_Parte = " + Id_Parte;

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             Maquina estamaquina = new Maquina();
                             estamaquina.id = Convert.ToInt16(dr[0]);
                             estamaquina.id_parte = Convert.ToInt16(dr[1]);
                             estamaquina.Horas = float.Parse(dr[2].ToString());
                             estamaquina.mqcod = Convert.ToInt16(dr[3]);
                             estasmaquinas.Add(estamaquina);

                         }

                         esteparte.Maquinas = estasmaquinas;

                         //Con la tarea ahora busco el Centro de Costo
                         List<CentroCosto> estoscentrosdecostos = new List<CentroCosto>();

                         sql = "select * from CASE_Partes_CentroCosto Where id_Parte = " + Id_Parte;

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             CentroCosto estecentrocosto = new CentroCosto();

                             estecentrocosto.id = Convert.ToInt16(dr[0]);
                             estecentrocosto.id_parte = Convert.ToInt16(dr[1]);
                             estecentrocosto.CtrDsc = dr[2].ToString();
                             estoscentrosdecostos.Add(estecentrocosto);

                         }

                         esteparte.CentrosdeCostos = estoscentrosdecostos;

                         //Con la tarea ahora busco las existencias (Stock Utilizado)
                         List<Existencia> estasexistencias = new List<Existencia>();

                         sql = "select * from CASE_Partes_Existencias Where id_Parte = " + Id_Parte;

                         da = new SqlDataAdapter(sql, conectionstring);
                         // Setting command timeout to 2 minutes
                         da.SelectCommand.CommandTimeout = 180;
                         ds = new DataSet();
                         dt = new DataTable();
                         da.Fill(dt);
                         foreach (DataRow dr in dt.Rows)
                         {
                             Existencia estaexistencia = new Existencia();

                             estaexistencia.id = Convert.ToInt16(dr[0]);
                             estaexistencia.id_parte = Convert.ToInt16(dr[1]);
                             estaexistencia.PileCod = dr[2].ToString();
                             estaexistencia.deposito = dr[3].ToString();
                             estaexistencia.producto = dr[4].ToString();
                             estaexistencia.lote = dr[5].ToString();
                             estaexistencia.loteref = dr[6].ToString();
                             estaexistencia.LoteCant = float.Parse(dr[7].ToString());

                             estasexistencias.Add(estaexistencia);
                         }

                         esteparte.Existencias = estasexistencias;                        

                         return JsonConvert.SerializeObject(esteparte); 
                         //   return "Usuario Validado";                

                     }
                     catch
                     {
                         resultado = "[{\"ERROR\":\"SE PRODUJO UN ERROR EN EL METODO\"}]";
                         return resultado;
                     }
                 }
                 
             }
             else
             {
                resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                return resultado;
                 // return Error de Acceso                 
             }

         }
        */
         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
         [SoapHeader("User", Required = true)]
         public string BACO_EnviaParteJSON(string IngresarParte)
         {
             string resultado = "0";
             if (ControlaAcceso())
             {
                 if (IngresarParte == "")
                 {                     
                     return resultado;
                 }
                 else
                 {
                     try
                     {
                        
                         // Doy de Alta la cabecera del Parte primero
                         //var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                         var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                         // Obtengo el valor desde <appSettings> Key Conexion                            
                         string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["CatalogBaco"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                         SqlConnection connection = new SqlConnection(conectionstring);
                         //abro la conexion para actualizar los datos.                            
                         connection.Open();

                         SqlCommand command = connection.CreateCommand();
                         SqlTransaction transaction;

                         // Begin Transaction 
                         transaction = connection.BeginTransaction("GraboParte");

                         // Must assign both transaction object and connection
                         // to Command object for a pending local transaction
                         command.Connection = connection;
                         command.Transaction = transaction;

                         try
                         {                             
                             // Convierto el JSON del parte a un objeto                             
                             PartesFinca esteparte = JsonConvert.DeserializeObject<PartesFinca>(IngresarParte) as PartesFinca;
                             
                             // Verifico que no existe el parte
                             command.CommandText = "select count(1) from CASE_Partes where Ltrim(Rtrim(id_UnicoTablet)) = '" + esteparte.idUnico.ToString() + "'";
                             int QtyParte = (int)command.ExecuteScalar();

                             // Solo si no existe dos de alta el Parte
                             if (QtyParte == 0)
                             {
                                 /*
                                 // Obtengo el proximo numero de clienteCRM                                                
                                 command.CommandText = "select right('000000'+ltrim(Str(IsNull(max(clr_cod ),0)+1)),6) from clientesCRM";
                                 string Cli_CodNuevoCRM = (string)command.ExecuteScalar();

                                 // Asigno como Numero de Cliente el mayor entre Clientes y ClienteCRM
                                 string Cli_CodNuevo = Math.Max(Convert.ToInt64(Cli_CodNuevoCli), Convert.ToInt64(Cli_CodNuevoCRM)).ToString().Trim().PadLeft(6, '0');
                                  * 
                                  */ 

                                 // Doy de Alta el Parte                                 
                                 command.CommandText = "insert into Case_Partes (id_unicotablet, fecha_parte, fecha_carga, descripcion, InsertDate, Estado, Observaciones, LastUpdate, Autorizado ) "+
                                        " Values ('" + esteparte.idUnico.ToString().Trim() + "','" + Convert.ToDateTime(esteparte.fechaDeCargar).ToShortDateString() + "','" + Convert.ToDateTime(esteparte.fechaActual).ToShortDateString() + "','" + esteparte.descripcion + "',GetDate(),0,'"+esteparte.Observaciones+"',GetDate(),0)";
                                          //  " Values ('" + esteparte.idUnico.ToString().Trim() + "','" + Convert.ToDateTime(esteparte.fechaDeCargar).ToShortDateString() + "','" + Convert.ToDateTime(esteparte.fechaActual).ToShortDateString() + "','" + esteparte.descripcion + "',GetDate(),0,'" + esteparte.Observaciones + "',GetDate(),0)";
                                 command.ExecuteNonQuery();

                                 // Recupero el Id del Nuevo Parte
                                 command.CommandText = "select Id from CASE_Partes where Ltrim(Rtrim(id_UnicoTablet)) = '" + esteparte.idUnico + "'";
                                 int New_IdParte = (int)command.ExecuteScalar();
                                                                  
                                 // Recorro los tratamientos y los doy de alta
                                 foreach (Tratamiento estetratamiento in esteparte.tratamiento)
                                 {
                                     // Doy de Alta cada item del tratamiento
                                     command.CommandText = "insert into Case_Partes_Tratamientos (id_parte, OperCod ) " +
                                                " Values (" + New_IdParte.ToString() + ",'" + estetratamiento.OperCod + "')";
                                     command.ExecuteNonQuery();
                                 }

                                 // Recorro los tratamientos y los doy de alta
                                 foreach (Parcela estaparcela in esteparte.parcelas)
                                 {
                                     // Doy de Alta cada item de las parcelas
                                     command.CommandText = "insert into Case_Partes_Parcelas (id_parte, CuadDes, VitiNro, VitSedeTim, Hileras ) " +
                                                " Values (" + New_IdParte.ToString() + ",'" + estaparcela.CuadDes.Trim() + "'," + estaparcela.VitiNro.ToString() + ",'" + estaparcela.VitSedeTim.Trim() + "',"+ estaparcela.hileras.ToString().Replace(",",".")+")";
                                     command.ExecuteNonQuery();
                                 }

                                 // Recorro la lista de Operarios y los doy de alta
                                 foreach (Operario esteoperario in esteparte.operarios)
                                 {
                                     // Doy de Alta cada item de los operarios
                                     command.CommandText = "insert into Case_Partes_Operarios (id_parte, horas, cantidad,PersCod ) " +
                                                " Values (" + New_IdParte.ToString() + "," + esteoperario.horas.ToString().Replace(",", ".") + "," + esteoperario.cantidad.ToString().Replace(",", ".") + "," + esteoperario.PersCod.ToString() + ")";
                                     command.ExecuteNonQuery();
                                 }

                                 // Recorro la lista de Materiales y los doy de alta
                                 foreach (MateriasPrima estematerial in esteparte.materiasPrimas)
                                 {                                     
                                     // Doy de Alta cada item de los operarios
                                     command.CommandText = "insert into Case_Partes_Materiales (id_parte, ElabNro, unidades, LoteNro, LoteRef, LoteCant, Deposito) " +
                                                " Values (" + New_IdParte.ToString() + ",'" + estematerial.ElabNro.Trim() + "'," + estematerial.unidades.ToString().Replace(",", ".") + "," + estematerial.LOTE + ",'" + estematerial.LoteRef.Trim() + "'," + estematerial.LoteCant.ToString().Replace(",", ".") + ",'"+estematerial.Deposito+"')";
                                     command.ExecuteNonQuery();
                                 }

                                 // Recorro la lista de Maquinas y los doy de alta
                                 foreach (Maquinaria estamaquina in esteparte.maquinarias)
                                 {
                                     // Doy de Alta cada item de los operarios
                                     command.CommandText = "insert into Case_Partes_Maquinas (id_parte,  horas, mqcod ) " +
                                                " Values (" + New_IdParte.ToString() + "," + estamaquina.horas.ToString().Replace(",", ".") + ","+estamaquina.MqCod.ToString()+")";
                                     command.ExecuteNonQuery();
                                 }

                                 /*
                                 // Recorro la lista de Existencias y los doy de alta
                                 foreach (MateriasPrima estaexistencia in esteparte.materiasPrimas)
                                 {
                                     // Doy de Alta cada item de las Existencias
                                     command.CommandText = "insert into Case_Partes_Existencias (id_parte, PileCod, deposito, producto, lote, loteref, LoteCant ) " +
                                                " Values (" + New_IdParte.ToString() + ",'" + estaexistencia.PileCod + "','" + estaexistencia.deposito + "','" + estaexistencia.producto + "','" + estaexistencia.lote + "','" + estaexistencia.loteref + "'," + estaexistencia.LoteCant.ToString().Replace(",",".") + ")";
                                     command.ExecuteNonQuery();
                                 }
                                 */


                                 // Recorro la lista de Centros de Costos
                                 foreach (CentroDeCostos estecentrocosto in esteparte.centroDeCostos)
                                 {
                                     // Doy de Alta cada item de las Existencias
                                     command.CommandText = "insert into Case_Partes_CentroCosto (id_parte, CtrDsc ) " +
                                                " Values (" + New_IdParte.ToString() + ",'" + estecentrocosto.CtrDsc + "')";
                                     command.ExecuteNonQuery();
                                 }

                                 // Se grabaron correctamente todos los datos del parte
                                 resultado = "1";
                             }

                             // Attempt to commit the transaction.
                             transaction.Commit();

                             // Todo estuvo Correcto, Cierro la conexion
                             connection.Close();
                             // Devuelvo el Numero de Cliente Asignado                            

                         }
                         catch (Exception ex)
                         {
                             // Devuelvo el detalle del error para analizarlo
                             resultado = "[{\"ERROR\":\" " + ex + "\"}]";
                             // Attempt to roll back the transaction.
                             try
                             {
                                 transaction.Rollback();
                                 connection.Close();
                             }
                             catch (Exception ex2)
                             {
                                 // This catch block will handle any errors that may have occurred
                                 // on the server that would cause the rollback to fail, such as
                                 // a closed connection.
                                 //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                                 //Console.WriteLine("  Message: {0}", ex2.Message);                         
                                 connection.Close();
                             }
                         }

                         return resultado;
                         //   return "Usuario Validado";                

                     }
                     catch (Exception ex)
                     {                         
                         resultado = "[{\"ERROR\":\" "+ex+"\"}]";
                         return resultado;
                     }
                 }

             }
             else
             {
                 resultado = "[{\"ACCESO\":\"DENEGADO\"}]";
                 return resultado;
                 // return Error de Acceso                 
             }

         }

         [WebMethod]
         [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
       //  [SoapHeader("User", Required = true)]
         public string MSG_MercadoLibreJSON(string Mensaje)
         {
             string resultado = "0";
         
                 if (Mensaje == "")
                 {
                     return resultado;
                 }
                 else
                 {
                     try
                     {

                         // Doy de Alta la cabecera del Parte primero
                         //var appSettings = System.Configuration.ConfigurationSettings.AppSettings;
                         var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                         // Obtengo el valor desde <appSettings> Key Conexion                            
                         string conectionstring = "Data Source=" + ConfigurationManager.AppSettings["DataSource"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Catalog"] + "; Persist Security Info=True; User ID=Bejerman; Password=tiMCLmu27qtQwD;";

                         SqlConnection connection = new SqlConnection(conectionstring);
                         //abro la conexion para actualizar los datos.                            
                         connection.Open();

                         SqlCommand command = connection.CreateCommand();
                         SqlTransaction transaction;

                         // Begin Transaction 
                         transaction = connection.BeginTransaction("GraboParte");

                         // Must assign both transaction object and connection
                         // to Command object for a pending local transaction
                         command.Connection = connection;
                         command.Transaction = transaction;

                         try
                         {
                             // Convierto el JSON del Mensaje
                             MensajeMLA  estemensaje = JsonConvert.DeserializeObject<MensajeMLA>(Mensaje) as MensajeMLA;

                             // Doy de Alta el Parte
                             command.CommandText = "Insert into MensajesML ( user_id, resource, topic, received, applicacion_id, sent, attemps, insertdate, estado) " +
                                            " Values (" + estemensaje.user_id + ",'" + estemensaje.resource + "','" + estemensaje.topic + "','" + estemensaje.received + "',"+estemensaje.application_id+",'"+estemensaje.send+"',"+estemensaje.attemps+",GetDate(),0)";
                             command.ExecuteNonQuery();

                             // Se grabaron correctamente todos los datos del parte
                             resultado = "1";
                             

                             // Attempt to commit the transaction.
                             transaction.Commit();

                             // Todo estuvo Correcto, Cierro la conexion
                             connection.Close();
                             // Devuelvo el Numero de Cliente Asignado                            

                         }
                         catch (Exception ex)
                         {
                             // Devuelvo el detalle del error para analizarlo
                             resultado = "[{\"ERROR\":\" " + ex + "\"}]";
                             // Attempt to roll back the transaction.
                             try
                             {
                                 transaction.Rollback();
                                 connection.Close();
                             }
                             catch (Exception ex2)
                             {
                                 // This catch block will handle any errors that may have occurred
                                 // on the server that would cause the rollback to fail, such as
                                 // a closed connection.
                                 //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                                 //Console.WriteLine("  Message: {0}", ex2.Message);                         
                                 connection.Close();
                             }
                         }

                         return resultado;
                         //   return "Usuario Validado";                

                     }
                     catch (Exception ex)
                     {
                         resultado = "[{\"ERROR\":\" " + ex + "\"}]";
                         return resultado;
                     }
                 }

             
          

         }
         
         private bool ControlaAcceso()
         // Function para el control de Acceso de usuarios    
         {
             // Rutina para controlar si habilito el control de usuarios a travez de Encabezado HTTP 
             // Si el control no esta habilitado retorna el resultado
             // Si el control esta valida el usuario y la contraseña

             bool ret;

             ret=false;
             
             if (ConfigurationManager.AppSettings["Habilitar"] == "NO")
             {
               // No esta habilitada la seguridad - permito el acceso

               //  NoValidarUsuario = true;
               //  OkUsuario = true;
                 ret=true;
             }
             else
             {
                // NoValidarUsuario = false;
                 if (User != null)
                 {
                     if (User.IsValid())
                     {
                  //       OkUsuario = true;
                         ret = true;
                     }
                     else
                     {
                   //      OkUsuario = false;
                         ret = false;
                     }
                 }
                 else
                 {
                   //  OkUsuario = false;
                     ret = false;
                 }
             }

             return ret;
         }

       
        
    }
    
         
  
}
