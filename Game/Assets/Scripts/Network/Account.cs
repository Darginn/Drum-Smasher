using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrumSmasher.Network
{
    public class Account : AccountData
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public short Permissions { get; set; }

        public Account(long id, string accountName, short permissions)
        {
            Id = id;
            AccountName = accountName;
            Permissions = permissions;
        }
    }
}
