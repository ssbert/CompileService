using RCS.Enums;

namespace RCS.Models
{
    public class CodeModel
    {
        public CodeModel()
        {
            Language = LanguageEnum.Cpp;
            Code = "";
            Param = "";
        }
        /// <summary>
        /// 语言
        /// </summary>
        public LanguageEnum Language { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Param { get; set; }
    }
}