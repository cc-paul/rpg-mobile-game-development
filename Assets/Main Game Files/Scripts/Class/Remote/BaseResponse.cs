using System;
using System.Collections.Generic;

[Serializable]
public class BaseResponseData<T> {
    public int rows_returned;
    public List<T> records;
}

[Serializable]
public class BaseResponse<T> {
    public int statusCode;
    public bool success;
    public List<string> messages;
    public T data;
}

