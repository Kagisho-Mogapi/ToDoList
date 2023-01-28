using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Client
{
    public class TodoDets
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public Priority Priority { get; set; }
        public DateTime Created { get; set; }
    }

    public enum Priority
    {
        Low, Medium, High
    }
}
