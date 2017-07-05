using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HackAtHome.Entities;
using Android;
using Android.Content;
using Android.Runtime;

using HackAtHome.SAL;


namespace HackAtHomeClient
{
	[Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		string sEmail;
		string sPassword;
		//public string nombreUsr;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			FindViewById<Button>(Resource.Id.button1).Click += Validar;
				
		}


		public async void Validar(object sender, EventArgs e)
		{
			try
			{
				sEmail = FindViewById<EditText>(Resource.Id.mycorreo).Text;
				sPassword = FindViewById<EditText>(Resource.Id.mypass).Text;

				if (string.IsNullOrEmpty(sEmail) || string.IsNullOrEmpty(sPassword))
				{
					Toast.MakeText(this, "Por favor introduce un email y password válido", ToastLength.Short).Show(); 
				}
				else
				{
					ResultInfo MyRes=await AutenticateAsync(sEmail,sPassword);
					if (string.IsNullOrEmpty(MyRes.Token))
					{
						Toast.MakeText(this, "Error: el usuario no existe", ToastLength.Long).Show();
					}
					else
					{
						//Toast.MakeText(this, "Cas  " + MyRes.Token, ToastLength.Long).Show();



						//ENVIAR REGISTRO A MICROSOFT
						/*
						string myDevice = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
						//Toast.MakeText(this, "Enviando Registro", ToastLength.Long).Show();
						var MicrosoftEvidence = new LabItem
						{
							Email = sEmail,
							Lab = "Hack@Home",
							DeviceId = myDevice
						};
						var MicrosoftClient = new MicrosoftServiceClient();
						await MicrosoftClient.SendEvidence(MicrosoftEvidence);
						*/



						//Toast.MakeText(this, "Resueltado listo", ToastLength.Long).Show();

						var intent = new Intent(this, typeof(EvidenciasActivity));
						intent.PutExtra("UsrName", MyRes.FullName);
						intent.PutExtra("UsrToken", MyRes.Token);
						//nombreUsr = MyRes.FullName;
                        StartActivityForResult(intent, 1);
					}
				}



			}
			catch(Exception exc)
			{
				Toast.MakeText(this, exc.Message, ToastLength.Long).Show();
			}
			
			
		}


		/// <summary>
		/// Realiza la autenticación al servicio Web API.
		/// </summary>
		/// <param name="studentEmail">Correo del usuario</param>
		/// <param name="studentPassword">Password del usuario</param>
		/// <returns>Objeto ResultInfo con los datos del usuario y un token de autenticación.</returns>
		public async Task<ResultInfo> AutenticateAsync(string studentEmail, string studentPassword)
		{
			ResultInfo Result = null;
			// Dirección base de la Web API
			string WebAPIBaseAddress = "https://ticapacitacion.com/hackathome/";
			// ID del diplomado.
			string EventID = "xamarin30";
			string RequestUri = "api/evidence/Authenticate";
			// El servicio requiere un objeto UserInfo con los datos del usuario y evento.
			UserInfo User = new UserInfo
			{
				Email = studentEmail,
				Password = studentPassword,
				EventID = EventID
			};
			// Utilizamos el objeto System.Net.Http.HttpClient para consumir el servicio REST
			// Debe instalarse el paquete NuGet System.Net.Http
			using (var Client = new HttpClient())
			{
				// Establecemos la dirección base del servicio REST
				Client.BaseAddress = new Uri(WebAPIBaseAddress);
				// Limpiamos encabezados de la petición.
				Client.DefaultRequestHeaders.Accept.Clear();
				// Indicamos al servicio que envie los datos en formato JSON.
				Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				try
				{
					// Serializamos a formato JSON el objeto a enviar.
					// Debe instalarse el paquete NuGet Newtonsoft.Json.
					var JSONUserInfo = JsonConvert.SerializeObject(User);
					// Hacemos una petición POST al servicio enviando el objeto JSON
					HttpResponseMessage Response = await Client.PostAsync(RequestUri,new StringContent(JSONUserInfo.ToString(),Encoding.UTF8, "application/json"));
					// Leemos el resultado devuelto.	
					var ResultWebAPI = await Response.Content.ReadAsStringAsync();	
					// Deserializamos e resultado JSON obtenido
					Result = JsonConvert.DeserializeObject<ResultInfo>(ResultWebAPI);
	        	}
		        catch (System.Exception exc)
		        {
		            // Aquí podemos poner el código para manejo de excepcios
					Toast.MakeText(this, exc.Message.ToString(), ToastLength.Short).Show();
				} 
			}
			//Toast.MakeText(this, Result.Status.ToString(), ToastLength.Short).Show();
			//Toast.MakeText(this, Result.Token, ToastLength.Short).Show();
	    	return Result;
		}



	}



}

