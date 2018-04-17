using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RCS.Enums;
using RCS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RCS.Controllers
{
    [Produces("application/json")]
    [Route("api/Complie")]
    public class ComplieController : Controller
    {
        private readonly AppSettings _config;
        private List<string> _outputList = new List<string>();
        private List<string> _errorList = new List<string>();

        public ComplieController(IOptions<AppSettings> settings)
        {
            _config = settings.Value;
        }

        // POST: api/Complie
        [HttpPost]
        public ResultModel<string> Post(CodeModel obj)
        {
            _outputList = new List<string>();
            _errorList = new List<string>();

            var result = new ResultModel<string>();

            var name = GetRandomString();
            var fileName = "myapp.c";
            var path = Path.Combine(Environment.CurrentDirectory, "temp", name);
            var dockerPath = "/home/user/temp";
            var compiler = "gcc";
            
            switch (obj.Language)
            {
                case LanguageEnum.C:
                    compiler = "gcc";
                    fileName = "myapp.c";
                    break;
                case LanguageEnum.Cpp:
                    compiler = "g++";
                    fileName = "myapp.cpp";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(obj.Param))
            {
                obj.Param = obj.Param.Trim();
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            System.IO.File.WriteAllText($"{path}/{fileName}", obj.Code, System.Text.Encoding.UTF8);

            var arguments = $"run --rm --name {name} -v {path}:{dockerPath} {_config.DockerImageName} /bin/bash /home/user/build.sh {compiler} {dockerPath} {fileName} {obj.Param}";
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "docker",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            bool isExit = false;
            try
            {
                p.ErrorDataReceived += P_ErrorDataReceived;
                p.OutputDataReceived += P_OutputDataReceived;

                p.Start();

                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

            }
            catch (Exception e)
            {
                result.Code = 1;
                result.Msg = e.StackTrace;
            }
            finally
            {
                if (!p.HasExited)
                {
                    isExit = p.WaitForExit(_config.DockerTimeout);
                }

                p.Close();
                p.Dispose();
            }

            if (isExit)
            {
                if (_errorList.Any())
                {
                    result.Code = 2;//有错误
                    result.Msg = string.Join("\n", _errorList);
                }
                else
                {
                    result.Result = string.Join("\n", _outputList);
                }
            }
            else
            {
                result.Code = 3;//超时
                result.Msg = "程序运行超时";
                try
                {
                    StopContainer(name);
                }
                catch (Exception e)
                {
                    result.Msg += "\n\n" + e.StackTrace;
                }

            }
            //删除创建的临时文件目录
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            return result;
        }

        private void StopContainer(string name)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "stop.sh");
            var p = new Process
            {
                StartInfo =
                {
                    FileName = "bash",
                    Arguments = $"{filePath} {name}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            try
            {
                p.Start();

                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

            }
            finally
            {
                if (!p.HasExited)
                {
                    p.WaitForExit();
                }

                p.Close();
                p.Dispose();
            }
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }
            _errorList.Add(e.Data);
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }
            _outputList.Add(e.Data);
        }

        private static string GetRandomString(int length = 8, bool useNum = true, bool useLow = true, bool useUpp = false, bool useSpe = false, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum) { str += "0123456789"; }
            if (useLow) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}
