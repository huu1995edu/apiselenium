using System;
using System.Collections;
using System.Collections.Generic;
using DockerApi;

namespace Response
{
    public class CustomResult_Int64
    {
        private long _intResult = 1;      // -1: Het session, 0: Không có dữ liệu, 1: OK, -2: Lỗi
        private string _strResult = string.Empty;
        private IEnumerable _dataResult;
        private string _message = string.Empty;
        private string _gotoLink = string.Empty;
        private PaginationHeader _pagination = new PaginationHeader();

        public CustomResult_Int64()
        {

        }

        public long IntResult
        {
            get { return this._intResult; }
            set { this._intResult = value; }
        }
        public string StrResult
        {
            get { return this._strResult; }
            set { this._strResult = value; }
        }
        public IEnumerable DataResult
        {
            get { return this._dataResult; }
            set
            {
                this._dataResult = value;
                this._intResult = this._dataResult != null ? 1 : 0;
            }
        }
        public PaginationHeader Pagination
        {
            get { return this._pagination; }
            //set { this._pagination = value; }
        }
        public string Message
        {
            get { return this._message; }
            set { this._message = value; this._intResult = 0; }
        }
        public string GotoLink
        {
            get { return this._gotoLink; }
            set { this._gotoLink = value; }
        }
        public void SetMessageLogout()
        {
            this._intResult = -1;
            this._message = Variables.MessageSessionTimeOut;
        }
        public void SetMessageInvalidRole()
        {
            this._intResult = -1;
            this._message = Variables.MessageSessionInvalidRole;
        }
        public void SetException(Exception ex)
        {
            this._intResult = -2;
            this._message = ex.Message;
        }
        public void SetException(string msg)
        {
            this._intResult = -2;
            this._message = msg;
        }
        public void AddPagination(int currentPage, int displayItems, int resultCount, long totalItems)
        {
            var totalPages = (long)Math.Round((double)totalItems / displayItems, 0, MidpointRounding.AwayFromZero);

            this.Pagination.CurrentPage = currentPage;
            this.Pagination.DisplayItems = displayItems;
            this.Pagination.ResultCount = resultCount;
            this.Pagination.TotalItems = totalItems;

            if (totalPages < 1) { totalPages = 1; }

            this.Pagination.TotalPages = totalPages;
        }
    }
}