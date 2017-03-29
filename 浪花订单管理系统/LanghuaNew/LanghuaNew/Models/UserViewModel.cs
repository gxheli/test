using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanghuaNew.Data;
namespace LanghuaNew.Models
{
    public class UserViewModel : User
    {
        public List<Role> AllRole { get; set; }

    }
    public class UsersList
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public List<UsersModel> data { get; set; }
        public SearchModel SearchModel { get; set; }
    }
    public class UsersModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string CreateTime { get; set; }
        public string LateOnLineTime { get; set; }
        public string UserRole { get; set; }
        public bool IsDelete { get; set; }


    }
    public class UserLogModel
    {
        public string OperUserNickName { get; set; }
        public string OperTime { get; set; }
        public string Operate { get; set; }
        public string Remark { get; set; }
    }
}