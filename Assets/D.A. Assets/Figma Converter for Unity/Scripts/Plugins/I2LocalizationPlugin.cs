#if UNITY_EDITOR && JSON_NET_EXISTS && I2LOC_EXISTS
#if TMPRO_EXISTS
using TMPro;
#endif
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using I2.Loc;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Plugins
{
    static class I2LocalizationPlugin
    {
        private static string locFilePath = $"{Application.dataPath}/{Constants.PUBLISHER}/{Constants.PRODUCT_NAME}/{Constants.LOCALIZATION_FILE_NAME}";
        public static void InstantiateI2LocalizationSource()
        {
            CreateLocFile();

            I2.Loc.LanguageSource locSource = UnityEngine.Object.FindObjectOfType<I2.Loc.LanguageSource>();

            if (locSource == null)
            {
                GameObject _gameObject = CanvasDrawer.CreateEmptyGameObj();
                _gameObject.name = Constants.I2LOC_GAMEOBJECT_NAME;
                FigmaConverterUnity.Instance.languageSource = _gameObject.AddComponent<I2.Loc.LanguageSource>();
            }
            else
            {
                FigmaConverterUnity.Instance.languageSource = locSource;
            }

            ImportCSV(locFilePath, eSpreadsheetUpdateMode.Merge);
        }
        public static void AddI2Localize(this FObject fobject)
        {
            I2.Loc.Localize i2l = fobject.GameObj.AddComponent<I2.Loc.Localize>();

            string subStr = fobject.Characters;

            if (subStr.Length >= 32)
            {
                subStr = fobject.Characters.Substring(0, 32);
            }

            string newKey = subStr.ReplaceInvalidFileNameChars().ToLower();

            if (TextExistsInFile(locFilePath, newKey) == false)
            {
                string newLine = $"{newKey};;;{fobject.Name}{Environment.NewLine}";
                File.AppendAllText(locFilePath, newLine);
            }

            i2l.Term = newKey;
        }
        private static void ImportCSV(string FileName, eSpreadsheetUpdateMode updateMode)
        {
            FigmaConverterUnity.Instance.languageSource.mSource.Import_CSV(
                string.Empty,
                LocalizationReader.ReadCSVfile(FileName, System.Text.Encoding.UTF8),
                updateMode,
                ';');

            FigmaConverterUnity.Instance.languageSource.mSource.Awake();
        }
        private static void CreateLocFile()
        {
            if (File.Exists(locFilePath) == false)
            {
                FileStream oFileStream = new FileStream(locFilePath, System.IO.FileMode.Create);
                oFileStream.Close();

                CheckLocHeader();
            }
            else
            {
                CheckLocHeader();
            }
        }

        private static void CheckLocHeader()
        {
            string fileHeader = "Key;Type;Desc;English";

            if (TextExistsInFile(locFilePath, fileHeader) == false)
            {
                string currentContent = File.ReadAllText(locFilePath);
                File.WriteAllText(locFilePath, $"{fileHeader}\n{currentContent}");
            }
        }
        private static bool TextExistsInFile(string filePath, string text)
        {
            string[] lines = File.ReadAllLines(filePath);
            bool contains = false;
            Parallel.ForEach(lines, (line) =>
            {
                if (line.Contains(text))
                {
                    contains = true;
                }
            });

            return contains;
        }
    }
}
#endif
