﻿using System;

namespace Kotocorn.Modules.Utility.Common.Exceptions
{
    public class StreamRolePermissionException : Exception
    {
        public StreamRolePermissionException() : base("Stream role was unable to be applied.")
        {
        }
    }
}
