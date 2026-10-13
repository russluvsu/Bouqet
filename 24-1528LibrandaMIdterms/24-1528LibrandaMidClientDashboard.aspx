<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="24-1528LibrandaMidClientDashboard.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidClientDashboard" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>Bouqet | Client Dashboard</title>

    <link
        href="Styles/CssPlannerDashboard.css"
        rel="stylesheet" />

    <link
        href="Styles/CssClientDashboard.css"
        rel="stylesheet" />

</head>

<body>

<form
    id="form1"
    runat="server">

    <div class="client-dashboard-shell">

<aside class="planner-sidebar client-sidebar" id="clientSidebar">

    <div class="planner-brand">

        <span class="planner-brand-mark">

            <svg
                viewBox="140 24 96 176"
                xmlns="http://www.w3.org/2000/svg"
                aria-hidden="true">

                <path
                    d="M150 120 L150 190"
                    fill="none"
                    stroke="#C9A24B"
                    stroke-width="7"
                    stroke-linecap="round" />

                <path
                    d="M150 50 L150 120"
                    fill="none"
                    stroke="#C98A97"
                    stroke-width="7"
                    stroke-linecap="round" />

                <circle
                    cx="188"
                    cy="88"
                    r="38"
                    fill="none"
                    stroke="#C98A97"
                    stroke-width="7" />

                <circle
                    cx="188"
                    cy="152"
                    r="38"
                    fill="none"
                    stroke="#C9A24B"
                    stroke-width="7" />

                <path
                    d="M159.76 113.43 A38 38 0 0 0 176.89 124.34"
                    fill="none"
                    stroke="#C98A97"
                    stroke-width="7"
                    stroke-linecap="round" />

                <polygon
                    points="181,28 195,28 204,38 188,54 172,38"
                    fill="#C9A24B"
                    stroke="#C9A24B"
                    stroke-width="1"
                    stroke-linejoin="round" />

                <path
                    d="M172 38 L204 38 M181 28 L180 38 M195 28 L196 38 M180 38 L188 54 M196 38 L188 54"
                    fill="none"
                    stroke="#FFFFFF"
                    stroke-width="1.6"
                    stroke-linejoin="round" />

            </svg>

        </span>

        <div class="planner-brand-copy">

            <strong>
                Bouqet
            </strong>

            <span>
                Wedding planning
            </span>

        </div>

    </div>

    <button
        type="button"
        class="planner-sidebar-toggle"
        id="clientSidebarToggle"
        aria-label="Expand navigation"
        aria-expanded="false">

        <span class="sidebar-toggle-icon">
            ›
        </span>

    </button>

    <nav class="planner-nav">

        <a
            href="#dashboard"
            class="planner-nav-link active"
            data-section="dashboard">

            <span class="planner-nav-icon">
                D
            </span>

            <span>
                Dashboard
            </span>

        </a>

        <a
            href="#timeline"
            class="planner-nav-link"
            data-section="timeline">

            <span class="planner-nav-icon">
                T
            </span>

            <span>
                Timeline
            </span>

        </a>

        <a
            href="#budget"
            class="planner-nav-link"
            data-section="budget">

            <span class="planner-nav-icon">
                B
            </span>

            <span>
                Budget
            </span>

        </a>

        <a
            href="#tasks"
            class="planner-nav-link"
            data-section="tasks">

            <span class="planner-nav-icon">
                C
            </span>

            <span>
                Checklist
            </span>

        </a>

        <a
            href="#requests"
            class="planner-nav-link"
            data-section="requests">

            <span class="planner-nav-icon">
                R
            </span>

            <span>
                Requests
            </span>

        </a>

    </nav>

    <div class="planner-sidebar-bottom">

        <asp:LinkButton
            ID="btnSignOut"
            runat="server"
            CssClass="planner-signout"
            OnClick="btnSignOut_Click">

            <span class="planner-nav-icon">
                S
            </span>

            <span>
                Sign out
            </span>

        </asp:LinkButton>

    </div>

</aside>

        <main class="planner-main client-main">

            <section
                id="dashboard"
                class="client-section client-home-section">

                <div class="client-page-header client-home-header">

                    <div>

                        <span class="client-eyebrow">
                            YOUR WEDDING
                        </span>

                        <h1>
                            Hi
                            <asp:Literal
                                ID="litClientName"
                                runat="server" />
                        </h1>

                        <p>
                            Here's what's happening with your wedding.
                        </p>

                    </div>

                    <div class="client-days-pill">

                        <strong>
                            <asp:Literal
                                ID="litDaysToGo"
                                runat="server" />
                        </strong>

                        <span>
                            days to go
                        </span>

                    </div>

                </div>

                <asp:Panel
                    ID="pnlError"
                    runat="server"
                    CssClass="client-alert client-alert-error"
                    Visible="false">

                    <asp:Literal
                        ID="litError"
                        runat="server" />

                </asp:Panel>

                <asp:Panel
                    ID="pnlSuccess"
                    runat="server"
                    CssClass="client-alert client-alert-success"
                    Visible="false">

                    <asp:Literal
                        ID="litSuccess"
                        runat="server" />

                </asp:Panel>

                <section class="client-confirmation-card">

                    <div class="client-card-heading">

                        <div>

                            <span class="client-eyebrow">
                                NEEDS YOUR CONFIRMATION
                            </span>

                            <h2>
                                Review your shared tasks
                            </h2>

                        </div>

                        <span class="client-counter-chip">

                            <asp:Literal
                                ID="litConfirmationCount"
                                runat="server" />

                        </span>

                    </div>

                    <asp:Panel
                        ID="pnlNoConfirmations"
                        runat="server"
                        CssClass="client-empty-confirmation"
                        Visible="false">

                        <span class="client-empty-icon">
                            ✓
                        </span>

                        <div>

                            <strong>
                                You're all caught up
                            </strong>

                            <p>
                                Your planner hasn't sent anything that needs your attention yet.
                            </p>

                        </div>

                    </asp:Panel>

                    <asp:Repeater
                        ID="rptConfirmations"
                        runat="server">

                        <HeaderTemplate>

                            <div class="client-confirmation-list">

                        </HeaderTemplate>

                        <ItemTemplate>

                            <article class="client-confirmation-item">

                                <div class="client-confirmation-marker waiting">
                                </div>

                                <div class="client-confirmation-content">

                                    <span class="client-task-type">
                                        <%# HttpUtility.HtmlEncode(Convert.ToString(Eval("TaskType"))) %>
                                    </span>

                                    <h3>
                                        <%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))) %>
                                    </h3>

                                    <div class="client-confirmation-meta">

                                        <span>
                                            Shared
                                            <%# FormatDate(Eval("SharedDate")) %>
                                        </span>

                                        <span>
                                            Due
                                            <%# FormatDate(Eval("DueDate")) %>
                                        </span>

                                    </div>

                                </div>

                                <div class="client-confirmation-actions">

                                    <span class="client-status-chip waiting">
                                        Waiting for you
                                    </span>

                                    <a
                                        href="#tasks"
                                        class="client-text-action"
                                        data-section="tasks">

                                        Review task

                                    </a>

                                </div>

                            </article>

                        </ItemTemplate>

                        <FooterTemplate>

                            </div>

                        </FooterTemplate>

                    </asp:Repeater>

                </section>

                <section class="client-wedding-card">

                    <div class="client-wedding-card-main">

                        <span class="client-eyebrow">
                            WEDDING
                        </span>

                        <h2>

                            <asp:Literal
                                ID="litPartners"
                                runat="server" />

                        </h2>

                        <div class="client-wedding-meta">

                            <span>

                                <strong>
                                    Date
                                </strong>

                                <asp:Literal
                                    ID="litWeddingDate"
                                    runat="server" />

                            </span>

                            <span>

                                <strong>
                                    Venue
                                </strong>

                                <asp:Literal
                                    ID="litVenue"
                                    runat="server" />

                            </span>

                        </div>

                    </div>

                    <div class="client-wedding-status">

                        <span>
                            STATUS
                        </span>

                        <strong>

                            <asp:Literal
                                ID="litWeddingStatus"
                                runat="server" />

                        </strong>

                    </div>

                </section>

                <section class="client-progress-grid">

                    <article class="client-progress-card">

                        <div class="client-progress-ring">

                            <div class="client-progress-ring-inner">

                                <strong>
                                    <asp:Literal
                                        ID="litTaskProgress"
                                        runat="server" />
                                </strong>

                            </div>

                        </div>

                        <div>

                            <span>
                                TASKS
                            </span>

                            <strong>
                                Progress
                            </strong>

                        </div>

                    </article>

                    <article class="client-progress-card">

                        <div class="client-progress-ring budget">

                            <div class="client-progress-ring-inner">

                                <strong>
                                    <asp:Literal
                                        ID="litBudgetProgress"
                                        runat="server" />
                                </strong>

                            </div>

                        </div>

                        <div>

                            <span>
                                BUDGET
                            </span>

                            <strong>
                                Used
                            </strong>

                        </div>

                    </article>

                    <article class="client-progress-card">

                        <div class="client-progress-ring rsvp">

                            <div class="client-progress-ring-inner">

                                <strong>
                                    —
                                </strong>

                            </div>

                        </div>

                        <div>

                            <span>
                                RSVP
                            </span>

                            <strong>
                                Coming soon
                            </strong>

                        </div>

                    </article>

                </section>

            </section>

           
<section
    id="timeline"
    class="client-section client-timeline-section">

    <div class="client-section-header">
        <div>
            <span class="client-eyebrow">YOUR TIMELINE</span>
            <h2>Wedding timeline</h2>
            <p>A read-only view of the milestones your planner has shared with you.</p>
        </div>

        <div class="client-timeline-legend">
            <span>
                <i class="legend-dot completed"></i>
                Done
            </span>
            <span>
                <i class="legend-dot upcoming"></i>
                Coming up
            </span>
            <span>
                <i class="legend-dot waiting"></i>
                Waiting for you
            </span>
            <span>
                <i class="legend-diamond"></i>
                Wedding day
            </span>
        </div>
    </div>

    <div class="client-timeline-wrapper">
        <asp:Literal
            ID="litTimeline"
            runat="server" />
    </div>

</section>


            <section
                id="budget"
                class="client-section client-budget-section">

                <div class="client-section-header">

                    <div>

                        <span class="client-eyebrow">
                            BUDGET
                        </span>

                        <h2>
                            Your wedding budget
                        </h2>

                        <p>
                            Your spending is based on completed wedding tasks.
                        </p>

                    </div>

                    <strong class="client-section-value">

                        <asp:Literal
                            ID="litPercentUsed"
                            runat="server" />

                    </strong>

                </div>

                <div class="client-budget-card">

                    <div class="client-budget-summary">

                        <div>

                            <span>
                                TOTAL BUDGET
                            </span>

                            <strong>
                                <asp:Literal
                                    ID="litTotalBudget"
                                    runat="server" />
                            </strong>

                        </div>

                        <div>

                            <span>
                                SPENT
                            </span>

                            <strong>
                                <asp:Literal
                                    ID="litTotalSpent"
                                    runat="server" />
                            </strong>

                        </div>

                        <div>

                            <span>
                                REMAINING
                            </span>

                            <strong>
                                <asp:Literal
                                    ID="litRemaining"
                                    runat="server" />
                            </strong>

                        </div>

                    </div>

                    <div class="client-budget-track">

                        <div
                            id="budgetProgressFill"
                            runat="server"
                            class="client-budget-fill">
                        </div>

                    </div>

                </div>

            </section>

            <section
                id="tasks"
                class="client-section client-tasks-section">

                <div class="client-section-header">

                    <div>

                        <span class="client-eyebrow">
                            CHECKLIST
                        </span>

                        <h2>
                            Your wedding tasks
                        </h2>

                        <p>
                            Follow the tasks your planner has shared with you.
                        </p>

                    </div>

                    <div class="client-task-count">

                        <asp:Literal
                            ID="litTaskCount"
                            runat="server" />

                    </div>

                </div>

                <asp:Panel
                    ID="pnlNoTasks"
                    runat="server"
                    CssClass="client-no-tasks"
                    Visible="false">

                    <div class="client-no-tasks-icon">
                        ✓
                    </div>

                    <h3>
                        No shared tasks yet
                    </h3>

                    <p>
                        Your planner will add wedding tasks here when they're ready for you.
                    </p>

                </asp:Panel>

                <asp:Repeater
                    ID="rptTasks"
                    runat="server"
                    OnItemCommand="rptTasks_ItemCommand">

                    <HeaderTemplate>

                        <div
                            class="client-task-list"
                            id="clientTaskList">

                    </HeaderTemplate>

                    <ItemTemplate>

                        <article
                            class="client-task-card"
                            data-task-title='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Title"))) %>'
                            data-task-type='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("TaskType"))) %>'
                            data-task-status='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Status"))) %>'
                            data-task-due='<%# Eval("DueDate") == DBNull.Value ? "" : Convert.ToDateTime(Eval("DueDate")).ToString("yyyy-MM-dd") %>'>

                            <div class='<%# "client-task-marker " + GetTaskMarkerClass(Eval("Status")) %>'>
                                <span></span>
                            </div>

                            <div class="client-task-content">

                                <span class="client-task-type">

                                    <%# HttpUtility.HtmlEncode(Convert.ToString(Eval("TaskType"))) %>

                                </span>

                                <h3>

                                    <%# HttpUtility.HtmlEncode(Convert.ToString(Eval("Title"))) %>

                                </h3>

                                <div class="client-task-meta">

                                    <span>

                                        <strong>
                                            Due
                                        </strong>

                                        <%# FormatDate(Eval("DueDate")) %>

                                    </span>

                                    <span>

                                        <strong>
                                            Cost
                                        </strong>

                                        ₱<%# FormatTaskCost(Eval("Cost")) %>

                                    </span>

                                </div>

                            </div>

                            <div class="client-task-actions">

                                <span
                                    class='<%# "client-task-status " + GetTaskStatusClass(Eval("Status")) %>'>

                                    <%# GetTaskStatusText(Eval("Status")) %>

                                </span>

                                <div class="client-task-buttons">

                                    <asp:Button
                                        ID="btnRateTask"
                                        runat="server"
                                        Text="Rate task"
                                        CssClass="client-rate-button"
                                        CommandName="RateTask"
                                        CommandArgument='<%# Eval("TaskId") %>'
                                        Visible='<%# CanRateTask(Eval("Status"), Eval("IsRated")) %>' />

                                    <asp:Label
                                        ID="lblRated"
                                        runat="server"
                                        Text="✓ Rated"
                                        CssClass="client-rated-label"
                                        Visible='<%# IsTaskRated(Eval("IsRated")) %>' />

                                    <asp:Button
                                        ID="btnRequestChanges"
                                        runat="server"
                                        Text="Request changes"
                                        CssClass="client-request-button"
                                        CommandName="RequestChange"
                                        CommandArgument='<%# Eval("TaskId") %>'
                                        Visible='<%# CanRequestChanges(Eval("Status")) %>' />

                                </div>

                            </div>

                        </article>

                    </ItemTemplate>

                    <FooterTemplate>

                        </div>

                    </FooterTemplate>

                </asp:Repeater>

            </section>

            <section
                id="requests"
                class="client-section client-requests-section">

                <div class="client-section-header">

                    <div>

                        <span class="client-eyebrow">
                            REQUESTS
                        </span>

                        <h2>
                            Change requests
                        </h2>

                        <p>
                            When you ask your planner to change a completed task, it will be handled from here.
                        </p>

                    </div>

                </div>

                <div class="client-request-info-card">

                    <div class="client-request-info-icon">
                        R
                    </div>

                    <div>

                        <h3>
                            Need something changed?
                        </h3>

                        <p>
                            Open a completed task from your checklist and send your planner a change request.
                        </p>

                        <a
                            href="#tasks"
                            class="client-primary-link"
                            data-section="tasks">

                            View completed tasks

                        </a>

                    </div>

                </div>

            </section>

        </main>

    </div>

    <asp:Panel
        ID="pnlRating"
        runat="server"
        CssClass="client-modal-overlay"
        Visible="false">

        <div class="client-modal">

            <div class="client-modal-header">

                <div>

                    <span class="client-eyebrow">
                        HOW DID WE DO?
                    </span>

                    <h2>
                        Rate this task
                    </h2>

                </div>

                <asp:Button
                    ID="btnCancelRating"
                    runat="server"
                    Text="×"
                    CssClass="client-modal-close"
                    OnClick="btnCancelRating_Click" />

            </div>

            <asp:HiddenField
                ID="hiddenRatingTaskId"
                runat="server" />

            <div class="client-rating-field">

                <label>
                    How would you rate this task?
                </label>

                <asp:RadioButtonList
                    ID="rblRating"
                    runat="server"
                    RepeatDirection="Horizontal"
                    CssClass="client-rating-list">

                    <asp:ListItem Value="1">
                        ★
                    </asp:ListItem>

                    <asp:ListItem Value="2">
                        ★★
                    </asp:ListItem>

                    <asp:ListItem Value="3">
                        ★★★
                    </asp:ListItem>

                    <asp:ListItem Value="4">
                        ★★★★
                    </asp:ListItem>

                    <asp:ListItem Value="5">
                        ★★★★★
                    </asp:ListItem>

                </asp:RadioButtonList>

            </div>

            <div class="client-modal-field">

                <label for="txtRatingComment">

                    Comment

                    <span>
                        Optional
                    </span>

                </label>

                <asp:TextBox
                    ID="txtRatingComment"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="4"
                    MaxLength="500"
                    CssClass="client-modal-textarea"
                    placeholder="Tell your planner what you think...">
                </asp:TextBox>

            </div>

            <div class="client-modal-actions">

                <asp:Button
                    ID="btnCancelRatingBottom"
                    runat="server"
                    Text="Cancel"
                    CssClass="client-modal-secondary"
                    OnClick="btnCancelRating_Click" />

                <asp:Button
                    ID="btnSubmitRating"
                    runat="server"
                    Text="Submit rating"
                    CssClass="client-modal-primary"
                    OnClick="btnSubmitRating_Click" />

            </div>

        </div>

    </asp:Panel>

    <asp:Panel
        ID="pnlChangeRequest"
        runat="server"
        CssClass="client-modal-overlay"
        Visible="false">

        <div class="client-modal">

            <div class="client-modal-header">

                <div>

                    <span class="client-eyebrow">
                        REQUEST A CHANGE
                    </span>

                    <h2>
                        Request changes
                    </h2>

                </div>

                <asp:Button
                    ID="btnCancelChangeRequest"
                    runat="server"
                    Text="×"
                    CssClass="client-modal-close"
                    OnClick="btnCancelChangeRequest_Click" />

            </div>

            <asp:HiddenField
                ID="hiddenChangeTaskId"
                runat="server" />

            <div class="client-modal-field">

                <label for="txtChangeRequest">
                    What needs to be changed?
                </label>

                <asp:TextBox
                    ID="txtChangeRequest"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="6"
                    MaxLength="500"
                    CssClass="client-modal-textarea"
                    placeholder="Describe the changes you would like your planner to make...">
                </asp:TextBox>

            </div>

            <div class="client-modal-actions">

                <asp:Button
                    ID="btnCancelChangeRequestBottom"
                    runat="server"
                    Text="Cancel"
                    CssClass="client-modal-secondary"
                    OnClick="btnCancelChangeRequest_Click" />

                <asp:Button
                    ID="btnSubmitChangeRequest"
                    runat="server"
                    Text="Send request"
                    CssClass="client-modal-primary"
                    OnClick="btnSubmitChangeRequest_Click" />

            </div>

        </div>

    </asp:Panel>

<script>
    document.addEventListener("DOMContentLoaded", function () {

        const navLinks = document.querySelectorAll(".planner-nav-link");
        const sections = document.querySelectorAll(".client-section");

        function setActive(sectionId) {
            navLinks.forEach(function (link) {
                link.classList.toggle(
                    "active",
                    link.dataset.section === sectionId
                );
            });
        }

        function scrollToSection(sectionId) {
            const section = document.getElementById(sectionId);

            if (!section) {
                return;
            }

            section.scrollIntoView({
                behavior: "smooth",
                block: "start"
            });
        }

        navLinks.forEach(function (link) {
            link.addEventListener("click", function (event) {
                const sectionId = link.dataset.section;

                if (!sectionId) {
                    return;
                }

                event.preventDefault();

                setActive(sectionId);
                scrollToSection(sectionId);
            });
        });

        if ("IntersectionObserver" in window) {
            const observer = new IntersectionObserver(
                function (entries) {
                    let visibleSection = null;
                    let highestVisibility = 0;

                    entries.forEach(function (entry) {
                        if (
                            entry.isIntersecting &&
                            entry.intersectionRatio > highestVisibility
                        ) {
                            highestVisibility = entry.intersectionRatio;
                            visibleSection = entry.target.id;
                        }
                    });

                    if (visibleSection) {
                        setActive(visibleSection);
                    }
                },
                {
                    root: null,
                    rootMargin: "-20% 0px -55% 0px",
                    threshold: [0.1, 0.25, 0.5, 0.75]
                }
            );

            sections.forEach(function (section) {
                observer.observe(section);
            });
        }

        const sidebar = document.getElementById("clientSidebar");
        const sidebarToggle = document.getElementById("clientSidebarToggle");

        if (sidebar && sidebarToggle) {

            const sidebarStorageKey = "bouqetClientSidebar";

            function updateSidebarState() {
                const expanded = sidebar.classList.contains("expanded");

                sidebarToggle.setAttribute(
                    "aria-expanded",
                    expanded ? "true" : "false"
                );

                sidebarToggle.setAttribute(
                    "aria-label",
                    expanded
                        ? "Collapse navigation"
                        : "Expand navigation"
                );
            }

            const savedSidebarState =
                localStorage.getItem(sidebarStorageKey);

            if (savedSidebarState === "expanded") {
                sidebar.classList.add("expanded");
            }

            updateSidebarState();

            sidebarToggle.addEventListener("click", function () {

                const expanded =
                    sidebar.classList.toggle("expanded");

                localStorage.setItem(
                    sidebarStorageKey,
                    expanded
                        ? "expanded"
                        : "collapsed"
                );

                updateSidebarState();
            });

            function updateProgressRing(selector, valueElement) {
                const ring = document.querySelector(selector);
                const value = document.getElementById(valueElement);

                if (!ring || !value) {
                    return;
                }

                let percentage =
                    parseFloat(
                        value.textContent
                            .replace("%", "")
                            .trim()
                    );

                if (isNaN(percentage)) {
                    percentage = 0;
                }

                percentage = Math.max(0, Math.min(100, percentage));

                const angle = percentage * 3.6;

                ring.style.setProperty(
                    "--progress-angle",
                    angle + "deg"
                );
            }

            updateProgressRing(
                ".client-progress-ring:not(.budget)",
                "<%= litTaskProgress.ClientID %>"
            );

            updateProgressRing(
                ".client-progress-ring.budget",
                "<%= litBudgetProgress.ClientID %>"
            );
        }

    });
</script>

</form>

</body>

</html>