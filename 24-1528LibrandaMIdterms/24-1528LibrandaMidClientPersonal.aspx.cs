
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Bouqet
{
    public partial class _24_1528LibrandaMidClientPersonal : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["BouqetDB"].ConnectionString;

        private int EditingTaskId
        {
            get
            {
                int taskId;
                return int.TryParse(Convert.ToString(ViewState["EditingTaskId"]), out taskId)
                    ? taskId
                    : 0;
            }
            set { ViewState["EditingTaskId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int plannerId;
            if (!TryGetPlannerId(out plannerId))
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!string.Equals(GetUserRole(), "Planner", StringComparison.OrdinalIgnoreCase))
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadClient(plannerId);
                LoadTasks();
            }
        }

        private string GetUserRole()
        {
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null || identity.Ticket == null)
                return string.Empty;

            string[] parts = (identity.Ticket.UserData ?? string.Empty).Split('|');
            return parts.Length > 0 ? parts[0] : string.Empty;
        }

        private bool TryGetPlannerId(out int plannerId)
        {
            plannerId = 0;
            FormsIdentity identity = User.Identity as FormsIdentity;

            if (identity == null || identity.Ticket == null)
                return false;

            string[] parts = (identity.Ticket.UserData ?? string.Empty).Split('|');

            return parts.Length >= 3
                && string.Equals(parts[0], "Planner", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(parts[2], out plannerId);
        }

        private int GetClientId()
        {
            int clientId;

            if (!int.TryParse(Request.QueryString["clientId"], out clientId) || clientId <= 0)
            {
                ShowError("The selected client could not be found.");
                return 0;
            }

            return clientId;
        }

        private void LoadClient(int plannerId)
        {
            int clientId = GetClientId();
            if (clientId <= 0)
                return;

            try
            {
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
                            ShowError("The selected client could not be found.");
                            return;
                        }

                        string displayName = Convert.ToString(reader["DisplayName"]);
                        string partner1First = Convert.ToString(reader["Partner1FirstName"]);
                        string partner1Last = Convert.ToString(reader["Partner1LastName"]);
                        string partner2First = Convert.ToString(reader["Partner2FirstName"]);
                        string partner2Last = Convert.ToString(reader["Partner2LastName"]);

                        litClientName.Text = HttpUtility.HtmlEncode(displayName);
                        litPartnerOne.Text = HttpUtility.HtmlEncode(BuildFullName(partner1First, partner1Last));
                        litPartnerTwo.Text = HttpUtility.HtmlEncode(BuildFullName(partner2First, partner2Last));

                        string weddingDate = FormatDate(reader["WeddingDate"]);
                        string weddingStatus = Convert.ToString(reader["WeddingStatus"]);

                        litWeddingDate.Text = HttpUtility.HtmlEncode(weddingDate);
                        litWeddingStatus.Text = HttpUtility.HtmlEncode(weddingStatus);
                        litOverviewWeddingDate.Text = HttpUtility.HtmlEncode(weddingDate);
                        litOverviewWeddingStatus.Text = HttpUtility.HtmlEncode(weddingStatus);

                        txtPartner1FirstName.Text = partner1First;
                        txtPartner1LastName.Text = partner1Last;
                        txtPartner2FirstName.Text = partner2First;
                        txtPartner2LastName.Text = partner2Last;

                        if (reader["WeddingDate"] != DBNull.Value)
                        {
                            txtClientWeddingDate.Text =
                                Convert.ToDateTime(reader["WeddingDate"]).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtClientWeddingDate.Text = string.Empty;
                        }

                        txtClientVenue.Text =
                            reader["Venue"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Venue"]);

                        txtEstimatedGuests.Text =
                            reader["EstimatedGuests"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(reader["EstimatedGuests"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't load the client right now. " + ex.Message);
            }
        }

        protected void btnEditClientDetails_Click(object sender, EventArgs e)
        {
            int plannerId;
            if (!TryGetPlannerId(out plannerId))
                return;

            LoadClient(plannerId);
            pnlClientDetailsForm.Visible = true;
        }

        protected void btnCancelClientDetails_Click(object sender, EventArgs e)
        {
            int plannerId;
            if (TryGetPlannerId(out plannerId))
                LoadClient(plannerId);

            pnlClientDetailsForm.Visible = false;
        }

        protected void btnSaveClientDetails_Click(object sender, EventArgs e)
        {
            int plannerId;
            if (!TryGetPlannerId(out plannerId))
                return;

            int clientId = GetClientId();
            if (clientId <= 0)
                return;

            string partner1First = txtPartner1FirstName.Text.Trim();
            string partner1Last = txtPartner1LastName.Text.Trim();
            string partner2First = txtPartner2FirstName.Text.Trim();
            string partner2Last = txtPartner2LastName.Text.Trim();
            string venue = txtClientVenue.Text.Trim();

            if (string.IsNullOrWhiteSpace(partner1First) ||
                string.IsNullOrWhiteSpace(partner1Last) ||
                string.IsNullOrWhiteSpace(partner2First) ||
                string.IsNullOrWhiteSpace(partner2Last))
            {
                ShowError("Enter the first and last names for both partners.");
                pnlClientDetailsForm.Visible = true;
                return;
            }

            if (partner1First.Length > 50 || partner1Last.Length > 50 ||
                partner2First.Length > 50 || partner2Last.Length > 50)
            {
                ShowError("Partner names cannot exceed 50 characters per field.");
                pnlClientDetailsForm.Visible = true;
                return;
            }

            if (venue.Length > 150)
            {
                ShowError("The venue cannot exceed 150 characters.");
                pnlClientDetailsForm.Visible = true;
                return;
            }

            DateTime weddingDate;
            if (!DateTime.TryParseExact(
                txtClientWeddingDate.Text.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out weddingDate))
            {
                ShowError("Enter a valid wedding date.");
                pnlClientDetailsForm.Visible = true;
                return;
            }

            object estimatedGuests = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtEstimatedGuests.Text))
            {
                int guests;
                if (!int.TryParse(txtEstimatedGuests.Text.Trim(), out guests) || guests <= 0)
                {
                    ShowError("Estimated guests must be a positive whole number, or left blank.");
                    pnlClientDetailsForm.Visible = true;
                    return;
                }

                estimatedGuests = guests;
            }

            int weddingId = GetWeddingId(plannerId);
            if (weddingId <= 0)
            {
                pnlClientDetailsForm.Visible = true;
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand clientCommand =
                               new SqlCommand("dbo.sp_Client_Update", connection, transaction))
                        {
                            clientCommand.CommandType = CommandType.StoredProcedure;
                            clientCommand.CommandTimeout = 10;

                            clientCommand.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                            clientCommand.Parameters.Add("@ClientId", SqlDbType.Int).Value = clientId;
                            clientCommand.Parameters.Add("@Partner1FirstName", SqlDbType.NVarChar, 50).Value = partner1First;
                            clientCommand.Parameters.Add("@Partner1LastName", SqlDbType.NVarChar, 50).Value = partner1Last;
                            clientCommand.Parameters.Add("@Partner2FirstName", SqlDbType.NVarChar, 50).Value = partner2First;
                            clientCommand.Parameters.Add("@Partner2LastName", SqlDbType.NVarChar, 50).Value = partner2Last;

                            clientCommand.ExecuteNonQuery();
                        }

                        using (SqlCommand weddingCommand =
                               new SqlCommand("dbo.sp_Wedding_Update", connection, transaction))
                        {
                            weddingCommand.CommandType = CommandType.StoredProcedure;
                            weddingCommand.CommandTimeout = 10;

                            weddingCommand.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                            weddingCommand.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;
                            weddingCommand.Parameters.Add("@WeddingDate", SqlDbType.Date).Value = weddingDate.Date;
                            weddingCommand.Parameters.Add("@Venue", SqlDbType.NVarChar, 150).Value =
                                string.IsNullOrWhiteSpace(venue) ? (object)DBNull.Value : venue;
                            weddingCommand.Parameters.Add("@EstimatedGuests", SqlDbType.Int).Value = estimatedGuests;

                            weddingCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        try { transaction.Rollback(); }
                        catch { }
                        throw;
                    }
                }
            }

            pnlClientDetailsForm.Visible = false;
            LoadClient(plannerId);
            LoadTasks();
            ShowSuccess("Client and wedding details updated successfully.");
        }

        private void LoadTasks()
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                int weddingId = GetWeddingId(plannerId);
                if (weddingId <= 0)
                    return;

                LoadBudgetSummary(plannerId, weddingId);

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_ListForPlanner", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;

                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        rptTasks.DataSource = table;
                        rptTasks.DataBind();
                        litTaskCount.Text = table.Rows.Count.ToString();
                        pnlNoTasks.Visible = table.Rows.Count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't load the tasks right now. " + ex.Message);
            }
        }

        private void LoadBudgetSummary(int plannerId, int weddingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Budget_GetSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = DBNull.Value;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return;

                        decimal totalBudget = reader["TotalBudget"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalBudget"]);
                        decimal totalSpent = reader["TotalSpent"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalSpent"]);
                        decimal remaining = reader["Remaining"] == DBNull.Value ? totalBudget - totalSpent : Convert.ToDecimal(reader["Remaining"]);
                        decimal percentUsed = reader["PercentUsed"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PercentUsed"]);
                        string alertLevel = Convert.ToString(reader["AlertLevel"]);

                        litTotalBudget.Text = FormatCurrency(totalBudget);
                        litTotalSpent.Text = FormatCurrency(totalSpent);
                        litRemaining.Text = FormatCurrency(remaining);
                        litPercentUsed.Text = percentUsed.ToString("0.0", CultureInfo.InvariantCulture) + "%";

                        budgetProgressFill.Style["width"] =
                            Math.Min(Math.Max(percentUsed, 0), 100)
                                .ToString("0.0", CultureInfo.InvariantCulture) + "%";

                        budgetProgressFill.Attributes["class"] =
                            "budget-progress-fill " + GetBudgetProgressClass(alertLevel);

                        litBudgetAlert.Text = GetBudgetAlertText(alertLevel, remaining);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't load the budget summary. " + ex.Message);
            }
        }

        protected void btnEditBudget_Click(object sender, EventArgs e)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                int weddingId = GetWeddingId(plannerId);
                if (weddingId <= 0)
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Budget_GetSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@ClientId", SqlDbType.Int).Value = DBNull.Value;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read() && reader["TotalBudget"] != DBNull.Value)
                        {
                            txtTotalBudget.Text = Convert.ToDecimal(reader["TotalBudget"])
                                .ToString("0.00", CultureInfo.InvariantCulture);
                        }
                    }
                }

                pnlBudgetForm.Visible = true;
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't open the budget editor. " + ex.Message);
            }
        }

        protected void btnCancelBudget_Click(object sender, EventArgs e)
        {
            pnlBudgetForm.Visible = false;
            txtTotalBudget.Text = string.Empty;
        }

        protected void btnSaveBudget_Click(object sender, EventArgs e)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                decimal totalBudget;
                if (!decimal.TryParse(txtTotalBudget.Text, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out totalBudget))
                {
                    ShowError("Please enter a valid budget.");
                    return;
                }

                if (totalBudget < 0)
                {
                    ShowError("Budget cannot be negative.");
                    return;
                }

                int weddingId = GetWeddingId(plannerId);
                if (weddingId <= 0)
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Wedding_UpdateBudget", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;

                    SqlParameter budgetParameter = command.Parameters.Add("@TotalBudget", SqlDbType.Decimal);
                    budgetParameter.Precision = 12;
                    budgetParameter.Scale = 2;
                    budgetParameter.Value = totalBudget;

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                pnlBudgetForm.Visible = false;
                txtTotalBudget.Text = string.Empty;
                LoadTasks();
                ShowSuccess("Wedding budget updated successfully.");
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't update the budget. " + ex.Message);
            }
        }

        protected void btnShowTaskForm_Click(object sender, EventArgs e)
        {
            EditingTaskId = 0;
            litTaskFormTitle.Text = "Add Task";
            btnSaveTask.Text = "Save Task";
            ClearTaskDetails();
            taskDetailsPanel.Visible = false;
            pnlVenueDetails.Visible = false;
            pnlTaskForm.Visible = true;
        }

        protected void ddlTaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            taskDetailsPanel.Visible = true;
            pnlVenueDetails.Visible = string.Equals(ddlTaskType.SelectedValue, "Venue", StringComparison.OrdinalIgnoreCase);

            if (!pnlVenueDetails.Visible)
                ClearTaskDetails();
        }

        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                int weddingId = GetWeddingId(plannerId);
                if (weddingId <= 0)
                    return;

                string taskType = ddlTaskType.SelectedValue.Trim();
                string title = txtTaskTitle.Text.Trim();
                string notes = txtPlannerNotes.Text.Trim();
                decimal cost;

                if (string.IsNullOrWhiteSpace(taskType))
                {
                    ShowError("Please select a task type.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    ShowError("Please enter a task title.");
                    return;
                }

                if (!decimal.TryParse(txtCost.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out cost))
                {
                    ShowError("Please enter a valid cost.");
                    return;
                }

                if (cost < 0)
                {
                    ShowError("Task cost cannot be negative.");
                    return;
                }

                DateTime dueDate;
                object dueDateValue = DBNull.Value;

                if (!string.IsNullOrWhiteSpace(txtDueDate.Text))
                {
                    if (!DateTime.TryParseExact(txtDueDate.Text, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate))
                    {
                        ShowError("Please enter a valid due date.");
                        return;
                    }

                    dueDateValue = dueDate.Date;
                }

                if (EditingTaskId > 0)
                {
                    UpdateTask(plannerId, EditingTaskId, taskType, title, dueDateValue, notes, cost);
                    SaveTaskDetails(plannerId, EditingTaskId, taskType);
                    ClearTaskForm();
                    LoadTasks();
                    ShowSuccess("Task updated successfully.");
                    return;
                }

                int newTaskId = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_Add", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@WeddingId", SqlDbType.Int).Value = weddingId;
                    command.Parameters.Add("@TaskType", SqlDbType.NVarChar, 30).Value = taskType;
                    command.Parameters.Add("@Title", SqlDbType.NVarChar, 150).Value = title;
                    command.Parameters.Add("@DueDate", SqlDbType.Date).Value = dueDateValue;
                    command.Parameters.Add("@PlannerNotes", SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes;

                    SqlParameter costParameter = command.Parameters.Add("@Cost", SqlDbType.Decimal);
                    costParameter.Precision = 12;
                    costParameter.Scale = 2;
                    costParameter.Value = cost;

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        newTaskId = Convert.ToInt32(result);
                }

                if (newTaskId <= 0)
                    throw new InvalidOperationException("The task ID could not be retrieved.");

                SaveTaskDetails(plannerId, newTaskId, taskType);
                ClearTaskForm();
                LoadTasks();
                ShowSuccess("Task added successfully.");
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't save the task. " + ex.Message);
            }
        }

        private void EditTask(int taskId)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_GetForPlanner", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowError("The selected task could not be found.");
                            return;
                        }

                        string status = Convert.ToString(reader["Status"]);

                        if (string.Equals(status, "Done", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(status, "Confirmed", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(status, "Removed", StringComparison.OrdinalIgnoreCase))
                        {
                            ShowError("This task can no longer be edited.");
                            return;
                        }

                        EditingTaskId = taskId;

                        string taskType = Convert.ToString(reader["TaskType"]);
                        if (ddlTaskType.Items.FindByValue(taskType) != null)
                            ddlTaskType.SelectedValue = taskType;
                        else
                            ddlTaskType.SelectedIndex = 0;

                        txtTaskTitle.Text = Convert.ToString(reader["Title"]);
                        txtCost.Text = Convert.ToDecimal(reader["Cost"]).ToString("0.00", CultureInfo.InvariantCulture);
                        txtDueDate.Text = reader["DueDate"] == DBNull.Value
                            ? string.Empty
                            : Convert.ToDateTime(reader["DueDate"]).ToString("yyyy-MM-dd");
                        txtPlannerNotes.Text = Convert.ToString(reader["PlannerNotes"]);

                        litTaskFormTitle.Text = "Edit Task";
                        btnSaveTask.Text = "Update Task";
                        pnlTaskForm.Visible = true;
                    }
                }

                LoadTaskDetails(plannerId, taskId, ddlTaskType.SelectedValue);
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't load the task for editing. " + ex.Message);
            }
        }

        private void UpdateTask(int plannerId, int taskId, string taskType, string title,
            object dueDate, string notes, decimal cost)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.sp_Task_Update", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                command.Parameters.Add("@TaskType", SqlDbType.NVarChar, 30).Value = taskType;
                command.Parameters.Add("@Title", SqlDbType.NVarChar, 150).Value = title;
                command.Parameters.Add("@DueDate", SqlDbType.Date).Value = dueDate;
                command.Parameters.Add("@PlannerNotes", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes;

                SqlParameter costParameter = command.Parameters.Add("@Cost", SqlDbType.Decimal);
                costParameter.Precision = 12;
                costParameter.Scale = 2;
                costParameter.Value = cost;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void LoadTaskDetails(int plannerId, int taskId, string taskType)
        {
            taskDetailsPanel.Visible = true;
            pnlVenueDetails.Visible = string.Equals(taskType, "Venue", StringComparison.OrdinalIgnoreCase);
            ClearTaskDetails();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.sp_Task_GetDetailsForPlanner", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 10;
                command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fieldName = Convert.ToString(reader["FieldName"]);
                        string fieldValue = Convert.ToString(reader["FieldValue"]);

                        switch (fieldName)
                        {
                            case "Address": txtVenueAddress.Text = fieldValue; break;
                            case "RentalStart": txtRentalStart.Text = NormalizeTimeValue(fieldValue); break;
                            case "RentalEnd": txtRentalEnd.Text = NormalizeTimeValue(fieldValue); break;
                            case "Pax": txtVenuePax.Text = fieldValue; break;
                        }
                    }
                }
            }
        }

        private void SaveTaskDetails(int plannerId, int taskId, string taskType)
        {
            if (!string.Equals(taskType, "Venue", StringComparison.OrdinalIgnoreCase))
                return;

            SaveTaskDetail(plannerId, taskId, "Address", txtVenueAddress.Text.Trim());
            SaveTaskDetail(plannerId, taskId, "RentalStart", txtRentalStart.Text.Trim());
            SaveTaskDetail(plannerId, taskId, "RentalEnd", txtRentalEnd.Text.Trim());
            SaveTaskDetail(plannerId, taskId, "Pax", txtVenuePax.Text.Trim());
        }

        private void SaveTaskDetail(int plannerId, int taskId, string fieldName, string fieldValue)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.sp_Task_SetDetail", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 10;
                command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                command.Parameters.Add("@FieldName", SqlDbType.NVarChar, 50).Value = fieldName;
                command.Parameters.Add("@FieldValue", SqlDbType.NVarChar, 300).Value =
                    string.IsNullOrWhiteSpace(fieldValue) ? (object)DBNull.Value : fieldValue;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private string NormalizeTimeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            TimeSpan time;
            if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out time))
                return time.ToString(@"hh\:mm");

            DateTime dateTime;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                return dateTime.ToString("HH:mm");

            return value;
        }

        private void ClearTaskDetails()
        {
            txtVenueAddress.Text = string.Empty;
            txtRentalStart.Text = string.Empty;
            txtRentalEnd.Text = string.Empty;
            txtVenuePax.Text = string.Empty;
        }

        protected void btnCancelTask_Click(object sender, EventArgs e)
        {
            ClearTaskForm();
        }

        private void ClearTaskForm()
        {
            EditingTaskId = 0;
            ddlTaskType.SelectedIndex = 0;
            txtTaskTitle.Text = string.Empty;
            txtCost.Text = string.Empty;
            txtDueDate.Text = string.Empty;
            txtPlannerNotes.Text = string.Empty;
            ClearTaskDetails();
            taskDetailsPanel.Visible = false;
            pnlVenueDetails.Visible = false;
            litTaskFormTitle.Text = "Add Task";
            btnSaveTask.Text = "Save Task";
            pnlTaskForm.Visible = false;
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int taskId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out taskId))
            {
                ShowError("Invalid task.");
                return;
            }

            switch (e.CommandName)
            {
                case "EditTask": EditTask(taskId); break;
                case "ShareTask": ShareTask(taskId); break;
                case "MarkDoneTask": MarkTaskDone(taskId); break;
                case "RemoveTask": RemoveTask(taskId); break;
            }
        }

        private void ShareTask(int taskId)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_Share", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadTasks();
                ShowSuccess("Task shared with the client.");
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't share the task right now. " + ex.Message);
            }
        }

        private void MarkTaskDone(int taskId)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_MarkDone", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadTasks();
                ShowSuccess("Task marked as done. The client can now rate it or request changes.");
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't mark the task as done. " + ex.Message);
            }
        }

        private void RemoveTask(int taskId)
        {
            try
            {
                int plannerId;
                if (!TryGetPlannerId(out plannerId))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("dbo.sp_Task_Remove", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 10;
                    command.Parameters.Add("@PlannerId", SqlDbType.Int).Value = plannerId;
                    command.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadTasks();
                ShowSuccess("Task removed successfully.");
            }
            catch (Exception ex)
            {
                Trace.Warn(ex.ToString());
                ShowError("We couldn't remove the task right now. " + ex.Message);
            }
        }

        private int GetWeddingId(int plannerId)
        {
            int clientId = GetClientId();
            if (clientId <= 0)
                return 0;

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
                        ShowError("The selected client could not be found.");
                        return 0;
                    }

                    return Convert.ToInt32(reader["WeddingId"]);
                }
            }
        }

        protected string FormatTaskDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "No due date";

            DateTime date;
            if (!DateTime.TryParse(Convert.ToString(value), out date))
                return "No due date";

            return date.ToString("MMM d, yyyy");
        }

        protected string FormatTaskCost(object value)
        {
            decimal cost;
            if (value == null || value == DBNull.Value ||
                !decimal.TryParse(Convert.ToString(value), out cost))
                cost = 0;

            return FormatCurrency(cost);
        }

        protected string GetTaskStatusClass(object status, object isComplete, object isOverdue)
        {
            bool complete = isComplete != null && isComplete != DBNull.Value && Convert.ToBoolean(isComplete);
            bool overdue = isOverdue != null && isOverdue != DBNull.Value && Convert.ToBoolean(isOverdue);
            string value = Convert.ToString(status);

            if (complete || string.Equals(value, "Done", StringComparison.OrdinalIgnoreCase))
                return "task-status task-status-complete";
            if (overdue)
                return "task-status task-status-overdue";

            switch (value)
            {
                case "Shared": return "task-status task-status-sent";
                case "Confirmed": return "task-status task-status-confirmed";
                case "Removed": return "task-status task-status-removed";
                default: return "task-status task-status-pending";
            }
        }

        protected string GetTaskStatusText(object status, object isComplete, object isOverdue)
        {
            bool complete = isComplete != null && isComplete != DBNull.Value && Convert.ToBoolean(isComplete);
            bool overdue = isOverdue != null && isOverdue != DBNull.Value && Convert.ToBoolean(isOverdue);
            string value = Convert.ToString(status);

            if (complete || string.Equals(value, "Done", StringComparison.OrdinalIgnoreCase))
                return "Completed";
            if (overdue)
                return "Overdue";

            switch (value)
            {
                case "Shared": return "Sent";
                case "Confirmed": return "Confirmed";
                case "Removed": return "Removed";
                default: return "Pending";
            }
        }

        protected bool CanShareTask(object status)
        {
            return string.Equals(Convert.ToString(status), "Draft", StringComparison.OrdinalIgnoreCase);
        }

        protected bool CanMarkTaskDone(object status)
        {
            return string.Equals(Convert.ToString(status), "Shared", StringComparison.OrdinalIgnoreCase);
        }

        protected bool CanEditTask(object status)
        {
            string value = Convert.ToString(status);
            return string.Equals(value, "Draft", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(value, "Shared", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildFullName(object firstName, object lastName)
        {
            return (Convert.ToString(firstName).Trim() + " " +
                    Convert.ToString(lastName).Trim()).Trim();
        }

        private string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "Not set";

            DateTime date;
            if (!DateTime.TryParse(Convert.ToString(value), out date))
                return "Not set";

            return date.ToString("MMMM d, yyyy");
        }

        private string FormatCurrency(decimal value)
        {
            return "₱" + value.ToString("N2", CultureInfo.InvariantCulture);
        }

        private string GetBudgetProgressClass(string alertLevel)
        {
            switch (alertLevel)
            {
                case "Near": return "budget-progress-near";
                case "Over": return "budget-progress-over";
                default: return "budget-progress-ok";
            }
        }

        private string GetBudgetAlertText(string alertLevel, decimal remaining)
        {
            switch (alertLevel)
            {
                case "Over":
                    return "Budget exceeded by " + FormatCurrency(Math.Abs(remaining)) + ".";
                case "Near":
                    return "You are approaching the wedding budget.";
                case "NoBudget":
                    return "No wedding budget has been set.";
                default:
                    return "Budget is within the planned amount.";
            }
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/24-1528LibrandaMidLogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void ShowError(string message)
        {
            pnlError.Visible = true;
            litError.Text = HttpUtility.HtmlEncode(message);
            pnlSuccess.Visible = false;
        }

        protected void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            litSuccess.Text = HttpUtility.HtmlEncode(message);
            pnlError.Visible = false;
        }
    }
}
