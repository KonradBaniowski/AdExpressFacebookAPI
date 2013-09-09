﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.17929.
// 
#pragma warning disable 1591

namespace KMI.PromoPSA.Rules.Rights {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RightsSoap", Namespace="http://tempuri.org/")]
    public partial class Rights : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback IsAccessibleOperationCompleted;
        
        private System.Threading.SendOrPostCallback CanAccessToProjectOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetModuleIdsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetFlagIdsOperationCompleted;
        
        private System.Threading.SendOrPostCallback CanAccessToEvaliantOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetEvaliantModuleIdsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetEvaliantLoginIdOperationCompleted;
        
        private System.Threading.SendOrPostCallback CanAccessToBastetOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Rights() {
            this.Url = global::KMI.PromoPSA.Rules.Properties.Settings.Default.KMI_PromoPSA_Rules_Rights_Rights;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event IsAccessibleCompletedEventHandler IsAccessibleCompleted;
        
        /// <remarks/>
        public event CanAccessToProjectCompletedEventHandler CanAccessToProjectCompleted;
        
        /// <remarks/>
        public event GetModuleIdsCompletedEventHandler GetModuleIdsCompleted;
        
        /// <remarks/>
        public event GetFlagIdsCompletedEventHandler GetFlagIdsCompleted;
        
        /// <remarks/>
        public event CanAccessToEvaliantCompletedEventHandler CanAccessToEvaliantCompleted;
        
        /// <remarks/>
        public event GetEvaliantModuleIdsCompletedEventHandler GetEvaliantModuleIdsCompleted;
        
        /// <remarks/>
        public event GetEvaliantLoginIdCompletedEventHandler GetEvaliantLoginIdCompleted;
        
        /// <remarks/>
        public event CanAccessToBastetCompletedEventHandler CanAccessToBastetCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IsAccessible", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool IsAccessible() {
            object[] results = this.Invoke("IsAccessible", new object[0]);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void IsAccessibleAsync() {
            this.IsAccessibleAsync(null);
        }
        
        /// <remarks/>
        public void IsAccessibleAsync(object userState) {
            if ((this.IsAccessibleOperationCompleted == null)) {
                this.IsAccessibleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIsAccessibleOperationCompleted);
            }
            this.InvokeAsync("IsAccessible", new object[0], this.IsAccessibleOperationCompleted, userState);
        }
        
        private void OnIsAccessibleOperationCompleted(object arg) {
            if ((this.IsAccessibleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IsAccessibleCompleted(this, new IsAccessibleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CanAccessToProject", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool CanAccessToProject(string login, string password, long projectId) {
            object[] results = this.Invoke("CanAccessToProject", new object[] {
                        login,
                        password,
                        projectId});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void CanAccessToProjectAsync(string login, string password, long projectId) {
            this.CanAccessToProjectAsync(login, password, projectId, null);
        }
        
        /// <remarks/>
        public void CanAccessToProjectAsync(string login, string password, long projectId, object userState) {
            if ((this.CanAccessToProjectOperationCompleted == null)) {
                this.CanAccessToProjectOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCanAccessToProjectOperationCompleted);
            }
            this.InvokeAsync("CanAccessToProject", new object[] {
                        login,
                        password,
                        projectId}, this.CanAccessToProjectOperationCompleted, userState);
        }
        
        private void OnCanAccessToProjectOperationCompleted(object arg) {
            if ((this.CanAccessToProjectCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CanAccessToProjectCompleted(this, new CanAccessToProjectCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetModuleIds", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long[] GetModuleIds(string login, string password, long projectId) {
            object[] results = this.Invoke("GetModuleIds", new object[] {
                        login,
                        password,
                        projectId});
            return ((long[])(results[0]));
        }
        
        /// <remarks/>
        public void GetModuleIdsAsync(string login, string password, long projectId) {
            this.GetModuleIdsAsync(login, password, projectId, null);
        }
        
        /// <remarks/>
        public void GetModuleIdsAsync(string login, string password, long projectId, object userState) {
            if ((this.GetModuleIdsOperationCompleted == null)) {
                this.GetModuleIdsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetModuleIdsOperationCompleted);
            }
            this.InvokeAsync("GetModuleIds", new object[] {
                        login,
                        password,
                        projectId}, this.GetModuleIdsOperationCompleted, userState);
        }
        
        private void OnGetModuleIdsOperationCompleted(object arg) {
            if ((this.GetModuleIdsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetModuleIdsCompleted(this, new GetModuleIdsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFlagIds", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long[] GetFlagIds(string login, string password, long projectId) {
            object[] results = this.Invoke("GetFlagIds", new object[] {
                        login,
                        password,
                        projectId});
            return ((long[])(results[0]));
        }
        
        /// <remarks/>
        public void GetFlagIdsAsync(string login, string password, long projectId) {
            this.GetFlagIdsAsync(login, password, projectId, null);
        }
        
        /// <remarks/>
        public void GetFlagIdsAsync(string login, string password, long projectId, object userState) {
            if ((this.GetFlagIdsOperationCompleted == null)) {
                this.GetFlagIdsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFlagIdsOperationCompleted);
            }
            this.InvokeAsync("GetFlagIds", new object[] {
                        login,
                        password,
                        projectId}, this.GetFlagIdsOperationCompleted, userState);
        }
        
        private void OnGetFlagIdsOperationCompleted(object arg) {
            if ((this.GetFlagIdsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFlagIdsCompleted(this, new GetFlagIdsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CanAccessToEvaliant", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool CanAccessToEvaliant(string login, string password) {
            object[] results = this.Invoke("CanAccessToEvaliant", new object[] {
                        login,
                        password});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void CanAccessToEvaliantAsync(string login, string password) {
            this.CanAccessToEvaliantAsync(login, password, null);
        }
        
        /// <remarks/>
        public void CanAccessToEvaliantAsync(string login, string password, object userState) {
            if ((this.CanAccessToEvaliantOperationCompleted == null)) {
                this.CanAccessToEvaliantOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCanAccessToEvaliantOperationCompleted);
            }
            this.InvokeAsync("CanAccessToEvaliant", new object[] {
                        login,
                        password}, this.CanAccessToEvaliantOperationCompleted, userState);
        }
        
        private void OnCanAccessToEvaliantOperationCompleted(object arg) {
            if ((this.CanAccessToEvaliantCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CanAccessToEvaliantCompleted(this, new CanAccessToEvaliantCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetEvaliantModuleIds", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long[] GetEvaliantModuleIds(string login, string password) {
            object[] results = this.Invoke("GetEvaliantModuleIds", new object[] {
                        login,
                        password});
            return ((long[])(results[0]));
        }
        
        /// <remarks/>
        public void GetEvaliantModuleIdsAsync(string login, string password) {
            this.GetEvaliantModuleIdsAsync(login, password, null);
        }
        
        /// <remarks/>
        public void GetEvaliantModuleIdsAsync(string login, string password, object userState) {
            if ((this.GetEvaliantModuleIdsOperationCompleted == null)) {
                this.GetEvaliantModuleIdsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetEvaliantModuleIdsOperationCompleted);
            }
            this.InvokeAsync("GetEvaliantModuleIds", new object[] {
                        login,
                        password}, this.GetEvaliantModuleIdsOperationCompleted, userState);
        }
        
        private void OnGetEvaliantModuleIdsOperationCompleted(object arg) {
            if ((this.GetEvaliantModuleIdsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetEvaliantModuleIdsCompleted(this, new GetEvaliantModuleIdsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetEvaliantLoginId", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long GetEvaliantLoginId(string login, string password) {
            object[] results = this.Invoke("GetEvaliantLoginId", new object[] {
                        login,
                        password});
            return ((long)(results[0]));
        }
        
        /// <remarks/>
        public void GetEvaliantLoginIdAsync(string login, string password) {
            this.GetEvaliantLoginIdAsync(login, password, null);
        }
        
        /// <remarks/>
        public void GetEvaliantLoginIdAsync(string login, string password, object userState) {
            if ((this.GetEvaliantLoginIdOperationCompleted == null)) {
                this.GetEvaliantLoginIdOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetEvaliantLoginIdOperationCompleted);
            }
            this.InvokeAsync("GetEvaliantLoginId", new object[] {
                        login,
                        password}, this.GetEvaliantLoginIdOperationCompleted, userState);
        }
        
        private void OnGetEvaliantLoginIdOperationCompleted(object arg) {
            if ((this.GetEvaliantLoginIdCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetEvaliantLoginIdCompleted(this, new GetEvaliantLoginIdCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CanAccessToBastet", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool CanAccessToBastet(string login, string password) {
            object[] results = this.Invoke("CanAccessToBastet", new object[] {
                        login,
                        password});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void CanAccessToBastetAsync(string login, string password) {
            this.CanAccessToBastetAsync(login, password, null);
        }
        
        /// <remarks/>
        public void CanAccessToBastetAsync(string login, string password, object userState) {
            if ((this.CanAccessToBastetOperationCompleted == null)) {
                this.CanAccessToBastetOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCanAccessToBastetOperationCompleted);
            }
            this.InvokeAsync("CanAccessToBastet", new object[] {
                        login,
                        password}, this.CanAccessToBastetOperationCompleted, userState);
        }
        
        private void OnCanAccessToBastetOperationCompleted(object arg) {
            if ((this.CanAccessToBastetCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CanAccessToBastetCompleted(this, new CanAccessToBastetCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void IsAccessibleCompletedEventHandler(object sender, IsAccessibleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IsAccessibleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal IsAccessibleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void CanAccessToProjectCompletedEventHandler(object sender, CanAccessToProjectCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CanAccessToProjectCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CanAccessToProjectCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetModuleIdsCompletedEventHandler(object sender, GetModuleIdsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetModuleIdsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetModuleIdsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetFlagIdsCompletedEventHandler(object sender, GetFlagIdsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFlagIdsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetFlagIdsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void CanAccessToEvaliantCompletedEventHandler(object sender, CanAccessToEvaliantCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CanAccessToEvaliantCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CanAccessToEvaliantCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetEvaliantModuleIdsCompletedEventHandler(object sender, GetEvaliantModuleIdsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetEvaliantModuleIdsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetEvaliantModuleIdsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetEvaliantLoginIdCompletedEventHandler(object sender, GetEvaliantLoginIdCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetEvaliantLoginIdCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetEvaliantLoginIdCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void CanAccessToBastetCompletedEventHandler(object sender, CanAccessToBastetCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CanAccessToBastetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CanAccessToBastetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591