﻿using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Vanjaro.Common.ASPNET.WebAPI;
using Vanjaro.Common.Engines.UIEngine;
using Vanjaro.UXManager.Library.Common;

namespace Vanjaro.UXManager.Extensions.Menu.EmailServiceProvider.Controllers
{
    [ValidateAntiForgeryToken]
    [AuthorizeAccessRoles(AccessRoles = "admin")]
    public class SettingController : UIEngineController
    {
        internal static List<IUIData> GetData(int PortalId)
        {
            bool IsSuperUser = UserController.Instance.GetCurrentUserInfo().IsSuperUser;
            string mode = IsSuperUser ? "h" : "p";
            Dictionary<string, IUIData> Settings = new Dictionary<string, IUIData>
            {
                { "IsSuperUser", new UIData { Name = "IsSuperUser", Options = IsSuperUser } },
                { "SMTPmode", new UIData { Name = "SMTPmode", Options = IsSuperUser? PortalController.GetPortalSetting("SMTPmode", PortalId, mode) == mode:false } },
                { "Host_Server", new UIData { Name = "Host_Server", Value = HostController.Instance.GetString("SMTPServer") } },
                { "Host_Username", new UIData { Name = "Host_Username", Value = HostController.Instance.GetString("SMTPUsername") } },
                { "Host_Password", new UIData { Name = "Host_Password", Value = HostController.Instance.GetEncryptedString("SMTPPassword", Config.GetDecryptionkey()) } },
                { "Host_EnableSSL", new UIData { Name = "Host_EnableSSL", Options = HostController.Instance.GetBoolean("SMTPEnableSSL", false) } },
                { "Portal_Server", new UIData { Name = "Portal_Server", Value = PortalController.GetPortalSetting("SMTPServer", PortalId, string.Empty) } },
                { "Portal_Username", new UIData { Name = "Portal_Username", Value = PortalController.GetPortalSetting("SMTPUsername", PortalId, string.Empty) } },
                { "Portal_Password", new UIData { Name = "Portal_Password", Value = PortalController.GetEncryptedString("SMTPPassword", PortalId, Config.GetDecryptionkey()) } },
                { "Portal_EnableSSL", new UIData { Name = "Portal_EnableSSL", Options = PortalController.GetPortalSetting("SMTPEnableSSL", PortalId, string.Empty) == "Y" } }
            };
            return Settings.Values.ToList();
        }

        [HttpPost]
        public ActionResult Update(dynamic Data)
        {
            ActionResult actionResult = new ActionResult();
            try
            {
                
                if (bool.Parse(Data.SMTPmode.ToString()))
                {
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SMTPmode", "h");
                    HostController.Instance.Update("SMTPServer", Data.Host_Server.ToString(), false);
                    HostController.Instance.Update("SMTPAuthentication", "1", false);
                    HostController.Instance.Update("SMTPUsername", Data.Host_Username.ToString(), false);
                    HostController.Instance.UpdateEncryptedString("SMTPPassword", Data.Host_Password.ToString(), Config.GetDecryptionkey());
                    HostController.Instance.Update("SMTPEnableSSL", bool.Parse(Data.Host_EnableSSL.ToString()) ? "Y" : "N", false);
                }
                else
                {
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SMTPmode", "p");
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SMTPAuthentication", "1");
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SMTPServer", Data.Portal_Server.ToString());
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SMTPUsername", Data.Portal_Username.ToString());
                    PortalController.UpdateEncryptedString(PortalSettings.PortalId, "SMTPPassword", Data.Portal_Password.ToString(), Config.GetDecryptionkey());
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SMTPEnableSSL", bool.Parse(Data.Portal_EnableSSL.ToString()) ? "Y" : "N");
                }

                DataCache.ClearCache();
                actionResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                actionResult.AddError("HttpStatusCode.InternalServerError", ex.Message);
            }
            return actionResult;
        }

        [HttpPost]
        public ActionResult SendTestEmail(dynamic Data)
        {
            string LocalResourcesFile = "~/DesktopModules/Vanjaro/UXManager/Extensions/Menu/" + ExtensionInfo.Name + "/Views/Setting/App_LocalResources/Setting.resx";
            ActionResult actionResult = new ActionResult();
            try
            {
                dynamic errMessage = Mail.SendMail(PortalSettings.UserInfo.Email,
                    PortalSettings.UserInfo.Email,
                    "",
                    "",
                    MailPriority.Normal,
                    Localization.GetSystemMessage(PortalSettings, "EMAIL_SMTP_TEST_SUBJECT"),
                    MailFormat.Text,
                    Encoding.UTF8,
                    "",
                    "",
                    Data.Server.ToString(),
                    "1",
                    Data.Username.ToString(),
                    Data.Password.ToString(),
                    bool.Parse(Data.EnableSSL.ToString()));
                if (string.IsNullOrEmpty(errMessage))
                {
                    actionResult.IsSuccess = true;
                    actionResult.Data = Localization.GetString("EmailSentMessage", LocalResourcesFile) + PortalSettings.UserInfo.Email + Localization.GetString("to", LocalResourcesFile) + PortalSettings.UserInfo.Email;
                }
                else
                {
                    actionResult.AddError("errMessage", errMessage);
                }
            }
            catch (Exception ex)
            {
                actionResult.AddError("HttpStatusCode.InternalServerError", ex.Message);
            }
            return actionResult;
        }

        public override string AccessRoles()
        {
            return Factories.AppFactory.GetAccessRoles(UserInfo);
        }
    }
}