using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Syncano.Data;

public class TestProfile : Profile {

	[JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
	public SyncanoFile Avatar { get; set; }

	public TestProfile() { }

	public TestProfile (SyncanoFile avatar)
	{
		Avatar = avatar;
	}
}