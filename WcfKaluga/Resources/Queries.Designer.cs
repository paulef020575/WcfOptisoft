﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfKaluga.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Queries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Queries() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WcfKaluga.Resources.Queries", typeof(Queries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebDeleteRollPackInPackQueue @RollPackNum.
        /// </summary>
        public static string DeleteRollPackInPackQueue {
            get {
                return ResourceManager.GetString("DeleteRollPackInPackQueue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebGetNextRollPackNum.
        /// </summary>
        public static string GetNextRollPackNum {
            get {
                return ResourceManager.GetString("GetNextRollPackNum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT SapStatus FROM PMMES.DocRollPack WHERE RollPackNum = @RollPackNum.
        /// </summary>
        public static string GetPackSapStatus {
            get {
                return ResourceManager.GetString("GetPackSapStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebGetRollPackByNum @RollPackNum.
        /// </summary>
        public static string GetRollPackByNum {
            get {
                return ResourceManager.GetString("GetRollPackByNum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebGetRollPackId @RollPackNum.
        /// </summary>
        public static string GetRollPackId {
            get {
                return ResourceManager.GetString("GetRollPackId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebGetRollPackNumByRollNum @RollNum.
        /// </summary>
        public static string GetRollPackNumByRollNum {
            get {
                return ResourceManager.GetString("GetRollPackNumByRollNum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.[WebGetRollPackProperties] @Id.
        /// </summary>
        public static string GetRollPackProperties {
            get {
                return ResourceManager.GetString("GetRollPackProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebGetLabQualityParamsByRollNums @RollNum.
        /// </summary>
        public static string GetRollQuality {
            get {
                return ResourceManager.GetString("GetRollQuality", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EXECUTE PMMES.WebUpdateRollPackSapStatus @RollPackNum, @SapStatus.
        /// </summary>
        public static string UpdateRollPackStatus {
            get {
                return ResourceManager.GetString("UpdateRollPackStatus", resourceCulture);
            }
        }
    }
}
