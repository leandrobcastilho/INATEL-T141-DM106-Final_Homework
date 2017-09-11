using System;

namespace INATEL_T141_DM106_Final_Homework.Controllers
{
    internal class Shipper
    {
        public Decimal PriceShip { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Erro { get; set; }
        public string MsgErro { get; set; }
    }
}