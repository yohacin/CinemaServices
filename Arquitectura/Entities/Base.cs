using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Serializable]
    public abstract class Base
    {
        [NotMapped]
        public Action Action { get; set; } = Action.None;

        public Base Clone()
        {
            return (Base)this.MemberwiseClone();
        }
    }
    
    public enum Action
    {
        None,Insert,Update,Delete
    }
}
