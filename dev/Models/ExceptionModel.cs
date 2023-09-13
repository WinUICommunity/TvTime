namespace TvTime.Models;
public class ExceptionModel
{
    public Exception Exception { get; set; }
    public string Name { get; set; }
    public string Server { get; set; }

    public ExceptionModel(Exception exception, string name, string server)
    {
        this.Exception = exception;
        this.Name = name;
        this.Server = server;
    }
}
