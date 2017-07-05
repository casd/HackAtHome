
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HackAtHome.Entities;
using Android;
using HackAtHome.CustomAdapters;




namespace HackAtHomeClient
{
	[Activity(Label = "@string/app_name", Icon = "@drawable/icon")]
	public class EvidenciasActivity : Activity
	{
		Complex Data;
		string usrt, usrn;
		string result = null;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Evidencias);

			usrn = Intent.GetStringExtra("UsrName");
			usrt = Intent.GetStringExtra("UsrToken");


			TextView txtUsuario = FindViewById<TextView>(Resource.Id.textView1);
			txtUsuario.Text = usrn;

			ListarEvidencias();


			/*

			Data = (Complex)this.FragmentManager.FindFragmentByTag("Data");
			//Toast.MakeText(this, Data.ToString(), ToastLength.Short).Show();
			if (Data == null)
			{
				//No ha sido almacenado, agregar el fragmento a la Activity
				Data = new Complex();
				var FTo = this.FragmentManager.BeginTransaction();
                ListarEvidencias();
				//Toast.MakeText(this, Data.ToString(), ToastLength.Short).Show();
				FTo.Add(Data, "Data");
				FTo.Commit();
			}

			if (savedInstanceState != null)
			{
				result= savedInstanceState.GetString("ResultadoEV", "");
				//Android.Util.Log.Debug("Lab11Log", "Activity A - Recovered Instance State");
			}
			if (result==null)
			{
				//Validate();                
				//ListarEvidencias();
				//Toast.MakeText(this, "VALIDANDO", ToastLength.Short).Show();
			}
			//si lo dejo aqui no va a hacer la persistencia de datos 
            //ListarEvidencias();
            */



		}


		protected override void OnSaveInstanceState(Bundle outState)
		{
			//Toast.MakeText(this, "Valor Guardado", ToastLength.Short).Show();
			//outState.PutInt("CounterValue", Counter);

			outState.PutString("ResultadoEV", result);
			base.OnSaveInstanceState(outState);
		}

		public async void ListarEvidencias()
		{
			var dataEvidence = (Complex)this.FragmentManager.FindFragmentByTag("Data");

			result = "as";

			var ListActividades = FindViewById<ListView>(Resource.Id.listView1);

			List<Evidence> Evidences;

			if (dataEvidence == null)
			{
				Toast.MakeText(this, "Cargando Evidencias", ToastLength.Long).Show();
				dataEvidence = new Complex();
				var FTo = this.FragmentManager.BeginTransaction();
				FTo.Add(dataEvidence, "Data");
				FTo.Commit();
				Evidences = await GetEvidencesAsync(usrt);
				dataEvidence.ListOfEvidences = Evidences;
			}
			else
			{
				Evidences = dataEvidence.ListOfEvidences;
			}




			//Toast.MakeText(this, "Cargando Evidencias", ToastLength.Long).Show();
			//List<Evidence> Evidences = await GetEvidencesAsync(usrt);

			var EvidenceAdapter = new EvidencesAdapter(
				this, Evidences, Resource.Layout.EvidenciasItem,
				Resource.Id.tevidencia, Resource.Id.eevidencia);




			ListActividades.Adapter = EvidenceAdapter;

			ListActividades.ItemClick += (s, ev) => {
				
				//long xss = new EvidencesAdapter(this, Evidences, Resource.Layout.EvidenciasItem, Resource.Id.tevidencia, Resource.Id.eevidencia).GetItemId(ev.Position);

				var MyEvidence = EvidenceAdapter[ev.Position];

				string abc = FindViewById<TextView>(Resource.Id.tevidencia).Text;

				var intent = new Intent(this, typeof(DetalleEvidenciaActivity));
				intent.PutExtra("UsrName", usrn);
				intent.PutExtra("UsrToken", usrt);
				intent.PutExtra("ActivdadNema", MyEvidence.Title);
				intent.PutExtra("ActivdadStatus", MyEvidence.Status);
				intent.PutExtra("UsrEvi", MyEvidence.EvidenceID.ToString());
	      		StartActivityForResult(intent, 1);
	        };
		}





		/// <summary>
		/// Obtiene la lista de evidencias.
		/// </summary>
		/// <param name="token">Token de autenticación del usuario.</param>
		/// <returns>Una lista con las evidencias.</returns>

		public async Task<List<Evidence>> GetEvidencesAsync(string token)
		{
			List<Evidence> Evidences = null;
			// Dirección base de la Web API
			string WebAPIBaseAddress = "https://ticapacitacion.com/hackathome/";
			// URI completo
			string URI = $"{WebAPIBaseAddress}api/evidence/getevidences?token={token}";
			using (var Client = new HttpClient())
			{
				Client.DefaultRequestHeaders.Accept.Clear();
				Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				try
				{
					// Realizamos una petición GET
					var Response = await Client.GetAsync(URI);
					if (Response.StatusCode == HttpStatusCode.OK)
					{
						// Si el estatus de la respuesta HTTP fue exitosa, leemos el valor devuelto.
						var ResultWebAPI = await Response.Content.ReadAsStringAsync();
						Evidences = JsonConvert.DeserializeObject<List<Evidence>>(ResultWebAPI);
					}
				}
				catch (Exception exc)
				{
					Toast.MakeText(this, exc.Message, ToastLength.Long).Show();
				}
			}
			return Evidences;
		}

	}
}
