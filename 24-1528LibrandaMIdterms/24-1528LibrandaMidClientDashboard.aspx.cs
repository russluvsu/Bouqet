
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidClientDashboard : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString;

        private DateTime? currentWeddingDate;
        private DataTable currentTaskTable;


        private FormsAuthenticationTicket GetAuthenticationTicket()
        {
            FormsIdentity identity = User.Identity as FormsIdentity;
            return identity == null ? null : identity.Ticket;
        }

        private string GetUserRole()
        {
            FormsAuthenticationTicket ticket = GetAuthenticationTicket();

            if (ticket == null)
                return string.Empty;

            string[] parts = (ticket.UserData ?? string.Empty).Split('|');

            return parts.Length > 0 ? parts[0] : string.Empty;
        }

        private bool TryGetClientId(out int clientId)
        {
            return TryGetTicketValue(1, out clientId);
        }

        private bool TryGetPlannerId(out int plannerId)
        {
            return TryGetTicketValue(2, out plannerId);
        }

        private bool TryGetWeddingId(out int weddingId)
        {
            return TryGetTicketValue(3, out weddingId);
        }

        private bool TryGetTicketValue(int index, out int value)
        {
            value = 0;

            FormsAuthenticationTicket ticket = GetAuthenticationTicket();

            if (ticket == null)
                return false;

            string[] parts = (ticket.UserData ?? string.Empty).Split('|');

            if (parts.Length <= index ||
                !string.Equals(parts[0], "Client", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return int.TryParse(parts[index], out value);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                RedirectToLogin();
                return;
            }

            if (!string.Equals(GetUserRole(), "Client", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/24-1528LibrandaMidPlannerDashboard.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int clientId;
            int weddingId;

            if (!TryGetClientId(out clientId) || !TryGetWeddingId(out weddingId))
            {
                FormsAuthentication.SignOut();
                RedirectToLogin();
                return;
            }

            if (!IsPostBack)
            {
                LoadClient();
                LoadBudgetSummary();
                LoadTasks();
            }
        }

        private void RedirectToLogin()
        {
            Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LoadClient()
        {
            try
            {
                int clientId;
                int plannerId;

                if (!TryGetClientId(out clientId) || !TryGetPlannerId(out plannerId))
                {
                    ShowError("Your client account could not be loaded.");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Client_GetForPlanner", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;

                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowError("Your wedding information could not be found.");
                            return;
                        }

                        string partnerOne = BuildFullName(
                            reader["Partner1FirstName"],
                            reader["Partner1LastName"]);

                        string partnerTwo = BuildFullName(
                            reader["Partner2FirstName"],
                            reader["Partner2LastName"]);

                        string displayNames = BuildDisplayNames(
                            reader["Partner1FirstName"],
                            reader["Partner2FirstName"]);

                        litClientName.Text = HttpUtility.HtmlEncode(displayNames);
                        litPartners.Text = HttpUtility.HtmlEncode(partnerOne + " & " + partnerTwo);
                        litWeddingDate.Text = FormatDate(reader["WeddingDate"]);

                        string venue = Convert.ToString(reader["Venue"]);

                        litVenue.Text = string.IsNullOrWhiteSpace(venue)
                            ? "Not assigned yet"
                            : HttpUtility.HtmlEncode(venue);

                        litWeddingStatus.Text = HttpUtility.HtmlEncode(
                            Convert.ToString(reader["WeddingStatus"]));

                        if (reader["WeddingDate"] != DBNull.Value)
                        {
                            currentWeddingDate = Convert.ToDateTime(reader["WeddingDate"]).Date;

                            int days = (currentWeddingDate.Value - DateTime.Today).Days;

                            litDaysToGo.Text = Math.Max(days, 0).ToString(
                                CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            currentWeddingDate = null;
                            litDaysToGo.Text = "—";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Warn("ClientDashboard", ex.ToString());
                ShowError("We couldn't load your wedding information. " + ex.Message);
            }
        }

        private void LoadBudgetSummary()
        {
            try
            {
                int clientId;
                int weddingId;

                if (!TryGetClientId(out clientId) || !TryGetWeddingId(out weddingId))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Budget_GetSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;

                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = DBNull.Value;
                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return;

                        decimal totalBudget = ReadDecimal(reader, "TotalBudget");
                        decimal totalSpent = ReadDecimal(reader, "TotalSpent");

                        decimal remaining = reader["Remaining"] == DBNull.Value
                            ? totalBudget - totalSpent
                            : Convert.ToDecimal(reader["Remaining"]);

                        decimal percentUsed = ReadDecimal(reader, "PercentUsed");
                        percentUsed = Math.Max(0, Math.Min(100, percentUsed));

                        litTotalBudget.Text = FormatCurrency(totalBudget);
                        litTotalSpent.Text = FormatCurrency(totalSpent);
                        litRemaining.Text = FormatCurrency(remaining);

                        litPercentUsed.Text = percentUsed.ToString(
                            "0.0", CultureInfo.InvariantCulture) + "%";

                        litBudgetProgress.Text = percentUsed.ToString(
                            "0", CultureInfo.InvariantCulture) + "%";

                        budgetProgressFill.Style["width"] = percentUsed.ToString(
                            "0.0", CultureInfo.InvariantCulture) + "%";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Warn("ClientDashboard", ex.ToString());
                ShowError("We couldn't load the budget summary. " + ex.Message);
            }
        }

        private decimal ReadDecimal(IDataRecord reader, string column)
        {
            return reader[column] == DBNull.Value
                ? 0
                : Convert.ToDecimal(reader[column]);
        }

        private void LoadTasks()
        {
            try
            {
                int clientId;
                int weddingId;

                if (!TryGetClientId(out clientId) || !TryGetWeddingId(out weddingId))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_ListForClient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;

                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;

                    DataTable table = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }

                    currentTaskTable = table;

                    rptTasks.DataSource = table;
                    rptTasks.DataBind();

                    DataView confirmationView = new DataView(table);
                    confirmationView.RowFilter = "Status = 'Shared'";

                    rptConfirmations.DataSource = confirmationView;
                    rptConfirmations.DataBind();

                    litConfirmationCount.Text = confirmationView.Count.ToString(
                        CultureInfo.InvariantCulture);

                    pnlNoConfirmations.Visible = confirmationView.Count == 0;

                    int taskCount = table.Rows.Count;
                    int completed = 0;

                    foreach (DataRow row in table.Rows)
                    {
                        string status = Convert.ToString(row["Status"]);

                        if (IsCompletedStatus(status))
                            completed++;
                    }

                    int taskProgress = taskCount == 0
                        ? 0
                        : (int)Math.Round(
                            completed * 100.0 / taskCount,
                            MidpointRounding.AwayFromZero);

                    taskProgress = Math.Max(0, Math.Min(100, taskProgress));

                    litTaskProgress.Text = taskProgress.ToString(
                        CultureInfo.InvariantCulture) + "%";

                    litTaskCount.Text = taskCount == 1
                        ? "1 task"
                        : taskCount + " tasks";

                    pnlNoTasks.Visible = taskCount == 0;

                    RenderClientTimeline(table);
                }
            }
            catch (Exception ex)
            {
                Trace.Warn("ClientDashboard", ex.ToString());
                ShowError("We couldn't load your wedding tasks. " + ex.Message);
            }
        }

        private void RenderClientTimeline(DataTable tasks)
        {
            StringBuilder html = new StringBuilder();

            DateTime? weddingDate = GetTimelineWeddingDate();

            string weddingDateText = weddingDate.HasValue
                ? weddingDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)
                : "Date not set";

            html.Append("<div class=\"client-timeline-top\"><div>");
            html.Append("<strong>");
            html.Append(HttpUtility.HtmlEncode(litPartners.Text));
            html.Append("</strong><span>");
            html.Append(HttpUtility.HtmlEncode(weddingDateText));
            html.Append("</span></div>");
            html.Append("<span class=\"client-timeline-readonly\">Read only</span>");
            html.Append("</div>");

            DataTable visibleTasks = tasks.Clone();

            foreach (DataRow row in tasks.Rows)
            {
                string status = Convert.ToString(row["Status"]);

                if (!string.Equals(status, "Draft", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(status, "Removed", StringComparison.OrdinalIgnoreCase))
                {
                    visibleTasks.ImportRow(row);
                }
            }

            DataView sortedTasks = new DataView(visibleTasks);
            sortedTasks.Sort = "DueDate ASC";

            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(-30);
            DateTime endDate = today.AddDays(180);

            if (weddingDate.HasValue)
            {
                if (weddingDate.Value < startDate)
                    startDate = weddingDate.Value.AddDays(-30);

                if (weddingDate.Value > endDate)
                    endDate = weddingDate.Value.AddDays(30);
            }

            foreach (DataRowView task in sortedTasks)
            {
                if (task["DueDate"] == DBNull.Value)
                    continue;

                DateTime dueDate = Convert.ToDateTime(task["DueDate"]).Date;

                if (dueDate < startDate)
                    startDate = dueDate.AddDays(-7);

                if (dueDate > endDate)
                    endDate = dueDate.AddDays(7);
            }

            if (endDate <= startDate)
                endDate = startDate.AddDays(180);

            double totalDays = (endDate - startDate).TotalDays;

            double completedPosition = 0;

            foreach (DataRowView task in sortedTasks)
            {
                if (!IsCompletedStatus(Convert.ToString(task["Status"])))
                    continue;

                if (task["DueDate"] == DBNull.Value)
                    continue;

                DateTime dueDate = Convert.ToDateTime(task["DueDate"]).Date;

                double position =
                    ((dueDate - startDate).TotalDays / totalDays) * 100.0;

                position = Math.Max(0, Math.Min(100, position));

                if (position > completedPosition)
                    completedPosition = position;
            }

            html.Append("<div class=\"client-timeline-canvas\">");
            html.Append("<div class=\"client-wedding-lane\">");
            html.Append("<div class=\"lane-label\"><strong>Wedding</strong><span>");
            html.Append(HttpUtility.HtmlEncode(weddingDateText));
            html.Append("</span></div>");

            html.Append("<div class=\"timeline-lane-track\">");
            html.Append("<div class=\"timeline-lane-track-line\">");
            html.Append("<div class=\"timeline-lane-track-fill\" style=\"width:");
            html.Append(completedPosition.ToString("0.0", CultureInfo.InvariantCulture));
            html.Append("%\"></div></div>");

            foreach (DataRowView task in sortedTasks)
            {
                DateTime? dueDate = task["DueDate"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(task["DueDate"]).Date;

                double position = dueDate.HasValue
                    ? ((dueDate.Value - startDate).TotalDays / totalDays) * 100.0
                    : 4.0;

                position = Math.Max(2, Math.Min(98, position));

                string status = Convert.ToString(task["Status"]);
                string milestoneClass = GetClientTimelineMilestoneClass(status, dueDate);
                string title = Convert.ToString(task["Title"]);

                string dateText = dueDate.HasValue
                    ? dueDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)
                    : "No due date";

                html.Append("<div class=\"timeline-milestone ");
                html.Append(milestoneClass);
                html.Append("\" style=\"left:");
                html.Append(position.ToString("0.0", CultureInfo.InvariantCulture));
                html.Append("%\" title=\"");
                html.Append(HttpUtility.HtmlAttributeEncode(title));
                html.Append("\">");

                html.Append("<span class=\"timeline-milestone-dot\"></span>");
                html.Append("<div class=\"timeline-milestone-label\"><strong>");
                html.Append(HttpUtility.HtmlEncode(title));
                html.Append("</strong><span>");
                html.Append(HttpUtility.HtmlEncode(dateText));
                html.Append("</span></div></div>");
            }

            if (weddingDate.HasValue)
            {
                double weddingPosition =
                    ((weddingDate.Value - startDate).TotalDays / totalDays) * 100.0;

                weddingPosition = Math.Max(2, Math.Min(98, weddingPosition));

                html.Append("<div class=\"client-wedding-day-marker\" style=\"left:");
                html.Append(weddingPosition.ToString("0.0", CultureInfo.InvariantCulture));
                html.Append("%\"><span class=\"client-wedding-diamond\"></span>");
                html.Append("<div class=\"client-wedding-day-label\"><strong>Wedding day</strong><span>");
                html.Append(HttpUtility.HtmlEncode(weddingDateText));
                html.Append("</span></div></div>");
            }

            html.Append("</div></div></div>");

            if (sortedTasks.Count == 0)
            {
                html.Append("<p class=\"client-timeline-empty\">");
                html.Append("Your planner hasn't shared any timeline milestones yet.");
                html.Append("</p>");
            }

            litTimeline.Text = html.ToString();
        }

        private DateTime? GetTimelineWeddingDate()
        {
            if (currentWeddingDate.HasValue)
                return currentWeddingDate.Value.Date;

            DateTime parsedDate;

            if (DateTime.TryParse(
                litWeddingDate.Text,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate))
            {
                return parsedDate.Date;
            }

            return null;
        }

        private bool IsCompletedStatus(string status)
        {
            return string.Equals(status, "Done", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, "Confirmed", StringComparison.OrdinalIgnoreCase);
        }

        private string GetClientTimelineMilestoneClass(
            string status,
            DateTime? dueDate)
        {
            if (IsCompletedStatus(status))
                return "completed";

            if (string.Equals(status, "Shared", StringComparison.OrdinalIgnoreCase))
                return "waiting";

            if (dueDate.HasValue && dueDate.Value < DateTime.Today)
                return "overdue";

            return "upcoming";
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int taskId;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out taskId))
            {
                ShowError("The selected task is invalid.");
                return;
            }

            if (string.Equals(e.CommandName, "RateTask", StringComparison.OrdinalIgnoreCase))
            {
                OpenRating(taskId);
                return;
            }

            if (string.Equals(e.CommandName, "RequestChange", StringComparison.OrdinalIgnoreCase))
                OpenChangeRequest(taskId);
        }

        private bool IsClientTaskOwned(int taskId)
        {
            int clientId;
            int weddingId;

            if (!TryGetClientId(out clientId) || !TryGetWeddingId(out weddingId))
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_GetForClient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;

                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;

                    connection.Open();

                    object result = command.ExecuteScalar();

                    return result != null &&
                           result != DBNull.Value &&
                           Convert.ToInt32(result) == 1;
                }
            }
            catch (Exception ex)
            {
                Trace.Warn("ClientDashboard", ex.ToString());
                return false;
            }
        }

        private void OpenRating(int taskId)
        {
            if (!IsClientTaskOwned(taskId))
            {
                ShowError("You cannot rate this task.");
                return;
            }

            hiddenRatingTaskId.Value = taskId.ToString(CultureInfo.InvariantCulture);
            rblRating.ClearSelection();
            txtRatingComment.Text = string.Empty;
            pnlRating.Visible = true;
            pnlChangeRequest.Visible = false;
        }

        private void OpenChangeRequest(int taskId)
        {
            if (!IsClientTaskOwned(taskId))
            {
                ShowError("You cannot request changes for this task.");
                return;
            }

            hiddenChangeTaskId.Value = taskId.ToString(CultureInfo.InvariantCulture);
            txtChangeRequest.Text = string.Empty;
            pnlChangeRequest.Visible = true;
            pnlRating.Visible = false;
        }

        protected void btnCancelRating_Click(object sender, EventArgs e)
        {
            pnlRating.Visible = false;
            hiddenRatingTaskId.Value = string.Empty;
            rblRating.ClearSelection();
            txtRatingComment.Text = string.Empty;
        }

        protected void btnSubmitRating_Click(object sender, EventArgs e)
        {
            int clientId;
            int taskId;
            int stars;

            if (!TryGetClientId(out clientId))
            {
                ShowError("Your client account could not be identified.");
                return;
            }

            if (!int.TryParse(hiddenRatingTaskId.Value, out taskId) ||
                !IsClientTaskOwned(taskId))
            {
                ShowError("You cannot rate this task.");
                return;
            }

            if (!int.TryParse(rblRating.SelectedValue, out stars) || stars < 1 || stars > 5)
            {
                ShowError("Please select a rating from 1 to 5 stars.");
                pnlRating.Visible = true;
                return;
            }

            string comment = txtRatingComment.Text.Trim();

            if (comment.Length > 500)
            {
                ShowError("Your rating comment cannot exceed 500 characters.");
                pnlRating.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_TaskRating_Submit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;

                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                    command.Parameters.Add("@Stars", SqlDbType.Int).Value = stars;
                    command.Parameters.Add("@Comment", SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(comment) ? (object)DBNull.Value : comment;

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                pnlRating.Visible = false;
                hiddenRatingTaskId.Value = string.Empty;
                rblRating.ClearSelection();
                txtRatingComment.Text = string.Empty;

                ShowSuccess("Your rating has been submitted. Thank you for the feedback.");
                LoadTasks();
            }
            catch (Exception ex)
            {
                Trace.Warn("ClientDashboard", ex.ToString());
                ShowError("We couldn't submit your rating right now. " + ex.Message);
                pnlRating.Visible = true;
            }
        }

        protected void btnCancelChangeRequest_Click(object sender, EventArgs e)
        {
            pnlChangeRequest.Visible = false;
            hiddenChangeTaskId.Value = string.Empty;
            txtChangeRequest.Text = string.Empty;
        }

        protected void btnSubmitChangeRequest_Click(object sender, EventArgs e)
        {
            int clientId;
            int taskId;

            if (!TryGetClientId(out clientId))
            {
                ShowError("Your client account could not be identified.");
                return;
            }

            if (!int.TryParse(hiddenChangeTaskId.Value, out taskId) ||
                !IsClientTaskOwned(taskId))
            {
                ShowError("You cannot request changes for this task.");
                return;
            }

            string requestText = txtChangeRequest.Text.Trim();

            if (string.IsNullOrWhiteSpace(requestText))
            {
                ShowError("Please describe the changes you would like to request.");
                pnlChangeRequest.Visible = true;
                return;
            }

            if (requestText.Length > 500)
            {
                ShowError("Your change request cannot exceed 500 characters.");
                pnlChangeRequest.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_RequestChange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;

                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                    command.Parameters.Add("@RequestText", SqlDbType.NVarChar, 500).Value = requestText;

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                pnlChangeRequest.Visible = false;
                hiddenChangeTaskId.Value = string.Empty;
                txtChangeRequest.Text = string.Empty;

                ShowSuccess("Your change request has been sent to your planner.");
                LoadTasks();
            }
            catch (Exception ex)
            {
                Trace.Warn("ClientDashboard", ex.ToString());
                ShowError("We couldn't send your change request right now. " + ex.Message);
                pnlChangeRequest.Visible = true;
            }
        }


        protected bool CanRateTask(object status, object isRated)
        {
            string taskStatus = Convert.ToString(status);

            bool canRate =
                string.Equals(taskStatus, "Confirmed", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(taskStatus, "Done", StringComparison.OrdinalIgnoreCase);

            return canRate && !IsTaskRated(isRated);
        }


        protected bool IsTaskRated(object isRated)
        {
            if (isRated == null || isRated == DBNull.Value)
                return false;

            int value;

            return int.TryParse(Convert.ToString(isRated), out value) && value == 1;
        }

        protected bool CanRequestChanges(object status)
        {
            return string.Equals(
                Convert.ToString(status),
                "Confirmed",
                StringComparison.OrdinalIgnoreCase);
        }

        protected string GetTaskStatusText(object status)
        {
            string value = Convert.ToString(status);

            if (string.Equals(value, "Done", StringComparison.OrdinalIgnoreCase))
                return "Completed";

            if (string.Equals(value, "Shared", StringComparison.OrdinalIgnoreCase))
                return "Waiting for you";

            if (string.Equals(value, "Draft", StringComparison.OrdinalIgnoreCase))
                return "Preparing";

            if (string.Equals(value, "Confirmed", StringComparison.OrdinalIgnoreCase))
                return "Confirmed";

            return value;
        }

        protected string GetTaskStatusClass(object status)
        {
            string value = Convert.ToString(status);

            if (IsCompletedStatus(value))
                return "completed";

            if (string.Equals(value, "Shared", StringComparison.OrdinalIgnoreCase))
                return "waiting";

            if (string.Equals(value, "Draft", StringComparison.OrdinalIgnoreCase))
                return "draft";

            return string.Empty;
        }

        protected string GetTaskMarkerClass(object status)
        {
            return IsCompletedStatus(Convert.ToString(status))
                ? "completed"
                : "waiting";
        }

        protected string FormatTaskCost(object value)
        {
            decimal cost;

            if (value == null || value == DBNull.Value ||
                !decimal.TryParse(Convert.ToString(value), out cost))
            {
                cost = 0;
            }

            return cost.ToString("N2", CultureInfo.InvariantCulture);
        }

        protected string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "Not set";

            DateTime date;

            if (!DateTime.TryParse(Convert.ToString(value), out date))
                return "Not set";

            return date.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }

        private string FormatCurrency(decimal value)
        {
            return "₱" + value.ToString("N2", CultureInfo.InvariantCulture);
        }

        private string BuildFullName(object firstName, object lastName)
        {
            return (Convert.ToString(firstName).Trim() + " " +
                    Convert.ToString(lastName).Trim()).Trim();
        }

        private string BuildDisplayNames(object firstName, object secondFirstName)
        {
            string first = Convert.ToString(firstName).Trim();
            string second = Convert.ToString(secondFirstName).Trim();

            if (string.IsNullOrWhiteSpace(first))
                return second;

            if (string.IsNullOrWhiteSpace(second))
                return first;

            return first + " and " + second;
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            RedirectToLogin();
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            litError.Text = HttpUtility.HtmlEncode(message);
            pnlSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            litSuccess.Text = HttpUtility.HtmlEncode(message);
            pnlError.Visible = false;
        }
    }
}
