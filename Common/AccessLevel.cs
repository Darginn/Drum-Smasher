using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon
{
    [Flags]
    public enum AccessLevel
    {
        User = 1,
        VIP = 2,

        //Chat Permissions
        MuteUser = 4,
        KickUser = 8,
        BanUser = 16,

        Moderator = User | MuteUser | KickUser | BanUser,
        Admin = Moderator,
        Dev = Admin | VIP,
    }
}
