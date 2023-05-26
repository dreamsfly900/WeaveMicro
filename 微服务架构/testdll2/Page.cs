using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// T_Page:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class T_Page
    {
        public T_Page()
        { }
        #region Model
        private int _id ;
        private string _pageName;
        private string _width;
        private string _height;
        private string _config;
        private string _createtime;
        private string _userId;
        private string _image;

        public int Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName
        {
            set { _pageName = value; }
            get { return _pageName; }
        }
        /// <summary>
        /// 页面分辨率 1024*768
        /// </summary>
        public string Width
        {
            set { _width = value; }
            get { return _width; }
        }
        public string Height
        {
            set { _height = value; }
            get { return _height; }
        }
        /// <summary>
        /// 页面配置
        /// </summary>
        public string Config
        {
            set { _config = value; }
            get { return _config; }
        }
        /// <summary>
        ///  
        /// </summary>
        public string Createtime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        ///  
        /// </summary>
        public string UserId
        {
            set { _userId = value; }
            get { return _userId; }
        }
        /// <summary>
        /// 推送频率（分钟）
        /// </summary>
        public string Pushrate
        {
            get
            {
                return _pushrate;
            }

            set
            {
                _pushrate = value;
            }
        }

        public string Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
            }
        }

        private string _pushrate;
        #endregion
    }
}
