using System;
using System.ComponentModel.DataAnnotations;

namespace wms.Client.Model.Entity
{
    public interface IBaseEntity
    {
        int Id { get; set; }
    }

    [Serializable]
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}