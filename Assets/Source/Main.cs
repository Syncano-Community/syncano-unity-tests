using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Syncano;
using Syncano.Data;
using Syncano.Enum;

public class Main : MonoBehaviour {

	public enum TEST_METHOD
	{
		CREATE_DATA_OBJECT, GET_DATA_OBJECT, MODIFY_DATA_OBJECT, DELETE_DATA_OBJECT, GET_LIST_OF_OBJECTS, SCRIPT_ENDPOINT, CREATE_CHANNEL, RECEIVE_FROM_CHANNEL, CREATE_NEW_USER, LOGIN_USER, MODIFY_USER
	}

	public TEST_METHOD Method;

	#region data object variables
	private const string BOOK_AUTHOR = "TestAuthor";
	private const string BOOK_TITLE = "TestTitle";

	private const string BOOK_AUTHOR_MODIFIED = "Author_Modified";
	private const string BOOK_TITLE_MODIFIED = "TestTitle_Modified";

	private SyncanoClient syncano;
	private static Book book1;
	private static Book book2;
	private List<Book> books;
	#endregion data object variables

	#region script endpoint variables
	private const string ENDPOINT_URL = "https://api.syncano.io/v1.1/instances/library-test/endpoints/scripts/p/0a8012f4870d6570a474f01e9e8218e1bb7e766e/library_test/";
	private const string ENDPOINT_EXPECTED_VALUE = "pass";
	#endregion script endpoint variables

	#region channel variables
	private static Channel channel;
	private const string CHANNEL_NAME = "new_channel";
	private const string CHANNEL_DESCRIPTION = "test description";
	private const string CHANNEL_MESSAGE = "test message";
	private const ChannelType CHANNEL_TYPE = ChannelType.DEFAULT;
	#endregion channel variables

	#region user variables
	private static User<TestProfile> user;
	private string USER_NAME = "TEST_USER";
	private const string PASSWORD = "PASSWORD123";
	public Image avatar;
	#endregion user variables

	IEnumerator Start ()
	{
		USER_NAME += Random.Range(0, int.MaxValue);
		syncano = SyncanoClient.Instance.Init("", "");

		yield return new WaitForSeconds(0.5f);

		switch (Method)
		{
			case TEST_METHOD.CREATE_DATA_OBJECT:
				yield return StartCoroutine(CreateNewDataObjectTest());
				break;

			case TEST_METHOD.MODIFY_DATA_OBJECT:
				yield return StartCoroutine(ModifyDataObjectTest());
				break;

			case TEST_METHOD.GET_DATA_OBJECT:
				yield return StartCoroutine(GetDataObjectTest());
				break;

			case TEST_METHOD.GET_LIST_OF_OBJECTS:
				yield return StartCoroutine(GetListOfObjectsTest());
				break;

			case TEST_METHOD.DELETE_DATA_OBJECT:
				yield return StartCoroutine(DeleteDataObjectTest());
				break;

			case TEST_METHOD.SCRIPT_ENDPOINT:
				yield return StartCoroutine(ScriptEndpointTest());
				break;

			case TEST_METHOD.CREATE_CHANNEL:
				yield return StartCoroutine(CreateNewChannelTest());
				break;

			case TEST_METHOD.RECEIVE_FROM_CHANNEL:
				yield return StartCoroutine(ReceiveFromChannelTest());
				break;

			case TEST_METHOD.CREATE_NEW_USER:
				yield return StartCoroutine(CreateNewUserTest());
				break;

			case TEST_METHOD.LOGIN_USER:
				yield return StartCoroutine(LoginUserTest());
				break;

			case TEST_METHOD.MODIFY_USER:
				yield return StartCoroutine(ModifyUser());
				break;

			default:
				throw new UnityException ("Not implemented");
			}
	}

	#region data objects
	private IEnumerator CreateNewDataObjectTest()
	{
		book1 = new Book { Author = BOOK_AUTHOR, Title = BOOK_TITLE, GroupPermissions = DataObjectPermissions.WRITE, OtherPermissions = DataObjectPermissions.WRITE, OwnerPermissions = DataObjectPermissions.WRITE };

		yield return syncano.Please().Save(book1, onSuccess, onFailure);
		yield return new WaitForSeconds(1);

		IntegrationTest.Assert(book1.Author.Equals(BOOK_AUTHOR), "AUTHOR DOES NOT MATCH");
		IntegrationTest.Assert(book1.Title.Equals(BOOK_TITLE), "TITLE DOES NOT MATCH");

		IntegrationTest.Assert(book1.GroupPermissions ==  DataObjectPermissions.WRITE, "GroupPermissions DO NOT MATCH");
		IntegrationTest.Assert(book1.OtherPermissions ==  DataObjectPermissions.WRITE, "OtherPermissions DO NOT MATCH");
		IntegrationTest.Assert(book1.OwnerPermissions ==  DataObjectPermissions.WRITE, "OwnerPermissions DO NOT MATCH");

		IntegrationTest.Pass();
	}

	private void onSuccess(Response<Book> response)
	{
		book1 = response.Data;
	}

	private void onSuccessDelete(Response<Book> response)
	{
		
	}

	private IEnumerator GetDataObjectTest()
	{
		yield return syncano.Please().Get<Book>(book1.Id, onSuccess, onFailure);
		yield return new WaitForSeconds(1);

		IntegrationTest.Assert(book1.Author.Equals(BOOK_AUTHOR), "AUTHOR DOES NOT MATCH");
		IntegrationTest.Assert(book1.Title.Equals(BOOK_TITLE), "TITLE DOES NOT MATCH");

		IntegrationTest.Pass();
	}

	private IEnumerator ModifyDataObjectTest()
	{
		book1.Author = BOOK_AUTHOR_MODIFIED;
		book1.Title = BOOK_TITLE_MODIFIED;

		yield return syncano.Please().Save(book1, onSuccess, onFailure);
		yield return new WaitForSeconds(1);

		IntegrationTest.Assert(book1.Author.Equals(BOOK_AUTHOR_MODIFIED), "AUTHOR MODIFIED DOES NOT MATCH");
		IntegrationTest.Assert(book1.Title.Equals(BOOK_TITLE_MODIFIED), "TITLE MODIFIED DOES NOT MATCH");

		IntegrationTest.Pass();
	}

	private IEnumerator GetListOfObjectsTest()
	{
		book2 = new Book { Author = BOOK_AUTHOR, Title = BOOK_TITLE, GroupPermissions = DataObjectPermissions.WRITE, OtherPermissions = DataObjectPermissions.WRITE, OwnerPermissions = DataObjectPermissions.WRITE };

		yield return syncano.Please().Save(book2, onSuccess2, onFailure);
		yield return new WaitForSeconds(0.5f);

		yield return syncano.Please().Get<Book>(onListSuccess, onFailure);
		yield return new WaitForSeconds(0.5f);

		IntegrationTest.Assert(books.Count == 2, "GET LIST OF BOOKS FAILED");

		IntegrationTest.Pass();
	}

	private void onSuccess2(Response<Book> response)
	{
		book2 = response.Data;
	}

	private void onListSuccess(ResponseGetList<Book> response)
	{
		books = response.Objects;		
	}

	private IEnumerator DeleteDataObjectTest()
	{
		yield return syncano.Please().Delete<Book>(book1, onSuccessDelete, onFailure);
		yield return syncano.Please().Delete<Book>(book2, onSuccessDelete, onFailure);
		yield return new WaitForSeconds(1);

		IntegrationTest.Assert(book1.Author.Equals(BOOK_AUTHOR_MODIFIED), "AUTHOR MODIFIED DOES NOT MATCH");
		IntegrationTest.Assert(book1.Title.Equals(BOOK_TITLE_MODIFIED), "TITLE MODIFIED DOES NOT MATCH");

		IntegrationTest.Pass();
	}

	private void onFailure(Response<Book> response)
	{
		if(response.IsSyncanoError)
		{
			Debug.LogError(response.syncanoError);
		}

		else 
		{
			Debug.LogError(response.webError);
		}

		IntegrationTest.Fail();
	}

	#endregion data objects

	#region script endpoint
	private IEnumerator ScriptEndpointTest()
	{
		yield return syncano.Please().RunScriptEndpointUrl(ENDPOINT_URL, scriptEndpointCallback);
		yield return new WaitForSeconds(0.5f);
	}

	private void scriptEndpointCallback(ScriptEndpoint response)
	{
		if(response.IsSuccess)
		{
			IntegrationTest.Assert(response.stdout.Equals(ENDPOINT_EXPECTED_VALUE));

			IntegrationTest.Pass();
		}

		else
		{
			IntegrationTest.Fail();
		}		
	}
	#endregion script endpoint




	#region channels
	private IEnumerator CreateNewChannelTest()
	{
		channel = new Channel(CHANNEL_NAME);
		channel.ChannelType = ChannelType.DEFAULT;
		channel.CustomPublish = true;
		channel.Description = CHANNEL_DESCRIPTION;

		syncano.Please().CreateChannel(channel, OnChannelCratedSuccess, OnChannelCratedFailure);
		yield return new WaitForSeconds(0.5f);

		IntegrationTest.Assert(channel.Name.Equals(CHANNEL_NAME), "CHANNEL NAME DOES NOT MATCH");
		IntegrationTest.Assert(channel.Description.Equals(CHANNEL_DESCRIPTION), "CHANNEL DESCRIPTION DOES NOT MATCH");
		IntegrationTest.Assert(channel.ChannelType == CHANNEL_TYPE, "CHANNEL TYPE DOES NOT MATCH");
		IntegrationTest.Assert(channel.CustomPublish == true, "CHANNEL CUSTOM PUBLISH DOES NOT MATCH");

		IntegrationTest.Pass();
	}

	private void OnChannelCratedSuccess(Response<Channel> response)
	{
		channel = response.Data;
	}

	private void OnChannelCratedFailure(Response<Channel> response)
	{
		if(response.IsSyncanoError)
		{
			if(response.syncanoError.Contains("This field must be unique."))
			{
				return;
			}


			Debug.LogError(response.syncanoError);
		}

		else 
		{
			Debug.LogError(response.webError);
		}

		IntegrationTest.Fail();
	}

	private string channelMessage;

	private IEnumerator ReceiveFromChannelTest()
	{
		ChannelConnection channelConnection = new ChannelConnection(this, onNotification, onError);
		channelConnection.Start(channel.Name);

		yield return syncano.PublishOnChannel(CHANNEL_NAME, new Notification(CHANNEL_MESSAGE));

		yield return new WaitForSeconds(0.5f);

		IntegrationTest.Assert(channelMessage.Equals(CHANNEL_MESSAGE), "WRONG MESSAGE RECEIVED FROM CHANNEL");
		IntegrationTest.Pass();
	}

	private void onNotification(Response<Notification> response)
	{
		channelMessage = response.Data.Result.Content;
	}

	private void onError(Response<Notification> response)
	{
		if(response.IsSyncanoError)
		{
			Debug.LogError(response.syncanoError);
		}

		else 
		{
			Debug.LogError(response.webError);
		}

		IntegrationTest.Fail();
	}
	#endregion channels

	#region users

	private IEnumerator CreateNewUserTest()
	{
		user = new User<TestProfile>(USER_NAME, PASSWORD);

		yield return user.Register(onUserSuccess, onUserFailure);
		yield return new WaitForSeconds(1);

		IntegrationTest.Assert(user.UserName.Equals(USER_NAME), "USERNAME DOES NOT MATCH");

		IntegrationTest.Pass();
	}

	private void onUserSuccess(Response<User<TestProfile>> response)
	{
		user = response.Data;
	}

	private void onUserFailure(Response<User<TestProfile>> response)
	{
		if(response.IsSyncanoError)
		{
			Debug.LogError(response.syncanoError);
		}

		else 
		{
			Debug.LogError(response.webError);
		}

		IntegrationTest.Fail();
	}

	private IEnumerator LoginUserTest()
	{
		user.Password = PASSWORD;
		yield return user.Login(onUserSuccess, onUserFailure);

		IntegrationTest.Pass();
	}

	private IEnumerator ModifyUser()
	{
		user.Password = PASSWORD;
		Texture2D texture = Resources.Load("avatar_1") as Texture2D;

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		SyncanoFile avatar = new SyncanoFile(sprite.texture.EncodeToPNG());
		user.Profile = new TestProfile(avatar);

		yield return user.UpdateCustomUser(onUserSuccessAvatar, onUserFailure);

		IntegrationTest.Pass();
	}

	private void onUserSuccessAvatar(Response<User<TestProfile>> response)
	{
		StartCoroutine(loadImage(response.Data.Profile.Avatar.Value));
	}

	private IEnumerator loadImage(string url)
	{
		Texture2D texture = new Texture2D(0, 0);

		WWW www = new WWW(url);
		yield return www;
		www.LoadImageIntoTexture(texture);

		Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		avatar.sprite = s;

		yield return null;
	}
	#endregion users
}
