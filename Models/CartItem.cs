using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class CartItem
    {

        [ForeignKey("Cart")]
        public int CartID { get; set; }
        public Cart Cart { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Cart cart, Product product, int quantity)
        {
            Cart = cart;
            CartID = cart.ID;
            Product = product;
            ProductID = product.ID;
            Quantity = quantity;
        }

        public decimal GetTotalPrice() => Product.Price * Quantity;

        public void UpdateQuantity(int newQuantity) => Quantity = newQuantity;
    }
}