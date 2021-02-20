using System;
using System.Collections.Generic;
using System.Linq;
using Instant.Server.Data.Entities;
using Instant.Server.Domain.Enums;

namespace Instant.Server.BLL
{
    public static class Chats
    {
        public static bool DoesUserHaveChatPermission(
            ChatPermissionTypes existingPermissionType,
            ChatPermissionTypes verifiablePermissionType)
        {
            bool result;
            
            switch (verifiablePermissionType)
            {
                case ChatPermissionTypes.Read:
                    result = existingPermissionType == ChatPermissionTypes.Read
                             || existingPermissionType == ChatPermissionTypes.ReadWrite
                             || existingPermissionType == ChatPermissionTypes.Moderate
                             || existingPermissionType == ChatPermissionTypes.Administrate;
                    break;
                case ChatPermissionTypes.ReadWrite:
                    result = existingPermissionType == ChatPermissionTypes.ReadWrite
                             || existingPermissionType == ChatPermissionTypes.Moderate
                             || existingPermissionType == ChatPermissionTypes.Administrate;
                    break;
                case ChatPermissionTypes.Moderate:
                    result = existingPermissionType == ChatPermissionTypes.Moderate
                             || existingPermissionType == ChatPermissionTypes.Administrate;
                    break;
                case ChatPermissionTypes.Administrate:
                    result = existingPermissionType == ChatPermissionTypes.Administrate;
                    break;
                default:
                    throw new ArgumentException("Unsupported for verification chat permission type");
            }
            
            return result;
        }
    }
}