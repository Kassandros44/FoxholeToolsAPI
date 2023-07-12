
namespace FoxholeToolsAPI.Models;

public class CrateModel {

    public string name = string.Empty;
    public int amount = 0;
    public int quota = 0;

    public CrateModel(JObject jobject){
        if(jobject.ContainsKey("name")){
            name = (string)jobject["name"]!;
        }
        if(jobject.ContainsKey("amount")){
            amount = (int)jobject["amount"]!;
        }
        if(jobject.ContainsKey("quota")){
            quota = (int)jobject["quota"]!;
        }
    }

}