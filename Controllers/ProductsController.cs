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
using INATEL_T141_DM106_Final_Homework.Models;

namespace INATEL_T141_DM106_Final_Homework.Controllers
{
    [Authorize]
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        private INATEL_T141_DM106_Final_HomeworkContext db = new INATEL_T141_DM106_Final_HomeworkContext();

        // GET: api/Products
        public List<Product> GetProducts()
        {
            //return db.Orders;
            return db.Products.ToList();
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            //var auxProducts = from p in db.Products
            //                  where ((p.Code.Equals(product.Code) || p.Model.Equals(product.Model) ) && !p.Id.Equals(product.Id))
            //                  select p;
            var auxProducts = db.Products.Where(p => (p.Code.Equals(product.Code) || p.Model.Equals(product.Model)) && !p.Id.Equals(product.Id)).ToList();
            if (auxProducts != null && auxProducts.Count > 0)
                return BadRequest("Product with code or model already exists");

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var auxProducts = from p in db.Products
            //            where(p.Code.Equals(product.Code) || p.Model.Equals(product.Model) )
            //            select p;
            var auxProducts = db.Products.Where(p => p.Code.Equals(product.Code) || p.Model.Equals(product.Model)).ToList();
            if (auxProducts != null && auxProducts.Count > 0)
                return BadRequest("Product with code or model already exists");

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}