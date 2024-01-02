using UnityEditor;
using UnityEngine;

namespace AE_FSM
{
    public class AE_FSMMenu
    {
        [MenuItem("Assets/Create/AE_FSM/FSMController")]
        private static void CreateFSM()
        {
            RunTimeFSMControllerCreator creator = ScriptableObject.CreateInstance<RunTimeFSMControllerCreator>();

            string name = GetName();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(creator.GetInstanceID(), creator, name, null, null);
        }

        private static string GetName(string tempName = "New FSMContorller", string suffix = "asset")
        {
            int i = 0;
            string name = $"{tempName}_{i}";
            string[] files = AssetDatabase.FindAssets(name);

            for (i += 1; files != null && files.Length > 0; i++)
            {
                name = $"{tempName}_{i}";
                files = AssetDatabase.FindAssets(name);
            }

            return $"{name}.{suffix}";
        }
    }
}