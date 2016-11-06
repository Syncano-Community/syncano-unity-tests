using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Syncano;

public class Book : SyncanoObject{

	[JsonProperty("title")]
	public string Title { get; set; }

	[JsonProperty("author")]
	public string Author { get; set; }
}
