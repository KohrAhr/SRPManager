using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPManagerV2.Core
{
    public static class AppConsts
    {
        /// <summary>
        ///     Название Memory Manager File
        /// </summary>
        public const string MEMORY_MANAGED_FILE = "SRPManagerV2_MEMORY_MANAGED_FILE";
//        public const string MEMORY_MANAGED_FILE = "Session\\1\\SRPManagerV2_MEMORY_MANAGED_FILE";



        /// <summary>
        ///     Длина блока в байтах
        /// </summary>
        public const int MEMORY_MANAGED_FILE_LENGTH = 8;

//        public const string MUTEX_ID = "Global\\{B2A3B94A-26BF-4CC6-AC2B-7DDA34EFD413}";



        public const string SERVICE_SRPV2 = "AppIDSvc";

        /// <summary>
        ///     Main registry node for monitoring (SRP)
        /// </summary>
        public const string KEY_SRP_NODE = "SOFTWARE\\Policies\\Microsoft\\Windows\\safer\\codeidentifiers\\";

        /// <summary>
        ///     AppLocker or SRPv2
        /// </summary>
        public const string KEY_SRPv2_NODE = "SOFTWARE\\Policies\\Microsoft\\Windows\\SrpV2\\";

        /// <summary>
        ///     Main registry key for monitoring (SRP)
        /// </summary>
        public const string KEY_SRP_DEFAULT_LEVEL = "DefaultLevel";

        /// <summary>
        ///     
        /// </summary>
        public const string KEY_SRP_POLICY_SCOPE = "PolicyScope";

        /// <summary>
        ///     
        /// </summary>
        public const string KEY_SRP_POLICY_SCORE_ON = "0";

        /// <summary>
        ///     
        /// </summary>
        public const string KEY_SRP_AUTHENTI_CODE_ENABLED = "AuthentiCodeEnabled";

        /// <summary>
        ///     
        /// </summary>
        public const string KEY_SRP_AUTHENTI_CODE_ENABLED_ON = "1";

        /// <summary>
        ///     
        /// </summary>
        public const string KEY_SRP_TRANSPARENT_ENABLED = "TransparentEnabled";

        /// <summary>
        ///     
        /// </summary>
        public const string KEY_SRP_TRANSPARENT_ENABLED_OFF = "1";

        /// <summary>
        ///     Represented as Decimal value. Stored as DWORD. Indicate that SRPv2 is running in White List
        /// </summary>
        public const string SRP_ON = "262144";

        /// <summary>
        ///     Represented as Decimal value. Stored as DWORD. Indicate that SRPv2 is running in Basic User mode
        ///     NOTE: basic user является фикцией, не работает никак (C) WindowsNT
        /// </summary>
        public const string SRP_BASIC = "131072";

        /// <summary>
        ///     Represented as Decimal value. Stored as DWORD. Indicate that SRPv2 is running in White List
        /// </summary>
        public const string SRP_OFF = "0";

        /// <summary>
        ///     SRP Log file name
        /// </summary>
        public const string KEY_SRP_LOG = "LogFileName";



        /// <summary>
        ///     GpUpdate command
        /// </summary>
        public const string CMD_GPUPDATE = "gpupdate.exe";

        /// <summary>
        ///     GpUpdate command parameters
        /// </summary>
        public const string CMD_GPUPDATE_PARAMS = "/force";
    }
}
