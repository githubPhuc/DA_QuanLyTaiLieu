using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsApp.EntityFramework;

namespace ToolsApp.Utilities
{
    public class AppGlobal
    {
        public int ID_COMPANY_PARTNER = 9;
        public int ID_TEXT_OUTGOING = 1;
        public int ID_TEXT_INGOING = 2;
        public int ID_TEXT_INTERNAL = 3;
        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
        public bool AddRecycleBin(int IdParent,string NameRecycle,string DescriptionRecycle, string TableName, int UserId)
        {

            try
            {
                using (QuanLiVanBanEntities db_ = new QuanLiVanBanEntities())
                {
                    var data = new RecycleBin()
                    {
                        IdParent = IdParent,
                        NameRecycle = NameRecycle,
                        TableName = TableName,
                        UserCreate = UserId,
                        DatetimeCreate = DateTime.Now,
                        UserDelete = UserId,
                        DatetimeDelete = DateTime.Now,
                        UserUpdate = UserId,
                        DatetimeUpdate = DateTime.Now,
                        Status = true,
                        isDelete = false,
                    };
                    db_.RecycleBins.Add(data);
                    db_.SaveChanges();
                }
                return true;
            }catch (Exception e) { 
                return false;
            }
        }
    }
}
