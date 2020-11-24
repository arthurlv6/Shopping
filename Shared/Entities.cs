using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public T ToModel<T>(IMapper mapper) where T : BaseDto
        {
            return mapper.Map<T>(this);
        }
    }
   
    public class Stock: BaseEntity
    {
        public int Quantity { get; set; }
        [StringLength(5000)]
        public string Note { get; set; }
    }
}
