using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kurs0._7.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddItem(Materials material, int quantity)
        {
            CartLine line = lineCollection
                .Where(g => g.material.Id == material.Id)
                .FirstOrDefault();
            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    material = material,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(Materials material)
        {
            lineCollection.RemoveAll(l => l.material.Id == material.Id);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.material.Price * e.Quantity);
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }
    }

    public class CartLine
    {
        public Materials material { get; set; }
        public int Quantity { get; set; }
    }
}