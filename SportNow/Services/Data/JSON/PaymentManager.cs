using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using SportNow.Model;
using System.Collections.ObjectModel;

namespace SportNow.Services.Data.JSON
{
	public class PaymentManager
	{
		//IRestService restService;

		HttpClient client;
		

		public PaymentManager()
		{
			HttpClientHandler clientHandler = new HttpClientHandler();
			clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
			client = new HttpClient(clientHandler);

		}

		/*public async Task<List<Class_Attendance>> Get_Invoice_byID(string invoiceid)
		{
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Invoice_byID + "?invoiceid=" + invoiceid, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);

				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					class_attendances = JsonConvert.DeserializeObject<List<Class_Attendance>>(content);
				}
				return class_attendances;
			}
			catch
			{
				Debug.WriteLine("http request error");
				return null;
			}
		}*/

		public async Task<string> CreateMbWayPayment(string memberid, string paymentid, string orderid, string phonenumber, string value, string email)
		{
			Debug.WriteLine("CreateMbWayPayment begin");
			Debug.WriteLine("paymentid = "+ paymentid);
			Debug.WriteLine("phonenumber = " + phonenumber);
			Debug.WriteLine("value = " + value);
			Debug.WriteLine("email = " + email);
			Debug.WriteLine("orderid = " + orderid);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Create_MbWay_Payment + "?userid=" + memberid + "&paymentid=" + paymentid + "&phonenumber=" + phonenumber + "&value=" + value + "&email=" + email + "&orderid=" + orderid, string.Empty));
			try {
				HttpResponseMessage response = await client.GetAsync(uri);
				string content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("content=" + content);

				if (response.IsSuccessStatusCode)
				{
					//return true;
					//string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					List<Result> createResultList = JsonConvert.DeserializeObject<List<Result>>(content);

					return createResultList[0].result;

				}
				else
				{
					Debug.WriteLine("error creating payment MBWay");
					return "-2";
				}

			}
			catch
			{
				Debug.WriteLine("http request error");
				return "-3";
			}
		}


	}
}