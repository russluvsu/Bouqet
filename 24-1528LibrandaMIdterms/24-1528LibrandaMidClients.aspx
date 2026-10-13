<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="24-1528LibrandaMidClients.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidClients" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bouqet | Clients</title>

    <link href="Styles/CssPlannerDashboard.css" rel="stylesheet" />
    <link href="Styles/CssClients.css" rel="stylesheet" />
</head>

<body>

    <form id="form1" runat="server">

        <div class="dashboard-shell" id="dashboardShell">

            <aside class="sidebar" id="sidebar">

                <div class="sidebar-logo">

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

                    <span class="logo-word">
                        Bouqet
                    </span>

                </div>

<nav class="sidebar-nav">

    <a class="nav-item"
       href="24-1528LibrandaMidPlannerDashboard.aspx"
       title="Dashboard">

        <span class="nav-icon">D</span>
        <span class="nav-label">Dashboard</span>

    </a>

    <a class="nav-item active"
       href="24-1528LibrandaMidClients.aspx"
       title="Clients">

        <span class="nav-icon">C</span>
        <span class="nav-label">Clients</span>

    </a>

    <a class="nav-item"
       href="24-1528LibrandaMidPlannerDashboard.aspx#weddings"
       title="Weddings">

        <span class="nav-icon">W</span>
        <span class="nav-label">Weddings</span>

    </a>

    <a class="nav-item"
       href="24-1528LibrandaMidPlannerDashboard.aspx#tasks"
       title="Tasks">

        <span class="nav-icon">T</span>
        <span class="nav-label">Tasks</span>

    </a>

    <a class="nav-item"
       href="24-1528LibrandaMidPlannerDashboard.aspx#budget"
       title="Budget">

        <span class="nav-icon">B</span>
        <span class="nav-label">Budget</span>

    </a>

    <a class="nav-item"
       href="24-1528LibrandaMidPlannerDashboard.aspx#requests"
       title="Requests">

        <span class="nav-icon">R</span>
        <span class="nav-label">Requests</span>

    </a>

    <a class="nav-item"
       href="24-1528LibrandaMidPlannerDashboard.aspx#portfolio"
       title="Portfolio">

        <span class="nav-icon">P</span>
        <span class="nav-label">Portfolio</span>

    </a>

</nav>

                <div class="sidebar-bottom">

                    <button type="button"
                            class="collapse-button"
                            id="collapseButton"
                            title="Collapse sidebar">

                        <span class="collapse-arrow">‹</span>
                        <span class="collapse-label">Collapse</span>

                    </button>

                    <asp:LinkButton
                        ID="btnSignOut"
                        runat="server"
                        CssClass="nav-item signout-item"
                        OnClick="btnSignOut_Click"
                        ToolTip="Sign out">

                        <span class="nav-icon">S</span>
                        <span class="nav-label">Sign out</span>

                    </asp:LinkButton>

                </div>

            </aside>

            <main class="main-content">

                <header class="top-header">

                    <div class="heading-area">

                        <div class="eyebrow">
                            Clients
                        </div>

                        <h1>
                            Your clients
                        </h1>

                        <p>
                            Manage the couples and wedding plans assigned to you.
                        </p>

                    </div>

                    <div class="header-actions">

                        <a href="24-1528LibrandaMidAddClient.aspx"
                           class="add-client-button">

                            <span class="plus-icon">+</span>
                            <span>Add client</span>

                        </a>

                    </div>

                </header>

                <asp:Panel
                    ID="pnlError"
                    runat="server"
                    CssClass="message error-message"
                    Visible="false">

                    <span class="message-icon">
                        !
                    </span>

                    <asp:Literal
                        ID="litError"
                        runat="server" />

                </asp:Panel>

                <section class="clients-section">

                    <div class="section-heading">

                        <div>

                            <span class="section-kicker">
                                CLIENT LIST
                            </span>

                            <h2>
                                Active wedding plans
                            </h2>

                        </div>

                        <span class="client-count">

                            <asp:Literal
                                ID="litClientCount"
                                runat="server" />

                        </span>

                    </div>

                    <asp:Panel
                        ID="pnlEmpty"
                        runat="server"
                        CssClass="empty-state"
                        Visible="false">

                        <div class="empty-mark">
                            B
                        </div>

                        <h2>
                            No clients yet
                        </h2>

                        <p>
                            Add your first couple to start building their wedding plan.
                        </p>

                        <a href="24-1528LibrandaMidAddClient.aspx"
                           class="empty-button">

                            Add your first client

                        </a>

                    </asp:Panel>

                    <asp:Panel
                        ID="pnlClients"
                        runat="server"
                        CssClass="clients-list"
                        Visible="false">

                        <asp:Repeater
                            ID="rptClients"
                            runat="server"
                            OnItemCommand="rptClients_ItemCommand">

                            <ItemTemplate>

                                <article class="client-row">

                                    <div class="client-main">

                                        <div class="client-info">

                                            <h3>
                                                <%# Server.HtmlEncode(
                                                    Convert.ToString(
                                                        Eval("DisplayName"))) %>
                                            </h3>

                                            <span class="client-email">
                                                <%# Server.HtmlEncode(
                                                    Convert.ToString(
                                                        Eval("Username"))) %>
                                            </span>

                                        </div>

                                    </div>

                                    <div class="client-details">

                                        <div class="detail">

                                            <span class="detail-label">
                                                Wedding
                                            </span>

                                            <span class="detail-value">
                                                <%# FormatDate(
                                                    Eval("WeddingDate")) %>
                                            </span>

                                        </div>

                                        <div class="detail">

                                            <span class="detail-label">
                                                Status
                                            </span>

                                            <span class="status-badge">
                                                <%# Server.HtmlEncode(
                                                    Convert.ToString(
                                                        Eval("Status"))) %>
                                            </span>

                                        </div>

                                        <div class="detail">

                                            <span class="detail-label">
                                                Account
                                            </span>

                                            <span class="status-badge">
                                                <%# Convert.ToBoolean(
                                                    Eval("IsActive"))
                                                    ? "Active"
                                                    : "Inactive" %>
                                            </span>

                                        </div>

                                    </div>

                                    <div class="client-action">

                                        <asp:LinkButton
                                            ID="btnOpenClient"
                                            runat="server"
                                            CssClass="open-button"
                                            CommandName="OpenClient"
                                            CommandArgument='<%# Eval("ClientId") %>'>

                                            <span>Open</span>
                                            <span>→</span>

                                        </asp:LinkButton>

                                    </div>

                                </article>

                            </ItemTemplate>

                        </asp:Repeater>

                    </asp:Panel>

                </section>

            </main>

        </div>

        <script src="Scripts/JsPlannerDashboard.js"></script>

    </form>

</body>
</html>