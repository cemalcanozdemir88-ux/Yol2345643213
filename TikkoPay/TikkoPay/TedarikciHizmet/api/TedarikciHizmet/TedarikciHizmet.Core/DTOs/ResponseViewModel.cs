namespace TedarikciHizmet.Core.DTOs
{
    public class ResponseViewModel<T> where T : class,new()
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Result { get; set; } = new T();
    }

    public class ResponseViewModelPagination<T> : ResponseViewModel<T> where T : class, new()
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
