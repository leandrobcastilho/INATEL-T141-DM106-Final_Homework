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
using INATEL_T141_DM106_Final_Homework.CRMClient;
using INATEL_T141_DM106_Final_Homework.br.com.correios.ws;

namespace INATEL_T141_DM106_Final_Homework.Controllers
{
    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private INATEL_T141_DM106_Final_HomeworkContext db = new INATEL_T141_DM106_Final_HomeworkContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            //return db.Orders;
           return db.Orders.Include(order => order.ProductOrders).ToList();
        }

        // GET: api/Orders
        [HttpGet]
        [Route("ByEmail")]
        [ResponseType(typeof(Order))]
        public List<Order> GetOrdersByEmail(string email)
        {
            List<Order> orders = db.Orders.Where(order => order.UserEmail == User.Identity.Name).ToList();
            if (User.IsInRole("USER") && !User.Identity.Name.Equals(email))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            return orders;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        [Route("api/Orders/{id}", Name = "GetOrderById")]
        public IHttpActionResult GetOrder(int id)
        {
            //Order order = db.Orders.Find(id);
            Order order = db.Orders.Where(o => o.Id == id).First();
            if (order == null)
            {
                return Content(HttpStatusCode.NotFound, "Order Not Found.");
                //return NotFound();
            }

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized to see this order");
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [HttpPut]
        [Route("CalculateShipPrice")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCalculateShipPrice(int id)
        {
            Order order = db.Orders
                //.Include(o =>o.ProductOrders)
                //.ThenInclude(po => po.Product)
                .Find(id);

            if (order == null)
            {
                return Content(HttpStatusCode.NotFound, "Order Not Found.");
                //return NotFound();
            }

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized to Calculate Ship Price for this order");
            }

            if (!order.Status.Equals(Status_New))
            {
                return BadRequest("The order status is different of new");

            }
            string customerZipCode = GetZipCode(order.UserEmail);
            if (customerZipCode == null)
            {
                return BadRequest("CEP not registered");
            }

            List<Product> allProducts = GetListProduct(order);
            if (allProducts.Count == 0)
            {
                return BadRequest("Order is empty");
            }

            Product productResult = CalculateProductResult(allProducts);

            Shipper shipper = CalculateShipper(customerZipCode, productResult);
            if (!shipper.Erro.Equals("0"))
            {
                return BadRequest("Código do erro: " + shipper.Erro + "-" + shipper.MsgErro);
            }

            var priceShip = shipper.PriceShip;
            DateTime deliveryDate = shipper.DeliveryDate;

            order.PriceShip = priceShip;
            order.DeliveryDate = deliveryDate;
            order.TotalPrice = Convert.ToDecimal(productResult.Price) + priceShip;
            order.TotalWeight = Convert.ToDecimal(productResult.Weight);

            return PutOrder(id, order);
        }

        private Shipper CalculateShipper(string customerZipCode, Product productResult)
        {
            Shipper shipper = new Shipper();

            string nCdEmpresa = "";
            string sDsSenha = "";

            string ServiceCode_SEDEX_Varejo = "40010";
            string ServiceCode_SEDEX_a_Cobrar_Varejo = "40045";
            string ServiceCode_SEDEX_10_Varejo = "40215";
            string ServiceCode_SEDEX_Hoje_Varejo = "40290";
            string ServiceCode_PAC_Varejo = "41106";

            string nCdServico = ServiceCode_SEDEX_Varejo;

            string sCepOrigem = "37540000";
            string sCepDestino = customerZipCode;
            string nVlPeso = Convert.ToString(productResult.Weight);

            int Format_CaixaPacote = 1;
            int Format_RoloPrisma = 2;

            int nCdFormato = Format_CaixaPacote;

            decimal nVlComprimento = Convert.ToDecimal(productResult.Length);
            decimal nVlAltura = Convert.ToDecimal(productResult.Height);
            decimal nVlLargura = Convert.ToDecimal(productResult.Width);
            decimal nVlDiametro = Convert.ToDecimal(productResult.Diameter);
            string sCdMaoPropria = "N";
            decimal nVlValorDeclarado = Convert.ToDecimal(productResult.Price);
            string sCdAvisoRecebimento = "S";
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado resultado = correios.CalcPrecoPrazo(nCdEmpresa, sDsSenha, nCdServico, sCepOrigem, sCepDestino, nVlPeso, nCdFormato, nVlComprimento, nVlAltura, nVlLargura, nVlDiametro, sCdMaoPropria, nVlValorDeclarado, sCdAvisoRecebimento);

            if (!resultado.Servicos[0].Erro.Equals("0"))
            {
                shipper.Erro = resultado.Servicos[0].Erro;
                shipper.MsgErro = resultado.Servicos[0].MsgErro;
            }
            shipper.Erro = "0";
            shipper.MsgErro = "";
            shipper.PriceShip = Convert.ToDecimal(resultado.Servicos[0].Valor);
            shipper.DeliveryDate = DateTime.Now.AddDays(Convert.ToInt32(resultado.Servicos[0].PrazoEntrega)).Date;

            return shipper;
        }

        private string GetZipCode(string userEmail)
        {
            string customerZipCode = null;
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(userEmail);
            if (customer != null)
            {
                customerZipCode = customer.zip;
            }
            return customerZipCode;
        }

        private static Product CalculateProductResult(List<Product> allProducts)
        {
            Product productResult = new Product();

            //Preço Total; 
            productResult.Price = allProducts.Sum(p => p.Price);

            //Peso Total; 
            productResult.Weight = allProducts.Sum(p => p.Weight);

            //Altura Maxima; 
            productResult.Height = allProducts.Max(p => p.Height);

            //Somatorio dimencao dos objeos circulres;
            double dimension = productResult.Diameter = allProducts.Where(p => p.Diameter > 0).Sum(p => p.Diameter);

            //Somatorio Largura objeos nao circulres; 
            double width = allProducts.Where(p => p.Diameter == 0).Sum(p => p.Width);

            //Somatorio Largura objeos nao circulres; 
            double length = allProducts.Where(p => p.Diameter == 0).Sum(p => p.Length);

            //Largura Total;
            productResult.Width = dimension + width;

            //Comprimento Total;
            productResult.Length = dimension + length;

            return productResult;
        }

        private static List<Product> GetListProduct(Order order)
        {
            List<Product> allProducts = new List<Product>();
            foreach (ProductOrder productOrder in order.ProductOrders)
            {
                Product product = productOrder.Product;
                allProducts.Add(product);
            }

            return allProducts;
        }

        // PUT: api/Orders/5
        [HttpPut]
        [Route("CloseOrder")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCloseOrder(int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Content(HttpStatusCode.NotFound, "Order Not Found.");
                //return NotFound();
            }

            if (order.Status.Equals(Status_New) || !order.Status.Equals(Status_Closed) || !order.Status.Equals(Status_Cancel) || !order.Status.Equals(Status_Delivered))
            {
                return BadRequest("Invalid Status");
            }

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized to close this order");
            }

            if (order.PriceShip <= 0)
            {
                return BadRequest("Order cannot be closed, ship price not calculated");
            }

            order.Status = Status_Closed;

            return PutOrder(id, order);
        }

        public static string Status_New = "New";
        public static string Status_Closed = "Closed";
        public static string Status_Cancel = "Cancel";
        public static string Status_Delivered = "Delivered";

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        private IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized change this order");
            }

            if (!order.Status.Equals(Status_New) || !order.Status.Equals(Status_Closed) || !order.Status.Equals(Status_Cancel) || !order.Status.Equals(Status_Delivered))
            {
                return BadRequest("Invalid Status");
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return Content(HttpStatusCode.NotFound, "Order Not Found.");
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [HttpPost]
        [Route("CreateOrder")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostCreateOrder(Order order)
        {

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized to create this order");
            }

            foreach (ProductOrder productOrder in order.ProductOrders)
            {
                productOrder.Product = db.Products.Find(productOrder.ProductId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.Status = Status_New;
            order.TotalWeight = 0;
            order.PriceShip = 0;
            order.TotalPrice = 0;
            order.Date = DateTime.Now.Date;
            order.DeliveryDate = DateTime.Now.AddDays(30).Date;

           db.Orders.Add(order);
           db.SaveChanges();

            //return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
            return CreatedAtRoute("GetOrderById", new { id = order.Id }, order);
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized to create this order");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.Status = Status_New;
            order.TotalWeight = 0;
            order.PriceShip = 0;
            order.TotalPrice = 0;
            order.Date = DateTime.Now.Date;
            order.DeliveryDate = DateTime.Now.AddDays(30).Date;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Content(HttpStatusCode.NotFound, "Order Not Found.");
                //return NotFound();
            }

            if (User.IsInRole("USER") && order.UserEmail != User.Identity.Name)
            {
                return Content(HttpStatusCode.Forbidden, "User not authorized to delete this order");
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}