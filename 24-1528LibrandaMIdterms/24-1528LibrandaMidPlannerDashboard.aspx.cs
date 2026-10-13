using System.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidPlannerDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            FormsIdentity identity = User.Identity as FormsIdentity;

            if (identity == null)
            {
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx");
                return;
            }

            FormsAuthenticationTicket ticket = identity.Ticket;

            if (ticket == null || string.IsNullOrWhiteSpace(ticket.UserData))
            {
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx");
                return;
            }

            string[] userData = ticket.UserData.Split('|');

            if (userData.Length < 2 ||
                !userData[0].Equals("Planner", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx");
                return;
            }

            int plannerId;

            if (!int.TryParse(userData[1], out plannerId))
            {
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx");
                return;
            }

            LoadPortfolio(plannerId);

            DataSet dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection(
    ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString))
            using (SqlCommand command = new SqlCommand("dbo.sp_Planner_GetTimeline", connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PlannerId", plannerId);

                adapter.Fill(dataSet);
            }

            if (dataSet.Tables.Count == 0)
            {
                ClearDashboard();
                return;
            }

            DataTable weddings = dataSet.Tables[0];

            DataTable tasks = dataSet.Tables.Count > 1
                ? dataSet.Tables[1]
                : new DataTable();

            if (weddings.Rows.Count == 0)
            {
                ClearDashboard();
                return;
            }

            litGreeting.Text = "Welcome back, Planner.";
            litSubtext.Text = "Here is your current wedding planning overview.";

            LoadOverview(weddings);
            LoadWeddingCards(weddings);
            LoadTaskAnalytics(tasks);
            LoadTaskChart(weddings);
            LoadBudgetAnalytics(weddings);
            LoadBudgetChart(weddings);
            LoadRequestAnalytics(weddings);
            LoadTimeline(weddings, tasks);

            pnlEmpty.Visible = false;
        }

        private void LoadPortfolio(int plannerId)
        {
            decimal average = 0;
            int ratingCount = 0;
            bool hasRatings = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Portfolio_GetSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PlannerId", plannerId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ratingCount = Convert.ToInt32(reader["RatingCount"]);

                            if (reader["AvgRating"] != DBNull.Value)
                            {
                                average = Convert.ToDecimal(reader["AvgRating"]);
                                hasRatings = ratingCount > 0;
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                litPortfolioSummary.Text =
                    "<div class=\"budget-empty\">Portfolio ratings are unavailable right now.</div>";

                return;
            }

            decimal fillPercent = hasRatings
                ? Math.Round(average * 100m / 5m, 2)
                : 0;

            string averageText = hasRatings
                ? average.ToString("0.0", CultureInfo.InvariantCulture) + " / 5"
                : "–";

            string averageDetail = hasRatings
                ? "Across all rated tasks"
                : "No ratings yet";

            string countDetail = ratingCount == 1
                ? "Rating from your clients"
                : "Ratings from your clients";

            StringBuilder html = new StringBuilder();

            html.Append("<article class=\"overview-card\">");

            html.Append("<div class=\"overview-card-top\">");
            html.Append("<span class=\"card-label\">Average rating</span>");
            html.Append("</div>");

            html.Append("<strong>")
                .Append(HttpUtility.HtmlEncode(averageText))
                .Append("</strong>");

            html.Append("<div class=\"progress-track\">");

            html.Append("<div class=\"progress-fill\" style=\"width:")
                .Append(fillPercent.ToString("0.##", CultureInfo.InvariantCulture))
                .Append("%\"></div>");

            html.Append("</div>");

            html.Append("<span class=\"card-detail\">")
                .Append(HttpUtility.HtmlEncode(averageDetail))
                .Append("</span>");

            html.Append("</article>");

            html.Append("<article class=\"overview-card\">");

            html.Append("<div class=\"overview-card-top\">");
            html.Append("<span class=\"card-label\">Total ratings</span>");
            html.Append("</div>");

            html.Append("<strong>")
                .Append(ratingCount)
                .Append("</strong>");

            html.Append("<span class=\"card-detail\">")
                .Append(HttpUtility.HtmlEncode(countDetail))
                .Append("</span>");

            html.Append("</article>");

            litPortfolioSummary.Text = html.ToString();
        }

        private void LoadOverview(DataTable weddings)
        {
            int totalWeddings = weddings.Rows.Count;
            int totalTasks = 0;
            int completedTasks = 0;
            int pendingRequests = 0;
            decimal totalBudget = 0;

            foreach (DataRow row in weddings.Rows)
            {
                totalTasks += GetInt(row, "TotalTasks");
                completedTasks += GetInt(row, "CompletedTasks");
                pendingRequests += GetInt(row, "PendingRequests");

                if (row["TotalBudget"] != DBNull.Value)
                {
                    totalBudget += Convert.ToDecimal(row["TotalBudget"]);
                }
            }

            decimal taskPercentage = totalTasks == 0
                ? 0
                : Math.Round(
                    completedTasks * 100m / totalTasks,
                    2);

            litTotalWeddings.Text = totalWeddings.ToString();
            litTotalTasks.Text = totalTasks.ToString();
            litCompletedTasks.Text = completedTasks.ToString();
            litTaskPercentage.Text = taskPercentage.ToString("0.#") + "%";
            litTotalBudget.Text = FormatCurrency(totalBudget);
            litPendingRequests.Text = pendingRequests.ToString();
        }

        private void LoadWeddingCards(DataTable weddings)
        {
            StringBuilder html = new StringBuilder();

            foreach (DataRow row in weddings.Rows)
            {
                int weddingId = GetInt(row, "WeddingId");
                string displayName = Convert.ToString(row["DisplayName"]);
                string weddingDate = FormatDate(row["WeddingDate"]);

                int totalTasks = GetInt(row, "TotalTasks");
                int completedTasks = GetInt(row, "CompletedTasks");
                int pendingRequests = GetInt(row, "PendingRequests");

                decimal taskPercentage = totalTasks == 0
                    ? 0
                    : Math.Round(
                        completedTasks * 100m / totalTasks,
                        2);

                decimal totalBudget = 0;

                if (row["TotalBudget"] != DBNull.Value)
                {
                    totalBudget = Convert.ToDecimal(row["TotalBudget"]);
                }

                int daysToGo = 0;

                if (row["WeddingDate"] != DBNull.Value)
                {
                    DateTime date = Convert.ToDateTime(row["WeddingDate"]).Date;
                    daysToGo = (date - DateTime.Today).Days;
                }

                string daysText;

                if (daysToGo > 1)
                    daysText = daysToGo + " days to go";
                else if (daysToGo == 1)
                    daysText = "1 day to go";
                else if (daysToGo == 0)
                    daysText = "Wedding day";
                else
                    daysText = "Passed";

                html.Append("<article class=\"wedding-card\" data-wedding-id=\"")
                    .Append(weddingId)
                    .Append("\">");

                html.Append("<div class=\"wedding-card-top\">");

                html.Append("<div>");

                html.Append("<div class=\"wedding-card-name\">")
                    .Append(HttpUtility.HtmlEncode(displayName))
                    .Append("</div>");

                html.Append("<div class=\"wedding-card-date\">")
                    .Append(HttpUtility.HtmlEncode(weddingDate))
                    .Append("</div>");

                html.Append("</div>");

                html.Append("<span class=\"days-pill\">")
                    .Append(HttpUtility.HtmlEncode(daysText))
                    .Append("</span>");

                html.Append("</div>");

                html.Append("<div class=\"wedding-card-stats\">");

                html.Append("<div class=\"wedding-stat\">");
                html.Append("<span>Tasks</span>");
                html.Append("<strong>")
                    .Append(completedTasks)
                    .Append("/")
                    .Append(totalTasks)
                    .Append("</strong>");
                html.Append("</div>");

                html.Append("<div class=\"wedding-stat\">");
                html.Append("<span>Progress</span>");
                html.Append("<strong>")
                    .Append(taskPercentage.ToString("0.#"))
                    .Append("%</strong>");
                html.Append("</div>");

                html.Append("<div class=\"wedding-stat\">");
                html.Append("<span>Budget</span>");
                html.Append("<strong>");

                if (totalBudget > 0)
                    html.Append(FormatCurrency(totalBudget));
                else
                    html.Append("Not set");

                html.Append("</strong>");
                html.Append("</div>");

                html.Append("<div class=\"wedding-stat\">");
                html.Append("<span>Requests</span>");
                html.Append("<strong>")
                    .Append(pendingRequests)
                    .Append("</strong>");
                html.Append("</div>");

                html.Append("</div>");

                html.Append("<div class=\"wedding-progress\">");

                html.Append("<div class=\"progress-track\">");

                html.Append("<div class=\"progress-fill\" style=\"width:")
                    .Append(taskPercentage.ToString(
                        "0.##",
                        CultureInfo.InvariantCulture))
                    .Append("%\"></div>");

                html.Append("</div>");

                html.Append("</div>");

                html.Append("</article>");
            }

            litWeddings.Text = html.ToString();
        }

        private void LoadTaskAnalytics(DataTable tasks)
        {
            int totalTasks = 0;
            int completedTasks = 0;
            int overdueTasks = 0;

            foreach (DataRow row in tasks.Rows)
            {
                totalTasks++;

                if (GetBool(row, "IsComplete"))
                {
                    completedTasks++;
                }

                if (GetInt(row, "IsOverdue") == 1)
                {
                    overdueTasks++;
                }
            }

            int remainingTasks = totalTasks - completedTasks;

            decimal percentage = totalTasks == 0
                ? 0
                : Math.Round(
                    completedTasks * 100m / totalTasks,
                    2);


            litAnalyticsCompleted.Text =
                completedTasks.ToString();

            litAnalyticsRemaining.Text =
                remainingTasks.ToString();

            litAnalyticsOverdue.Text =
                overdueTasks.ToString();

            litTaskDonut.Text =
                "<div class=\"donut-chart\" style=\"--progress:" +
                percentage.ToString(
                    "0.##",
                    CultureInfo.InvariantCulture) +
                "%;\"><div class=\"donut-inner\"><strong>" +
                percentage.ToString("0.#") +
                "%</strong><span>complete</span></div></div>";
        }

        private void LoadTaskChart(DataTable weddings)
        {
            StringBuilder html = new StringBuilder();

            foreach (DataRow wedding in weddings.Rows)
            {
                string displayName =
                    Convert.ToString(wedding["DisplayName"]);

                int totalTasks =
                    GetInt(wedding, "TotalTasks");

                int completedTasks =
                    GetInt(wedding, "CompletedTasks");

                decimal percentage = totalTasks == 0
                    ? 0
                    : Math.Round(
                        completedTasks * 100m / totalTasks,
                        2);

                html.Append("<div class=\"chart-row\">");

                html.Append("<span class=\"chart-name\">")
                    .Append(HttpUtility.HtmlEncode(displayName))
                    .Append("</span>");

                html.Append("<div class=\"chart-track\">");

                html.Append("<div class=\"chart-bar\" style=\"width:")
                    .Append(percentage.ToString(
                        "0.##",
                        CultureInfo.InvariantCulture))
                    .Append("%\"></div>");

                html.Append("</div>");

                html.Append("<span class=\"chart-value\">")
                    .Append(percentage.ToString("0.#"))
                    .Append("%</span>");

                html.Append("</div>");
            }

            litTaskChart.Text = html.ToString();
        }

        private void LoadBudgetAnalytics(DataTable weddings)
        {
            decimal totalBudget = 0;

            foreach (DataRow row in weddings.Rows)
            {
                if (row["TotalBudget"] != DBNull.Value)
                {
                    totalBudget += Convert.ToDecimal(row["TotalBudget"]);
                }
            }

            if (totalBudget > 0)
            {
                litBudgetTotal.Text = FormatCurrency(totalBudget);
                litBudgetStatus.Text = "Budget planned";
            }
            else
            {
                litBudgetTotal.Text = "No budget yet";
                litBudgetStatus.Text = "No budget yet";
            }
        }

        private void LoadBudgetChart(DataTable weddings)
        {
            StringBuilder html = new StringBuilder();

            decimal maximumBudget = 0;

            foreach (DataRow wedding in weddings.Rows)
            {
                if (wedding["TotalBudget"] == DBNull.Value)
                    continue;

                decimal budget =
                    Convert.ToDecimal(wedding["TotalBudget"]);

                if (budget > maximumBudget)
                {
                    maximumBudget = budget;
                }
            }

            if (maximumBudget <= 0)
            {
                litBudgetChart.Text =
                    "<div class=\"budget-empty\">No budget data available yet.</div>";

                return;
            }

            foreach (DataRow wedding in weddings.Rows)
            {
                string displayName =
                    Convert.ToString(wedding["DisplayName"]);

                decimal budget = 0;

                if (wedding["TotalBudget"] != DBNull.Value)
                {
                    budget =
                        Convert.ToDecimal(wedding["TotalBudget"]);
                }

                decimal percentage = budget <= 0
                    ? 0
                    : Math.Round(
                        budget * 100m / maximumBudget,
                        2);

                html.Append("<div class=\"budget-row\">");

                html.Append("<span class=\"budget-name\">")
                    .Append(HttpUtility.HtmlEncode(displayName))
                    .Append("</span>");

                html.Append("<div class=\"budget-track\">");

                html.Append("<div class=\"budget-bar\" style=\"width:")
                    .Append(percentage.ToString(
                        "0.##",
                        CultureInfo.InvariantCulture))
                    .Append("%\"></div>");

                html.Append("</div>");

                html.Append("<span class=\"budget-value\">");

                if (budget > 0)
                {
                    html.Append(FormatCurrency(budget));
                }
                else
                {
                    html.Append("Not set");
                }

                html.Append("</span>");

                html.Append("</div>");
            }

            litBudgetChart.Text = html.ToString();
        }

        private void LoadRequestAnalytics(DataTable weddings)
        {
            int pendingRequests = 0;

            foreach (DataRow row in weddings.Rows)
            {
                pendingRequests +=
                    GetInt(row, "PendingRequests");
            }

            litRequestsTotal.Text =
                pendingRequests.ToString();

            if (pendingRequests == 0)
            {
                litRequestsStatus.Text =
                    "You're all caught up";
            }
            else if (pendingRequests == 1)
            {
                litRequestsStatus.Text =
                    "1 request needs attention";
            }
            else
            {
                litRequestsStatus.Text =
                    pendingRequests +
                    " requests need attention";
            }
        }


        private void LoadTimeline(DataTable weddings, DataTable tasks)
        {
            if (weddings.Rows.Count == 0)
            {
                litTimeline.Text =
                    "<div class=\"timeline-empty\">No timeline data available.</div>";
                return;
            }

            DateTime today = DateTime.Today;
            DateTime earliestDate = today.AddDays(-30);
            DateTime latestDate = today.AddMonths(5);

            foreach (DataRow wedding in weddings.Rows)
            {
                if (wedding["WeddingDate"] != DBNull.Value)
                {
                    DateTime weddingDate = Convert.ToDateTime(wedding["WeddingDate"]).Date;

                    if (weddingDate < earliestDate)
                        earliestDate = weddingDate;

                    if (weddingDate > latestDate)
                        latestDate = weddingDate;
                }
            }

            foreach (DataRow task in tasks.Rows)
            {
                if (task["DueDate"] != DBNull.Value)
                {
                    DateTime dueDate = Convert.ToDateTime(task["DueDate"]).Date;

                    if (dueDate < earliestDate)
                        earliestDate = dueDate;

                    if (dueDate > latestDate)
                        latestDate = dueDate;
                }
            }

            earliestDate = new DateTime(
                earliestDate.Year,
                earliestDate.Month,
                1);

            latestDate = new DateTime(
                latestDate.Year,
                latestDate.Month,
                DateTime.DaysInMonth(latestDate.Year, latestDate.Month));

            int totalDays = Math.Max(1, (latestDate - earliestDate).Days);

            StringBuilder html = new StringBuilder();

            html.Append("<div class=\"planner-timeline\">");

            html.Append("<div class=\"timeline-scroll\">");

            html.Append("<div class=\"timeline-canvas\">");

            html.Append("<div class=\"timeline-axis\">");

            DateTime axisMonth = earliestDate;

            while (axisMonth <= latestDate)
            {
                int monthOffset = (axisMonth - earliestDate).Days;

                decimal monthPosition =
                    monthOffset * 100m / totalDays;

                html.Append("<div class=\"timeline-month\" style=\"left:")
                    .Append(monthPosition.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("%\">");

                html.Append("<span>")
                    .Append(axisMonth.ToString("MMM", CultureInfo.InvariantCulture))
                    .Append("</span>");

                html.Append("<small>")
                    .Append(axisMonth.Year)
                    .Append("</small>");

                html.Append("</div>");

                axisMonth = axisMonth.AddMonths(1);
            }

            html.Append("</div>");

            int todayOffset = (today - earliestDate).Days;

            if (todayOffset >= 0 && todayOffset <= totalDays)
            {
                decimal todayPosition =
                    todayOffset * 100m / totalDays;

                html.Append("<div class=\"timeline-today\" style=\"left:")
                    .Append(todayPosition.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("%\">");

                html.Append("<span>Today</span>");
                html.Append("</div>");
            }

            html.Append("<div class=\"timeline-lanes\">");

            foreach (DataRow wedding in weddings.Rows)
            {
                int weddingId = GetInt(wedding, "WeddingId");
                string displayName = Convert.ToString(wedding["DisplayName"]);
                string weddingDateText = FormatDate(wedding["WeddingDate"]);

                DateTime weddingDate = today;

                if (wedding["WeddingDate"] != DBNull.Value)
                    weddingDate = Convert.ToDateTime(wedding["WeddingDate"]).Date;

                DataRow[] weddingTasks =
                    tasks.Select("WeddingId = " + weddingId);

                html.Append("<div class=\"timeline-lane\" data-wedding-id=\"")
                    .Append(weddingId)
                    .Append("\">");

                html.Append("<div class=\"timeline-lane-info\">");

                html.Append("<strong>")
                    .Append(HttpUtility.HtmlEncode(displayName))
                    .Append("</strong>");

                html.Append("<span>")
                    .Append(HttpUtility.HtmlEncode(weddingDateText))
                    .Append("</span>");

                html.Append("</div>");

                html.Append("<div class=\"timeline-lane-track\">");

                foreach (DataRow task in weddingTasks)
                {
                    if (task["DueDate"] == DBNull.Value)
                        continue;

                    DateTime dueDate =
                        Convert.ToDateTime(task["DueDate"]).Date;

                    int dayOffset = (dueDate - earliestDate).Days;

                    decimal position =
                        dayOffset * 100m / totalDays;

                    if (position < 0)
                        position = 0;

                    if (position > 100)
                        position = 100;

                    bool complete = GetBool(task, "IsComplete");
                    bool overdue = GetInt(task, "IsOverdue") == 1;

                    string stateClass;

                    if (complete)
                        stateClass = "done";
                    else if (overdue)
                        stateClass = "overdue";
                    else
                        stateClass = "upcoming";

                    string title = Convert.ToString(task["Title"]);
                    string taskType = Convert.ToString(task["TaskType"]);
                    string status = Convert.ToString(task["Status"]);

                    html.Append("<button type=\"button\" ")
                        .Append("class=\"timeline-milestone ")
                        .Append(stateClass)
                        .Append("\" ")
                        .Append("style=\"left:")
                        .Append(position.ToString("0.##", CultureInfo.InvariantCulture))
                        .Append("%\" ")
                        .Append("data-task-id=\"")
                        .Append(GetInt(task, "TaskId"))
                        .Append("\" ")
                        .Append("data-wedding-id=\"")
                        .Append(weddingId)
                        .Append("\" ")
                        .Append("title=\"")
                        .Append(HttpUtility.HtmlAttributeEncode(
                            title + " • " + dueDate.ToString("MMMM d, yyyy")))
                        .Append("\">");

                    html.Append("<span class=\"milestone-dot\"></span>");

                    html.Append("<span class=\"milestone-tooltip\">");

                    html.Append("<strong>")
                        .Append(HttpUtility.HtmlEncode(title))
                        .Append("</strong>");

                    html.Append("<span>")
                        .Append(HttpUtility.HtmlEncode(taskType))
                        .Append("</span>");

                    html.Append("<small>")
                        .Append(dueDate.ToString("MMM d, yyyy", CultureInfo.InvariantCulture))
                        .Append(" · ")
                        .Append(HttpUtility.HtmlEncode(status))
                        .Append("</small>");

                    html.Append("</span>");

                    html.Append("</button>");
                }

                int pendingRequests = GetInt(wedding, "PendingRequests");

                if (pendingRequests > 0)
                {
                    int requestOffset = (today - earliestDate).Days;

                    decimal requestPosition =
                        requestOffset * 100m / totalDays;

                    if (requestPosition < 0)
                        requestPosition = 0;

                    if (requestPosition > 100)
                        requestPosition = 100;

                    html.Append("<button type=\"button\" ")
                        .Append("class=\"timeline-request\" ")
                        .Append("style=\"left:")
                        .Append(requestPosition.ToString("0.##", CultureInfo.InvariantCulture))
                        .Append("%\" ")
                        .Append("title=\"")
                        .Append(pendingRequests)
                        .Append(pendingRequests == 1
                            ? " pending request"
                            : " pending requests")
                        .Append("\">");

                    html.Append("<span></span>");

                    html.Append("<small>")
                        .Append(pendingRequests)
                        .Append("</small>");

                    html.Append("</button>");
                }

                if (weddingDate >= earliestDate &&
                    weddingDate <= latestDate)
                {
                    int weddingOffset =
                        (weddingDate - earliestDate).Days;

                    decimal weddingPosition =
                        weddingOffset * 100m / totalDays;

                    html.Append("<button type=\"button\" ")
                        .Append("class=\"timeline-wedding-day\" ")
                        .Append("style=\"left:")
                        .Append(weddingPosition.ToString("0.##", CultureInfo.InvariantCulture))
                        .Append("%\" ")
                        .Append("title=\"Wedding day · ")
                        .Append(HttpUtility.HtmlAttributeEncode(weddingDateText))
                        .Append("\">");

                    html.Append("<span>◆</span>");

                    html.Append("</button>");
                }

                html.Append("</div>");
                html.Append("</div>");
            }

            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");

            litTimeline.Text = html.ToString();
        }


        protected void btnSignOut_Click(
            object sender,
            EventArgs e)
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect(
                "~/24-1528LibrandaMidLogin.aspx");
        }

        private void ClearDashboard()
        {
            litGreeting.Text = "Welcome back, Planner.";
            litSubtext.Text =
                "Your wedding planning workspace is ready.";

            litTotalWeddings.Text = "0";
            litTotalTasks.Text = "0";
            litCompletedTasks.Text = "0";
            litTaskPercentage.Text = "0%";
            litTotalBudget.Text = "No budget yet";
            litPendingRequests.Text = "0";


            litAnalyticsCompleted.Text = "0";
            litAnalyticsRemaining.Text = "0";
            litAnalyticsOverdue.Text = "0";

            litTaskDonut.Text =
                "<div class=\"donut-chart\" style=\"--progress:0%;\">" +
                "<div class=\"donut-inner\">" +
                "<strong>0%</strong>" +
                "<span>complete</span>" +
                "</div></div>";

            litBudgetTotal.Text = "No budget yet";
            litBudgetStatus.Text = "No budget yet";

            litRequestsTotal.Text = "0";
            litRequestsStatus.Text = "You're all caught up";

            litWeddings.Text = "";

            litTaskChart.Text =
                "<div class=\"budget-empty\">No task data available yet.</div>";

            litBudgetChart.Text =
                "<div class=\"budget-empty\">No budget data available yet.</div>";

            litTimeline.Text =
                "<div class=\"timeline-empty\">No timeline data available.</div>";

            pnlEmpty.Visible = true;
        }

        private string FormatCurrency(decimal value)
        {
            return "₱" +
                   value.ToString(
                       "#,##0.00",
                       CultureInfo.InvariantCulture);
        }

        private string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "No date";

            DateTime date;

            if (!DateTime.TryParse(
                Convert.ToString(value),
                out date))
            {
                return "No date";
            }

            return date.ToString(
                "MMMM d, yyyy",
                CultureInfo.InvariantCulture);
        }

        private int GetInt(
            DataRow row,
            string column)
        {
            if (!row.Table.Columns.Contains(column))
                return 0;

            if (row[column] == DBNull.Value)
                return 0;

            int value;

            return int.TryParse(
                Convert.ToString(row[column]),
                out value)
                ? value
                : 0;
        }

        private bool GetBool(
            DataRow row,
            string column)
        {
            if (!row.Table.Columns.Contains(column))
                return false;

            if (row[column] == DBNull.Value)
                return false;

            if (row[column] is bool)
                return Convert.ToBoolean(row[column]);

            int value;

            if (int.TryParse(
                Convert.ToString(row[column]),
                out value))
            {
                return value == 1;
            }

            bool result;

            return bool.TryParse(
                Convert.ToString(row[column]),
                out result) && result;
        }
    }
}