﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4952
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.1432.
// 
namespace ScalaWS.Thumbnail {
    using System.Xml.Serialization;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Diagnostics;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="thumbnailServiceSoapBinding", Namespace="http://v1.2.api.cm.scala.com")]
    public partial class thumbnailService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback getThumbnailAsBytesOperationCompleted;
        
        private System.Threading.SendOrPostCallback getThumbnailURLOperationCompleted;
        
        private System.Threading.SendOrPostCallback generateThumbnailOperationCompleted;
        
        private System.Threading.SendOrPostCallback isDoneOperationCompleted;
        
        private string auth;
        
        public thumbnailService(string username, string password, string url) {
 	        this.Url = url + "/api/v1.2/thumbnail";
 	        auth = username + ":" + password;
 	        byte[] binaryData = new Byte[auth.Length];
 	        binaryData = System.Text.Encoding.UTF8.GetBytes(auth);
 	        auth = Convert.ToBase64String(binaryData);
 	        auth = "Basic " + auth;
        }
        
        /// <remarks/>
        public event getThumbnailAsBytesCompletedEventHandler getThumbnailAsBytesCompleted;
        
        /// <remarks/>
        public event getThumbnailURLCompletedEventHandler getThumbnailURLCompleted;
        
        /// <remarks/>
        public event generateThumbnailCompletedEventHandler generateThumbnailCompleted;
        
        /// <remarks/>
        public event isDoneCompletedEventHandler isDoneCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("thumbnailPNG", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getThumbnailAsBytes([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string uuid) {
            object[] results = this.Invoke("getThumbnailAsBytes", new object[] {
                        uuid});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BegingetThumbnailAsBytes(string uuid, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("getThumbnailAsBytes", new object[] {
                        uuid}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndgetThumbnailAsBytes(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getThumbnailAsBytesAsync(string uuid) {
            this.getThumbnailAsBytesAsync(uuid, null);
        }
        
        /// <remarks/>
        public void getThumbnailAsBytesAsync(string uuid, object userState) {
            if ((this.getThumbnailAsBytesOperationCompleted == null)) {
                this.getThumbnailAsBytesOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetThumbnailAsBytesOperationCompleted);
            }
            this.InvokeAsync("getThumbnailAsBytes", new object[] {
                        uuid}, this.getThumbnailAsBytesOperationCompleted, userState);
        }
        
        private void OngetThumbnailAsBytesOperationCompleted(object arg) {
            if ((this.getThumbnailAsBytesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getThumbnailAsBytesCompleted(this, new getThumbnailAsBytesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("thumbnailUrl", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] getThumbnailURL([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string uuid) {
            object[] results = this.Invoke("getThumbnailURL", new object[] {
                        uuid});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BegingetThumbnailURL(string uuid, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("getThumbnailURL", new object[] {
                        uuid}, callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndgetThumbnailURL(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void getThumbnailURLAsync(string uuid) {
            this.getThumbnailURLAsync(uuid, null);
        }
        
        /// <remarks/>
        public void getThumbnailURLAsync(string uuid, object userState) {
            if ((this.getThumbnailURLOperationCompleted == null)) {
                this.getThumbnailURLOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetThumbnailURLOperationCompleted);
            }
            this.InvokeAsync("getThumbnailURL", new object[] {
                        uuid}, this.getThumbnailURLOperationCompleted, userState);
        }
        
        private void OngetThumbnailURLOperationCompleted(object arg) {
            if ((this.getThumbnailURLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getThumbnailURLCompleted(this, new getThumbnailURLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("generateThumbnail", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string generateThumbnail([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int mediaId, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool mediaIdSpecified, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int width, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool widthSpecified, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int height, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool heightSpecified, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] thumbnailFormat thumbnailFormat, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool thumbnailFormatSpecified) {
            object[] results = this.Invoke("generateThumbnail", new object[] {
                        mediaId,
                        mediaIdSpecified,
                        width,
                        widthSpecified,
                        height,
                        heightSpecified,
                        thumbnailFormat,
                        thumbnailFormatSpecified});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BegingenerateThumbnail(int mediaId, bool mediaIdSpecified, int width, bool widthSpecified, int height, bool heightSpecified, thumbnailFormat thumbnailFormat, bool thumbnailFormatSpecified, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("generateThumbnail", new object[] {
                        mediaId,
                        mediaIdSpecified,
                        width,
                        widthSpecified,
                        height,
                        heightSpecified,
                        thumbnailFormat,
                        thumbnailFormatSpecified}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndgenerateThumbnail(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void generateThumbnailAsync(int mediaId, bool mediaIdSpecified, int width, bool widthSpecified, int height, bool heightSpecified, thumbnailFormat thumbnailFormat, bool thumbnailFormatSpecified) {
            this.generateThumbnailAsync(mediaId, mediaIdSpecified, width, widthSpecified, height, heightSpecified, thumbnailFormat, thumbnailFormatSpecified, null);
        }
        
        /// <remarks/>
        public void generateThumbnailAsync(int mediaId, bool mediaIdSpecified, int width, bool widthSpecified, int height, bool heightSpecified, thumbnailFormat thumbnailFormat, bool thumbnailFormatSpecified, object userState) {
            if ((this.generateThumbnailOperationCompleted == null)) {
                this.generateThumbnailOperationCompleted = new System.Threading.SendOrPostCallback(this.OngenerateThumbnailOperationCompleted);
            }
            this.InvokeAsync("generateThumbnail", new object[] {
                        mediaId,
                        mediaIdSpecified,
                        width,
                        widthSpecified,
                        height,
                        heightSpecified,
                        thumbnailFormat,
                        thumbnailFormatSpecified}, this.generateThumbnailOperationCompleted, userState);
        }
        
        private void OngenerateThumbnailOperationCompleted(object arg) {
            if ((this.generateThumbnailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.generateThumbnailCompleted(this, new generateThumbnailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void isDone([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string uuid, [System.Xml.Serialization.XmlElementAttribute("isDone", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] out bool isDone1, [System.Xml.Serialization.XmlElementAttribute("isDone", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] out bool isDone1Specified) {
            object[] results = this.Invoke("isDone", new object[] {
                        uuid});
            isDone1 = ((bool)(results[0]));
            isDone1Specified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginisDone(string uuid, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("isDone", new object[] {
                        uuid}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndisDone(System.IAsyncResult asyncResult, out bool isDone1, out bool isDone1Specified) {
            object[] results = this.EndInvoke(asyncResult);
            isDone1 = ((bool)(results[0]));
            isDone1Specified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public void isDoneAsync(string uuid) {
            this.isDoneAsync(uuid, null);
        }
        
        /// <remarks/>
        public void isDoneAsync(string uuid, object userState) {
            if ((this.isDoneOperationCompleted == null)) {
                this.isDoneOperationCompleted = new System.Threading.SendOrPostCallback(this.OnisDoneOperationCompleted);
            }
            this.InvokeAsync("isDone", new object[] {
                        uuid}, this.isDoneOperationCompleted, userState);
        }
        
        private void OnisDoneOperationCompleted(object arg) {
            if ((this.isDoneCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.isDoneCompleted(this, new isDoneCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }

        /// override this to support basic authentication
        protected override System.Net.WebRequest GetWebRequest(Uri uri) {
            System.Net.WebRequest request = base.GetWebRequest(uri);

            request.Headers["AUTHORIZATION"] = auth;
            return request;
        }

    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://v1.2.api.cm.scala.com")]
    public enum thumbnailFormat {
        
        /// <remarks/>
        PNG,
        
        /// <remarks/>
        JPEG,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void getThumbnailAsBytesCompletedEventHandler(object sender, getThumbnailAsBytesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getThumbnailAsBytesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getThumbnailAsBytesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void getThumbnailURLCompletedEventHandler(object sender, getThumbnailURLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getThumbnailURLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getThumbnailURLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void generateThumbnailCompletedEventHandler(object sender, generateThumbnailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class generateThumbnailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal generateThumbnailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void isDoneCompletedEventHandler(object sender, isDoneCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class isDoneCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal isDoneCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool isDone1 {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public bool isDone1Specified {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[1]));
            }
        }
    }
}
