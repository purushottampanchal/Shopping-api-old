﻿namespace ShoppingApi.Models
{
    public class CardDetail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NameOnCard { get; set; }

        public string CardNo { get; set; }
        public string CVV { get; set; }
        public string Expiration{ get; set; }


    }
}
