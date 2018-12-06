using System;
using System.Threading.Tasks;

namespace LinkImager
{
    public interface IShare
    {
        Task Show(string title, string message, string filePath);
    }
}
