using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanghuaForSup.Models
{
    public class UserViewModel : SupplierUser
    {
        public List<SupplierRole> AllRole { get; set; }
    }
}