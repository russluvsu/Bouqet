using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidClients : System.Web.UI.Page
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

                Response.Redirect(
                    "~/24-1528LibrandaMidLogin.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadClients(plannerId);
            }
        }

        private bool TryGetPlannerId(out int plannerId)
        {
            plannerId = 0;

            if (!Request.IsAuthenticated)
                return false;

            HttpCookie cookie =
                Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie == null ||
                string.IsNullOrWhiteSpace(cookie.Value))
            {
                return false;
            }

            try
            {
                FormsAuthenticationTicket ticket =
                    FormsAuthentication.Decrypt(cookie.Value);

                if (ticket == null ||
                    string.IsNullOrWhiteSpace(ticket.UserData))
                {
                    return false;
                }

                string[] parts =
                    ticket.UserData.Split('|');

                if (parts.Length < 4 ||
                    !string.Equals(
                        parts[0],
                        "Planner",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return int.TryParse(
                    parts[2],
                    out plannerId) &&
                    plannerId > 0;
            }
            catch
            {
                plannerId = 0;
                return false;
            }
        }

        private void LoadClients(int plannerId)
        {
            try
            {
                DataTable clients = new DataTable();

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                using (SqlCommand command =
                    new SqlCommand(
                        "dbo.sp_Client_ListByPlanner",
                        connection))
                {
                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.CommandTimeout = 10;

                    command.Parameters.Add(
                        "@PlannerId",
                        SqlDbType.Int).Value = plannerId;

                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(command))
                    {
                        adapter.Fill(clients);
                    }
                }

                litClientCount.Text =
                    clients.Rows.Count == 1
                        ? "1 client"
                        : clients.Rows.Count + " clients";

                if (clients.Rows.Count == 0)
                {
                    pnlEmpty.Visible = true;
                    pnlClients.Visible = false;
                    return;
                }

                pnlEmpty.Visible = false;
                pnlClients.Visible = true;

                rptClients.DataSource = clients;
                rptClients.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    ex.ToString());

                pnlEmpty.Visible = false;
                pnlClients.Visible = false;
                litClientCount.Text = string.Empty;

                ShowError(
                    "We couldn't load your clients right now. " +
                    ex.Message);
            }
        }

        protected void rptClients_ItemCommand(
            object source,
            System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!string.Equals(
                e.CommandName,
                "OpenClient",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string clientIdValue =
                Convert.ToString(e.CommandArgument);

            if (!int.TryParse(
                clientIdValue,
                out int clientId) ||
                clientId <= 0)
            {
                ShowError(
                    "The selected client could not be opened.");

                return;
            }

            Response.Redirect(
                "~/24-1528LibrandaMidClientPersonal.aspx?clientId=" +
                clientId,
                false);

            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnSignOut_Click(
            object sender,
            EventArgs e)
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect(
                "~/24-1528LibrandaMidLogin.aspx",
                false);

            Context.ApplicationInstance.CompleteRequest();
        }

        protected string FormatDate(object value)
        {
            if (value == null ||
                value == DBNull.Value)
            {
                return "Not set";
            }

            if (DateTime.TryParse(
                Convert.ToString(value),
                out DateTime date))
            {
                return date.ToString("MMMM d, yyyy");
            }

            return "Not set";
        }

        private void ShowError(string message)
        {
            litError.Text =
                HttpUtility.HtmlEncode(message);

            pnlError.Visible = true;
        }
    }
}