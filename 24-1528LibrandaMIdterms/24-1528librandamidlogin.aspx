    <%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="24-1528LibrandaMidLogin.aspx.cs" Inherits="Bouqet._24_1528LibrandaMidLogin" %>

    <!DOCTYPE html>
    <html lang="en">
    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <title>Bouqet | Sign in</title>
        <link rel="stylesheet" href="Styles/Csslogin.css" />
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="on">
            <main class="login">

                <%-- Brand panel (becomes a small header on narrow screens) --%>
                <section class="brand">
                    <svg class="brand-logo" viewBox="140 24 96 176" xmlns="http://www.w3.org/2000/svg" aria-hidden="true" focusable="false">
                        <path d="M150 120 L150 190" fill="none" stroke="#F2D27A" stroke-width="7" stroke-linecap="round" />
                        <path d="M150 50 L150 120" fill="none" stroke="#FFFFFF" stroke-width="7" stroke-linecap="round" />
                        <circle cx="188" cy="88" r="38" fill="none" stroke="#FFFFFF" stroke-width="7" />
                        <circle cx="188" cy="152" r="38" fill="none" stroke="#F2D27A" stroke-width="7" />
                        <path d="M159.76 113.43 A38 38 0 0 0 176.89 124.34" fill="none" stroke="#FFFFFF" stroke-width="7" stroke-linecap="round" />
                        <polygon points="181,28 195,28 204,38 188,54 172,38" fill="#F2D27A" stroke="#F2D27A" stroke-width="1" stroke-linejoin="round" />
                        <path d="M172 38 L204 38 M181 28 L180 38 M195 28 L196 38 M180 38 L188 54 M196 38 L188 54" fill="none" stroke="#C07F8E" stroke-width="1.6" stroke-linejoin="round" />
                    </svg>
                    <p class="brand-name">Bouqet</p>
                    <hr class="brand-rule" />
                    <p class="brand-tagline">Every detail, beautifully planned.</p>
                </section>

                <%-- Sign-in form --%>
                <section class="panel">
                    <div class="panel-inner">
                        <span class="gold-rule"></span>
                        <h1 id="heading" runat="server" class="heading">Welcome back</h1>
                        <p id="subtext" runat="server" class="subtext">Sign in to follow your wedding plans.</p>

                        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert" role="alert">
                            <asp:Literal ID="litError" runat="server" />
                        </asp:Panel>

                        <div class="field">
                            <label for="txtEmail">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email"
                                MaxLength="150" autocomplete="username" required="required" />
                        </div>

                        <div class="field">
                            <label for="txtPassword">Password</label>
                            <div class="input-wrap">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password"
                                    MaxLength="128" autocomplete="current-password" required="required" />
                                <button type="button" id="togglePassword" class="show-btn" aria-pressed="false">Show</button>
                            </div>
                        </div>

                        <div class="row">
                            <asp:CheckBox ID="chkRemember" runat="server" Text="Remember me" CssClass="check" />
                            <button type="button" id="forgotButton" class="link-btn" aria-expanded="false" aria-controls="forgotNote">Forgot password?</button>
                        </div>

                        <p id="forgotNote" class="forgot-note" hidden></p>

                        <asp:Button ID="btnSignIn" runat="server" Text="Sign in" CssClass="btn-primary" OnClick="btnSignIn_Click" />

                        <div class="footer-row">
                            <p id="footNote" runat="server" class="foot-note">Your planner will give you your login details.</p>
                            <button type="button" id="roleToggle" class="toggle" role="switch" aria-checked="false" aria-label="Planner sign in">
                                <span class="toggle-label" id="roleLabel">Client</span>
                                <span class="toggle-knob"></span>
                            </button>
                        </div>

                        <asp:HiddenField ID="hfMode" runat="server" Value="client" />
                    </div>
                </section>

            </main>
        </form>
        <script src="Scripts/Jslogin.js"></script>
    </body>
    </html>
