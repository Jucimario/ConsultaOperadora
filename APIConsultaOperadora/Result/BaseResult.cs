namespace APIConsultaOperadora.Result;

public class BaseResult
{  
    public string? MsgCatch { get; set; }
    public string? MsgError { get; set; }
    public bool ProcessOk { get; set; }
}
