using System;
using System.Collections.Generic;
using System.Text;

namespace wRPC
{
    public enum FunAttribute { Get,POST,RPC,ALL }
    public class InstallFunAttribute : System.Attribute
    {
        /// <summary>
        /// 标识这个方法是执行一次即卸载，还是长期执行
        /// </summary>
        /// <param name="type">forever,或noce</param>
        public InstallFunAttribute(FunAttribute type)
        {
            Type = type;
        }

        public InstallFunAttribute(FunAttribute type, String annotation)
        {
            Type = type;
            Annotation = annotation;
        }

        public FunAttribute Type { get; set; }

        public String Annotation { get; set; } 
    }
}
