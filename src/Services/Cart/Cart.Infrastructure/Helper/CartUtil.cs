using Cart.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Infrastructure.Helper
{
    public class CartUtil
    {
        public static Dictionary<int, CartItemDTO> GetCartFromCookie(string cookieValue)
        {
            Dictionary<int, CartItemDTO> cart = new Dictionary<int, CartItemDTO>();
            string decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
            string[] itemsList = decodedString.Split('|');

            foreach (string strItem in itemsList)
            {
                if (!string.IsNullOrEmpty(strItem))
                {
                    string[] arrItemDetail = strItem.Split(',');
                    int itemId = int.Parse(arrItemDetail[0].Trim());
                    //string itemName = arrItemDetail[1].Trim();
                    //int quantity = int.Parse(arrItemDetail[2].Trim());
                    //decimal unitPrice = decimal.Parse(arrItemDetail[3].Trim());

                    CartItemDTO item = new CartItemDTO()
                    {
                        ItemId = itemId,
                        //ItemName = itemName,
                        //Price = unitPrice
                    };
                    cart[itemId] = item;
                }
            }

            return cart;
        }

        public static Cookie GetCookieByName(HttpRequest request, string cookieName)
        {
            if (request.Cookies.TryGetValue(cookieName, out string cookieValue))
            {
                return new Cookie(cookieName, cookieValue);
            }
            return null;
        }

        public static void SaveCartToCookie(HttpRequest request, HttpResponse response, string strItemsInCart, string userId)
        {
            string cookieName = "Cart_" + userId;
            CookieOptions options = new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(120),
                HttpOnly = true,
                IsEssential = true
            };

            response.Cookies.Append(cookieName, strItemsInCart, options);
        }

        public static void DeleteCartToCookie(HttpRequest request, HttpResponse response, string userId)
        {
            string cookieName = "Cart_" + userId;
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = true,
                IsEssential = true
            };

            response.Cookies.Delete(cookieName, options);
        }

        public static string ConvertCartToString(List<CartItemDTO> itemsList)
        {
            StringBuilder strItemsInCart = new StringBuilder();
            foreach (CartItemDTO item in itemsList)
            {
                strItemsInCart.Append($"{item.ItemId}|");
            }
            string encodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(strItemsInCart.ToString()));
            return encodedString;
        }

        public static List<string> CookieNames(HttpRequest request)
        {
            return request.Cookies.Keys.ToList();
        }
    }
}
