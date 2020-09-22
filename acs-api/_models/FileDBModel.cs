using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acs._models
{
    public class FileDBModel
    {
        public LinkModel LiveLink { get; set; }
        public List<LinkModel> RecordingLinks { get; set; }
    }
}
