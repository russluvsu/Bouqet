<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="24-1528LibrandaMidAddClient.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidAddClient" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Add Client | Bouqet</title>
    <link rel="stylesheet" href="Styles/CssAddClient.css" />
</head>
<body>
    <form id="form1" runat="server">

        <div class="page-shell">

            <header class="page-header">
                <a href="24-1528LibrandaMidPlannerDashboard.aspx" class="back-link">
                    <span>←</span>
                    <span>Dashboard</span>
                </a>

                <div class="header-copy">
                    <span class="eyebrow">CLIENT SETUP</span>
                    <h1>Add a client</h1>
                    <p>Create the couple's account and start their wedding plan.</p>
                </div>
            </header>

            <asp:Panel ID="pnlError" runat="server" CssClass="message error-message" Visible="false">
                <span class="message-icon">!</span>
                <asp:Literal ID="litError" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlSuccess" runat="server" CssClass="message success-message" Visible="false">
                <span class="message-icon">✓</span>
                <asp:Literal ID="litSuccess" runat="server" />
            </asp:Panel>

            <section class="form-layout">

                <div class="form-card">
                    <div class="card-heading">
                        <span class="section-number">01</span>
                        <div>
                            <h2>The couple</h2>
                            <p>Enter the names exactly as you want them displayed.</p>
                        </div>
                    </div>

                    <div class="couple-grid">

                        <div class="partner-block">
                            <span class="partner-label">Partner 1</span>

                            <div class="field-row">
                                <div class="field">
                                    <label for="txtPartner1FirstName">First name</label>
                                    <asp:TextBox ID="txtPartner1FirstName" runat="server"
                                        CssClass="text-input"
                                        MaxLength="50" />
                                </div>

                                <div class="field">
                                    <label for="txtPartner1LastName">Last name</label>
                                    <asp:TextBox ID="txtPartner1LastName" runat="server"
                                        CssClass="text-input"
                                        MaxLength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="partner-block">
                            <span class="partner-label">Partner 2</span>

                            <div class="field-row">
                                <div class="field">
                                    <label for="txtPartner2FirstName">First name</label>
                                    <asp:TextBox ID="txtPartner2FirstName" runat="server"
                                        CssClass="text-input"
                                        MaxLength="50" />
                                </div>

                                <div class="field">
                                    <label for="txtPartner2LastName">Last name</label>
                                    <asp:TextBox ID="txtPartner2LastName" runat="server"
                                        CssClass="text-input"
                                        MaxLength="50" />
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="form-card">
                    <div class="card-heading">
                        <span class="section-number">02</span>
                        <div>
                            <h2>Client access</h2>
                            <p>Create the login credentials you will give to the couple.</p>
                        </div>
                    </div>

                    <div class="field">
                        <label for="txtUsername">Login email</label>
                        <asp:TextBox ID="txtUsername" runat="server"
                            CssClass="text-input"
                            TextMode="Email"
                            MaxLength="150" />
                        <span class="field-help">This will be their username.</span>
                    </div>

                    <div class="field">
                        <label for="txtTemporaryPassword">Temporary password</label>

                        <div class="password-row">
                            <asp:TextBox ID="txtTemporaryPassword" runat="server"
                                CssClass="text-input"
                                TextMode="Password"
                                MaxLength="128"
                                ClientIDMode="Static" />

                            <button type="button" class="generate-button" id="btnGeneratePassword">
                                Generate
                            </button>
                        </div>

                        <span class="field-help">
                            The client will be required to change this after their first login.
                        </span>
                    </div>
                </div>

                <div class="form-card">
                    <div class="card-heading">
                        <span class="section-number">03</span>
                        <div>
                            <h2>Wedding details</h2>
                            <p>These details start the couple's wedding timeline.</p>
                        </div>
                    </div>

                    <div class="field-row">
                        <div class="field">
                            <label for="txtWeddingDate">Wedding date</label>
                            <asp:TextBox ID="txtWeddingDate" runat="server"
                                CssClass="text-input"
                                TextMode="Date" />
                        </div>

                        <div class="field">
                            <label for="txtEstimatedGuests">Estimated guests</label>
                            <asp:TextBox ID="txtEstimatedGuests" runat="server"
                                CssClass="text-input"
                                TextMode="Number"
                                min="0"
                                MaxLength="6" />
                        </div>
                    </div>
                </div>

                <div class="form-actions">
                    <a href="24-1528LibrandaMidPlannerDashboard.aspx" class="cancel-button">
                        Cancel
                    </a>

                    <asp:Button ID="btnCreateClient" runat="server"
                        Text="Create client"
                        CssClass="create-button"
                        OnClick="btnCreateClient_Click" />
                </div>

            </section>

        </div>

    </form>

    <script src="Scripts/JsAddClient.js"></script>
</body>
</html>