<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="24-1528LibrandaMidPlannerDashboard.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidPlannerDashboard" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>Bouqet | Planner Dashboard</title>

    <link href="Styles/CssPlannerDashboard.css" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server">

        <aside class="sidebar" id="sidebar">

            <div class="sidebar-brand">
                <div class="logo-mark">
                    <svg viewBox="140 24 96 176" xmlns="http://www.w3.org/2000/svg">
                        <path d="M150 120 L150 190" fill="none" stroke="#C9A24B" stroke-width="7" stroke-linecap="round"/>
                        <path d="M150 50 L150 120" fill="none" stroke="#C98A97" stroke-width="7" stroke-linecap="round"/>
                        <circle cx="188" cy="88" r="38" fill="none" stroke="#C98A97" stroke-width="7"/>
                        <circle cx="188" cy="152" r="38" fill="none" stroke="#C9A24B" stroke-width="7"/>
                        <path d="M159.76 113.43 A38 38 0 0 0 176.89 124.34" fill="none" stroke="#C98A97" stroke-width="7" stroke-linecap="round"/>
                        <polygon points="181,28 195,28 204,38 188,54 172,38" fill="#C9A24B" stroke="#C9A24B" stroke-width="1" stroke-linejoin="round"/>
                        <path d="M172 38 L204 38 M181 28 L180 38 M195 28 L196 38 M180 38 L188 54 M196 38 L188 54" fill="none" stroke="#FFFFFF" stroke-width="1.6" stroke-linejoin="round"/>
                    </svg>
                </div>

                <div class="brand-text">
                    <span>Bouqet</span>
                    <small>Planner</small>
                </div>
            </div>

            <nav class="sidebar-nav">

                <a href="#dashboard"
                   class="nav-item active"
                   data-section="dashboard">
                    <span class="nav-icon">D</span>
                    <span class="nav-label">Dashboard</span>
                </a>

                <a href="24-1528LibrandaMidClients.aspx"
                   class="nav-item">
                    <span class="nav-icon">C</span>
                    <span class="nav-label">Clients</span>
                </a>

                <a href="#weddings"
                   class="nav-item"
                   data-section="weddings">
                    <span class="nav-icon">W</span>
                    <span class="nav-label">Weddings</span>
                </a>

                <a href="#tasks"
                   class="nav-item"
                   data-section="tasks">
                    <span class="nav-icon">T</span>
                    <span class="nav-label">Tasks</span>
                </a>

                <a href="#budget"
                   class="nav-item"
                   data-section="budget">
                    <span class="nav-icon">B</span>
                    <span class="nav-label">Budget</span>
                </a>

                <a href="#requests"
                   class="nav-item"
                   data-section="requests">
                    <span class="nav-icon">R</span>
                    <span class="nav-label">Requests</span>
                </a>

                <a href="#portfolio"
                   class="nav-item"
                   data-section="portfolio">
                    <span class="nav-icon">P</span>
                    <span class="nav-label">Portfolio</span>
                </a>

            </nav>

            <div class="sidebar-bottom">
                <asp:LinkButton
                    ID="btnSignOut"
                    runat="server"
                    CssClass="nav-item signout-item"
                    OnClick="btnSignOut_Click">
                    <span class="nav-icon">S</span>
                    <span class="nav-label">Sign out</span>
                </asp:LinkButton>
            </div>

        </aside>

        <main class="dashboard-main">

            <section id="dashboard" class="dashboard-section dashboard-hero">

                <div class="hero-content">
                    <span class="hero-eyebrow">Planner workspace</span>

                    <h1>
                        <asp:Literal ID="litGreeting" runat="server" />
                    </h1>

                    <p>
                        <asp:Literal ID="litSubtext" runat="server" />
                    </p>
                </div>

                <div class="hero-decoration">
                    <div class="hero-ring hero-ring-one"></div>
                    <div class="hero-ring hero-ring-two"></div>
                    <div class="hero-diamond">◆</div>
                </div>

            </section>

            <section class="overview-grid">

                <article class="overview-card">
                    <div class="overview-card-top">
                        <span class="card-label">Active weddings</span>
                        <span class="overview-icon">W</span>
                    </div>

                    <strong>
                        <asp:Literal ID="litTotalWeddings" runat="server" />
                    </strong>

                    <span class="card-detail">Currently in planning</span>
                </article>

                <article class="overview-card">
                    <div class="overview-card-top">
                        <span class="card-label">Total tasks</span>
                        <span class="overview-icon">T</span>
                    </div>

                    <strong>
                        <asp:Literal ID="litTotalTasks" runat="server" />
                    </strong>

                    <span class="card-detail">Across active weddings</span>
                </article>

                <article class="overview-card">
                    <div class="overview-card-top">
                        <span class="card-label">Completed tasks</span>
                        <span class="overview-icon">✓</span>
                    </div>

                    <strong>
                        <asp:Literal ID="litCompletedTasks" runat="server" />
                    </strong>

                    <span class="card-detail">
                        <asp:Literal ID="litTaskPercentage" runat="server" />
                        complete
                    </span>
                </article>

                <article class="overview-card">
                    <div class="overview-card-top">
                        <span class="card-label">Total budget</span>
                        <span class="overview-icon">₱</span>
                    </div>

                    <strong>
                        <asp:Literal ID="litTotalBudget" runat="server" />
                    </strong>

                    <span class="card-detail">Planned wedding budgets</span>
                </article>

                <article class="overview-card">
                    <div class="overview-card-top">
                        <span class="card-label">Pending requests</span>
                        <span class="overview-icon">R</span>
                    </div>

                    <strong>
                        <asp:Literal ID="litPendingRequests" runat="server" />
                    </strong>

                    <span class="card-detail">Awaiting attention</span>
                </article>

            </section>

            <section id="weddings" class="dashboard-section weddings-section">

                <div class="section-heading">
                    <div>
                        <span class="section-eyebrow">Your couples</span>
                        <h2>Active weddings</h2>
                        <p>Everything currently in your planning pipeline.</p>
                    </div>

                    <div class="section-count">
                        <span>Active</span>
                    </div>
                </div>

                <div class="weddings-grid">
                    <asp:Literal ID="litWeddings" runat="server" />
                </div>

            </section>

            <section id="tasks" class="dashboard-section analytics-section">

                <div class="section-heading">
                    <div>
                        <span class="section-eyebrow">Planning overview</span>
                        <h2>Task analytics</h2>
                        <p>Track how your active weddings are progressing.</p>
                    </div>
                </div>

                <div class="analytics-layout">

                    <div class="analytics-card task-summary-card">

                        <div class="analytics-card-header">
                            <div>
                                <span class="card-label">Overall completion</span>
                                <h3>Tasks</h3>
                            </div>

                            <span class="analytics-icon">✓</span>
                        </div>

                        <div class="donut-wrap">
                            <asp:Literal
                                ID="litTaskDonut"
                                runat="server" />
                        </div>

                        <div class="task-summary-stats">

                            <div>
                                <strong>
                                    <asp:Literal
                                        ID="litAnalyticsCompleted"
                                        runat="server" />
                                </strong>
                                <span>Completed</span>
                            </div>

                            <div>
                                <strong>
                                    <asp:Literal
                                        ID="litAnalyticsRemaining"
                                        runat="server" />
                                </strong>
                                <span>Remaining</span>
                            </div>

                            <div>
                                <strong>
                                    <asp:Literal
                                        ID="litAnalyticsOverdue"
                                        runat="server" />
                                </strong>
                                <span>Overdue</span>
                            </div>

                        </div>

                    </div>

                    <div class="analytics-card wedding-task-chart">

                        <div class="analytics-card-header">
                            <div>
                                <span class="card-label">By wedding</span>
                                <h3>Task progress</h3>
                            </div>

                            <span class="chart-caption">Completion</span>
                        </div>

                        <div class="horizontal-chart">
                            <asp:Literal
                                ID="litTaskChart"
                                runat="server" />
                        </div>

                    </div>

                </div>

            </section>

            <section id="budget" class="dashboard-section analytics-section">

                <div class="section-heading">

                    <div>
                        <span class="section-eyebrow">Financial overview</span>
                        <h2>Wedding budgets</h2>
                        <p>Keep each wedding's planned budget visible at a glance.</p>
                    </div>

                    <div class="section-total">
                        <span>Total planned</span>
                        <strong>
                            <asp:Literal
                                ID="litBudgetTotal"
                                runat="server" />
                        </strong>
                    </div>

                </div>

                <div class="budget-dashboard-card">

                    <div class="budget-card-header">

                        <div>
                            <span class="card-label">Budget allocation</span>
                            <h3>By wedding</h3>
                        </div>

                        <span class="budget-status">
                            <asp:Literal
                                ID="litBudgetStatus"
                                runat="server" />
                        </span>

                    </div>

                    <div class="budget-chart">
                        <asp:Literal
                            ID="litBudgetChart"
                            runat="server" />
                    </div>

                </div>

            </section>

            <section id="requests" class="dashboard-section analytics-section">

                <div class="section-heading">

                    <div>
                        <span class="section-eyebrow">Client communication</span>
                        <h2>Requests</h2>
                        <p>Stay on top of changes requested by your couples.</p>
                    </div>

                </div>

                <div class="request-dashboard-card">

                    <div class="request-main">

                        <div class="request-number">
                            <asp:Literal
                                ID="litRequestsTotal"
                                runat="server" />
                        </div>

                        <div>
                            <span class="card-label">Pending requests</span>

                            <h3>
                                <asp:Literal
                                    ID="litRequestsStatus"
                                    runat="server" />
                            </h3>
                        </div>

                    </div>

                    <div class="request-decoration">
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>

                </div>

            </section>

            <section id="timeline" class="dashboard-section timeline-section">

                <div class="section-heading">

                    <div>
                        <span class="section-eyebrow">Planning journey</span>
                        <h2>Timeline</h2>
                        <p>A visual overview of what's coming next.</p>
                    </div>

                </div>

                <div class="timeline-wrapper">
                    <asp:Literal
                        ID="litTimeline"
                        runat="server" />
                </div>

            </section>

            <section id="portfolio" class="dashboard-section portfolio-section">

                <div class="section-heading">

                    <div>
                        <span class="section-eyebrow">Your work</span>
                        <h2>Portfolio</h2>
                        <p>Showcase completed weddings and memorable events.</p>
                    </div>

                </div>

                <div class="portfolio-summary overview-grid">
                    <asp:Literal
                        ID="litPortfolioSummary"
                        runat="server" />
                </div>

            </section>

            <asp:Panel
                ID="pnlEmpty"
                runat="server"
                CssClass="empty-state"
                Visible="false">

                <div class="empty-state-icon">♡</div>

                <h2>No active weddings</h2>

                <p>
                    Once you have an active wedding assigned to you,
                    it will appear here.
                </p>

            </asp:Panel>

        </main>

    </form>

    <script src="Scripts/JsPlannerDashboard.js"></script>
</body>
</html>
