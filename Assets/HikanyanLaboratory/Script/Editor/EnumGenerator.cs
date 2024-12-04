using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HikanyanLaboratory.Audio.Editor
{
    public class EnumGenerator : UnityEditor.Editor
    {
        [MenuItem("HikanyanTools/Generate SoundType Enums")]
        public static void GenerateEnums()
        {
            string directoryPath = "Assets/HikanyanLaboratory/GameData/ScriptableObject/Audio/";
            // ScriptableObjectを取得する
            string[] guids = AssetDatabase.FindAssets("t:AudioClipRegistrar",
                new[] { directoryPath });
            if (guids.Length == 0)
            {
                Debug.LogError("AudioClipRegistrarが見つかりませんでした。");
                return;
            }

            // ScriptableObjectをロード
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var audioClipRegistrar = AssetDatabase.LoadAssetAtPath<AudioClipRegistrar>(path);

            if (audioClipRegistrar == null)
            {
                Debug.LogError("AudioClipRegistrarが見つかりませんでした。");
                return;
            }

            // ディレクトリパス
            string generateDirectoryPath = "Assets/HikanyanLaboratory/Script/Generate/Audio/";
            if (!Directory.Exists(generateDirectoryPath))
            {
                Directory.CreateDirectory(generateDirectoryPath);
            }

            // BGM Enumを生成
            GenerateEnumFile("BGMType", audioClipRegistrar.BGMClips, generateDirectoryPath);

            // SE Enumを生成
            GenerateEnumFile("SEType", audioClipRegistrar.SeClips, generateDirectoryPath);

            // Voice Enumを生成
            GenerateEnumFile("VoiceType", audioClipRegistrar.VoiceClips, generateDirectoryPath);

            AssetDatabase.Refresh();//生成されたファイルがエディタに即座に反映され、Unity内で利用できるようにする
            Debug.Log("Enumファイルが生成されました。");
        }

        /// <summary>
        /// Enumのcsファイルを生成します
        /// </summary>
        /// <param name="enumName"></param>
        /// <param name="clips"></param>
        /// <param name="directoryPath"></param>
        private static void GenerateEnumFile(string enumName, List<AudioClip> clips, string directoryPath)
        {
            string enumFilePath = Path.Combine(directoryPath, enumName + ".cs");

            using (StreamWriter writer = new StreamWriter(enumFilePath))
            {
                string enumContent = GenerateEnumScript(enumName, clips.ToArray());
                writer.Write(enumContent);
            }
            Debug.Log($"{enumName} Enumファイルが生成されました: {enumFilePath}");
        }

        /// <summary>
        /// Enumの中身を生成
        /// </summary>
        /// <param name="enumName"></param>
        /// <param name="clips"></param>
        /// <returns></returns>
        private static string GenerateEnumScript(string enumName, AudioClip[] clips)
        {
            string entries = string.Join(",\n        ", FormatEnumEntries(clips)); // エントリのインデントを調整

            return $@"
namespace HikanyanLaboratory.Audio
{{
    public enum {enumName}
    {{
        {entries}
    }}
}}";
        }

        /// <summary>
        /// 名前のフォーマット調整
        /// </summary>
        /// <param name="clips"></param>
        /// <returns></returns>
        private static string[] FormatEnumEntries(AudioClip[] clips)
        {
            string[] entries = new string[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    entries[i] = FormatEnumEntry(clips[i].name);
                }
            }
            return entries;
        }

        private static string FormatEnumEntry(string clipName)
        {
            // 無効な文字をアンダースコアに置き換え、Enum名として有効な形式に変換
            clipName = clipName.Replace(" ", "_").Replace("-", "_");
            return char.ToUpper(clipName[0]) + clipName.Substring(1);
        }
    }
}