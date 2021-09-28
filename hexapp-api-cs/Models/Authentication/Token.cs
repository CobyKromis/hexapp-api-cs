using System;
using System.Collections.Generic;

#nullable disable

namespace hexapp_api_cs.Models.Authentication
{
    public partial class Token
    {
        public int TokenId { get; set; }
        public int? UserId { get; set; }
        public string Type { get; set; }
        public string TokenString { get; set; }
        public DateTime? CreationTimestampUtc { get; set; }
        public bool? UsedFlag { get; set; }
        public bool? ValidFlag { get; set; }

        public Token(int tokenId, int? userId, string type, string tokenString, DateTime? creationTimestampUtc, bool? usedFlag, bool? validFlag)
        {
            TokenId = tokenId;
            UserId = userId;
            Type = type;
            TokenString = tokenString;
            CreationTimestampUtc = creationTimestampUtc;
            UsedFlag = usedFlag;
            ValidFlag = validFlag;
        }
    }
}
