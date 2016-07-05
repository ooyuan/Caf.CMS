﻿using CAF.Infrastructure.Core.Configuration;
using CAF.Infrastructure.Core.Utilities;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Security
{
    public class SecuritySettings : ISettings
    {
		public SecuritySettings()
		{
			EncryptionKey = CommonHelper.GenerateRandomDigitCode(16);
			AdminAreaAllowedIpAddresses = new List<string>();
		}
		
		/// <summary>
        /// Gets or sets a value indicating whether all pages will be forced to use SSL (no matter of a specified [HttpsRequirementAttribute] attribute)
        /// </summary>
        public bool ForceSslForAllPages { get; set; }

        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a list of adminn area allowed IP addresses
        /// </summary>
        public List<string> AdminAreaAllowedIpAddresses { get; set; }

        /// <summary>
        /// Gets or sets a vaule indicating whether to hide admin menu items based on ACL
        /// </summary>
        public bool HideAdminMenuItemsBasedOnPermissions { get; set; }

        public int TimeOut { get; set; }

        public bool EnabledOAuthSSO { get; set; }
    }
}