﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EndpointChecker.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HasSavedFormWindowSizeAndPosition {
            get {
                return ((bool)(this["HasSavedFormWindowSizeAndPosition"]));
            }
            set {
                this["HasSavedFormWindowSizeAndPosition"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0, 0")]
        public global::System.Drawing.Point FormWindow_Location {
            get {
                return ((global::System.Drawing.Point)(this["FormWindow_Location"]));
            }
            set {
                this["FormWindow_Location"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0, 0")]
        public global::System.Drawing.Size FormWindow_Size {
            get {
                return ((global::System.Drawing.Size)(this["FormWindow_Size"]));
            }
            set {
                this["FormWindow_Size"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Normal")]
        public global::System.Windows.Forms.FormWindowState FormWindow_WindowState {
            get {
                return ((global::System.Windows.Forms.FormWindowState)(this["FormWindow_WindowState"]));
            }
            set {
                this["FormWindow_WindowState"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HasSavedListViewColumnsWidthAndOrder {
            get {
                return ((bool)(this["HasSavedListViewColumnsWidthAndOrder"]));
            }
            set {
                this["HasSavedListViewColumnsWidthAndOrder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DisabledItemsList {
            get {
                return ((string)(this["DisabledItemsList"]));
            }
            set {
                this["DisabledItemsList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_EnableAutomaticRefresh {
            get {
                return ((bool)(this["Config_EnableAutomaticRefresh"]));
            }
            set {
                this["Config_EnableAutomaticRefresh"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_EnableContinuousRefresh {
            get {
                return ((bool)(this["Config_EnableContinuousRefresh"]));
            }
            set {
                this["Config_EnableContinuousRefresh"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_AutoAdjustRefreshInterval {
            get {
                return ((bool)(this["Config_AutoAdjustRefreshInterval"]));
            }
            set {
                this["Config_AutoAdjustRefreshInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal Config_AutomaticRefreshIntervalSeconds {
            get {
                return ((decimal)(this["Config_AutomaticRefreshIntervalSeconds"]));
            }
            set {
                this["Config_AutomaticRefreshIntervalSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal Config_HTTP_RequestTimeoutSeconds {
            get {
                return ((decimal)(this["Config_HTTP_RequestTimeoutSeconds"]));
            }
            set {
                this["Config_HTTP_RequestTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal Config_FTP_RequestTimeoutSeconds {
            get {
                return ((decimal)(this["Config_FTP_RequestTimeoutSeconds"]));
            }
            set {
                this["Config_FTP_RequestTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_EnableTrayNotificationsOnError {
            get {
                return ((bool)(this["Config_EnableTrayNotificationsOnError"]));
            }
            set {
                this["Config_EnableTrayNotificationsOnError"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_AllowAutoRedirect {
            get {
                return ((bool)(this["Config_AllowAutoRedirect"]));
            }
            set {
                this["Config_AllowAutoRedirect"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ValidateSSLCertificate {
            get {
                return ((bool)(this["Config_ValidateSSLCertificate"]));
            }
            set {
                this["Config_ValidateSSLCertificate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Config_EndpointsStatusExportDirectory {
            get {
                return ((string)(this["Config_EndpointsStatusExportDirectory"]));
            }
            set {
                this["Config_EndpointsStatusExportDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal Config_ParallelThreadsCount {
            get {
                return ((decimal)(this["Config_ParallelThreadsCount"]));
            }
            set {
                this["Config_ParallelThreadsCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ResolveNetworkShares {
            get {
                return ((bool)(this["Config_ResolveNetworkShares"]));
            }
            set {
                this["Config_ResolveNetworkShares"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ExportEndpointsStatus_XLSX {
            get {
                return ((bool)(this["Config_ExportEndpointsStatus_XLSX"]));
            }
            set {
                this["Config_ExportEndpointsStatus_XLSX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HasSavedConfiguration {
            get {
                return ((bool)(this["HasSavedConfiguration"]));
            }
            set {
                this["HasSavedConfiguration"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Service {
            get {
                return ((int)(this["ListView_ColWidth_Service"]));
            }
            set {
                this["ListView_ColWidth_Service"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Protocol {
            get {
                return ((int)(this["ListView_ColWidth_Protocol"]));
            }
            set {
                this["ListView_ColWidth_Protocol"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Port {
            get {
                return ((int)(this["ListView_ColWidth_Port"]));
            }
            set {
                this["ListView_ColWidth_Port"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Endpoint {
            get {
                return ((int)(this["ListView_ColWidth_Endpoint"]));
            }
            set {
                this["ListView_ColWidth_Endpoint"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_IPAddress {
            get {
                return ((int)(this["ListView_ColWidth_IPAddress"]));
            }
            set {
                this["ListView_ColWidth_IPAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_ResponseTime {
            get {
                return ((int)(this["ListView_ColWidth_ResponseTime"]));
            }
            set {
                this["ListView_ColWidth_ResponseTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Code {
            get {
                return ((int)(this["ListView_ColWidth_Code"]));
            }
            set {
                this["ListView_ColWidth_Code"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Message {
            get {
                return ((int)(this["ListView_ColWidth_Message"]));
            }
            set {
                this["ListView_ColWidth_Message"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_PingTime {
            get {
                return ((int)(this["ListView_ColWidth_PingTime"]));
            }
            set {
                this["ListView_ColWidth_PingTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Server {
            get {
                return ((int)(this["ListView_ColWidth_Server"]));
            }
            set {
                this["ListView_ColWidth_Server"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_UserName {
            get {
                return ((int)(this["ListView_ColWidth_UserName"]));
            }
            set {
                this["ListView_ColWidth_UserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_NetworkShares {
            get {
                return ((int)(this["ListView_ColWidth_NetworkShares"]));
            }
            set {
                this["ListView_ColWidth_NetworkShares"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_DNSName {
            get {
                return ((int)(this["ListView_ColWidth_DNSName"]));
            }
            set {
                this["ListView_ColWidth_DNSName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Service {
            get {
                return ((int)(this["ListView_DisplayIndex_Service"]));
            }
            set {
                this["ListView_DisplayIndex_Service"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Protocol {
            get {
                return ((int)(this["ListView_DisplayIndex_Protocol"]));
            }
            set {
                this["ListView_DisplayIndex_Protocol"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Port {
            get {
                return ((int)(this["ListView_DisplayIndex_Port"]));
            }
            set {
                this["ListView_DisplayIndex_Port"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Endpoint {
            get {
                return ((int)(this["ListView_DisplayIndex_Endpoint"]));
            }
            set {
                this["ListView_DisplayIndex_Endpoint"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_IPAddress {
            get {
                return ((int)(this["ListView_DisplayIndex_IPAddress"]));
            }
            set {
                this["ListView_DisplayIndex_IPAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_ResponseTime {
            get {
                return ((int)(this["ListView_DisplayIndex_ResponseTime"]));
            }
            set {
                this["ListView_DisplayIndex_ResponseTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Code {
            get {
                return ((int)(this["ListView_DisplayIndex_Code"]));
            }
            set {
                this["ListView_DisplayIndex_Code"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Message {
            get {
                return ((int)(this["ListView_DisplayIndex_Message"]));
            }
            set {
                this["ListView_DisplayIndex_Message"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_PingTime {
            get {
                return ((int)(this["ListView_DisplayIndex_PingTime"]));
            }
            set {
                this["ListView_DisplayIndex_PingTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Server {
            get {
                return ((int)(this["ListView_DisplayIndex_Server"]));
            }
            set {
                this["ListView_DisplayIndex_Server"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_UserName {
            get {
                return ((int)(this["ListView_DisplayIndex_UserName"]));
            }
            set {
                this["ListView_DisplayIndex_UserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_NetworkShares {
            get {
                return ((int)(this["ListView_DisplayIndex_NetworkShares"]));
            }
            set {
                this["ListView_DisplayIndex_NetworkShares"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_DNSName {
            get {
                return ((int)(this["ListView_DisplayIndex_DNSName"]));
            }
            set {
                this["ListView_DisplayIndex_DNSName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_SaveResponse {
            get {
                return ((bool)(this["Config_SaveResponse"]));
            }
            set {
                this["Config_SaveResponse"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ResolvePageMetaInfo {
            get {
                return ((bool)(this["Config_ResolvePageMetaInfo"]));
            }
            set {
                this["Config_ResolvePageMetaInfo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UpgradeRequired {
            get {
                return ((bool)(this["UpgradeRequired"]));
            }
            set {
                this["UpgradeRequired"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int Config_MaximumEndpointReferencesCount {
            get {
                return ((int)(this["Config_MaximumEndpointReferencesCount"]));
            }
            set {
                this["Config_MaximumEndpointReferencesCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool FormWindow_Hidden {
            get {
                return ((bool)(this["FormWindow_Hidden"]));
            }
            set {
                this["FormWindow_Hidden"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_ContentType {
            get {
                return ((int)(this["ListView_ColWidth_ContentType"]));
            }
            set {
                this["ListView_ColWidth_ContentType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_ContentType {
            get {
                return ((int)(this["ListView_DisplayIndex_ContentType"]));
            }
            set {
                this["ListView_DisplayIndex_ContentType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal Config_PingTimeoutSeconds {
            get {
                return ((decimal)(this["Config_PingTimeoutSeconds"]));
            }
            set {
                this["Config_PingTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int Config_ValidationMethod {
            get {
                return ((int)(this["Config_ValidationMethod"]));
            }
            set {
                this["Config_ValidationMethod"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_MACAddress {
            get {
                return ((int)(this["ListView_ColWidth_MACAddress"]));
            }
            set {
                this["ListView_ColWidth_MACAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_MACAddress {
            get {
                return ((int)(this["ListView_DisplayIndex_MACAddress"]));
            }
            set {
                this["ListView_DisplayIndex_MACAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_ContentLenght {
            get {
                return ((int)(this["ListView_DisplayIndex_ContentLenght"]));
            }
            set {
                this["ListView_DisplayIndex_ContentLenght"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_ContentLenght {
            get {
                return ((int)(this["ListView_ColWidth_ContentLenght"]));
            }
            set {
                this["ListView_ColWidth_ContentLenght"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ExportEndpointsStatus_HTML {
            get {
                return ((bool)(this["Config_ExportEndpointsStatus_HTML"]));
            }
            set {
                this["Config_ExportEndpointsStatus_HTML"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ExportEndpointsStatus_JSON {
            get {
                return ((bool)(this["Config_ExportEndpointsStatus_JSON"]));
            }
            set {
                this["Config_ExportEndpointsStatus_JSON"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ExportEndpointsStatus_XML {
            get {
                return ((bool)(this["Config_ExportEndpointsStatus_XML"]));
            }
            set {
                this["Config_ExportEndpointsStatus_XML"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_LastSeenOnline {
            get {
                return ((int)(this["ListView_ColWidth_LastSeenOnline"]));
            }
            set {
                this["ListView_ColWidth_LastSeenOnline"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_LastSeenOnline {
            get {
                return ((int)(this["ListView_DisplayIndex_LastSeenOnline"]));
            }
            set {
                this["ListView_DisplayIndex_LastSeenOnline"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_Expires {
            get {
                return ((int)(this["ListView_DisplayIndex_Expires"]));
            }
            set {
                this["ListView_DisplayIndex_Expires"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_Expires {
            get {
                return ((int)(this["ListView_ColWidth_Expires"]));
            }
            set {
                this["ListView_ColWidth_Expires"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_RemoveURLParameters {
            get {
                return ((bool)(this["Config_RemoveURLParameters"]));
            }
            set {
                this["Config_RemoveURLParameters"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Config_ResolvePageLinks {
            get {
                return ((bool)(this["Config_ResolvePageLinks"]));
            }
            set {
                this["Config_ResolvePageLinks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_DisplayIndex_ETag {
            get {
                return ((int)(this["ListView_DisplayIndex_ETag"]));
            }
            set {
                this["ListView_DisplayIndex_ETag"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int ListView_ColWidth_ETag {
            get {
                return ((int)(this["ListView_ColWidth_ETag"]));
            }
            set {
                this["ListView_ColWidth_ETag"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Config_Executable_VNCViewer {
            get {
                return ((string)(this["Config_Executable_VNCViewer"]));
            }
            set {
                this["Config_Executable_VNCViewer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Config_Executable_Putty {
            get {
                return ((string)(this["Config_Executable_Putty"]));
            }
            set {
                this["Config_Executable_Putty"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/98.100.4758.66 Safari/537.36")]
        public string Config_HTTP_UserAgent {
            get {
                return ((string)(this["Config_HTTP_UserAgent"]));
            }
            set {
                this["Config_HTTP_UserAgent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5242880")]
        public int Config_HTTP_SaveResponse_MaxLenght_Bytes {
            get {
                return ((int)(this["Config_HTTP_SaveResponse_MaxLenght_Bytes"]));
            }
            set {
                this["Config_HTTP_SaveResponse_MaxLenght_Bytes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AIzaSyBerCYP2d7XxMWy9DMHlS09BZKFh5NKCas")]
        public string GoogleMaps_API_Key {
            get {
                return ((string)(this["GoogleMaps_API_Key"]));
            }
            set {
                this["GoogleMaps_API_Key"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14")]
        public int GoogleMaps_API_ZoomFactor {
            get {
                return ((int)(this["GoogleMaps_API_ZoomFactor"]));
            }
            set {
                this["GoogleMaps_API_ZoomFactor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string VirusTotal_API_Key {
            get {
                return ((string)(this["VirusTotal_API_Key"]));
            }
            set {
                this["VirusTotal_API_Key"] = value;
            }
        }
    }
}
