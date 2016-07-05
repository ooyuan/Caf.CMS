﻿using System;
using System.Collections.Generic;


namespace CAF.Infrastructure.Core.Domain.Cms.Orders
{
    public partial class OrganizedShoppingCartItem
    {
        public OrganizedShoppingCartItem(ShoppingCartItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Item = item;	// must not be null
            ChildItems = new List<OrganizedShoppingCartItem>();
            //BundleItemData = new ProductBundleItemData(item.BundleItem);
        }

        public ShoppingCartItem Item { get; private set; }
        public IList<OrganizedShoppingCartItem> ChildItems { get; set; }
        //public ProductBundleItemData BundleItemData { get; set; }
    }
}
