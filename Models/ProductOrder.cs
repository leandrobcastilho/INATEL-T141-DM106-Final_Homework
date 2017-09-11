using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INATEL_T141_DM106_Final_Homework.Models
{
    public class ProductOrder
    {
        public ProductOrder()
        {
            this.Product = new Product();
        }

        //Identificação única do item de pedido; 
        public int Id { get; set; }

        //Quantidade de produtos do item de pedido;
        public double Quantity { get; set; }

        //Identificação única do produto presente no item;
        public int ProductId { get; set; }

        //Identificação única do pedido a qual o item pertence;
        public int OrderId { get; set; }

        //O modelo também deverá considerar a existência de uma referência ao produto em si, para que suas informações possam ser exibidas nas respostas das operações dos pedidos.
        public virtual Product Product
        {
            get; set;
        }

    }
}