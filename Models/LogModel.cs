
namespace FoxholeToolsAPI.Models;

public class LogModel {

    public string username = string.Empty;
    public string timestamp = string.Empty;
    public string action = string.Empty;
    public string item = string.Empty;
    public string amount = string.Empty;

    public LogModel(JObject jobject){
        if(jobject.ContainsKey("username")){
            username = (string)jobject["username"]!;
        }
        if(jobject.ContainsKey("timestamp")){
            timestamp = (string)jobject["timestamp"]!;
        }
        if(jobject.ContainsKey("action")){
            action = (string)jobject["action"]!;
        }
        if(jobject.ContainsKey("item")){
            item = (string)jobject["item"]!;
        }
        if(jobject.ContainsKey("amount")){
            amount = (string)jobject["amount"]!;
        }
    }

}