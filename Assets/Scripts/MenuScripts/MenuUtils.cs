using ProjectEnums;
using UnityEngine;
using MenuScripts.MenuManager;

namespace MenuScripts
{
    
    public static class MenuUtils
    {

        private static MenuPanelBehaviour menuPnel = null;
        
        private static MenuPanelBehaviour GetMenuPanel()
        {
            if (menuPnel == null)
            {
                menuPnel = GameObject.FindGameObjectWithTag("MenuPanelTag").GetComponent<MenuPanelBehaviour>();
            }
            return menuPnel;
        }
        
        public static void CloseMenu(bool toOverride=false)
        {
            if (!GetMenuPanel().isClosed() || toOverride)
                GetMenuPanel().toggleMenu(false);
        }

        public static void OpenMenu(bool  toOverride=false)
        {
            if (GetMenuPanel().isClosed() || toOverride)
                GetMenuPanel().toggleMenu(true);
        }

        public static void SetPage(MenuEnum selectedMenu)
        {
            if (GetMenuPanel().isClosed())
                OpenMenu();
            GetMenuPanel().setMenuWindow(selectedMenu);
        }

        public static bool IsOpen()
        {
            return !menuPnel.isClosed();
        }
    }
}