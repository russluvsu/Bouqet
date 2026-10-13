using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidChangePassword : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect(
                    "~/24-1528LibrandaMidLogin.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string[] parts = GetTicketParts();

            if (parts == null ||
                parts.Length < 4 ||
                !string.Equals(parts[0], "Client", StringComparison.OrdinalIgnoreCase))
            {
                FormsAuthentication.SignOut();

                Response.Redirect(
                    "~/24-1528LibrandaMidLogin.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int clientId;

            if (!int.TryParse(parts[1], out clientId) || clientId <= 0)
            {
                FormsAuthentication.SignOut();

                Response.Redirect(
                    "~/24-1528LibrandaMidLogin.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!GetMustChangePassword(clientId))
            {
                Response.Redirect(
                    "~/24-1528LibrandaMidClientDashboard.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            pnlError.Visible = false;
            pnlSuccess.Visible = false;
            litError.Text = string.Empty;
            litSuccess.Text = string.Empty;

            string[] parts = GetTicketParts();

            if (parts == null ||
                parts.Length < 4 ||
                !string.Equals(parts[0], "Client", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Your session is invalid. Please log in again.");
                return;
            }

            int clientId;

            if (!int.TryParse(parts[1], out clientId) || clientId <= 0)
            {
                ShowError("Your account session is invalid. Please log in again.");
                return;
            }

            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                ShowError("Please enter your temporary password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowError("Please enter a new password.");
                return;
            }

            if (newPassword.Length < 8)
            {
                ShowError("Your new password must be at least 8 characters long.");
                return;
            }

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ShowError("The new passwords do not match.");
                return;
            }

            if (string.Equals(currentPassword, newPassword, StringComparison.Ordinal))
            {
                ShowError("Your new password must be different from your temporary password.");
                return;
            }

            try
            {
                string storedHash = GetClientPasswordHash(clientId);

                if (string.IsNullOrWhiteSpace(storedHash))
                {
                    ShowError("Your account does not have a valid password. Please contact your planner.");
                    return;
                }

                if (!PasswordHasher.Verify(currentPassword, storedHash))
                {
                    PasswordHasher.BurnTime(currentPassword);
                    ShowError("The temporary password is incorrect.");
                    return;
                }

                string newPasswordHash = PasswordHasher.Hash(newPassword);

                SetClientPassword(clientId, newPasswordHash);

                txtCurrentPassword.Text = string.Empty;
                txtNewPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;

                pnlSuccess.Visible = true;
                litSuccess.Text = "Password changed successfully. Redirecting...";

                Response.Redirect(
                    "~/24-1528LibrandaMidClientDashboard.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Trace.Warn(
                    "ChangePassword",
                    "Password change failed.",
                    ex);

                ShowError("We could not change your password right now. Please try again.");
            }
        }

        private string[] GetTicketParts()
        {
            FormsAuthenticationTicket ticket = null;

            try
            {
                HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
                    return null;

                ticket = FormsAuthentication.Decrypt(cookie.Value);
            }
            catch
            {
                return null;
            }

            if (ticket == null || string.IsNullOrWhiteSpace(ticket.UserData))
                return null;

            return ticket.UserData.Split('|');
        }

        private bool GetMustChangePassword(int clientId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(
                @"SELECT MustChangePassword
                  FROM dbo.Clients
                  WHERE Id = @ClientId
                    AND IsActive = 1;",
                connection))
            {
                command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;

                connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Client account not found.");

                return Convert.ToBoolean(result);
            }
        }

        private string GetClientPasswordHash(int clientId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(
                @"SELECT PasswordHash
                  FROM dbo.Clients
                  WHERE Id = @ClientId
                    AND IsActive = 1;",
                connection))
            {
                command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;

                connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToString(result);
            }
        }

        private void SetClientPassword(int clientId, string passwordHash)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(
                "dbo.sp_Client_SetPassword",
                connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;
                command.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 500).Value = passwordHash;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            litError.Text = message;
        }


        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
    }