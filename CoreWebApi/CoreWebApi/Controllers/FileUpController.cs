using CoreWebApi.Dto;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [EnableCors("AllowCors")]
    [Route("api/[controller]/[action]")]//修改路由
    //[Route("api/[controller]")]//默认路由
    [ApiController]
    public class FileUpController : ControllerBase
    {
        public IHostingEnvironment env;
        public FileUpController(IHostingEnvironment _env)
        {
            env = _env;
        }
        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HttpGet,Route("Test")]//默认路由

        public string Test()
        {
            return "这是测试接口";
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<OutPutDto> FileUp()
        {
            var ret = new OutPutDto();
            try
            {
                //不能用FromBody
                var dto = JsonConvert.DeserializeObject<ImagesDto>(Request.Form["ImageModelInfo"]);//文件类实体参数
                var files = Request.Form.Files;//接收上传的文件，可能多个 看前台
                if (files.Count > 0)
                {
                    var path = env.ContentRootPath + @"/Uploads/Images/";//绝对路径
                    string dirPath = Path.Combine(path, dto.Type + "/");//绝对径路 储存文件路径的文件夹
                    if (!Directory.Exists(dirPath))//查看文件夹是否存在
                        Directory.CreateDirectory(dirPath);
                    var file = files.Where(x => true).FirstOrDefault();//只取多文件的一个
                    var fileNam = $"{Guid.NewGuid():N}_{file.FileName}";//新文件名
                    string snPath = $"{dirPath + fileNam}";//储存文件路径
                    using var stream = new FileStream(snPath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    //次出还可以进行数据库操作 保存到数据库
                    ret = new OutPutDto { Code = 200, Msg = "上传成功", Success = true };
                }
                else//没有图片
                {
                    ret = new OutPutDto { Code = 400, Msg = "请上传图片", Success = false };
                }
            }
            catch (Exception ex)
            {
                ret = new OutPutDto { Code = 500, Msg = $"异常：{ex.Message}", Success = false };
            }
            return ret;
        }
    }
}
