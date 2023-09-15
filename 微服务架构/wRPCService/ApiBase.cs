
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wRPC;

namespace wRPC.DataLogic
{
    /// <summary>
    /// Api接口基类
    /// </summary>
    /// <typeparam name="T">模型对象类型，必须是数据库一致的类型</typeparam>
    public class ApiBase<T> : FunctionBase where T : class, new()
    {
        

        /// <summary>
        /// TryCatch封装，成功返回200，操作失败（ApiException）返回0，异常返回500.
        /// </summary>
        /// <typeparam name="TModel">返回结果类型</typeparam>
        /// <param name="fun">需要执行的方法体</param>
        /// <returns>返回接口调用结果</returns>
        protected async Task<ApiResult<TModel>> TRY<TModel>(Func<Task<TModel>> fun)
        {
            var o = new ApiResult<TModel>() { code = 200, message = "操作成功" };
            try
            {
                if (fun != null) o.data = await fun();
                return o;
            }
            //异步改同步在这里判断异常类型
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is ApiException)
                    {
                        o.code = 0;
                        o.message = $"操作失败：{x.Message}";
                        o.errData = (x as ApiException).data;
                    }
                    else
                    {
                        o.code = 500;
                        o.message = $"接口调用异常：{x.Message}";
                    }
                    return true;
                });
                return o;
            }
            catch (ApiException e)
            {
                o.code = 0;
                o.message = $"操作失败：{e.Message}";
                o.errData = e.data;
                return o;
            }
            catch (Exception e)
            {
                o.code = 500;
                o.message = $"接口调用异常：{e.Message}";
                return o;
            }
        }

        /// <summary>
        /// 检查模型数据有效性
        /// </summary>
        /// <param name="t">模型对象</param>
        /// <exception cref="ApiException">数据无效</exception>
        protected virtual void CheckModel(T t)
        {
            //if (!decimal.TryParse(t.Areacode, out decimal _)) throw new ApiException("区域编码格式不正确！", false);
        }

        /// <summary>
        /// 检查模型数据有效性
        /// </summary>
        /// <param name="list">模型对象列表</param>
        /// <exception cref="ApiException">数据无效</exception>
        protected void CheckModel(List<T> list)
        {
            foreach (var item in list)
            {
                CheckModel(item);
            }
        }
    }
    /// <summary>
    /// Api返回结果
    /// </summary>
  
    /// <summary>
    /// Api异常（操作失败，状态码0）
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// 操作失败的相关数据
        /// </summary>
        public dynamic data { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">操作失败状态描述</param>
        /// <param name="data">操作失败的相关数据</param>
        public ApiException(string message, dynamic data = null) : base(message) => this.data = data;
    }
}