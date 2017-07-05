
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
using Android.Webkit;
using Android.Graphics;

namespace HackAtHomeClient
{
	[Activity(Label = "@string/app_name",Icon = "@drawable/icon")]
	public class DetalleEvidenciaActivity : Activity
	{
		string a, b, c, d, f;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.DetalleEvidencia);

			a = Intent.GetStringExtra("UsrName");
			b = Intent.GetStringExtra("UsrToken");
			c = Intent.GetStringExtra("UsrEvi");
			d = Intent.GetStringExtra("ActivdadNema");
			f = Intent.GetStringExtra("ActivdadStatus");

			TextView txtUsuario = FindViewById<TextView>(Resource.Id.usrTV);
			TextView txtDTV = FindViewById<TextView>(Resource.Id.descripcionTV);
			TextView txtDTV2 = FindViewById<TextView>(Resource.Id.actividade);
			txtUsuario.Text = a;
			txtDTV2.Text = d;
			txtDTV.Text = f;

			DEvidencia();
		}


		public async void DEvidencia()
		{
			Toast.MakeText(this, "Cargando Informacion Detalle", ToastLength.Long).Show();
			EvidenceDetail Result = await GetEvidenceByIDAsync(b,Convert.ToInt32(c));

			WebView myWebV = FindViewById<WebView>(Resource.Id.webView1);

			myWebV.SetBackgroundColor(Color.Transparent);

			string WebViewContent = $"<div style='color:#BCBCBC'>{Result.Description}</div>";

			myWebV.LoadDataWithBaseURL(null, WebViewContent, "text/html", "utf-8", null);


			ImageView myImg = FindViewById<ImageView>(Resource.Id.imageView1);
			Koush.UrlImageViewHelper.SetUrlDrawable(myImg, Result.Url);


			//Toast.MakeText(this, Result.Description, ToastLength.Long).Show();
		}


		/// <summary>
		/// Obtiene el detalle de una evidencia.
		/// </summary>
		/// <param name="token">Token de autenticación del usuario</param>
		/// <param name="evidenceID">Identificador de la evidencia.</param>
		/// <returns>Información de la evidencia.</returns>


		public async Task<EvidenceDetail> GetEvidenceByIDAsync(string token, int evidenceID)
		{
			EvidenceDetail Result = null;
			// Dirección base de la Web API
			string WebAPIBaseAddress = "https://ticapacitacion.com/hackathome/";
			// URI de la evidencia.
			string URI = $"{WebAPIBaseAddress}api/evidence/getevidencebyid?token={token}&&evidenceid={evidenceID}";
			using (var Client = new HttpClient())
			{
				Client.DefaultRequestHeaders.Accept.Clear();
				Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				try
				{
					// Realizamos una petición GET
					var Response = await Client.GetAsync(URI);
					var ResultWebAPI = await Response.Content.ReadAsStringAsync();
					if (Response.StatusCode == HttpStatusCode.OK)
					{
						// Si el estatus de la respuesta HTTP fue exitosa, leemos
						// el valor devuelto.
						Result = JsonConvert.DeserializeObject<EvidenceDetail>(ResultWebAPI);
					}
				}
				catch (System.Exception exc)
				{
					Toast.MakeText(this, exc.Message, ToastLength.Long).Show();
				}
			}
			return Result;
		}

	}
}
