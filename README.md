# TourneyManagerApi
Api library for C# to use api of TourneyManager.

### Step 1
Initialize Api
```
 ApiManager.Init("My client id", "My secret", "My scope");
```

### Step 2
Prepare token
```
 var task = ApiManager.PrepareToken(b => {
                Debug.WriteLine($"Token attempt: {b} Token: {ApiManager.Token.AccessToken}");
            });
 task.RunSynchronously();
```

### Step 3.5
Make your models
```
[JsonObject(MemberSerialization.OptIn)]
public class Beatmap
{
    [JsonProperty(PropertyName = "id")]
    public int ID { get; private set; }
    [JsonProperty(PropertyName = "beatmapset_id")]
    public int SetId { get; private set; }
    [JsonProperty(PropertyName = "artist")]
    public string Artist { get; private set; }
    [JsonProperty(PropertyName = "title")]
    public string Title { get; private set; }
    [JsonProperty(PropertyName = "version")]
    public string Version { get; private set; }

    public Beatmap() {}

    public override string ToString() {
        return $"{SetId}/{ID} {Artist} - {Title} [{Version}]";
    }
}
```

### Step 3
Run your requests
```
var resp = ApiManager.ApiCallGet<List<Beatmap>>("http://tournament-manager.ml/api/beatmaps?page=1&difficultyrating_gt=4.5&diff_size_gte=5&mode=0&approved_gte=1");
foreach (var beatmap in resp.Data) {
    Debug.WriteLine(beatmap.ToString());
}
```
