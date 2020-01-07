using Lib.Strings;
using Lib.System;
using Microsoft.Win32;
using SRPManagerV2.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SRPManagerV2.Core
{
    /// <summary>
    ///     Proceed SRP rules
    /// </summary>
    public class SrpRulesCore
    {
        private RegistryFunctions registryFunctions = new RegistryFunctions();

        /// <summary>
        ///     Main function. Load all SRP rules from registry.
        /// </summary>
        public void ProceedSrp(ObservableCollection<AwlRuleType> rules)
        {
            const string CONST_PC = "PC";

            RegistryKey registryKey = Registry.LocalMachine;

            using (RegistryKey key = registryKey.OpenSubKey(AppConsts.KEY_SRP_NODE + AppConsts.SRP_OFF))
            {
                if (key != null)
                {
                    ProceedSrpValues(key, rules, StringsFunctions.ResourceString("resDisallowed"), CONST_PC);
                }
            }

            using (RegistryKey key = registryKey.OpenSubKey(AppConsts.KEY_SRP_NODE + AppConsts.SRP_ON))
            {
                if (key != null)
                {
                    ProceedSrpValues(key, rules, StringsFunctions.ResourceString("resUnrestricted"), CONST_PC);
                }
            }

            using (RegistryKey key = registryKey.OpenSubKey(AppConsts.KEY_SRP_NODE + AppConsts.SRP_BASIC))
            {
                if (key != null)
                {
                    ProceedSrpValues(key, rules, StringsFunctions.ResourceString("resBasic"), CONST_PC);
                }
            }

            //------

            //registryKey = Registry.CurrentUser;

            //using (RegistryKey key = registryKey.OpenSubKey(AppConsts.KEY_SRP_NODE + AppConsts.SRP_OFF))
            //{
            //    if (key != null)
            //    {
            //        ProceedSrpValues(key, rules, StringsFunctions.ResourceString("resDisallowed"), "User");
            //    }
            //}

            //using (RegistryKey key = registryKey.OpenSubKey(AppConsts.KEY_SRP_NODE + AppConsts.SRP_ON))
            //{
            //    if (key != null)
            //    {
            //        ProceedSrpValues(key, rules, StringsFunctions.ResourceString("resUnrestricted"), "User");
            //    }
            //}

            //using (RegistryKey key = registryKey.OpenSubKey(AppConsts.KEY_SRP_NODE + AppConsts.SRP_BASIC))
            //{
            //    if (key != null)
            //    {
            //        ProceedSrpValues(key, rules, "Basic", "User");
            //    }
            //}
        }

        private void ProceedSrpValues(RegistryKey key, ObservableCollection<AwlRuleType> rules, string subType, string scope)
        {
            const string CONST_SRP = "SRP";

            string[] items = new string[]
            {
                "Hashes",
                "Paths"
            };

            foreach (string item in items)
            {
                try
                {
                    using (RegistryKey registryKey = key.OpenSubKey(item))
                    {
                        if (registryKey != null)
                        {
                            ProceedSrpSubValues(registryKey, rules, CONST_SRP, item, subType, scope);
                        }
                    }
                }
                catch (NullReferenceException)
                {
                }
                catch (SecurityException)
                {
                }
            }
        }

        private void ProceedSrpSubValues(RegistryKey key, ObservableCollection<AwlRuleType> rules, string awlType, string subType, string subSubType, string scope)
        {
            foreach (string subKey in key.GetSubKeyNames())
            {
                switch (subType)
                {
                    case "Paths":
                        {
                            AwlRuleType item = ProceedSrpPathsValue(key, subKey);
                            if (!String.IsNullOrEmpty(item.Value))
                            {
                                item.AwlType = awlType;
                                item.SubType = "Path";
                                item.SubSubType = subSubType;
                                item.Scope = scope;
                                rules.Add(item);
                            }

                            break;
                        }
                    case "Hashes":
                        {
                            AwlRuleType item = ProceedSrpHashValue(key, subKey);
                            if (!String.IsNullOrEmpty(item.Value))
                            {
                                item.AwlType = awlType;
                                item.SubType = "Hash";
                                item.SubSubType = subSubType;
                                item.Scope = scope;
                                rules.Add(item);
                            }


                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
        }

        /// <summary>
        ///     Load SRP record type "Path" from registryc
        /// </summary>
        /// <param name="key">
        ///     Current Registry Key
        /// </param>
        /// <param name="subKey">
        ///     Requested Registry SubKey
        /// </param>
        /// <returns>
        ///     Data from registry
        /// </returns>
        private AwlRuleType ProceedSrpPathsValue(RegistryKey key, string subKey)
        {
            AwlRuleType result = new Types.AwlRuleType();

            try
            {
                using (RegistryKey registryKey = key.OpenSubKey(subKey))
                {
                    if (registryKey != null)
                    {
                        result.Value = registryFunctions.GetRegKeyValueObject(registryKey, "ItemData", RegistryValueKind.String);

                        if (String.IsNullOrEmpty(result.Value))
                        {
                            result.Value = registryFunctions.GetRegKeyValueObject(registryKey, "ItemData", RegistryValueKind.ExpandString);
                        }

                        result.Value2 = registryFunctions.GetRegKeyValueObject(registryKey, "SaferFlags", RegistryValueKind.DWord);

                        result.Description = registryFunctions.GetRegKeyValueObject(registryKey, "Description", RegistryValueKind.String);

                        DateTime lastModified = new DateTime(registryFunctions.GetRegKeyValueObjectAsInt(registryKey, "LastModified", RegistryValueKind.QWord)).ToLocalTime().AddYears(1600);
                        result.LastModified = lastModified;
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (SecurityException)
            {
            }

            return result;
        }

        /// <summary>
        ///     Load SRP record type "Hash" from registryc
        /// </summary>
        /// <param name="key">
        ///     Current Registry Key
        /// </param>
        /// <param name="subKey">
        ///     Requested Registry SubKey
        /// </param>
        /// <returns>
        ///     Data from registry
        /// </returns>
        private AwlRuleType ProceedSrpHashValue(RegistryKey key, string subKey)
        {
            AwlRuleType result = new AwlRuleType();

            try
            {
                using (RegistryKey registryKey = key.OpenSubKey(subKey))
                {
                    if (registryKey != null)
                    {
                        result.Value = registryFunctions.GetRegKeyValueObject(registryKey, "FriendlyName", RegistryValueKind.String);

                        result.Value2 = registryFunctions.GetRegKeyValueObject(registryKey, "SaferFlags", RegistryValueKind.DWord);

                        result.Description = registryFunctions.GetRegKeyValueObject(registryKey, "Description", RegistryValueKind.String);

                        DateTime lastModified = new DateTime(registryFunctions.GetRegKeyValueObjectAsInt(registryKey, "LastModified", RegistryValueKind.QWord)).ToLocalTime().AddYears(1600);
                        result.LastModified = lastModified;
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (SecurityException)
            {
            }

            return result;
        }
    }
}
