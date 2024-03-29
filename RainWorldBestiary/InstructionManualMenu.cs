#if DEBUG

using Menu;
using System.Collections.Generic;

namespace RainWorldBestiary
{
    internal class InstructionManualMenu : Dialog
    {
        private List<TabLayout> Tabs = new List<TabLayout>();
        private int currentTabIndex = 0;

        public InstructionManualMenu(ProcessManager manager) : base(manager)
        {
            
        }
    }

    internal class TabLayout
    {
        public List<MenuObject> MenuObjects = new List<MenuObject>();
        public List<FNode> FNodes = new List<FNode>();

        public void Add(params MenuObject[] menuObjects) => MenuObjects.AddRange(menuObjects);
        public void Add(params FNode[] fNodes) => FNodes.AddRange(fNodes);
    }
}

#endif