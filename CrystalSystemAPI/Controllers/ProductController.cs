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
    public class ProductController : ApiController
    {
        //DT 2020-03-09 call our database entities
        private CerinimbusDevEntities db = new CerinimbusDevEntities();

        //DT 2020-03-09 create a function controller that will get all the product data
        public IQueryable<product> Getproducts()
        {
            //DT 2020-03-09 return the products data
            return db.products;

        }

        // GET: api/Product/5
        //DT 2020-03-09 create a function controller that will get all the product data with the product id filter
        [ResponseType(typeof(product))]
        public IHttpActionResult Getproduct(string id)
        {
            //product product = db.products.Find(id);
            product product = db.products.FirstOrDefault(e => e.ProductID == id || e.PurchaseUnit == id);
            if (product == null)
            {
                //DT 2020-03-09 return NotFound() function if there is no product found
                return NotFound();
            }

            //DT 2020-03-09 return the product data
            return Ok(product);
        }

        //// PUT: api/Product/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult Putproduct(string id, product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != product.ProductID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(product).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!productExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/Product
        //[ResponseType(typeof(product))]
        //public IHttpActionResult Postproduct(product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.products.Add(product);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (productExists(product.ProductID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        //}

        //// DELETE: api/Product/5
        //[ResponseType(typeof(product))]
        //public IHttpActionResult Deleteproduct(string id)
        //{
        //    product product = db.products.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    db.products.Remove(product);
        //    db.SaveChanges();

        //    return Ok(product);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool productExists(string id)
        //{
        //    return db.products.Count(e => e.ProductID == id) > 0;
        //}
    }
}