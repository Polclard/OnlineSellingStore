using OnlineSellingStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.DataAccess.Data
{
    public class CategoryNotFoundException<T>
    {
        public override string ToString()
        {
            return $"Category Not Found!!!";
        }
    }

}
