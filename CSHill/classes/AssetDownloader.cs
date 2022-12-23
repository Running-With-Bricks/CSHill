using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security;

public class AssetData
{
    public string mesh;
    public string texture;
    public AssetData(string texture = null,string mesh = null)
    {
        this.mesh = mesh;
        this.texture = texture;
    }
}

public class AssetDownloader
{
    public static string AssetAPIUrl(uint itemid) { return "https://api.brick-hill.com/v1/assets/getPoly/1/" + itemid.ToString(); }
    public static string GetAssetDataUrl(uint assetId) { return "https://api.brick-hill.com/v1/assets/get/" + assetId.ToString(); }

    public static Dictionary<uint, AssetData> Cache = new();

    public static async void FetchAssetUUID(uint assetId)
    {
        HttpResponseMessage response = await Server.client.GetAsync(GetAssetDataUrl(assetId));
        Console.WriteLine(response.RequestMessage.RequestUri.ToString());
    }

    public static async Task<AssetData> GetAssetData(uint itemid)
    {
        if (Cache.ContainsKey(itemid)) return Cache[itemid];

        HttpResponseMessage response = await Server.client.GetAsync(AssetAPIUrl(itemid));
        string stringe = await response.Content.ReadAsStringAsync();
        Console.WriteLine(AssetAPIUrl(itemid));
        _ItemData item = JsonConvert.DeserializeObject<_ItemData[]>(stringe)[0];

        FetchAssetUUID(uint.Parse(item.mesh.Remove(0, 8)));

        Cache[itemid] = new AssetData( item.texture, item.mesh);

        return Cache[itemid];
    }

    private class _ItemData
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("texture")]
        public string texture { get; set; }

        [JsonProperty("texture")]
        public string mesh { get; set; }
    }

}
