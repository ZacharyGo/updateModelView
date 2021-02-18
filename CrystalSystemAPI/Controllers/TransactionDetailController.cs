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
    public class TransactionDetailController : ApiController
    {

        private CerinimbusDevEntities db = new CerinimbusDevEntities();

        [HttpGet, Route("api/transactionDetails/")]
        public IHttpActionResult GetTransactionDetails(string orderNumber, string transacNumber, string prodCategory)
        {
            //JM: 2020-06-23 added try catch and generate a proper api response
            try
            {
                prodCategory = prodCategory ?? "";

                var data = db.cvINVTransactionDetailCrystalControls
                   .Join(
                       db.products,
                       detail => detail.ProductID,
                       prod => prod.ProductID,
                       (detail, prod) => new
                       {
                           details = detail,
                           prods = prod
                       }
                   ).Where(e =>
               e.details.RegNumber.ToString() == orderNumber &&
               e.details.TransactionNumber.ToString() == transacNumber &&
               e.details.TransactionType != "P" &&
               e.prods.AssemblyType != "v" &&
               (e.details.FProductCategory.ToString().Contains(prodCategory) || prodCategory.Contains(e.details.FProductCategory.ToString()))
               )
               .Where(u => db.tbProductComponents
                        .Where(pc => pc.GUIDProduct == u.details.GUIDProduct).Count() <= 0
                )
               //story 3035 - Sort page 2 by ProductID and PKIDInvTransactionDetail, commented out the current order and just use ProductID and PKIDInvTransactionDetail
               //.OrderBy(e => e.details.RegNumber).ThenBy(e => e.details.TransactionNumber).ThenBy(e => e.details.Sequence).ThenBy(e => e.details.ProductID).ThenBy(e => e.details.Description).ToList();
               .OrderBy(e => e.details.ProductID).ThenBy(e => e.details.PKIDINVTransactionDetail).ToList();
                List<cvINVTransactionDetailCrystalControl> transacDetails = new List<cvINVTransactionDetailCrystalControl>();

                foreach (var detail in data)
                {
                    transacDetails.Add(detail.details);

                }

                //List<cvINVTransactionDetailCrystalControl> transacDetails;

                //transacDetails = db.cvINVTransactionDetailCrystalControls.AsNoTracking().Where(
                //e =>
                //    e.RegNumber.ToString() == orderNumber &&
                //    e.TransactionNumber.ToString() == transacNumber &&
                //    e.TransactionType != "P"
                //).OrderBy(e => e.RegNumber).ThenBy(e => e.TransactionNumber).ToList();


                return Ok(transacDetails);
            }
            catch(Exception e)
            {
                var message = e.InnerException.Message.ToString();
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }
            
        }

        [HttpGet, Route("api/productDetails/")]
        public IHttpActionResult GetPropductDetails(string productId)
        {

            //JM: 2020-06-23 added try catch and generate a proper api response
            try
            {
                List<product> productDetails;
                productDetails = db.products.AsNoTracking().Where(
                e =>
                    e.ProductID.ToString() == productId
                ).ToList();

                //cvAssemblyOrdersUnlinked order = new cvAssemblyOrdersUnlinked();
                //order = db.cvAssemblyOrdersUnlinkeds.AsNoTracking().Where(
                //e =>
                //    e.Session.ToString() == orderNumber &&
                //    e.TransactionNumber.ToString() == transacNumber
                //).FirstOrDefault();


                return Ok(productDetails);
            }
            catch (Exception e)
            {
                var message = e.InnerException.Message.ToString();
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }

        }

        [HttpPost, Route("api/saveImage/")]
        public HttpResponseMessage SaveImageUpload(ImageUploadData imageUpload) {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            try
            {
                string query = "exec cspImageUploadInvTrx @imagename, @transactionname, @transactiondetail, @imagepath";
                SqlParameter imagename = new SqlParameter("imagename", imageUpload.ImageName);
                SqlParameter imagepath = new SqlParameter("imagepath", imageUpload.ImagePath);
                SqlParameter transactionname = new SqlParameter("transactionname", imageUpload.TransactionName);
                SqlParameter transactiondetail = new SqlParameter("transactiondetail", imageUpload.TransactionDetail);
                //SqlParameter Message = new SqlParameter("Message", imageUpload.ImageName);

                object[] parameters = new object[] { imagename, imagepath, transactionname, transactiondetail };
                int result = db.Database.ExecuteSqlCommand(query, parameters);
                
                if (result >= 1)
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    var message = "The strored Procedure cspImageUploadInvTrx had some error.";
                    throw new HttpResponseException(
                        Request.CreateErrorResponse(HttpStatusCode.NotFound, message));

                }
            }
            catch (Exception ex)
            {
                var message = "The table cvINVTransactionDetailCrystalControl may not be available or one of its columns is not correctly set.";

                //JM: 2020-07-09 added more exception message
                var exception_message = ex.Message.ToString();
                if (exception_message == "" || exception_message == null)
                {
                }
                else
                {
                    message = exception_message;
                }

                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));

            }


            return responseMessage;
        }

        //NB: 2020-10-12 add route for signatureSave, which will add row to tbInvTransactionDetailControl
        [HttpPost, Route("api/saveSignature/")]
        public HttpResponseMessage SaveSignature(ImageUploadData imageUpload)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            try
            {
                //string query = "insert into tbInvTransactionDetailControl values(NEWID(),@transactiondetail,CONVERT(VARBINARY(MAX), @signature))";
                string query = "insert into tbInvTransactionDetailControl values(NEWID(),@transactiondetail,@signature)";


                //SqlParameter signature = new SqlParameter("signature", imageUpload.ImagePath);
                SqlParameter signature = new SqlParameter("signature", Convert.FromBase64String(imageUpload.ImagePath.Substring(imageUpload.ImagePath.IndexOf(',') + 1)));
                SqlParameter transactiondetail = new SqlParameter("transactiondetail", imageUpload.TransactionDetail);
                //SqlParameter Message = new SqlParameter("Message", imageUpload.ImageName);

                object[] parameters = new object[] { signature, transactiondetail };
                int result = db.Database.ExecuteSqlCommand(query, parameters);

                if (result >= 1)
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    var message = "The insert query to tbINVTransactionDetailControl had some error.";
                    throw new HttpResponseException(
                        Request.CreateErrorResponse(HttpStatusCode.NotFound, message));

                }
            }
            catch (Exception ex)
            {
                var message = "The table cvINVTransactionDetailCrystalControl may not be available or one of its columns is not correctly set.";

                //JM: 2020-07-09 added more exception message
                var exception_message = ex.Message.ToString();
                if (exception_message == "" || exception_message == null)
                {
                }
                else
                {
                    message = exception_message;
                }

                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));

            }


            return responseMessage;
        }

        [HttpPost, Route("api/saveTestInput/")]
        public HttpResponseMessage UpdateTestInput(BatchOutputData batchdata)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            try
            {
                //NB: 2020-09-23 update AssemblyQuantity if batchdata.isProductionQuantity is true
                string query = "";
                int result;
                if (batchdata.isProductionQuantity)
                {
                    query = "Update cvINVTransactionDetailCrystalControl Set Note = @note, Quantity=@assemblyquantity, ComponentQuantity=ABS(@assemblyquantity)/ParentQuantity Where RegNumber = @order And TransactionNumber = @orderTransactionNum And GUIDINVTransactionDetail = @GUIDINVTransactionDetail";
                    SqlParameter assemblyquantity = new SqlParameter("assemblyquantity", batchdata.AssemblyQuantity);

                    SqlParameter note = new SqlParameter("note", batchdata.FinalOutPut);
                    SqlParameter order = new SqlParameter("order", batchdata.RegNumber);
                    SqlParameter orderTransactionNum = new SqlParameter("orderTransactionNum", batchdata.TransacNumber);
                    //SqlParameter productID = new SqlParameter("productID", batchdata.ProductId);
                    SqlParameter GUIDINVTransactionDetail = new SqlParameter("GUIDINVTransactionDetail", batchdata.GUIDINVTransactionDetail);

                    object[] parameters = new object[] { note, order, orderTransactionNum, GUIDINVTransactionDetail, assemblyquantity };
                    result = db.Database.ExecuteSqlCommand(query, parameters);
                }
                else {
                    query = "Update cvINVTransactionDetailCrystalControl Set Note = @note Where RegNumber = @order And TransactionNumber = @orderTransactionNum And GUIDINVTransactionDetail = @GUIDINVTransactionDetail";

                    SqlParameter note = new SqlParameter("note", batchdata.FinalOutPut);
                    SqlParameter order = new SqlParameter("order", batchdata.RegNumber);
                    SqlParameter orderTransactionNum = new SqlParameter("orderTransactionNum", batchdata.TransacNumber);
                    //SqlParameter productID = new SqlParameter("productID", batchdata.ProductId);
                    SqlParameter GUIDINVTransactionDetail = new SqlParameter("GUIDINVTransactionDetail", batchdata.GUIDINVTransactionDetail);

                    object[] parameters = new object[] { note, order, orderTransactionNum, GUIDINVTransactionDetail };
                    result = db.Database.ExecuteSqlCommand(query, parameters);
                }

                
               


                if (result >= 1)
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    //JM: 2020-06-24 changed the message returned
                    //responseMessage.StatusCode = HttpStatusCode.NoContent;
                    var message = "The table cvINVTransactionDetailCrystalControl may not be available or one of its columns is not correctly set.";
                    throw new HttpResponseException(
                        Request.CreateErrorResponse(HttpStatusCode.NotFound, message));

                }
            }catch(Exception ex)
            {
                //JM: 2020-06-24 changed the message returned
                //responseMessage.StatusCode = HttpStatusCode.BadRequest;
                //responseMessage.ReasonPhrase = ex.Message;

                var message = "The table cvINVTransactionDetailCrystalControl may not be available or one of its columns is not correctly set.";

                //JM: 2020-07-09 added more exception message
                var exception_message = ex.Message.ToString();
                if (exception_message == "" || exception_message == null)
                {
                }
                else
                {
                    message = exception_message;
                }

                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));

            }


            return responseMessage;
        }



        

    }
}
