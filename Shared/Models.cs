using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Shared
{
    public abstract class BaseDto
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PatchUpdate
    {
        public string op { get; set; }
        public string path { get; set; }
        public string value { get; set; }
    }
    public enum PatchUpdateItem
    {
        Name,
        Title,
        Paragraph,
        Image,
        Order,
        Disabled
    }
    public class PaginationModel
    {
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 10;
    }
    public class PageDataModel<T>
    {
        public List<T> Data { get; set; }
        public int PageQuantity { get; set; }
    }

    public class StockDto : BaseDto
    {
        public int Quantity { get; set; }
        [StringLength(5000)]
        public string Note { get; set; }
    }
    public class UploadProductLinkModel
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }
        public Stream Image { get; set; }
        public string ImageName { get; set; }
    }
    public class CartDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<LineDto> Lines { get; set; }
    }
    public class LineDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
    public class CheckoutReturnDto
    {
        public string Paid { get; set; }
        public string Shipped { get; set; }
        public string InventoryDone { get; set; }
        public string Token { get; set; }
    }
}
