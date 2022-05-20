using SqlSugar;
using System.Collections.Generic;
using FaceRecognition.NET.Entity;

namespace FaceRecognition.NET.DbContext
{
    public class UserDao
    {
        public SqlSugarProvider _context = null;
        public UserDao() {
            _context = new DbContext().Client.Context;
        }

        public List<User> GetAllUser() {
            return _context.Queryable<User>().ToList();
        }

        public int InsertUser(User user)
        {
            if (!IsStuIdExits(user.StuId) && user != null)
            {
                //1 成功
                return _context.Insertable(user).ExecuteCommand();
            }
            else {
                return 0;
            }
        }

        public int DelUser(int Id) {
            if (IsIdExits(Id))
            {
                return _context.Deleteable<User>().Where(u => u.Id == Id).ExecuteCommand();
            }
            else {
                return 0;
            }
        }

        public int UpdateUser(User user) {
            if (user != null && IsIdExits(user.Id))
            {
                var up =_context.Updateable<User>().SetColumns(it=>new User() { 
                    Name = user.Name,
                    Role = user.Role,
                    Passw = user.Passw,
                }).Where(w=>w.Id==user.Id).ExecuteCommand();
                return up;
            }
            else {
                return 0;
            }
        }

        private bool IsStuIdExits(string StuId) { 
            return _context.Queryable<User>().Any(x => x.StuId == StuId);
        }

        private bool IsIdExits(int Id)
        {
            return _context.Queryable<User>().Any(x => x.Id == Id);
        }
    }
}
