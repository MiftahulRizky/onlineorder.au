<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Forgot.aspx.vb" Inherits="Account_Forgot" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <meta http-equiv="X-UA-Compatible" content="ie=edge"/>
    <title>Login - Online Order</title>
    <link href="../Content/dist/css/tabler.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-flags.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-payments.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-vendors.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/demo.min.css?1692870487" rel="stylesheet" />
    <link rel="icon" type="image/x-icon" href="../favicon.ico" />
</head>
<body class=" d-flex flex-column">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Content/dist/js/demo-theme.min.js?1692870487" />
                <asp:ScriptReference Path="~/Content/dist/js/tabler.min.js?1692870487" />
                <asp:ScriptReference Path="~/Content/dist/js/demo.min.js?1692870487" />
            </Scripts>
        </asp:ScriptManager>

        <div runat="server" visible="false">
            <asp:Label runat="server" ID="lblUserId"></asp:Label>
            <asp:Label runat="server" ID="lblCode"></asp:Label>

            <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET ForgotCode=@Code WHERE UserId=@UserId">
                <UpdateParameters>
                    <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                    <asp:ControlParameter ControlID="lblCode" Name="Code" PropertyName="Text" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>

        <div class="page page-center mt-6">
            <div class="container container-tight py-4">
                <div class="card card-md mt-7">
                    <div class="card-body">
                        <h2 class="card-title text-center mb-4">Forgot password</h2>
                        <p class="text-secondary mb-4">Enter your email address and your password will be reset and emailed to you.</p>
                        <div class="mb-3">
                            <label class="form-label">Email address</label>
                            <asp:TextBox runat="server" TextMode="Email" ID="txtEmail" CssClass="form-control" placeholder="Email" autocomplete="off"></asp:TextBox>
                        </div>
                        <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
                            <div class="d-flex">
                                <div>
                                    <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                </div>
                                <div>
                                    <span runat="server" id="msgError"></span>
                                </div>
                            </div>
                            <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                        </div>

                        <div class="card-body border-bottom py-3" runat="server" id="divSuccess">
                            <div class="alert alert-important alert-success alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgSuccess"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>

                        <div class="form-footer">
                            <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary w-100" Text="Send me new password" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>

                <div class="text-center text-secondary mt-3">
                    Have you remembered your password? Try <a runat="server" href="~/account/login">logging</a> again.
                </div>
            </div>
        </div>
    </form>
</body>
</html>