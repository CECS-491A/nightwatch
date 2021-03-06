﻿using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ManagerLayer.AccessControl;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.Controllers
{
    public class ControllerHelpers
    {
        public readonly static string Redirect = "https://kfc-sso.com/#/login";
        public static void ValidateModelAndPayload(ModelStateDictionary modelState, object payload)
        {
            if (!modelState.IsValid || payload == null)
            {
                throw new InvalidModelPayloadException("Invalid payload.");
            }
        }

        private static string GetToken(HttpRequestMessage request)
        {
            var token = GetHeader(request, "token");

            //allows the token to be stored in either location in the header
            if(token.Length < 1)
            {
                token = GetHeader(request, "Token");
            }

            if (token.Length < 1)
            {
                throw new NoTokenProvidedException("No token provided.");
            }
            return token;
        }

        private static string GetHeader(HttpRequestMessage request, string header)
        {
            IEnumerable<string> headerValues;
            var nameFilter = string.Empty;
            if (request.Headers.TryGetValues(header, out headerValues))
            {
                nameFilter = headerValues.FirstOrDefault();
            }
            return nameFilter;
        }

        public static Guid ParseAndCheckId(string id)
        {
            Guid guid;

            // check if valid SSO ID format
            var validParse = Guid.TryParse(id, out guid);
            if (!validParse)
            {
                throw new InvalidGuidException("Invalid Id.");
            }
            return guid;
        }

        // Verify if token is valid session
        public static Session ValidateSession(HttpRequestMessage request)
        {
            var token = GetToken(request);
            using (var _db = new DatabaseContext())
            {
                var _sessionService = new SessionService(_db);
                var session = _sessionService.ValidateSession(token);
                if (session == null)
                {
                    throw new SessionNotFoundException("Session is no longer available.");
                }
                return session;
            }
        }

        public static Session ValidateAndUpdateSession(HttpRequestMessage request)
        {
            var token = GetToken(request);
            using (var _sessionDb = new DatabaseContext())
            { 
                AuthorizationManager _authorizationManager = new AuthorizationManager(_sessionDb);
                var session = _authorizationManager.ValidateAndUpdateSession(token);
                if (session == null)
                {
                    throw new SessionNotFoundException("Session is no longer available.");
                }
                _sessionDb.SaveChanges();
                return session;
            }
        }
    }
}