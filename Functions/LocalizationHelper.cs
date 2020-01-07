using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml;

namespace SRPManagerV2.Functions
{
    /// <summary>
    /// Deals with localization behaviors.
    /// </summary>
    public static class LocalizationHelper
    {
        public static void SelectCulture(string culture)
        {
            const string Resources = "Resources\\StringResources.{0}.xaml";
            const string DefaultCulture = "en-US";

            #region Validation

            if (string.IsNullOrEmpty(culture))
            {
                return;
            }

            if (culture.Equals("auto"))
            {
                var ci = CultureInfo.InstalledUICulture;
                culture = ci.Name;
            }

            #endregion

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

            #region Selected Culture

            //Search for the specified culture.     
            string requestedCulture = String.Format(Resources, culture);
            var requestedResource = dictionaryList.FirstOrDefault(d => d.Source?.OriginalString == requestedCulture);

            #endregion

            #region Generic Branch Fallback

            //Fallback to a more generic version of the language. Example: pt-BR to pt.
            if (requestedResource == null && culture.Length > 2 && !culture.StartsWith("en"))
            {
                culture = culture.Substring(0, 2); //TODO: Support for language code like syr-SY (3 initial letters)
                requestedCulture = String.Format(Resources, culture);
                requestedResource = dictionaryList.FirstOrDefault(d => d.Source?.OriginalString == requestedCulture);
            }

            #endregion

            #region English Fallback

            //If not present, fall back to english.
            if (requestedResource == null)
            {
                requestedCulture = String.Format(Resources, DefaultCulture);
                requestedResource = dictionaryList.FirstOrDefault(d => d.Source?.OriginalString == requestedCulture);
            }

            #endregion

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our current string table.      
            Application.Current.Resources.MergedDictionaries.Remove(requestedResource);
            Application.Current.Resources.MergedDictionaries.Add(requestedResource);

            #region English Fallback of the Current Language

            //Only non-English resources need a fallback, because the English resource is evergreen.
            if (culture.StartsWith("en"))
            {
                return;
            }

            string def = String.Format(Resources, DefaultCulture);
            var englishResource = dictionaryList.FirstOrDefault(d => d.Source?.OriginalString == def);

            if (englishResource != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(englishResource);
                Application.Current.Resources.MergedDictionaries.Insert(Application.Current.Resources.MergedDictionaries.Count - 1, englishResource);
            }

            #endregion

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            GC.Collect(2);
        }
    }
}
