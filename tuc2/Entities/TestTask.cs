using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tuc2.Entities
{
    public class TestTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string InputExample { get; set; }
        public string OutputExample { get; set; }
        public virtual List<Media> Medias { get; set; }
        public virtual List<Test> Tests { get; set; }
    }
}
