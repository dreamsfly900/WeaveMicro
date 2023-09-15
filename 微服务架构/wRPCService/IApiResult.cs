namespace wRPC
{
    /// <summary>
    /// API返回类型
    /// </summary>
    public interface IApiResult { }
    public class ApiResult<T> : IApiResult
    {
        /// <summary>
        /// 状态码：200成功500异常0操作失败
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 异常数据
        /// </summary>
        public dynamic errData { get; set; }
    }
}
