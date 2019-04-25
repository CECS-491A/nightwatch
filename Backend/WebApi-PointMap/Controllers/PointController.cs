﻿using ManagerLayer;
using System;
using System.Web.Http;
using WebApi_PointMap.Models;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using ManagerLayer.AccessControl;
using System.Web.Script.Serialization;
using DataAccessLayer.Models;
using DataAccessLayer.Database;
using WebApi_PointMap.ErrorHandling;
using static ServiceLayer.Services.ExceptionService;

namespace WebApi_PointMap.Controllers
{
    public class PointController : ApiController
    {
        private PointManager _pm;
	    private DatabaseContext _db;

        public PointController()
        {
            _db = new DatabaseContext();
            _pm = new PointManager(_db);
        }

        // retrieves a point
        [HttpGet]
        [Route("api/point/{guid}")]
        public IHttpActionResult Get(string guid)
        {
            try
            { 
                var pointId = ControllerHelpers.ParseAndCheckId(guid);     
                var token = ControllerHelpers.GetToken(Request);
                ControllerHelpers.ValidateAndUpdateSession(_db, token);
                Guid id = new Guid(guid);

                var point = _pm.GetPoint(id);
                _db.SaveChanges();

                return Ok(point);
            }
            catch (Exception e)
            {
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        // creates a point
        [HttpPost]
        [Route("api/point")]
        public IHttpActionResult Post([FromBody] PointPOST pointPost)
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request);
                ControllerHelpers.ValidateAndUpdateSession(_db, token);
                var point = _pm.CreatePoint(pointPost.Longitude, pointPost.Latitude, pointPost.Description, pointPost.Name);

                _db.SaveChanges();

                return Ok(point);
            }
            catch(Exception e)
            {
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        // updates a point
        [HttpPut]
        [Route("api/point/{guid}")]
        public IHttpActionResult Put(string guid, [FromBody] PointPOST pointPost)
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request);
                ControllerHelpers.ValidateAndUpdateSession(_db, token);

                Guid id = new Guid(guid);
                pointPost.Id = id;
                var point = _pm.UpdatePoint(id, pointPost.Longitude, pointPost.Latitude,
                                            pointPost.Description, pointPost.Name,
                                            pointPost.CreatedAt);
                _db.SaveChanges();

                return Ok(point);
            }
            catch (Exception e)
            {
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        //Deletes a point
        [HttpDelete]
        [Route("api/point/{guid}")]
        public IHttpActionResult Delete(string guid)
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request);
                ControllerHelpers.ValidateAndUpdateSession(_db, token);

                var id = new Guid(guid);

                _pm.DeletePoint(id);
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return ResponseMessage(PointErrorHandler.HandleException(e, _db));
            }
        }

        [HttpGet]
        [Route("api/points")]
        public HttpResponseMessage GetPoints()
        {
            try
            {
                var token = ControllerHelpers.GetToken(Request);

                var session = ControllerHelpers.ValidateAndUpdateSession(_db, token);

                var headers = Request.Headers;

                if(headers.Contains("minLng") && headers.Contains("maxLng") && 
                    headers.Contains("minLat") && headers.Contains("maxLat"))
                {
                    object pointList;
                    try
                    {
                        float minLng = float.Parse(headers.GetValues("minLng").First());
                        float minLat = float.Parse(headers.GetValues("minLat").First());
                        float maxLng = float.Parse(headers.GetValues("maxLng").First());
                        float maxLat = float.Parse(headers.GetValues("maxLat").First());
                        pointList = _pm.GetAllPoints(minLat, minLng, maxLat, maxLng);
                    }
                    catch(FormatException)
                    {
                        throw new InvalidHeaderException("Invalid field formatting.");
                    }
                            
                    if (pointList != null)
                    {
                        var jsonContent = new JavaScriptSerializer().Serialize(pointList);
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        return response;
                    }
                }
                throw new InvalidHeaderException("Invalid field formatting.");
            }
            catch(Exception e)
            {
                return PointErrorHandler.HandleException(e, _db);
            }
        }
    }
}
