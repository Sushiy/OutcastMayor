
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace UtilityAI
{
    public class UtilityEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Outcast Mayor/UtilityAI")]

        private static void Open()
        {
            var window = GetWindow<UtilityEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.0f;
            tree.Config.DrawSearchToolbar = true;

            tree.AddAllAssetsAtPath("Actions", "Assets/Data/UtilityAI/Actions", typeof(Action), true, true);
            tree.AddAllAssetsAtPath("Considerations", "Assets/Data/UtilityAI/Considerations", typeof(Consideration), true, true)
                .ForEach(this.AddDragHandles);

            // Add drag handles to items, so they can be easily dragged into the inventory if characters etc...
            tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);

            return tree;
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Action")))
                {
                    ScriptableObjectCreator.ShowDialog<Action>("Assets/Data/UtilityAI/Actions", obj =>
                    {
                        obj.Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj);
                    });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Consideration")))
                {
                    ScriptableObjectCreator.ShowDialog<Consideration>("Assets/Data/UtilityAI/Considerations", obj =>
                    {
                        obj.Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

}
