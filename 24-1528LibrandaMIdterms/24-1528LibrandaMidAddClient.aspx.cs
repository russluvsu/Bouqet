using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidAddClient : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (!TryGetPlannerId(out int plannerId))
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                txtWeddingDate.Text = DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd");
            }
        }

        private bool TryGetPlannerId(out int plannerId)
        {
            plannerId = 0;

            if (!Request.IsAuthenticated)
                return false;

            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
                return false;

            try
            {
                FormsAuthenticationTicket ticket =
                    FormsAuthentication.Decrypt(cookie.Value);

                if (ticket == null || string.IsNullOrWhiteSpace(ticket.UserData))
                    return false;

                string[] parts = ticket.UserData.Split('|');

                if (parts.Length < 4 ||
                    !string.Equals(parts[0], "Planner", StringComparison.OrdinalIgnoreCase))
                    return false;

                return int.TryParse(parts[2], out plannerId) && plannerId > 0;
            }
            catch
            {
                plannerId = 0;
                return false;
            }
        }

        protected void btnCreateClient_Click(object sender, EventArgs e)
        {
            HideMessages();

            if (!TryGetPlannerId(out int plannerId))
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string partner1FirstName = txtPartner1FirstName.Text.Trim();
            string partner1LastName = txtPartner1LastName.Text.Trim();
            string partner2FirstName = txtPartner2FirstName.Text.Trim();
            string partner2LastName = txtPartner2LastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtTemporaryPassword.Text;

            if (partner1FirstName.Length == 0 ||
                partner1LastName.Length == 0 ||
                partner2FirstName.Length == 0 ||
                partner2LastName.Length == 0)
            {
                ShowError("Please enter both partners' first and last names.");
                return;
            }

            if (username.Length == 0 ||
                username.Length > 150 ||
                !IsValidEmail(username))
            {
                ShowError("Please enter a valid client email address.");
                return;
            }

            if (password.Length < 8 || password.Length > 128)
            {
                ShowError("The temporary password must be between 8 and 128 characters.");
                return;
            }

            if (!DateTime.TryParse(txtWeddingDate.Text, out DateTime weddingDate))
            {
                ShowError("Please enter a valid wedding date.");
                return;
            }

            if (weddingDate.Date < DateTime.Today)
            {
                ShowError("The wedding date cannot be in the past.");
                return;
            }

            int? estimatedGuests = null;

            if (!string.IsNullOrWhiteSpace(txtEstimatedGuests.Text))
            {
                if (!int.TryParse(txtEstimatedGuests.Text, out int guests) || guests < 0)
                {
                    ShowError("Estimated guests must be a valid number.");
                    return;
                }

                estimatedGuests = guests;
            }

            try
            {
                string passwordHash = PasswordHasher.Hash(password);

                int clientId;
                int weddingId;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command =
                    new SqlCommand("dbo.sp_Client_Add", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@Partner1FirstName", SqlDbType.NVarChar, 50).Value = partner1FirstName;
                    command.Parameters.Add("@Partner1LastName", SqlDbType.NVarChar, 50).Value = partner1LastName;
                    command.Parameters.Add("@Partner2FirstName", SqlDbType.NVarChar, 50).Value = partner2FirstName;
                    command.Parameters.Add("@Partner2LastName", SqlDbType.NVarChar, 50).Value = partner2LastName;
                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 150).Value = username;
                    command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = passwordHash;
                    command.Parameters.Add("@WeddingDate", SqlDbType.Date).Value = weddingDate.Date;
                    command.Parameters.Add("@EstimatedGuests", SqlDbType.Int).Value =
                        estimatedGuests.HasValue
                            ? (object)estimatedGuests.Value
                            : DBNull.Value;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowError("The client was not created.");
                            return;
                        }

                        clientId = Convert.ToInt32(reader["ClientId"]);
                        weddingId = Convert.ToInt32(reader["WeddingId"]);
                    }
                }

                Response.Redirect(
                    "~/24-1528LibrandaMidPlannerDashboard.aspx?clientCreated=1",
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50001)
                {
                    ShowError("That client login email is already in use.");
                    return;
                }

                if (ex.Number == 50404)
                {
                    ShowError("Your planner account could not be verified.");
                    return;
                }

                System.Diagnostics.Trace.TraceError(ex.ToString());
                ShowError("We couldn't create the client right now. Please try again.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                ShowError("Something went wrong while creating the client.");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void HideMessages()
        {
            pnlError.Visible = false;
            pnlSuccess.Visible = false;
            litError.Text = string.Empty;
            litSuccess.Text = string.Empty;
        }

        private void ShowError(string message)
        {
            litError.Text = HttpUtility.HtmlEncode(message);
            pnlError.Visible = true;
        }
    }
}