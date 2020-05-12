
// 
// Este código fuente fue generado automáticamente por wsdl, Versión=4.0.30319.33440.
// 
namespace Microsoft.SqlServer.WebServiceFlex
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;


    /// <remarks/>
   // [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "WebService1Soap", Namespace = "http://juan/WebServiceFlex/")]   
    public partial class WebService1 : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback getUsuarioOperationCompleted;

        private System.Threading.SendOrPostCallback getUsuarioJSONOperationCompleted;

        private System.Threading.SendOrPostCallback IngresarComprobanteVentasJSONOperationCompleted;

        private System.Threading.SendOrPostCallback RecibirUsuarioJSONOperationCompleted;

        private System.Threading.SendOrPostCallback ConsultarComprobanteVentasOperationCompleted;

        private System.Threading.SendOrPostCallback ConsultarItemsVentasOperationCompleted;

        private System.Threading.SendOrPostCallback ConsultarComprobanteIVAOperationCompleted;

        private System.Threading.SendOrPostCallback ConsultarCondicionVentaOperationCompleted;

        private System.Threading.SendOrPostCallback getArticulosJSONOperationCompleted;

        private System.Threading.SendOrPostCallback InsertarNuevoUsuarioOperationCompleted;

        /// <remarks/>
        public WebService1()
        {
            this.Url = "http://JUAN/WebServiceFlex/WebServiceFlex.asmx";          
        }

        /// <remarks/>
        public event getUsuarioCompletedEventHandler getUsuarioCompleted;

        /// <remarks/>
        public event getUsuarioJSONCompletedEventHandler getUsuarioJSONCompleted;

        /// <remarks/>
        public event IngresarComprobanteVentasJSONCompletedEventHandler IngresarComprobanteVentasJSONCompleted;

        /// <remarks/>
        public event RecibirUsuarioJSONCompletedEventHandler RecibirUsuarioJSONCompleted;

        /// <remarks/>
        public event ConsultarComprobanteVentasCompletedEventHandler ConsultarComprobanteVentasCompleted;

        /// <remarks/>
        public event ConsultarItemsVentasCompletedEventHandler ConsultarItemsVentasCompleted;

        /// <remarks/>
        public event ConsultarComprobanteIVACompletedEventHandler ConsultarComprobanteIVACompleted;

        /// <remarks/>
        public event ConsultarCondicionVentaCompletedEventHandler ConsultarCondicionVentaCompleted;

        /// <remarks/>
        public event getArticulosJSONCompletedEventHandler getArticulosJSONCompleted;

        /// <remarks/>
        public event InsertarNuevoUsuarioCompletedEventHandler InsertarNuevoUsuarioCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/getUsuario", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet getUsuario()
        {
            object[] results = this.Invoke("getUsuario", new object[0]);
            return ((System.Data.DataSet)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetUsuario(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getUsuario", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public System.Data.DataSet EndgetUsuario(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }

        /// <remarks/>
        public void getUsuarioAsync()
        {
            this.getUsuarioAsync(null);
        }

        /// <remarks/>
        public void getUsuarioAsync(object userState)
        {
            if ((this.getUsuarioOperationCompleted == null))
            {
                this.getUsuarioOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetUsuarioOperationCompleted);
            }
            this.InvokeAsync("getUsuario", new object[0], this.getUsuarioOperationCompleted, userState);
        }

        private void OngetUsuarioOperationCompleted(object arg)
        {
            if ((this.getUsuarioCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getUsuarioCompleted(this, new getUsuarioCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/getUsuarioJSON", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getUsuarioJSON()
        {
            object[] results = this.Invoke("getUsuarioJSON", new object[0]);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetUsuarioJSON(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getUsuarioJSON", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public string EndgetUsuarioJSON(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void getUsuarioJSONAsync()
        {
            this.getUsuarioJSONAsync(null);
        }

        /// <remarks/>
        public void getUsuarioJSONAsync(object userState)
        {
            if ((this.getUsuarioJSONOperationCompleted == null))
            {
                this.getUsuarioJSONOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetUsuarioJSONOperationCompleted);
            }
            this.InvokeAsync("getUsuarioJSON", new object[0], this.getUsuarioJSONOperationCompleted, userState);
        }

        private void OngetUsuarioJSONOperationCompleted(object arg)
        {
            if ((this.getUsuarioJSONCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getUsuarioJSONCompleted(this, new getUsuarioJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/IngresarComprobanteVentasJSON", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string IngresarComprobanteVentasJSON(string json)
        {
            object[] results = this.Invoke("IngresarComprobanteVentasJSON", new object[] {
                        json});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginIngresarComprobanteVentasJSON(string json, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("IngresarComprobanteVentasJSON", new object[] {
                        json}, callback, asyncState);
        }

        /// <remarks/>
        public string EndIngresarComprobanteVentasJSON(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void IngresarComprobanteVentasJSONAsync(string json)
        {
            this.IngresarComprobanteVentasJSONAsync(json, null);
        }

        /// <remarks/>
        public void IngresarComprobanteVentasJSONAsync(string json, object userState)
        {
            if ((this.IngresarComprobanteVentasJSONOperationCompleted == null))
            {
                this.IngresarComprobanteVentasJSONOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIngresarComprobanteVentasJSONOperationCompleted);
            }
            this.InvokeAsync("IngresarComprobanteVentasJSON", new object[] {
                        json}, this.IngresarComprobanteVentasJSONOperationCompleted, userState);
        }

        private void OnIngresarComprobanteVentasJSONOperationCompleted(object arg)
        {
            if ((this.IngresarComprobanteVentasJSONCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IngresarComprobanteVentasJSONCompleted(this, new IngresarComprobanteVentasJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/RecibirUsuarioJSON", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string RecibirUsuarioJSON(string json)
        {
            object[] results = this.Invoke("RecibirUsuarioJSON", new object[] {
                        json});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginRecibirUsuarioJSON(string json, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RecibirUsuarioJSON", new object[] {
                        json}, callback, asyncState);
        }

        /// <remarks/>
        public string EndRecibirUsuarioJSON(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void RecibirUsuarioJSONAsync(string json)
        {
            this.RecibirUsuarioJSONAsync(json, null);
        }

        /// <remarks/>
        public void RecibirUsuarioJSONAsync(string json, object userState)
        {
            if ((this.RecibirUsuarioJSONOperationCompleted == null))
            {
                this.RecibirUsuarioJSONOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRecibirUsuarioJSONOperationCompleted);
            }
            this.InvokeAsync("RecibirUsuarioJSON", new object[] {
                        json}, this.RecibirUsuarioJSONOperationCompleted, userState);
        }

        private void OnRecibirUsuarioJSONOperationCompleted(object arg)
        {
            if ((this.RecibirUsuarioJSONCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RecibirUsuarioJSONCompleted(this, new RecibirUsuarioJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/ConsultarComprobanteVentas", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ConsultarComprobanteVentas(string NroComprobante)
        {
            object[] results = this.Invoke("ConsultarComprobanteVentas", new object[] {
                        NroComprobante});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginConsultarComprobanteVentas(string NroComprobante, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ConsultarComprobanteVentas", new object[] {
                        NroComprobante}, callback, asyncState);
        }

        /// <remarks/>
        public string EndConsultarComprobanteVentas(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void ConsultarComprobanteVentasAsync(string NroComprobante)
        {
            this.ConsultarComprobanteVentasAsync(NroComprobante, null);
        }

        /// <remarks/>
        public void ConsultarComprobanteVentasAsync(string NroComprobante, object userState)
        {
            if ((this.ConsultarComprobanteVentasOperationCompleted == null))
            {
                this.ConsultarComprobanteVentasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultarComprobanteVentasOperationCompleted);
            }
            this.InvokeAsync("ConsultarComprobanteVentas", new object[] {
                        NroComprobante}, this.ConsultarComprobanteVentasOperationCompleted, userState);
        }

        private void OnConsultarComprobanteVentasOperationCompleted(object arg)
        {
            if ((this.ConsultarComprobanteVentasCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultarComprobanteVentasCompleted(this, new ConsultarComprobanteVentasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/ConsultarItemsVentas", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ConsultarItemsVentas(string NroComprobante)
        {
            object[] results = this.Invoke("ConsultarItemsVentas", new object[] {
                        NroComprobante});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginConsultarItemsVentas(string NroComprobante, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ConsultarItemsVentas", new object[] {
                        NroComprobante}, callback, asyncState);
        }

        /// <remarks/>
        public string EndConsultarItemsVentas(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void ConsultarItemsVentasAsync(string NroComprobante)
        {
            this.ConsultarItemsVentasAsync(NroComprobante, null);
        }

        /// <remarks/>
        public void ConsultarItemsVentasAsync(string NroComprobante, object userState)
        {
            if ((this.ConsultarItemsVentasOperationCompleted == null))
            {
                this.ConsultarItemsVentasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultarItemsVentasOperationCompleted);
            }
            this.InvokeAsync("ConsultarItemsVentas", new object[] {
                        NroComprobante}, this.ConsultarItemsVentasOperationCompleted, userState);
        }

        private void OnConsultarItemsVentasOperationCompleted(object arg)
        {
            if ((this.ConsultarItemsVentasCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultarItemsVentasCompleted(this, new ConsultarItemsVentasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/ConsultarComprobanteIVA", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ConsultarComprobanteIVA(string NroComprobante)
        {
            object[] results = this.Invoke("ConsultarComprobanteIVA", new object[] {
                        NroComprobante});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginConsultarComprobanteIVA(string NroComprobante, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ConsultarComprobanteIVA", new object[] {
                        NroComprobante}, callback, asyncState);
        }

        /// <remarks/>
        public string EndConsultarComprobanteIVA(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void ConsultarComprobanteIVAAsync(string NroComprobante)
        {
            this.ConsultarComprobanteIVAAsync(NroComprobante, null);
        }

        /// <remarks/>
        public void ConsultarComprobanteIVAAsync(string NroComprobante, object userState)
        {
            if ((this.ConsultarComprobanteIVAOperationCompleted == null))
            {
                this.ConsultarComprobanteIVAOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultarComprobanteIVAOperationCompleted);
            }
            this.InvokeAsync("ConsultarComprobanteIVA", new object[] {
                        NroComprobante}, this.ConsultarComprobanteIVAOperationCompleted, userState);
        }

        private void OnConsultarComprobanteIVAOperationCompleted(object arg)
        {
            if ((this.ConsultarComprobanteIVACompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultarComprobanteIVACompleted(this, new ConsultarComprobanteIVACompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/ConsultarCondicionVenta", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ConsultarCondicionVenta(string CondVenta)
        {
            object[] results = this.Invoke("ConsultarCondicionVenta", new object[] {
                        CondVenta});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginConsultarCondicionVenta(string CondVenta, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ConsultarCondicionVenta", new object[] {
                        CondVenta}, callback, asyncState);
        }

        /// <remarks/>
        public string EndConsultarCondicionVenta(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void ConsultarCondicionVentaAsync(string CondVenta)
        {
            this.ConsultarCondicionVentaAsync(CondVenta, null);
        }

        /// <remarks/>
        public void ConsultarCondicionVentaAsync(string CondVenta, object userState)
        {
            if ((this.ConsultarCondicionVentaOperationCompleted == null))
            {
                this.ConsultarCondicionVentaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultarCondicionVentaOperationCompleted);
            }
            this.InvokeAsync("ConsultarCondicionVenta", new object[] {
                        CondVenta}, this.ConsultarCondicionVentaOperationCompleted, userState);
        }

        private void OnConsultarCondicionVentaOperationCompleted(object arg)
        {
            if ((this.ConsultarCondicionVentaCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultarCondicionVentaCompleted(this, new ConsultarCondicionVentaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/getArticulosJSON", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getArticulosJSON(string CodigoGenerico)
        {
            object[] results = this.Invoke("getArticulosJSON", new object[] {
                        CodigoGenerico});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BegingetArticulosJSON(string CodigoGenerico, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getArticulosJSON", new object[] {
                        CodigoGenerico}, callback, asyncState);
        }

        /// <remarks/>
        public string EndgetArticulosJSON(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void getArticulosJSONAsync(string CodigoGenerico)
        {
            this.getArticulosJSONAsync(CodigoGenerico, null);
        }

        /// <remarks/>
        public void getArticulosJSONAsync(string CodigoGenerico, object userState)
        {
            if ((this.getArticulosJSONOperationCompleted == null))
            {
                this.getArticulosJSONOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetArticulosJSONOperationCompleted);
            }
            this.InvokeAsync("getArticulosJSON", new object[] {
                        CodigoGenerico}, this.getArticulosJSONOperationCompleted, userState);
        }

        private void OngetArticulosJSONOperationCompleted(object arg)
        {
            if ((this.getArticulosJSONCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getArticulosJSONCompleted(this, new getArticulosJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://JUAN/WebServiceFlex/InsertarNuevoUsuario", RequestNamespace = "http://juan/WebServiceFlex/", ResponseNamespace = "http://juan/WebServiceFlex/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int InsertarNuevoUsuario(string Nombre, string Apellidos, string Telefono, string Email)
        {
            object[] results = this.Invoke("InsertarNuevoUsuario", new object[] {
                        Nombre,
                        Apellidos,
                        Telefono,
                        Email});
            return ((int)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginInsertarNuevoUsuario(string Nombre, string Apellidos, string Telefono, string Email, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("InsertarNuevoUsuario", new object[] {
                        Nombre,
                        Apellidos,
                        Telefono,
                        Email}, callback, asyncState);
        }

        /// <remarks/>
        public int EndInsertarNuevoUsuario(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void InsertarNuevoUsuarioAsync(string Nombre, string Apellidos, string Telefono, string Email)
        {
            this.InsertarNuevoUsuarioAsync(Nombre, Apellidos, Telefono, Email, null);
        }

        /// <remarks/>
        public void InsertarNuevoUsuarioAsync(string Nombre, string Apellidos, string Telefono, string Email, object userState)
        {
            if ((this.InsertarNuevoUsuarioOperationCompleted == null))
            {
                this.InsertarNuevoUsuarioOperationCompleted = new System.Threading.SendOrPostCallback(this.OnInsertarNuevoUsuarioOperationCompleted);
            }
            this.InvokeAsync("InsertarNuevoUsuario", new object[] {
                        Nombre,
                        Apellidos,
                        Telefono,
                        Email}, this.InsertarNuevoUsuarioOperationCompleted, userState);
        }

        private void OnInsertarNuevoUsuarioOperationCompleted(object arg)
        {
            if ((this.InsertarNuevoUsuarioCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.InsertarNuevoUsuarioCompleted(this, new InsertarNuevoUsuarioCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void getUsuarioCompletedEventHandler(object sender, getUsuarioCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getUsuarioCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getUsuarioCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public System.Data.DataSet Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void getUsuarioJSONCompletedEventHandler(object sender, getUsuarioJSONCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getUsuarioJSONCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getUsuarioJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void IngresarComprobanteVentasJSONCompletedEventHandler(object sender, IngresarComprobanteVentasJSONCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IngresarComprobanteVentasJSONCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal IngresarComprobanteVentasJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void RecibirUsuarioJSONCompletedEventHandler(object sender, RecibirUsuarioJSONCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RecibirUsuarioJSONCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal RecibirUsuarioJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void ConsultarComprobanteVentasCompletedEventHandler(object sender, ConsultarComprobanteVentasCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultarComprobanteVentasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ConsultarComprobanteVentasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void ConsultarItemsVentasCompletedEventHandler(object sender, ConsultarItemsVentasCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultarItemsVentasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ConsultarItemsVentasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void ConsultarComprobanteIVACompletedEventHandler(object sender, ConsultarComprobanteIVACompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultarComprobanteIVACompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ConsultarComprobanteIVACompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void ConsultarCondicionVentaCompletedEventHandler(object sender, ConsultarCondicionVentaCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultarCondicionVentaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ConsultarCondicionVentaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void getArticulosJSONCompletedEventHandler(object sender, getArticulosJSONCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getArticulosJSONCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal getArticulosJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    public delegate void InsertarNuevoUsuarioCompletedEventHandler(object sender, InsertarNuevoUsuarioCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class InsertarNuevoUsuarioCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal InsertarNuevoUsuarioCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
}