using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DockerApi;

namespace Response
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Extension method to add pagination info to Response headers
        /// </summary>
        /// <param name="response"></param>
        /// <param name="currentPage"></param>
        /// <param name="displayItems"></param>
        /// <param name="resultCount"></param>
        /// <param name="totalItems"></param>
        /// <param name="totalPages"></param>
        public static void AddPagination(this HttpResponse response, int currentPage, int displayItems, int resultCount, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, displayItems, resultCount, totalItems, totalPages);

            response.Headers.Add("Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));
            // CORS
            //response.Headers.Add("access-control-expose-headers", "Pagination");
        }

        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            // CORS
            //response.Headers.Add("access-control-expose-headers", "Application-Error");
        }

        public static OkObjectResult OkObjectResult(this HttpResponse response, CustomResult cusRes)
        {

            if (cusRes == null)
            {
                cusRes = new CustomResult();
            }

            string res = string.Empty;

            try
            {
                res = CommonMethods.SerializeToJSON(cusRes);
            }
            catch (Exception ex)
            {
                cusRes = new CustomResult();
                cusRes.SetException(ex.ToString());
                res = CommonMethods.SerializeToJSON(cusRes);
            }

            return new OkObjectResult(res);
        }

        public static OkObjectResult OkObjectResult(this HttpResponse response, CustomResult_Int64 cusRes)
        {

            if (cusRes == null)
            {
                cusRes = new CustomResult_Int64();
            }

            string res = string.Empty;

            try
            {
                res = CommonMethods.SerializeToJSON(cusRes);
            }
            catch (Exception ex)
            {
                cusRes = new CustomResult_Int64();
                cusRes.SetException(ex.ToString());
                res = CommonMethods.SerializeToJSON(cusRes);
            }

            return new OkObjectResult(res);
        }

        // nguyencuongcs 20181230: dotnet core 2.2 chưa hỗ trợ IHttp2Feature (server push của http2)
        // public static void PushPromise(this HttpResponse response, string path)
        // {
        //     response.PushPromise(path, "GET", null);
        // }
        // public static void PushPromise(this HttpResponse response, string path, string method, IHeaderDictionary headers)
        // {
        //     IHttp2Feature http2Feature = response.HttpContext.Features.Get<IHttp2Feature>();
        //     http2Feature?.PushPromise(path, method, headers);
        // }
    }
}