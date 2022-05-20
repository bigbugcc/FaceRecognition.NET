using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.NET.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StuId { get; set; }
        public string Passw { get; set; }
        public string Role { get; set; }
        public DateTime CreatedTime { get; set; }

        public User()
        {
            this.Id = GetRandomNum();
            this.CreatedTime = DateTime.Now;
            this.Role = "B";
            this.Name = string.Empty;
            this.StuId = string.Empty;
            this.Passw = string.Empty;
        }

        //Return a random number
        public static int GetRandomNum()
        {
            Random rand = new Random();
            return rand.Next(0, 999999);
        }
    }
}
