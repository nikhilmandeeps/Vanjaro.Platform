﻿using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanjaro.Common.ASPNET.WebAPI;
using Vanjaro.Common.Engines.UIEngine;
using Vanjaro.Core;
using Vanjaro.UXManager.Library.Common;

namespace Vanjaro.UXManager.Extensions.Menu.GoogleReCaptcha.Controllers
{
    [ValidateAntiForgeryToken]    
    public class SettingController : UIEngineController
    {
        internal static List<IUIData> GetData(int portalId, UserInfo userInfo)
        {
            Dictionary<string, IUIData> Settings = new Dictionary<string, IUIData>();
            string Host_SiteKey = Managers.SettingManager.GetHostSetting(Core.Services.Captcha.SiteKey, true);
            string Host_SecretKey = Managers.SettingManager.GetHostSetting(Core.Services.Captcha.SecretKey, true);
            bool Host_Enabled = Managers.SettingManager.GetHostSettingAsBoolean(Core.Services.Captcha.Enabled, false);
            string Site_SiteKey = Managers.SettingManager.GetPortalSetting(Core.Services.Captcha.SiteKey, true);
            string Site_SecretKey = Managers.SettingManager.GetPortalSetting(Core.Services.Captcha.SecretKey, true);
            bool Site_Enabled = Managers.SettingManager.GetPortalSettingAsBoolean(Core.Services.Captcha.Enabled);

            Settings.Add("IsSuperUser", new UIData { Name = "IsSuperUser", Options = UserController.Instance.GetCurrentUserInfo().IsSuperUser });
            Settings.Add("ApplyTo", new UIData { Name = "ApplyTo", Options = false });
            Settings.Add("Host_SiteKey", new UIData { Name = "Host_SiteKey", Value = Host_SiteKey });
            Settings.Add("Host_SecretKey", new UIData { Name = "Host_SecretKey", Value = Host_SecretKey });
            Settings.Add("Host_Enabled", new UIData { Name = "Host_Enabled", Options = Host_Enabled });
            Settings.Add("Site_SiteKey", new UIData { Name = "Site_SiteKey", Value = Site_SiteKey });
            Settings.Add("Site_SecretKey", new UIData { Name = "Site_SecretKey", Value = Site_SecretKey });
            Settings.Add("Site_Enabled", new UIData { Name = "Site_Enabled", Options = Site_Enabled });
            return Settings.Values.ToList();
        }

        [AuthorizeAccessRoles(AccessRoles = "admin")]
        [HttpPost]
        public void Save(dynamic Data)
        {
            if (bool.Parse(Data.ApplyTo.ToString()))
            {
                HostController hostController = new HostController();
                hostController.UpdateEncryptedString(Core.Services.Captcha.SiteKey, Data.Host_SiteKey.ToString(), Config.GetDecryptionkey());
                hostController.UpdateEncryptedString(Core.Services.Captcha.SecretKey, Data.Host_SecretKey.ToString(), Config.GetDecryptionkey());
                hostController.Update(Core.Services.Captcha.Enabled, Data.Host_Enabled.ToString(), false);

            }
            else
            {
                PortalController.UpdateEncryptedString(PortalSettings.PortalId, Core.Services.Captcha.SiteKey, Data.Site_SiteKey.ToString(), Config.GetDecryptionkey());
                PortalController.UpdateEncryptedString(PortalSettings.PortalId, Core.Services.Captcha.SecretKey, Data.Site_SecretKey.ToString(), Config.GetDecryptionkey());
                PortalController.UpdatePortalSetting(PortalSettings.PortalId, Core.Services.Captcha.Enabled, Data.Site_Enabled.ToString(), Config.GetDecryptionkey());
            }
        }
        
        public override string AccessRoles()
        {
            return Factories.AppFactory.GetAccessRoles(UserInfo);
        }
    }
}