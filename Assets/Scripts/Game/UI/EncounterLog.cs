using System.Collections.Generic;
using Game.Encounters;
using UnityEngine;

namespace Game.UI
{
    public class EncounterLog : MonoBehaviour
    {
        public Transform encounterLogContent;
        [Space] public EncounterUserDialog userDialogPrefab;
        public EncounterEnemyDialog enemyDialogPrefab;
        public EncounterHint hintPrefab;
        
        [HideInInspector] public List<GameObject> encounterLog = new ();
        
        public void ProcessHint(Hint hint, CardSuit lastUserSuit)
        {
            // Show Hint
            EncounterHint encounterHint = Instantiate(hintPrefab, encounterLogContent);
            encounterHint.SetHint(hint);
            encounterLog.Add(encounterHint.gameObject);
            
            // Show Enemy Dialog
            if (hint.hintEnemyName.Length == 0 ||
                hint.hintDialogue.Length == 0) return;
            
            EncounterEnemyDialog encounterEnemyDialog = Instantiate(enemyDialogPrefab, encounterLogContent);
            encounterEnemyDialog.SetHint(hint);
            encounterLog.Add(encounterEnemyDialog.gameObject);
            
            // return; // TODO fix this tomorrow
            
            // Show User Dialog
            EncounterUserDialog encounterUserDialog = Instantiate(userDialogPrefab, encounterLogContent);
            string userDialogue = UserDialogGen.GenerateDialog(lastUserSuit, hint.context);
            encounterUserDialog.SetValue("Your Response:", userDialogue, lastUserSuit);
            encounterLog.Add(encounterUserDialog.gameObject);
        }
        
        public void ClearLog()
        {
            foreach (var log in encounterLog)
            {
                Destroy(log);
            }
            encounterLog.Clear();
        }
    }
}