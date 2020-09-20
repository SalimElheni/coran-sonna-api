using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using acs._models;
using acs._services;
using ACS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace acs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private ICourseService _courseService { get; set; }
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [AllowAnonymous]
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var courses = _courseService.Get();

            return Ok(ResponseModel<List<LinkModel>>.SuccessResponse(courses));
        }
        [AllowAnonymous]
        [HttpGet("get")]
        public IActionResult Get(int id)
        {
            var courses = _courseService.Get(id);

            return Ok(ResponseModel<LinkModel>.SuccessResponse(courses));
        }
        [AllowAnonymous]
        [HttpPost("add")]
        public IActionResult Add(LinkModel model)
        {
            var courses = _courseService.Add(model);

            return Ok(ResponseModel<int>.SuccessResponse(courses));
        }
        [AllowAnonymous]
        [HttpPost("update")]
        public IActionResult Update(LinkModel model)
        {
            var courses = _courseService.Update(model);

            return Ok(ResponseModel<int>.SuccessResponse(courses));
        }
        [AllowAnonymous]
        [HttpGet("delete")]
        public IActionResult Delete(int id)
        {
            var courses = _courseService.Delete(id);

            return Ok(ResponseModel<int>.SuccessResponse(courses));
        }
    }
}
