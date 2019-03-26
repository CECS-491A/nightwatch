﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ServiceLayer.Services.ExceptionService;
using ManagerLayer.Login;
using ManagerLayer.AccessControl;
using DataAccessLayer.Models;
using static UnitTesting.TestingUtils;

namespace UnitTesting
{
    /// <summary>
    /// Summary description for LoginManagerUT
    /// </summary>
    [TestClass]
    public class UserLoginManagerUT
    {
        readonly TestingUtils ut;
        readonly UserLoginManager _loginManager;
        readonly AuthorizationManager _authManager;

        public UserLoginManagerUT()
        {
            ut = new TestingUtils();
            _loginManager = new UserLoginManager();
            _authManager = new AuthorizationManager();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEmailException))]
        public void Login_NewUser_InvalidUserName_Failure_ExceptionThrown()
        {
            var invalid_username = Guid.NewGuid() + ".com";
            var valid_ssoID = Guid.NewGuid();
            var timestamp = 8283752242;
            string preSignatureString = ut.GeneratePreSignatureString(valid_ssoID, invalid_username, timestamp);
            string Signature = ut.GenerateTokenSignature(valid_ssoID, invalid_username, 8283752242);

            using (var _db = ut.CreateDataBaseContext())
            {
                _loginManager.LoginFromSSO(_db, invalid_username, valid_ssoID, Signature, preSignatureString);
            }

            //Assert - catch exception
        }

        [TestMethod]
        public void Login_NewUser_ValidUserName_Success()
        {
            var valid_username = Guid.NewGuid() + "@mail.com";
            var valid_ssoID = Guid.NewGuid();
            var timestamp = 8283752242;
            MockLoginPayload mock_payload = new MockLoginPayload
            {
                email = valid_username,
                ssoUserId = valid_ssoID,
                timestamp = timestamp
            };

            using (var _db = ut.CreateDataBaseContext())
            {
                var response = _loginManager.LoginFromSSO(_db, valid_username, valid_ssoID, mock_payload.Signature(), mock_payload.PreSignatureString());
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public void Login_ExistingUser_Success()
        {
            var existing_user = ut.CreateSSOUserInDb();
            var existing_username = existing_user.Username;
            var existing_ssoID = existing_user.Id;
            var timestamp = 12312312;
            var mock_payload = ut.GenerateLoginPayloadWithSignature(existing_ssoID, existing_username, timestamp);

            using (var _db = ut.CreateDataBaseContext())
            {
                var response = _loginManager.LoginFromSSO(_db, existing_username, existing_ssoID, mock_payload.Signature(), mock_payload.PreSignatureString());
                Assert.IsNotNull(response);
            }
        }
    }
}