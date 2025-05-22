using System.Text;

namespace sigma_music;

public class Results
{
    public string CallFunc { get; init; }
    public IEnumerable<object?> CallParam { get; init; }
    public List<string> Messages { get; }
    public List<object>? ReturnValues { get; private set; }
    public bool Result { get; private set; }
    public Results? NextResult { get; private set; }
    
    public Results(string callFunc, IEnumerable<object>? callParam, List<string> messages, Results nextResult, List<object>? returnValues, bool result)
    {
        CallFunc = callFunc;
        CallParam = callParam??[];
        Messages = messages;
        NextResult = nextResult;
        ReturnValues = returnValues;
        Result = result;
    }

    public Results(string callFunc, IEnumerable<object?> callParam)
    {
        CallFunc = callFunc;
        CallParam = callParam??[];
        Messages = [];
        NextResult = null;
        ReturnValues = [];
        Result = false;
    }

    public void AddMessage(string message)
    {
        Messages?.Add(message);
    }

    public void SetReturnValue(List<object> returnValue)
    {
        ReturnValues = returnValue;
    }

    public void AddReturnValue(object returnValue)
    {
        ReturnValues?.Add(returnValue);
    }
    
    public void AddReturnValue(IEnumerable<object> returnValue)
    {
        foreach (var item in returnValue)
        {
            AddReturnValue(item);
        }
    }

    public void SetResult(bool result)
    {
        Result = result;
    }

    public void SetChildrenResult(Results nextResult)
    {
        NextResult = nextResult;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Function Call: {CallFunc}");
        sb.AppendLine($"Parameters: {string.Join(", ", CallParam)}");
        sb.AppendLine($"Result: {(Result ? "Success" : "Failure")}");
    
        if (Messages.Count > 0)
        {
            sb.AppendLine("Messages:");
            sb.AppendLine(string.Join("\n", Messages));
        }
    
        if (ReturnValues is { Count: > 0 })
        {
            sb.AppendLine("Return Values:");
            sb.AppendLine(string.Join(", ", ReturnValues));
        }

        if (NextResult == null) return sb.ToString();
        sb.AppendLine("Next Result:");
        sb.AppendLine(NextResult.ToString());
        return sb.ToString();
    }

}