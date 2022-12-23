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
    public AssetData(string texture = null, string mesh = null)
    {
        this.mesh = mesh;
        this.texture = texture;
    }
}

public class AssetDownloader
{
    public static string AssetAPIUrl(uint itemid) { return "https://api.brick-hill.com/v1/assets/getPoly/1/" + itemid.ToString(); }
    public static string GetAssetDataUrl(string assetId) { return "https://api.brick-hill.com/v1/assets/get/" + assetId; }

    public static Dictionary<uint, AssetData> Cache = new();

    public static async Task<string> FetchAssetUUID(string assetId)
    {
        HttpResponseMessage response = await Server.client.GetAsync(GetAssetDataUrl(assetId));
        return response.RequestMessage.RequestUri.ToString().Remove(0,29);
    }

    public static async Task<AssetData> GetAssetData(uint itemid)
    {
        if (Cache.ContainsKey(itemid)) return Cache[itemid];

        HttpResponseMessage response = await Server.client.GetAsync(AssetAPIUrl(itemid));
        string stringe = await response.Content.ReadAsStringAsync();
        _ItemData[] item = {};
        try
        {
             item = JsonConvert.DeserializeObject<_ItemData[]>(stringe);
        }
        catch (Exception)
        {
            return new AssetData("none","none");
        }

        string texture = await FetchAssetUUID(item[0].texture.Remove(0, 8));
        string mesh = item[0].mesh == null ? "none" : await FetchAssetUUID(item[0].mesh.Remove(0, 8));

        Cache[itemid] = new AssetData(texture,mesh);

        return Cache[itemid];
    }

    private class _ItemData
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("texture")]
        public string texture { get; set; }

        [JsonProperty("mesh")]
        public string mesh { get; set; }
    }

}
