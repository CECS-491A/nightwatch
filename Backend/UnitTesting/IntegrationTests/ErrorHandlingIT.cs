﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApi_PointMap.Controllers;
using DataAccessLayer.Models;
using System.Net.Http;
using System.Net;
using DTO.UserManagementAPI;
using DTO.KFCSSO_API;
using System.Web.Http.Results;
using DTO.PointAPI;
using UnitTesting;

namespace Testing.IntegrationTests
{
    [TestClass]
    public class ErrorHandlingIT
    {
        const string API_ROUTE_LOCAL = "http://localhost:58896";
        PointController _pointController;
        UserManagementController _umController;
        TestingUtils _tu;
        User newUser;

        public ErrorHandlingIT()
        {
            _pointController = new PointController();
            _umController = new UserManagementController();
            _tu = new TestingUtils();
        }

        [TestMethod]
        public void DeleteUser_NoUserId_412()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/users/delete/";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            //passing null parameter creates InvalidModelPayloadException that should be caught
            //  and return a 400
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.DeleteUser((string)null);

            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode);
        }

        [TestMethod]
        public void GetAllUsers_UserIsNotAdministrator_400()
        {
            newUser = _tu.CreateUserObject();
            newUser.IsAdministrator = false;
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/users";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            //user is not adminstrator and therefore cannot return all users.
            //  should result in an UserIsNotAdministratorException
            //  and return a 401
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.GetAllUsers();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual("Non-administrators cannot view all users.", response.Content.ToString());
        }

        [TestMethod]
        public void Delete_InvalidUserId_400()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);
            var badId = "q7h493proannaosnfdo";

            var endpoint = API_ROUTE_LOCAL + "/users/delete/";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            //passing a incorrectly formatted user Id should result in an InvalidGuidException
            //  and return a 400
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.DeleteUser(badId);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void DeleteUser_NoTokenProvided_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();

            _umController.Request = request;

            //no token provided should result in NoTokenProvidedException
            //  and return a 401
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.DeleteUser(newUser.Id.ToString());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void DeleteUser_NoUserIdProvided_412()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            //passing null parameter creates InvalidModelPayloadException that should be caught
            //  and return a 400
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.DeleteUser((string)null);

            Assert.AreEqual(HttpStatusCode.PreconditionFailed, response.StatusCode);
        }

        [TestMethod]
        public void DeleteUser_SessionExpired_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            //_tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/delete";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            //passing user with no session should result in SessionNotFoundException
            // and return a 401
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.DeleteUser(newUser.Id.ToString());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void UpdateUser_InvalidUserId_404()
        {
            newUser = _tu.CreateUserObject();
            newUser.IsAdministrator = true;
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/user/update";
            _umController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var userDTO = new UpdateUserRequestDTO
            {
                Id = Guid.NewGuid().ToString(),
                City = newUser.City,
                State = newUser.State,
                Country = newUser.Country,
                Manager = newUser.ManagerId.ToString(),
                IsAdmin = newUser.IsAdministrator,
                Disabled = newUser.Disabled
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _umController.Request = request;

            //non existent userID should result in UserNotFoundException
            //  and return a 404
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_umController.UpdateUser(userDTO);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void DeleteUserSSO_InvalidtokenSignature_401()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/sso/user/delete";
            var _userController = new UserController();
            _userController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var loginDTO = new LoginRequestPayload
            {
                SSOUserId = Guid.NewGuid().ToString(),
                Email = "something@email.com",
                Timestamp = 1928743091,
                Signature = "ahsbdkfhasjdfln",
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);

            _userController.Request = request;

            //invalid signature should throw and InvalidTokenSignatureException
            //  and return a 401
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_userController.DeleteViaSSO(loginDTO);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void CreatePoint_InvalidLongLat_400()
        {
            newUser = _tu.CreateUserObject();
            Session newSession = _tu.CreateSessionObject(newUser);
            _tu.CreateSessionInDb(newSession);

            var endpoint = API_ROUTE_LOCAL + "/api/point";
            _pointController.Request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint)
            };

            var point = new PointCreateDTO
            {
                Longitude = 185,
                Latitude = 85,
                Description = "bad longitude value",
                Name = "test bad point",
            };

            var request = new HttpRequestMessage();
            request.Headers.Add("token", newSession.Token);
            
            _pointController.Request = request;

            //out of range Longitude should throw InvalidPointException
            //  and return a 400
            NegotiatedContentResult<string> response = (NegotiatedContentResult<string>)_pointController.Post(point);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Longitude/Latitude value invalid.", response.Content.ToString());
        }
    }
}
