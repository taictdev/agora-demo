public interface IResponseData
{
}

/// <summary>
/// The most simple data structure from api response
/// </summary>
public class BasicData<T> : IResponseData
{
    public int status;
    public string message;
    public T data;
}

public class DumpData : IResponseData
{
}