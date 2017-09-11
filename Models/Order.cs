using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace INATEL_T141_DM106_Final_Homework.Models
{
    public class Order
    {
        public Order()
        {
            this.ProductOrders = new HashSet<ProductOrder>();
        }

        //Identificação única do pedido;
        public int Id { get; set; }

        //E-mail do usuário dono do pedido;
        public string UserEmail { get; set; }

        //Data do pedido;
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        //Data de entrega do pedido;
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        //String com o status do pedido(novo, fechado, cancelado, entregue);
        public string Status { get; set; }

        //Preço total do pedido;
        public decimal TotalPrice { get; set; }

        //Peso total do pedido;
        public decimal TotalWeight { get; set; }

        //Preço do frete;
        public decimal PriceShip { get; set; }

        //Lista de itens do pedido.
        public virtual ICollection<ProductOrder> ProductOrders
        {
            get; set;
        }

    }
}