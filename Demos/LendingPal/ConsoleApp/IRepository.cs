using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public interface IRepository<T>
    {
         List<T> GetAll();
         void Add(T record);
         void Remove(T record);
    }
}