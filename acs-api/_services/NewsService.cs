using acs._models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acs._services
{
    public interface INewsService
    {
        public List<LinkModel> Get();
        public LinkModel Get(int id);
        public int Add(LinkModel model);
        public int Update(LinkModel model);
        public int Delete(int id);
    }
    public class NewsService : INewsService
    {
        private IFileService _fileService;
        public NewsService(IFileService fileService)
        {
            _fileService = fileService;
        }
        public int Add(LinkModel model)
        {
            var db = JsonConvert.DeserializeObject<FileDBModel>(_fileService.GetContent());
            var links = db?.NewsLinks;

            var maxId = 0;
            if (links != null && links.Count > 0)
            {
                maxId = links.Max(x => x.Id);
            }
            else
            {
                links = new List<LinkModel> { };
            }
            
            model.LastEdit = DateTime.Now;
            model.Id = maxId + 1;
            links.Add(model);

            // Save content to file
            db = db ?? new FileDBModel();
            db.NewsLinks = links;
            _fileService.SetContent(JsonConvert.SerializeObject(db));

            return maxId + 1;
        }

        public int Delete(int id)
        {
            var db = JsonConvert.DeserializeObject<FileDBModel>(_fileService.GetContent());
            var links = db?.NewsLinks;

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
            db = db ?? new FileDBModel();
            db.NewsLinks = links;
            _fileService.SetContent(JsonConvert.SerializeObject(db));
            return idx;
        }

        public List<LinkModel> Get()
        {
            return JsonConvert.DeserializeObject<FileDBModel>(_fileService.GetContent())?.NewsLinks;
        }

        public LinkModel Get(int id)
        {
            var db = JsonConvert.DeserializeObject<FileDBModel>(_fileService.GetContent());
            var links = db?.NewsLinks;

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
            var db = JsonConvert.DeserializeObject<FileDBModel>(_fileService.GetContent());
            var links = db?.NewsLinks;

            if (links != null && links.Count > 0)
            {
                var idx = links.FindIndex(x => x.Id == model.Id);
                if (idx >= 0)
                {
                    model.LastEdit = DateTime.Now;
                    links[idx] = model;

                    // Save content to file
                    db = db ?? new FileDBModel();
                    db.NewsLinks = links;
                    _fileService.SetContent(JsonConvert.SerializeObject(db));
                    return model.Id;
                }
            }

            return -1; 
        }
    }
}
