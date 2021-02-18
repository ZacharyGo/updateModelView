using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CrystalSystemAPI.Models;
using System.Data.SqlClient;

namespace CrystalSystemAPI.Controllers
{
    public class LoginAccountController : ApiController
    {
        private CerinimbusDevEntities db = new CerinimbusDevEntities();

        [HttpGet, Route("api/authentication/")]
        public IHttpActionResult AuthenticationLogin(string Email, string Password)
        {
            //JM: 2020-06-23 added try catch and generate a proper api response
            try
            {
                tbAccess tAccess;
                tAccess = db.tbAccesses.AsNoTracking().Where(
                e =>
                    e.EMail.ToString() == Email /*&&*/
                                                //e.Password.ToString() == Password //JM: 2020-06-11 bypass password for now per request
                ).FirstOrDefault();



                return Ok(tAccess);
            }
            catch (Exception e)
            {
                var message = e.InnerException.Message.ToString();
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }
            
        }

        //NB: 2020-10 add this second authentication script which will be used by 3rd page Signature datatype which requires username/password
        [HttpGet, Route("api/authentication2/")]
        public IHttpActionResult AuthenticationLogin2(string Email, string Password)
        {
            //JM: 2020-06-23 added try catch and generate a proper api response
            try
            {
                tbAccess tAccess;
                tAccess = db.tbAccesses.AsNoTracking().Where(
                e =>
                    e.EMail.ToString() == Email &&
                    e.Password.ToString() == Password 
                ).FirstOrDefault();



                return Ok(tAccess);
            }
            catch (Exception e)
            {
                var message = e.InnerException.Message.ToString();
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }


        }

        //JM: 2020-06-15 added checking database connection
        [HttpGet, Route("api/database/check")]
        public IHttpActionResult CheckDatabaseConnection()
        {
            try
            {
                db.Database.Connection.Open();
                db.Database.Connection.Close();
            }
            catch (SqlException)
            {
                return Ok(false);
            }
            return Ok(true);
        }

    }
}