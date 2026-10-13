using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidLogin : System.Web.UI.Page
    {
        // Where each role lands after a successful sign-in (pages still to be built).
        private const string PlannerHome = "~/24-1528LibrandaMidPlannerDashboard.aspx";
        private const string ClientHome = "~/24-1528LibrandaMidClientDashboard.aspx";
        private const string ChangePasswordPage = "~/24-1528LibrandaMidChangePassword.aspx";

        // Sign-in cookie payload (FormsAuthenticationTicket.UserData):
        //   "Role|UserId|PlannerId|WeddingId"
        //   Planner: "Planner|{plannerId}|{plannerId}|0"
        //   Client:  "Client|{clientId}|{plannerId}|{weddingId}"
        // Other pages read this to enforce the role on every load.

        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutWindow = TimeSpan.FromMinutes(5);
        private static readonly object AttemptLock = new object();
        private static readonly Regex EmailPattern =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private sealed class AccountRow
        {
            public int Id;
            public int PlannerId;
            public int WeddingId;
            public string PasswordHash;
            public bool MustChangePassword;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Never let the browser or a proxy keep a copy of the sign-in page.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                hfMode.Value = "client";
            }
            ApplyModeCopy();
        }

        // The toggle only changes the wording. The real role is decided by
        // whichever account the credentials belong to.
        private void ApplyModeCopy()
        {
            if (hfMode.Value != "planner")
            {
                hfMode.Value = "client";
            }

            bool planner = hfMode.Value == "planner";
            heading.InnerText = planner ? "Planner sign in" : "Welcome back";
            subtext.InnerText = planner
                ? "Manage your clients and weddings."
                : "Sign in to follow your wedding plans.";
            footNote.InnerText = planner
                ? "Add clients and issue their login details from your dashboard."
                : "Your planner will give you your login details.";
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.Text ?? string.Empty).Trim();
            string password = txtPassword.Text ?? string.Empty;

            if (email.Length == 0 || email.Length > 150 || !EmailPattern.IsMatch(email) ||
                password.Length == 0 || password.Length > 128)
            {
                ShowError("Please enter a valid email and your password.");
                return;
            }

            if (IsLockedOut(email))
            {
                ShowError("Too many attempts. Please wait a few minutes and try again.");
                return;
            }

            try
            {
                bool plannerMode = string.Equals(
                    hfMode.Value,
                    "planner",
                    StringComparison.OrdinalIgnoreCase
                );

                if (plannerMode)
                {
                    AccountRow planner = FindPlanner(email);

                    if (planner != null &&
                        PasswordHasher.Verify(password, planner.PasswordHash))
                    {
                        ClearFailures(email);
                        IssueTicket(email, "Planner", planner.Id, planner.Id, 0);
                        Redirect(PlannerHome);
                        return;
                    }
                }
                else
                {
                    AccountRow client = FindClient(email);

                    if (client != null &&
                        PasswordHasher.Verify(password, client.PasswordHash))
                    {
                        ClearFailures(email);
                        IssueTicket(email, "Client", client.Id, client.PlannerId, client.WeddingId);
                        Redirect(client.MustChangePassword ? ChangePasswordPage : ClientHome);
                        return;
                    }
                }

                PasswordHasher.BurnTime(password);
                RegisterFailure(email);
                ShowError("That email or password isn't right.");
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                ShowError("We couldn't sign you in right now. Please try again.");
            }
        }
        // ---- Database lookups (stored procedures only) -------------------

        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString; }
        }

        private static AccountRow FindPlanner(string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_Planner_GetForLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!r.Read()) return null;

                    AccountRow row = new AccountRow();
                    row.Id = r.GetInt32(r.GetOrdinal("Id"));
                    row.PasswordHash = r.GetString(r.GetOrdinal("PasswordHash"));
                    return row;
                }
            }
        }

        private static AccountRow FindClient(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_Client_GetForLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 150).Value = username;

                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!r.Read()) return null;

                    AccountRow row = new AccountRow();
                    row.Id = r.GetInt32(r.GetOrdinal("ClientId"));
                    row.PlannerId = r.GetInt32(r.GetOrdinal("PlannerId"));
                    row.WeddingId = r.GetInt32(r.GetOrdinal("WeddingId"));
                    row.PasswordHash = r.GetString(r.GetOrdinal("PasswordHash"));
                    row.MustChangePassword = r.GetBoolean(r.GetOrdinal("MustChangePassword"));
                    return row;
                }
            }
        }

        // ---- Sign-in cookie -----------------------------------------------

        private void IssueTicket(string email, string role, int userId, int plannerId, int weddingId)
        {
            bool remember = chkRemember.Checked;
            DateTime now = DateTime.Now;
            DateTime expires = now.AddMinutes(remember ? 60 * 24 * 7 : 30);
            string userData = string.Join("|", role, userId, plannerId, weddingId);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1, email, now, expires, remember, userData, FormsAuthentication.FormsCookiePath);

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.SameSite = SameSiteMode.Lax;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (remember)
            {
                cookie.Expires = expires;
            }

            Response.Cookies.Add(cookie);
        }

        // Always goes to a fixed page. ReturnUrl is ignored on purpose so the
        // sign-in page can never be used to bounce someone to another site.
        private void Redirect(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // ---- Failed-attempt throttle (in memory, resets when the app restarts)

        private static string AttemptKey(string email)
        {
            return "loginfail:" + email.ToLowerInvariant();
        }

        private static bool IsLockedOut(string email)
        {
            object value = HttpRuntime.Cache[AttemptKey(email)];
            return value is int && (int)value >= MaxFailedAttempts;
        }

        private static void RegisterFailure(string email)
        {
            lock (AttemptLock)
            {
                string key = AttemptKey(email);
                object value = HttpRuntime.Cache[key];
                int count = value is int ? (int)value : 0;

                HttpRuntime.Cache.Insert(key, count + 1, null,
                    DateTime.UtcNow.Add(LockoutWindow),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        private static void ClearFailures(string email)
        {
            HttpRuntime.Cache.Remove(AttemptKey(email));
        }

        private void ShowError(string message)
        {
            litError.Text = HttpUtility.HtmlEncode(message);
            pnlError.Visible = true;
        }
    }
}