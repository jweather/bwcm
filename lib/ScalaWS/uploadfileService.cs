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
namespace ScalaWS.UploadFile {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="uploadfileServiceSoapBinding", Namespace="http://v1.2.api.cm.scala.com")]
    public partial class uploadfileService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback getMissingFileListStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback downloadMissingFileListAsXmlOperationCompleted;
        
        private System.Threading.SendOrPostCallback getStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback requestMissingFileListOperationCompleted;
        
        private System.Threading.SendOrPostCallback requestUploadOperationCompleted;
        
        private System.Threading.SendOrPostCallback uploadFinishedOperationCompleted;
        
        private string auth;
        
        /// <remarks/>
        public uploadfileService(string username, string password, string url) {
 	        this.Url = url + "/api/v1.2/uploadfile";
 	        auth = username + ":" + password;
 	        byte[] binaryData = new Byte[auth.Length];
 	        binaryData = System.Text.Encoding.UTF8.GetBytes(auth);
 	        auth = Convert.ToBase64String(binaryData);
 	        auth = "Basic " + auth;
        }
        
        /// <remarks/>
        public event getMissingFileListStatusCompletedEventHandler getMissingFileListStatusCompleted;
        
        /// <remarks/>
        public event downloadMissingFileListAsXmlCompletedEventHandler downloadMissingFileListAsXmlCompleted;
        
        /// <remarks/>
        public event getStatusCompletedEventHandler getStatusCompleted;
        
        /// <remarks/>
        public event requestMissingFileListCompletedEventHandler requestMissingFileListCompleted;
        
        /// <remarks/>
        public event requestUploadCompletedEventHandler requestUploadCompleted;
        
        /// <remarks/>
        public event uploadFinishedCompletedEventHandler uploadFinishedCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void getMissingFileListStatus([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string uuid, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] out uploadStatusEnum @return, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] out bool returnSpecified) {
            object[] results = this.Invoke("getMissingFileListStatus", new object[] {
                        uuid});
            @return = ((uploadStatusEnum)(results[0]));
            returnSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BegingetMissingFileListStatus(string uuid, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("getMissingFileListStatus", new object[] {
                        uuid}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndgetMissingFileListStatus(System.IAsyncResult asyncResult, out uploadStatusEnum @return, out bool returnSpecified) {
            object[] results = this.EndInvoke(asyncResult);
            @return = ((uploadStatusEnum)(results[0]));
            returnSpecified = ((bool)(results[1]));
        }
        
        /// <remarks/>
        public void getMissingFileListStatusAsync(string uuid) {
            this.getMissingFileListStatusAsync(uuid, null);
        }
        
        /// <remarks/>
        public void getMissingFileListStatusAsync(string uuid, object userState) {
            if ((this.getMissingFileListStatusOperationCompleted == null)) {
                this.getMissingFileListStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetMissingFileListStatusOperationCompleted);
            }
            this.InvokeAsync("getMissingFileListStatus", new object[] {
                        uuid}, this.getMissingFileListStatusOperationCompleted, userState);
        }
        
        private void OngetMissingFileListStatusOperationCompleted(object arg) {
            if ((this.getMissingFileListStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getMissingFileListStatusCompleted(this, new getMissingFileListStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("missingFileList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string downloadMissingFileListAsXml([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string uuid) {
            object[] results = this.Invoke("downloadMissingFileListAsXml", new object[] {
                        uuid});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BegindownloadMissingFileListAsXml(string uuid, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("downloadMissingFileListAsXml", new object[] {
                        uuid}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EnddownloadMissingFileListAsXml(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void downloadMissingFileListAsXmlAsync(string uuid) {
            this.downloadMissingFileListAsXmlAsync(uuid, null);
        }
        
        /// <remarks/>
        public void downloadMissingFileListAsXmlAsync(string uuid, object userState) {
            if ((this.downloadMissingFileListAsXmlOperationCompleted == null)) {
                this.downloadMissingFileListAsXmlOperationCompleted = new System.Threading.SendOrPostCallback(this.OndownloadMissingFileListAsXmlOperationCompleted);
            }
            this.InvokeAsync("downloadMissingFileListAsXml", new object[] {
                        uuid}, this.downloadMissingFileListAsXmlOperationCompleted, userState);
        }
        
        private void OndownloadMissingFileListAsXmlOperationCompleted(object arg) {
            if ((this.downloadMissingFileListAsXmlCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.downloadMissingFileListAsXmlCompleted(this, new downloadMissingFileListAsXmlCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public fileUploadStatusTO getStatus([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool arg0Specified) {
            object[] results = this.Invoke("getStatus", new object[] {
                        arg0,
                        arg0Specified});
            return ((fileUploadStatusTO)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BegingetStatus(int arg0, bool arg0Specified, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("getStatus", new object[] {
                        arg0,
                        arg0Specified}, callback, asyncState);
        }
        
        /// <remarks/>
        public fileUploadStatusTO EndgetStatus(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((fileUploadStatusTO)(results[0]));
        }
        
        /// <remarks/>
        public void getStatusAsync(int arg0, bool arg0Specified) {
            this.getStatusAsync(arg0, arg0Specified, null);
        }
        
        /// <remarks/>
        public void getStatusAsync(int arg0, bool arg0Specified, object userState) {
            if ((this.getStatusOperationCompleted == null)) {
                this.getStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStatusOperationCompleted);
            }
            this.InvokeAsync("getStatus", new object[] {
                        arg0,
                        arg0Specified}, this.getStatusOperationCompleted, userState);
        }
        
        private void OngetStatusOperationCompleted(object arg) {
            if ((this.getStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getStatusCompleted(this, new getStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("missingFileList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string requestMissingFileList([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int fileId, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool fileIdSpecified) {
            object[] results = this.Invoke("requestMissingFileList", new object[] {
                        fileId,
                        fileIdSpecified});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginrequestMissingFileList(int fileId, bool fileIdSpecified, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("requestMissingFileList", new object[] {
                        fileId,
                        fileIdSpecified}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndrequestMissingFileList(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void requestMissingFileListAsync(int fileId, bool fileIdSpecified) {
            this.requestMissingFileListAsync(fileId, fileIdSpecified, null);
        }
        
        /// <remarks/>
        public void requestMissingFileListAsync(int fileId, bool fileIdSpecified, object userState) {
            if ((this.requestMissingFileListOperationCompleted == null)) {
                this.requestMissingFileListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnrequestMissingFileListOperationCompleted);
            }
            this.InvokeAsync("requestMissingFileList", new object[] {
                        fileId,
                        fileIdSpecified}, this.requestMissingFileListOperationCompleted, userState);
        }
        
        private void OnrequestMissingFileListOperationCompleted(object arg) {
            if ((this.requestMissingFileListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.requestMissingFileListCompleted(this, new requestMissingFileListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public fileUploadTO requestUpload([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] requestFileTO arg0) {
            object[] results = this.Invoke("requestUpload", new object[] {
                        arg0});
            return ((fileUploadTO)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginrequestUpload(requestFileTO arg0, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("requestUpload", new object[] {
                        arg0}, callback, asyncState);
        }
        
        /// <remarks/>
        public fileUploadTO EndrequestUpload(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((fileUploadTO)(results[0]));
        }
        
        /// <remarks/>
        public void requestUploadAsync(requestFileTO arg0) {
            this.requestUploadAsync(arg0, null);
        }
        
        /// <remarks/>
        public void requestUploadAsync(requestFileTO arg0, object userState) {
            if ((this.requestUploadOperationCompleted == null)) {
                this.requestUploadOperationCompleted = new System.Threading.SendOrPostCallback(this.OnrequestUploadOperationCompleted);
            }
            this.InvokeAsync("requestUpload", new object[] {
                        arg0}, this.requestUploadOperationCompleted, userState);
        }
        
        private void OnrequestUploadOperationCompleted(object arg) {
            if ((this.requestUploadCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.requestUploadCompleted(this, new requestUploadCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://v1.2.api.cm.scala.com", ResponseNamespace="http://v1.2.api.cm.scala.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void uploadFinished([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool arg0Specified) {
            this.Invoke("uploadFinished", new object[] {
                        arg0,
                        arg0Specified});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginuploadFinished(int arg0, bool arg0Specified, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("uploadFinished", new object[] {
                        arg0,
                        arg0Specified}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EnduploadFinished(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        public void uploadFinishedAsync(int arg0, bool arg0Specified) {
            this.uploadFinishedAsync(arg0, arg0Specified, null);
        }
        
        /// <remarks/>
        public void uploadFinishedAsync(int arg0, bool arg0Specified, object userState) {
            if ((this.uploadFinishedOperationCompleted == null)) {
                this.uploadFinishedOperationCompleted = new System.Threading.SendOrPostCallback(this.OnuploadFinishedOperationCompleted);
            }
            this.InvokeAsync("uploadFinished", new object[] {
                        arg0,
                        arg0Specified}, this.uploadFinishedOperationCompleted, userState);
        }
        
        private void OnuploadFinishedOperationCompleted(object arg) {
            if ((this.uploadFinishedCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.uploadFinishedCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://v1.api.cm.scala.com")]
    public enum uploadStatusEnum {
        
        /// <remarks/>
        FINISHED,
        
        /// <remarks/>
        UPLOADING,
        
        /// <remarks/>
        INCOMPLETE,
        
        /// <remarks/>
        DELETED,
        
        /// <remarks/>
        NOT_EXISTS,
        
        /// <remarks/>
        ERROR,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://v1.api.cm.scala.com")]
    public partial class fileUploadStatusTO {
        
        private long currentUploadSizeField;
        
        private string filesizeField;
        
        private string statusMessageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long currentUploadSize {
            get {
                return this.currentUploadSizeField;
            }
            set {
                this.currentUploadSizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string filesize {
            get {
                return this.filesizeField;
            }
            set {
                this.filesizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string statusMessage {
            get {
                return this.statusMessageField;
            }
            set {
                this.statusMessageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://v1.api.cm.scala.com")]
    public partial class fileUploadTO {
        
        private int fileIdField;
        
        private bool fileIdFieldSpecified;
        
        private int mediaItemIdField;
        
        private bool mediaItemIdFieldSpecified;
        
        private bool skipableField;
        
        private bool skipableFieldSpecified;
        
        private string uploadAsFilenameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int fileId {
            get {
                return this.fileIdField;
            }
            set {
                this.fileIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fileIdSpecified {
            get {
                return this.fileIdFieldSpecified;
            }
            set {
                this.fileIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int mediaItemId {
            get {
                return this.mediaItemIdField;
            }
            set {
                this.mediaItemIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool mediaItemIdSpecified {
            get {
                return this.mediaItemIdFieldSpecified;
            }
            set {
                this.mediaItemIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool skipable {
            get {
                return this.skipableField;
            }
            set {
                this.skipableField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool skipableSpecified {
            get {
                return this.skipableFieldSpecified;
            }
            set {
                this.skipableFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string uploadAsFilename {
            get {
                return this.uploadAsFilenameField;
            }
            set {
                this.uploadAsFilenameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://v1.api.cm.scala.com")]
    public partial class requestFileTO {
        
        private string filenameField;
        
        private string pathField;
        
        private long sizeField;
        
        private bool sizeFieldSpecified;
        
        private uploadTypeEnum typeField;
        
        private bool typeFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string filename {
            get {
                return this.filenameField;
            }
            set {
                this.filenameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long size {
            get {
                return this.sizeField;
            }
            set {
                this.sizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool sizeSpecified {
            get {
                return this.sizeFieldSpecified;
            }
            set {
                this.sizeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uploadTypeEnum type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool typeSpecified {
            get {
                return this.typeFieldSpecified;
            }
            set {
                this.typeFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://v1.api.cm.scala.com")]
    public enum uploadTypeEnum {
        
        /// <remarks/>
        MAINTENANCE,
        
        /// <remarks/>
        MEDIA,
        
        /// <remarks/>
        SCALASCRIPT,
        
        /// <remarks/>
        SCALASCRIPT_PARTFILE,
        
        /// <remarks/>
        TEMPLATE,
        
        /// <remarks/>
        TEMPLATE_PARTFILE,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void getMissingFileListStatusCompletedEventHandler(object sender, getMissingFileListStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getMissingFileListStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getMissingFileListStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public uploadStatusEnum @return {
            get {
                this.RaiseExceptionIfNecessary();
                return ((uploadStatusEnum)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public bool returnSpecified {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void downloadMissingFileListAsXmlCompletedEventHandler(object sender, downloadMissingFileListAsXmlCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class downloadMissingFileListAsXmlCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal downloadMissingFileListAsXmlCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public delegate void getStatusCompletedEventHandler(object sender, getStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public fileUploadStatusTO Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((fileUploadStatusTO)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void requestMissingFileListCompletedEventHandler(object sender, requestMissingFileListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class requestMissingFileListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal requestMissingFileListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public delegate void requestUploadCompletedEventHandler(object sender, requestUploadCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class requestUploadCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal requestUploadCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public fileUploadTO Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((fileUploadTO)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    public delegate void uploadFinishedCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}
