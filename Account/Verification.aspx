<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Verification.aspx.vb" Inherits="Account_Verification" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <meta http-equiv="X-UA-Compatible" content="ie=edge"/>
    <title>Verification - My Online Order</title>
    <link href="../Content/dist/css/tabler.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-flags.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-payments.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-vendors.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/demo.min.css?1692870487" rel="stylesheet"/>
    <link rel="icon" type="image/x-icon" href="../Content/static/favicon.ico" />
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
            <asp:Label runat="server" ID="lblPasswordHash"></asp:Label>
            

            <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET Password='123456', PasswordHash=@PasswordHash WHERE UserId=@UserId">
                <UpdateParameters>
                    <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                    <asp:ControlParameter ControlID="lblPasswordHash" Name="PasswordHash" PropertyName="Text" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>

        <div class="page page-center mt-7">
            <div class="container container-tight py-4">
                <div class="card card-md">
                    <div class="card-body">
                        <h2 class="card-title card-title-lg text-center mb-4">Authenticate Your Account</h2>
                        <p class="my-4 text-center">Please confirm your account by entering the authorization code sent to <strong runat="server" id="thisEmail"></strong>.</p>
                        <div class="my-5">
                            <div class="row g-4">
                                <div class="col">
                                    <div class="row g-2">
                                        <div class="col">
                                            <input type="text" runat="server" id="i1" class="form-control form-control-lg text-center py-3" maxlength="1" inputmode="numeric" pattern="[0-9]*" data-code-input />
                                        </div>
                                        <div class="col">
                                            <input type="text" runat="server" id="i2" class="form-control form-control-lg text-center py-3" maxlength="1" inputmode="numeric" pattern="[0-9]*" data-code-input />
                                        </div>
                                        <div class="col">
                                            <input type="text" runat="server" id="i3" class="form-control form-control-lg text-center py-3" maxlength="1" inputmode="numeric" pattern="[0-9]*" data-code-input />
                                        </div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="row g-2">
                                        <div class="col">
                                            <input type="text" runat="server" id="i4" class="form-control form-control-lg text-center py-3" maxlength="1" inputmode="numeric" pattern="[0-9]*" data-code-input />
                                        </div>
                                        <div class="col">
                                            <input type="text" runat="server" id="i5" class="form-control form-control-lg text-center py-3" maxlength="1" inputmode="numeric" pattern="[0-9]*" data-code-input />
                                        </div>
                                        <div class="col">
                                            <input type="text" runat="server" id="i6" class="form-control form-control-lg text-center py-3" maxlength="1" inputmode="numeric" pattern="[0-9]*" data-code-input />
                                        </div>
                                    </div>
                                </div>
                            </div>
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
                        
                        <div class="form-footer">
                            <div class="btn-list flex-nowrap">
                                <asp:Button runat="server" ID="btnCancel" CssClass="btn w-100" Text="Cancel" OnClick="btnCancel_Click"/>
                                <asp:Button runat="server" ID="btnVerify" CssClass="btn btn-primary w-100" Text="Verify" OnClick="btnVerify_Click" />
                            </div>
                        </div>
                    </div>                    
                </div>

                <div class="text-center text-secondary mt-3">
                    It may take a minute to receive your code.<br />
                    Haven't received it? <a href="./">Resend a new code.</a>
                </div>
            </div>
        </div>
        <script>
            document.addEventListener("DOMContentLoaded", function () {
                var inputs = document.querySelectorAll('[data-code-input]');
                for (let i = 0; i < inputs.length; i++) {
                    inputs[i].addEventListener('input', function (e) {
                        if (e.target.value.length === e.target.maxLength && i + 1 < inputs.length) {
                            inputs[i + 1].focus();
                        }
                    });

                    inputs[i].addEventListener('keydown', function (e) {
                        if (e.target.value.length === 0 && e.keyCode === 8 && i > 0) {
                            inputs[i - 1].focus();
                        }
                    });
                }
            });
        </script>
    </form>
</body>
</html>