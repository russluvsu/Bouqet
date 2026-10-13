<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="24-1528LibrandaMidChangePassword.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidChangePassword" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>Bouqet | Change Password</title>

    <link href="Styles/CssPlannerDashboard.css" rel="stylesheet" />
    <link href="Styles/CssChangePassword.css" rel="stylesheet" />

    <script src="Scripts/JsPlannerDashboard.js"></script>
</head>

<body>
    <form id="form1" runat="server">

        <div class="dashboard-layout">

            <aside class="sidebar">

                <div class="sidebar-brand">
                    <div class="brand-icon">B</div>

                    <div class="brand-text">
                        <span class="brand-name">Bouqet</span>
                        <span class="brand-subtitle">Wedding Planning</span>
                    </div>
                </div>

                <nav class="sidebar-nav">

                    <a href="24-1528LibrandaMidPlannerDashboard.aspx" class="nav-item">
                        <span class="nav-icon">⌂</span>
                        <span>Dashboard</span>
                    </a>

                    <a href="24-1528LibrandaMidClients.aspx" class="nav-item">
                        <span class="nav-icon">♧</span>
                        <span>Clients</span>
                    </a>

                </nav>

                <div class="sidebar-bottom">

                    <asp:LinkButton
                        ID="btnSignOut"
                        runat="server"
                        CssClass="nav-item sign-out"
                        OnClick="btnSignOut_Click">
                        <span class="nav-icon">↪</span>
                        <span>Sign Out</span>
                    </asp:LinkButton>

                </div>

            </aside>

            <main class="main-content">

                <header class="top-header">

                    <div>
                        <div class="page-kicker">ACCOUNT</div>
                        <h1>Change Password</h1>
                        <p class="page-subtitle">
                            Update your temporary password before continuing.
                        </p>
                    </div>

                </header>

                <section class="change-password-content">

                    <div class="password-card">

                        <div class="password-card-header">
                            <div>
                                <span class="section-label">FIRST LOGIN</span>

                                <h2>Create Your Password</h2>

                                <p>
                                    You're currently using a temporary password.
                                    Set a new password for your account to continue
                                    using Bouqet.
                                </p>
                            </div>
                        </div>

                        <asp:Panel
                            ID="pnlError"
                            runat="server"
                            CssClass="password-message password-error"
                            Visible="false">

                            <asp:Literal
                                ID="litError"
                                runat="server" />

                        </asp:Panel>

                        <asp:Panel
                            ID="pnlSuccess"
                            runat="server"
                            CssClass="password-message password-success"
                            Visible="false">

                            <asp:Literal
                                ID="litSuccess"
                                runat="server" />

                        </asp:Panel>

                        <div class="password-form">

                            <div class="form-field">

                                <asp:Label
                                    ID="lblCurrentPassword"
                                    runat="server"
                                    AssociatedControlID="txtCurrentPassword"
                                    Text="Temporary Password" />

                                <asp:TextBox
                                    ID="txtCurrentPassword"
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="dashboard-input"
                                    autocomplete="current-password" />

                            </div>

                            <div class="form-field">

                                <asp:Label
                                    ID="lblNewPassword"
                                    runat="server"
                                    AssociatedControlID="txtNewPassword"
                                    Text="New Password" />

                                <asp:TextBox
                                    ID="txtNewPassword"
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="dashboard-input"
                                    autocomplete="new-password" />

                            </div>

                            <div class="form-field">

                                <asp:Label
                                    ID="lblConfirmPassword"
                                    runat="server"
                                    AssociatedControlID="txtConfirmPassword"
                                    Text="Confirm New Password" />

                                <asp:TextBox
                                    ID="txtConfirmPassword"
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="dashboard-input"
                                    autocomplete="new-password" />

                            </div>

                            <div class="password-requirement">
                                <span class="requirement-mark">✓</span>
                                <span>Password must contain at least 8 characters.</span>
                            </div>

                            <div class="password-actions">

                                <asp:Button
                                    ID="btnChangePassword"
                                    runat="server"
                                    Text="Set New Password"
                                    CssClass="dashboard-primary-button"
                                    OnClick="btnChangePassword_Click" />

                            </div>

                        </div>

                    </div>

                </section>

            </main>

        </div>

    </form>
</body>
</html>