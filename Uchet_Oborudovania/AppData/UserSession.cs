using System;

namespace Uchet_Oborudovania.AppData
{
    public static class UserSession
    {
        public static int CurrentUserID { get; set; }
        public static string CurrentUserFIO { get; set; }
        public static string CurrentUserLogin { get; set; }
        public static int CurrentUserRoleID { get; set; }
        public static string CurrentUserRoleName { get; set; }

        public static bool IsAuthenticated => CurrentUserID > 0;

        // Проверка роли
        public static bool HasRole(params string[] roleNames)
        {
            if (!IsAuthenticated) return false;
            foreach (var role in roleNames)
            {
                if (CurrentUserRoleName == role)
                    return true;
            }
            return false;
        }

        // Проверка, является ли пользователь Администратором
        public static bool IsAdmin => CurrentUserRoleName == "Администратор";

        // Проверка, является ли пользователь Директором
        public static bool IsDirector => CurrentUserRoleName == "Директор";

        // Проверка, является ли пользователь Техником
        public static bool IsTechnician => CurrentUserRoleName == "Техник";

        // Проверка, является ли пользователь Сотрудником
        public static bool IsEmployee => CurrentUserRoleName == "Сотрудник";

        public static void Clear()
        {
            CurrentUserID = 0;
            CurrentUserFIO = string.Empty;
            CurrentUserLogin = string.Empty;
            CurrentUserRoleID = 0;
            CurrentUserRoleName = string.Empty;
        }
    }
}