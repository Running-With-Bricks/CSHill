using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

public class AssetData
{
    public string mesh;
    public string texture;
    public AssetData(string mesh, string texture)
    {
        this.mesh = mesh;
        this.texture = texture;
    }
}

public class AssetDownloader
{
    private static HttpClient client = new();
    public static string AssetAPIUrl(uint assetId) { return "https://api.brick-hill.com/v1/assets/getPoly/1/" + assetId.ToString(); }
    public static string GetAssetDataUrl(uint assetId) { return "https://api.brick-hill.com/v1/assets/get/" + assetId.ToString(); }

    public static Dictionary<uint, AssetData> Cache = new();
    
    public static void FetchAssetUUID(string type, uint assetId)
    {
        //https request stuff
        //UNFINISHED
    }

    public static async Task<AssetData> GetAssetData(uint assetId)
    {
        if (Cache.ContainsKey(assetId)) return Cache[assetId];

        //AssetData assetData = new();

        HttpResponseMessage response = await client.GetAsync(GetAssetDataUrl(assetId));
        Console.WriteLine(response.Content);
        return new AssetData("piisss", "shiiit");
    }
}
