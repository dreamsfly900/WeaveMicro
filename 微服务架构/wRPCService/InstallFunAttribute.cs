﻿using System;
using System.Collections.Generic;
using System.Text;

namespace wRPC
{
   
    public enum FunAttribute { NONE,Get,POST,file }
    public class InstallFunAttribute : System.Attribute
    {
        /// <summary>
        /// 标识这个方法是执行一次即卸载，还是长期执行
        /// </summary>
        /// <param name="type">forever,或noce</param>
        public InstallFunAttribute(FunAttribute type= FunAttribute.NONE)
        {
            Type = type;
        }

        public InstallFunAttribute(FunAttribute type= FunAttribute.NONE, String annotation="",string contentType="")
        {
            Type = type;
            Annotation = annotation;
            ContentType= contentType;
        }

        public FunAttribute Type { get; set; }

        public String Annotation { get; set; }
        public String ContentType { get; set; }
    }
    public class SteamAttribute : System.Attribute
    { }
    public class SliceAttribute : System.Attribute
    { }
    public class AuthorizeAttribute : System.Attribute
    { }
    public class ParamAttribute : System.Attribute
    {
        public String explain { get; set; }
        public ParamAttribute(String explain)
        {
            this.explain = explain;
        }
    }
    public class RouteAttribute : System.Attribute
    {
        /// <summary>
        /// 标识这个方法是执行一次即卸载，还是长期执行
        /// </summary>
        /// <param name="type">forever,或noce</param>
        public RouteAttribute(String route)
        {
            Route = route;
        }

       

        public String Route { get; set; }

      
    }
    
}
