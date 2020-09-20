using acs._models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acs._services
{
    public interface ICourseService
    {
        public List<LinkModel> Get();
        public LinkModel Get(int id);
        public int Add(LinkModel model);
        public int Update(LinkModel model);
        public int Delete(int id);
    }
    public class CourseService : ICourseService
    {
        private IFileService _fileService;
        public CourseService(IFileService fileService)
        {
            _fileService = fileService;
        }
        public int Add(LinkModel model)
        {
            var links = JsonConvert.DeserializeObject<List<LinkModel>>(_fileService.GetContent());

            var maxId = 0;
            if (links != null && links.Count > 0)
            {
                maxId = links.Max(x => x.Id);
            }
            else
            {
                links = new List<LinkModel> { };
            }

            model.Id = maxId + 1;
            links.Add(model);

            // Save content to file
            _fileService.SetContent(JsonConvert.SerializeObject(links));

            return maxId + 1;
        }

        public int Delete(int id)
        {
            var links = JsonConvert.DeserializeObject<List<LinkModel>>(_fileService.GetContent());

            var idx = -1;
            if (links != null && links.Count > 0)
            {
                idx = links.FindIndex(x => x.Id == id);
                if (idx >= 0)
                {
                    links.RemoveAt(idx);
                }
            }

            // Save content to file
            _fileService.SetContent(JsonConvert.SerializeObject(links));
            return idx;
        }

        public List<LinkModel> Get()
        {
            return JsonConvert.DeserializeObject<List<LinkModel>>(_fileService.GetContent());
        }

        public LinkModel Get(int id)
        {
            var links = JsonConvert.DeserializeObject<List<LinkModel>>(_fileService.GetContent());

            LinkModel link = null;
            if (links != null && links.Count > 0)
            {
                var idx = links.FindIndex(x => x.Id == id);
                if (idx >= 0)
                {
                    link = links[idx];
                }
            }

            return link;
        }

        public int Update(LinkModel model)
        {
            var links = JsonConvert.DeserializeObject<List<LinkModel>>(_fileService.GetContent());

            if (links != null && links.Count > 0)
            {
                var idx = links.FindIndex(x => x.Id == model.Id);
                if (idx >= 0)
                {
                    links[idx] = model;

                    // Save content to file
                    _fileService.SetContent(JsonConvert.SerializeObject(links));
                    return model.Id;
                }
            }

            return -1; // Add(model);
        }
    }
}
