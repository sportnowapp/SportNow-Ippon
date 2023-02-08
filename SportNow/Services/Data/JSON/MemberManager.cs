using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using SportNow.Model;
using SportNow.Views;
using System.Collections.ObjectModel;

namespace SportNow.Services.Data.JSON
{
	public class MemberManager
	{
		//IRestService restService;

		HttpClient client;

		public List<Member> members;
		public ObservableCollection<Member> members_observable;

		public List<Payment> payments { get; private set; }


		public MemberManager()
		{
			HttpClientHandler clientHandler = new HttpClientHandler();
			clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
			client = new HttpClient(clientHandler);
		}

		public async Task<int> updateToken(string memberid, string token)
		{
			Debug.WriteLine("MemberManager.updateToken "+ memberid+" "+ token);
			if ((memberid == null) | (token == null) | (memberid == "") | (token == ""))
			{
				Debug.WriteLine("MemberManager.updateToken memberid or token empty");
				return -4;
			}
			Uri uri = new Uri(string.Format(Constants.RestUrl_Update_Token + "?userid=" + memberid + "&token=" + token, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					List<Login> loginResultList = JsonConvert.DeserializeObject<List<Login>>(content);
					return loginResultList[0].login;
				}
				else
				{
					Debug.WriteLine("login not ok");
					return -2;
				}
			}
			catch
			{
				Debug.WriteLine("http request error");
				return -3;
			}
		}


		/*public async Task<string> Login(User user)
		{
			var return_value = "0";
			Debug.WriteLine("Login");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Login + "?username=" + user.Username + "&password=" + user.Password, string.Empty));
			try
			{

				return_value = "-3";
				HttpResponseMessage response = await client.GetAsync(uri);
				return_value = "-4";
				if (response.IsSuccessStatusCode)
				{
					//return true;
					return_value = "-5";
					string content = await response.Content.ReadAsStringAsync();
					return_value = content;
					Debug.WriteLine("content=" + content);
					List<Login> loginResultList= JsonConvert.DeserializeObject<List<Login>>(content);
					Debug.WriteLine("loginResultList=" + loginResultList[0].login);
					return_value = "-7";
					return "1";
				}
				else
				{
					Debug.WriteLine("login not ok");
					return "-2";
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("http request error "+ e.ToString());
				return return_value+ e.ToString();
			}
		}*/

		public async Task<string> Login(User user)
		{
			Debug.WriteLine("Login "+ Constants.RestUrl_Login + "?username=" + user.Username + "&password=" + user.Password);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Login + "?username=" + user.Username + "&password=" + user.Password, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					List<Login> loginResultList = JsonConvert.DeserializeObject<List<Login>>(content);
					return Convert.ToString(loginResultList[0].login);
				}
				else
				{
					Debug.WriteLine("login not ok");
					return "-2";
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("http request error");
				return "-3 "+e.Message +" "+e.InnerException.Message; ;
			}
		}

		public async Task<List<Member>> GetMembers(User user)
		{
			Debug.WriteLine("GetMembers "+ user.Username);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Member_Info + "?username=" + user.Username ));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);
			

				if (response.IsSuccessStatusCode)
				{
					Debug.WriteLine("login ok AQUI");
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content = "+ content);
					members = JsonConvert.DeserializeObject<List<Member>>(content);

				}
				else
				{
					Debug.WriteLine("login not ok");
				}
				return members;
			}
			catch (Exception e)
			{
				Debug.WriteLine("http request error");
				Debug.Print(e.StackTrace);
				return null;
			}

		}

		public async Task<List<Member>> GetMemberStudents(string memberid)
		{
			Debug.WriteLine("GetMemberStudents");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Member_Students_Info + "?userid=" + memberid));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);


				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content = " + content);
					members = JsonConvert.DeserializeObject<List<Member>>(content);

				}
				else
				{
					Debug.WriteLine("login not ok");
				}
				return members;
			}
			catch (Exception e)
			{
				Debug.WriteLine("http request error");
				Debug.Print(e.StackTrace);
				return null;
			}

		}

		public async Task<List<Member>> GetStudentsDojo(string memberid, string dojo)
		{
			Debug.WriteLine("GetStudentsDojo dojo="+dojo);
			Debug.Print(Constants.RestUrl_Get_Students_Dojo_Info + "?userid=" + memberid + "&dojo=" + dojo);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Students_Dojo_Info + "?userid=" + memberid+ "&dojo="+dojo));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);


				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content = " + content);
					members= JsonConvert.DeserializeObject<List<Member>>(content);

				}
				else
				{
					Debug.WriteLine("login not ok");
				}
				return members;
			}
			catch (Exception e)
			{
				Debug.WriteLine("http request error");
				Debug.Print(e.StackTrace);
				return null;
			}

		}


		public async Task<int> GetMemberStudents_Count(string memberid)
		{
			Debug.WriteLine("MemberManager.GetMemberStudents_Count");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Member_Students_Count + "?userid=" + memberid));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);

				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content = " + content);
					List<Result> createResultList = JsonConvert.DeserializeObject<List<Result>>(content);

					return Convert.ToInt32(createResultList[0].result);
				}
				else
				{
					Debug.WriteLine("login not ok");
					return -1;
				}	
			}
			catch (Exception e)
			{
				Debug.WriteLine("http request error");
				Debug.Print(e.StackTrace);
				return -2;
			}
		}


		public async Task<int> GetExaminations(Member member)
		{
			Debug.WriteLine("GetExaminations");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Examinations + "?userid=" + member.id, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);
				var result = 0;
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					member.examinations = JsonConvert.DeserializeObject<List<Examination>>(content);
					result = 1;
				}
				else
				{
					Debug.WriteLine("login not ok");
					result = -1;
				}

				return result;
			}
			catch
			{
				Debug.WriteLine("http request error");
				return -1;
			}
		}


		/*public async Task<int> GetFees(Member member)
		{
			Debug.WriteLine("GetFees begin");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Fees + "?userid=" + member.id, string.Empty));
			HttpResponseMessage response = await client.GetAsync(uri);
			var result = 0;
			if (response.IsSuccessStatusCode)
			{
				//return true;
				string content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("content=" + content);
				List<Fee> feesTemp = JsonConvert.DeserializeObject<List<Fee>>(content);
				member.currentFee = feesTemp[0];
				result = 1;
			}
			else
			{
				Debug.WriteLine("error getting fees");
				result = -1;
			}

			return result;
		}*/

		public async Task<int> GetPastFees(Member member)
		{
			Debug.WriteLine("GetPastFees");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Past_Fees + "?userid=" + member.id, string.Empty));
			try {
				HttpResponseMessage response = await client.GetAsync(uri);
				var result = 0;
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					member.pastFees = JsonConvert.DeserializeObject<List<Fee>>(content);
					result = 1;
				}
				else
				{
					Debug.WriteLine("error getting fees");
					result = -1;
				}

				return result;
			}
			catch
			{
				Debug.WriteLine("http request error");
				return -1;
			}
		}


		public async Task<int> GetCurrentFees(Member member)
		{
			Debug.WriteLine("GetCurrentFees begin");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Current_Fees + "?userid=" + member.id, string.Empty));
			try
            {

            
				HttpResponseMessage response = await client.GetAsync(uri);
				var result = 0;
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					List<Fee> feesTemp = JsonConvert.DeserializeObject<List<Fee>>(content);
					if ((feesTemp != null) & (feesTemp.Count > 0))

					{
						member.currentFee = feesTemp[0];
					}
					result = 1;
				}
				else
				{
					Debug.WriteLine("error getting fees");
					result = -1;
				}

				return result;
			}
			catch (Exception e)
			{
				Debug.WriteLine("MemberManager.GetCurrentFees http request error "+e.ToString());
				return -1;
			}
		}

		public async Task<int> CreateFee(Member member, string period)
		{
			Debug.WriteLine("CreateFee begin "+ Constants.RestUrl_Create_Fee + "?userid=" + member.id + "&period=" + period);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Create_Fee+ "?userid=" + member.id + "&period=" + period, string.Empty));
			try {
				HttpResponseMessage response = await client.GetAsync(uri);
				var result = 0;
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					//member.fees = JsonConvert.DeserializeObject<List<Fee>>(content);
					result = 1;
				}
				else
				{
					Debug.WriteLine("error creating fee");
					result = -1;
				}

				return result;
			}
			catch
			{
				Debug.WriteLine("http request error");
				return -1;
			}
		}

		public async Task<int> ChangePassword(string email, string newpassword)
		{
			Uri uri = new Uri(string.Format(Constants.RestUrl_Update_Password + "?username=" + email + "&password=" + newpassword, string.Empty));
			try {
				HttpResponseMessage response = await client.GetAsync(uri);

				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					List<Login> loginResultList = JsonConvert.DeserializeObject<List<Login>>(content);

					return loginResultList[0].login;
				}
				else
				{
					Debug.WriteLine("login not ok");
					return -2;
				}
			}
			catch
			{
				Debug.WriteLine("http request error");
				return -3;
			}
		}


		public async Task<int> RecoverPassword(string email)
		{
			Uri uri = new Uri(string.Format(Constants.RestUrl_Recover_Password + "?username=" + email, string.Empty));
			Debug.Print(Constants.RestUrl_Recover_Password + "?username=" + email);
			try { 
				HttpResponseMessage response = await client.GetAsync(uri);

				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					List<Login> loginResultList = JsonConvert.DeserializeObject<List<Login>>(content);

					return loginResultList[0].login;
				}
				else
				{
					Debug.WriteLine("login not ok");
					return -2;
				}
			}
			catch 
			{
				Debug.WriteLine("http request error");
				return -3;
			}
		}

		public async Task<string> UpdateMemberInfo(Member member)
		{

			string member_first_name = member.name.Substring(0, member.name.IndexOf(" "));
			string member_last_name = member.name.Substring(member.name.IndexOf(" ")+1);

			Debug.Print("UpdateMemberInfo");
			Uri uri = new Uri(string.Format(Constants.RestUrl_Update_Member_Info + "?memberid=" + member.id
				+ "&memberfirstname=" + member_first_name + "&memberlastname=" + member_last_name + "&membernif=" + member.nif + "&membercc=" + member.cc_number
				+ "&memberphone=" + member.phone + "&memberemail=" + member.email + "&memberaddress=" + member.address
				+ "&membercity=" + member.city + "&memberpostalcode=" + member.postalcode
				+ "&membernameenc1=" + member.name_enc1 + "&memberemailenc1=" + member.mail_enc1 + "&memberphoneenc1=" + member.phone_enc1
				+ "&membernameenc2=" + member.name_enc2 + "&memberemailenc2=" + member.mail_enc2 + "&memberphoneenc2=" + member.phone_enc2
				, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);
				var result = "0";

				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					List<Result> createResultList = JsonConvert.DeserializeObject<List<Result>>(content);

					return createResultList[0].result;
				}
				else
				{
					Debug.WriteLine("error updating member info");
					result = "-1";
				}

				return result;
			}
			catch
			{
				Debug.WriteLine("http request error");
				return "-1";
			}
		}

		public async Task<List<Payment>> GetFeePayment(string feeid)
		{
			Debug.Print("GetFeePayment feeid = "+ Constants.RestUrl_Get_FeePayment + "?feeid=" + feeid);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_FeePayment + "?feeid=" + feeid, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);

				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					payments = JsonConvert.DeserializeObject<List<Payment>>(content);
				}

				return payments;
			}
			catch
			{
				Debug.WriteLine("http request error");
				return null;
			}
		}
	}
}