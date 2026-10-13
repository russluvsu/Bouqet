
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="24-1528LibrandaMidClientPersonal.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidClientPersonal" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Client | Bouqet</title>
    <link href="Styles/CssPlannerDashboard.css" rel="stylesheet" />
    <link href="Styles/CssClientPersonal.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">
    <div class="dashboard-shell">
        <aside class="sidebar" id="sidebar">
            <div class="sidebar-logo">
                <div class="logo-mark">
                    <svg viewBox="140 24 96 176" xmlns="http://www.w3.org/2000/svg">
                        <path d="M150 120 L150 190" fill="none" stroke="#C9A24B" stroke-width="7" stroke-linecap="round" />
                        <path d="M150 50 L150 120" fill="none" stroke="#C98A97" stroke-width="7" stroke-linecap="round" />
                        <circle cx="188" cy="88" r="38" fill="none" stroke="#C98A97" stroke-width="7" />
                        <circle cx="188" cy="152" r="38" fill="none" stroke="#C9A24B" stroke-width="7" />
                        <path d="M159.76 113.43 A38 38 0 0 0 176.89 124.34" fill="none" stroke="#C98A97" stroke-width="7" stroke-linecap="round" />
                        <polygon points="181,28 195,28 204,38 188,54 172,38" fill="#C9A24B" stroke="#C9A24B" stroke-width="1" stroke-linejoin="round" />
                        <path d="M172 38 L204 38 M181 28 L180 38 M195 28 L196 38 M180 38 L188 54 M196 38 L188 54" fill="none" stroke="#FFFFFF" stroke-width="1.6" stroke-linejoin="round" />
                    </svg>
                </div>
                <span class="logo-word">Bouqet</span>
            </div>

            <nav class="sidebar-nav">
                <a class="nav-item" href="24-1528LibrandaMidPlannerDashboard.aspx" title="Dashboard">
                    <span class="nav-icon">D</span><span class="nav-label">Dashboard</span>
                </a>
                <a class="nav-item active" href="24-1528LibrandaMidClients.aspx" title="Client">
                    <span class="nav-icon">C</span><span class="nav-label">Client</span>
                </a>
                <a class="nav-item" href="#tasks" title="Tasks">
                    <span class="nav-icon">T</span><span class="nav-label">Tasks</span>
                </a>
            </nav>

            <div class="sidebar-bottom">
                <button type="button" class="collapse-button" id="collapseButton" title="Collapse sidebar">
                    <span class="collapse-arrow">‹</span><span class="collapse-label">Collapse</span>
                </button>
                <asp:LinkButton ID="btnSignOut" runat="server" CssClass="nav-item signout-item" OnClick="btnSignOut_Click" ToolTip="Sign out">
                    <span class="nav-icon">S</span><span class="nav-label">Sign out</span>
                </asp:LinkButton>
            </div>
        </aside>

        <main class="main-content client-personal-main">
            <asp:Panel ID="pnlError" runat="server" CssClass="message-panel message-error" Visible="false">
                <span class="message-symbol">!</span><asp:Literal ID="litError" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="message-panel message-success" Visible="false">
                <span class="message-symbol">✓</span><asp:Literal ID="litSuccess" runat="server" />
            </asp:Panel>

            <section class="client-hero-section">
                <div class="client-hero">
                    <div class="client-hero-content">
                        <span class="page-eyebrow">CLIENT WORKSPACE</span>
                        <h1><asp:Literal ID="litClientName" runat="server" /></h1>
                        <p>A complete view of this couple's wedding plan.</p>
                        <asp:Button ID="btnEditClientDetails" runat="server" Text="Edit Client Details" CssClass="client-details-open-button" CausesValidation="false" OnClick="btnEditClientDetails_Click" />
                    </div>
                    <div class="client-hero-date">
                        <span class="hero-date-label">WEDDING DATE</span>
                        <strong><asp:Literal ID="litWeddingDate" runat="server" /></strong>
                        <span class="hero-status"><asp:Literal ID="litWeddingStatus" runat="server" /></span>
                    </div>
                </div>

                <div class="client-overview-grid">
                    <div class="client-overview-card overview-card-primary">
                        <span class="overview-icon">01</span>
                        <div><span class="overview-label">Partner One</span><strong><asp:Literal ID="litPartnerOne" runat="server" /></strong></div>
                    </div>
                    <div class="client-overview-card">
                        <span class="overview-icon">02</span>
                        <div><span class="overview-label">Partner Two</span><strong><asp:Literal ID="litPartnerTwo" runat="server" /></strong></div>
                    </div>
                    <div class="client-overview-card">
                        <span class="overview-icon">03</span>
                        <div><span class="overview-label">Wedding Date</span><strong><asp:Literal ID="litOverviewWeddingDate" runat="server" /></strong></div>
                    </div>
                    <div class="client-overview-card">
                        <span class="overview-icon">04</span>
                        <div><span class="overview-label">Wedding Status</span><strong><asp:Literal ID="litOverviewWeddingStatus" runat="server" /></strong></div>
                    </div>
                </div>
            </section>

            <asp:Panel ID="pnlClientDetailsForm" runat="server" CssClass="client-details-editor" Visible="false">
                <div class="client-details-editor-header">
                    <div>
                        <span class="section-eyebrow">CLIENT INFORMATION</span>
                        <h3>Edit Client Details</h3>
                        <p>Update the couple's names and wedding information.</p>
                    </div>
                </div>

                <div class="client-details-grid">
                    <div class="client-details-divider">Partner One</div>
                    <div class="form-field">
                        <label for="txtPartner1FirstName">First Name</label>
                        <asp:TextBox ID="txtPartner1FirstName" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>
                    <div class="form-field">
                        <label for="txtPartner1LastName">Last Name</label>
                        <asp:TextBox ID="txtPartner1LastName" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>

                    <div class="client-details-divider">Partner Two</div>
                    <div class="form-field">
                        <label for="txtPartner2FirstName">First Name</label>
                        <asp:TextBox ID="txtPartner2FirstName" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>
                    <div class="form-field">
                        <label for="txtPartner2LastName">Last Name</label>
                        <asp:TextBox ID="txtPartner2LastName" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>

                    <div class="client-details-divider">Wedding Information</div>
                    <div class="form-field">
                        <label for="txtClientWeddingDate">Wedding Date</label>
                        <asp:TextBox ID="txtClientWeddingDate" runat="server" CssClass="form-input" TextMode="Date" />
                    </div>
                    <div class="form-field">
                        <label for="txtEstimatedGuests">Estimated Guests</label>
                        <asp:TextBox ID="txtEstimatedGuests" runat="server" CssClass="form-input" TextMode="Number" />
                    </div>
                    <div class="form-field form-field-wide">
                        <label for="txtClientVenue">Venue</label>
                        <asp:TextBox ID="txtClientVenue" runat="server" CssClass="form-input" MaxLength="150" />
                    </div>
                </div>

                <p class="client-details-hint">Wedding status and budget are managed separately. Use the Wedding Budget editor below to change the budget.</p>

                <div class="client-details-actions">
                    <asp:Button ID="btnSaveClientDetails" runat="server" Text="Save Client Details" CssClass="primary-action-button" OnClick="btnSaveClientDetails_Click" />
                    <asp:Button ID="btnCancelClientDetails" runat="server" Text="Cancel" CssClass="secondary-action-button" CausesValidation="false" OnClick="btnCancelClientDetails_Click" />
                </div>
            </asp:Panel>

            <section class="budget-section">
                <div class="section-heading">
                    <div>
                        <span class="section-eyebrow">FINANCIAL OVERVIEW</span>
                        <h2>Wedding Budget</h2>
                        <p class="section-description">Keep track of the planned amount and current spending.</p>
                    </div>
                    <asp:Button ID="btnEditBudget" runat="server" Text="Edit Budget" CssClass="secondary-action-button" OnClick="btnEditBudget_Click" />
                </div>

                <asp:Panel ID="pnlBudgetForm" runat="server" CssClass="editor-card" Visible="false">
                    <div class="editor-heading">
                        <span class="section-eyebrow">BUDGET EDITOR</span>
                        <h3>Update wedding budget</h3>
                    </div>
                    <div class="task-form-grid">
                        <div class="form-field">
                            <label for="txtTotalBudget">Total Budget</label>
                            <asp:TextBox ID="txtTotalBudget" runat="server" CssClass="form-input" TextMode="Number" />
                        </div>
                    </div>
                    <div class="task-form-actions">
                        <asp:Button ID="btnSaveBudget" runat="server" Text="Save Budget" CssClass="primary-action-button" OnClick="btnSaveBudget_Click" />
                        <asp:Button ID="btnCancelBudget" runat="server" Text="Cancel" CssClass="secondary-action-button" OnClick="btnCancelBudget_Click" />
                    </div>
                </asp:Panel>

                <div class="budget-layout">
                    <div class="budget-main-card">
                        <div class="budget-main-header">
                            <div><span class="budget-card-label">TOTAL BUDGET</span><strong class="budget-total"><asp:Literal ID="litTotalBudget" runat="server" /></strong></div>
                            <div class="budget-used-badge"><asp:Literal ID="litPercentUsed" runat="server" /> used</div>
                        </div>
                        <div class="budget-progress-large">
                            <div class="budget-progress-track">
                                <div id="budgetProgressFill" runat="server" class="budget-progress-fill"></div>
                            </div>
                        </div>
                        <div class="budget-progress-footer"><span><asp:Literal ID="litBudgetAlert" runat="server" /></span></div>
                    </div>
                    <div class="budget-stats">
                        <div class="budget-stat-card"><span class="budget-stat-label">SPENT</span><strong><asp:Literal ID="litTotalSpent" runat="server" /></strong></div>
                        <div class="budget-stat-card budget-stat-remaining"><span class="budget-stat-label">REMAINING</span><strong><asp:Literal ID="litRemaining" runat="server" /></strong></div>
                    </div>
                </div>
            </section>

            <section class="tasks-section" id="tasks">
                <div class="section-heading tasks-heading">
                    <div>
                        <span class="section-eyebrow">WEDDING PLAN</span>
                        <h2>Tasks <span class="task-count"><asp:Literal ID="litTaskCount" runat="server" /></span></h2>
                        <p class="section-description">Plan, share, and manage every part of this wedding.</p>
                    </div>
                    <asp:Button ID="btnShowTaskForm" runat="server" Text="+ Add Task" CssClass="primary-action-button" CausesValidation="false" OnClick="btnShowTaskForm_Click" />
                </div>

                <asp:Panel ID="pnlTaskForm" runat="server" CssClass="editor-card task-editor" Visible="false">
                    <div class="editor-heading">
                        <span class="section-eyebrow">WEDDING PLAN</span>
                        <h3><asp:Literal ID="litTaskFormTitle" runat="server" Text="Add Task" /></h3>
                    </div>
                    <div class="task-form-grid">
                        <div class="form-field">
                            <label for="ddlTaskType">Task Type</label>
                            <asp:DropDownList ID="ddlTaskType" runat="server" CssClass="form-input" AutoPostBack="true" OnSelectedIndexChanged="ddlTaskType_SelectedIndexChanged">
                                <asp:ListItem Value="">Select task type</asp:ListItem>
                                <asp:ListItem Value="Venue">Venue</asp:ListItem>
                                <asp:ListItem Value="Catering">Catering</asp:ListItem>
                                <asp:ListItem Value="Photography">Photography</asp:ListItem>
                                <asp:ListItem Value="Videography">Videography</asp:ListItem>
                                <asp:ListItem Value="Attire">Attire</asp:ListItem>
                                <asp:ListItem Value="Flowers">Flowers</asp:ListItem>
                                <asp:ListItem Value="Music">Music</asp:ListItem>
                                <asp:ListItem Value="Decoration">Decoration</asp:ListItem>
                                <asp:ListItem Value="Transportation">Transportation</asp:ListItem>
                                <asp:ListItem Value="Other">Other</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-field">
                            <label for="txtTaskTitle">Task Title</label>
                            <asp:TextBox ID="txtTaskTitle" runat="server" CssClass="form-input" MaxLength="150" />
                        </div>
                        <div class="form-field">
                            <label for="txtCost">Cost</label>
                            <asp:TextBox ID="txtCost" runat="server" CssClass="form-input" TextMode="Number" />
                        </div>
                        <div class="form-field">
                            <label for="txtDueDate">Due Date</label>
                            <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-input" TextMode="Date" />
                        </div>
                        <div class="form-field form-field-wide">
                            <label for="txtPlannerNotes">Planner Notes</label>
                            <asp:TextBox ID="txtPlannerNotes" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="4" MaxLength="500" />
                        </div>
                    </div>

                    <div id="taskDetailsPanel" runat="server" class="task-details-panel" visible="false">
                        <div class="task-details-heading">
                            <span class="section-eyebrow">TASK DETAILS</span>
                            <h3>Additional Information</h3>
                            <p>Complete the required information before sharing this task with the client.</p>
                        </div>
                        <asp:Panel ID="pnlVenueDetails" runat="server" CssClass="task-details-fields" Visible="false">
                            <div class="form-field form-field-wide">
                                <label for="txtVenueAddress">Address</label>
                                <asp:TextBox ID="txtVenueAddress" runat="server" CssClass="form-input" MaxLength="300" />
                            </div>
                            <div class="form-field">
                                <label for="txtRentalStart">Rental Start</label>
                                <asp:TextBox ID="txtRentalStart" runat="server" CssClass="form-input" TextMode="Time" />
                            </div>
                            <div class="form-field">
                                <label for="txtRentalEnd">Rental End</label>
                                <asp:TextBox ID="txtRentalEnd" runat="server" CssClass="form-input" TextMode="Time" />
                            </div>
                            <div class="form-field">
                                <label for="txtVenuePax">Pax</label>
                                <asp:TextBox ID="txtVenuePax" runat="server" CssClass="form-input" TextMode="Number" />
                            </div>
                        </asp:Panel>
                    </div>

                    <div class="task-form-actions">
                        <asp:Button ID="btnSaveTask" runat="server" Text="Save Task" CssClass="primary-action-button" OnClick="btnSaveTask_Click" />
                        <asp:Button ID="btnCancelTask" runat="server" Text="Cancel" CssClass="secondary-action-button" CausesValidation="false" OnClick="btnCancelTask_Click" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlNoTasks" runat="server" CssClass="empty-state" Visible="false">
                    <div class="empty-state-mark">+</div>
                    <h3>No tasks yet</h3>
                    <p>Add the first task for this wedding plan.</p>
                </asp:Panel>

                <div class="task-list">
                    <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand">
                        <ItemTemplate>
                            <article class="task-card">
                                <div class="task-card-main">
                                    <div class="task-card-icon"><span>◆</span></div>
                                    <div class="task-card-content">
                                        <div class="task-card-topline">
                                            <span class="task-type-label"><%# HttpUtility.HtmlEncode(Convert.ToString(Eval("TaskType"))) %></span>
                                            <span class='<%# GetTaskStatusClass(Eval("Status"), Eval("IsComplete"), Eval("IsOverdue")) %>'><%# GetTaskStatusText(Eval("Status"), Eval("IsComplete"), Eval("IsOverdue")) %></span>
                                        </div>
                                        <h3><%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))) %></h3>
                                        <p class="task-card-notes"><%# HttpUtility.HtmlEncode(Convert.ToString(Eval("PlannerNotes"))) %></p>
                                        <div class="task-card-meta">
                                            <span><b>Cost</b> <%# FormatTaskCost(Eval("Cost")) %></span>
                                            <span><b>Due</b> <%# FormatTaskDate(Eval("DueDate")) %></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="task-card-actions">
                                    <asp:Button ID="btnEditTask" runat="server" Text="Edit" CssClass="task-action-button edit-task-button" CommandName="EditTask" CommandArgument='<%# Eval("TaskId") %>' Visible='<%# CanEditTask(Eval("Status")) %>' />
                                    <asp:Button ID="btnShareTask" runat="server" Text="Share" CssClass="task-action-button share-task-button" CommandName="ShareTask" CommandArgument='<%# Eval("TaskId") %>' Visible='<%# CanShareTask(Eval("Status")) %>' />
                                    <asp:Button ID="btnMarkDone" runat="server" Text="Mark as Done" CssClass="task-action-button done-task-button" CommandName="MarkDoneTask" CommandArgument='<%# Eval("TaskId") %>' Visible='<%# CanMarkTaskDone(Eval("Status")) %>' OnClientClick="return confirm('Mark this task as done? The client will then be able to rate it or request changes.');" />
                                    <asp:Button ID="btnRemoveTask" runat="server" Text="Remove" CssClass="task-action-button remove-task-button" CommandName="RemoveTask" CommandArgument='<%# Eval("TaskId") %>' OnClientClick="return confirm('Remove this task? This task will no longer count toward the wedding plan or budget.');" />
                                </div>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </main>
    </div>
</form>
<script src="Scripts/JsPlannerDashboard.js"></script>
</body>
</html>
