<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Login.aspx.vb" Inherits="Account_Login" %>

<!DOCTYPE html>

<html lang="en" data-bs-theme="light">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <meta http-equiv="X-UA-Compatible" content="ie=edge"/>
    <title>Login - Online Order</title>
    <link href="../Content/dist/css/tabler.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-flags.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-payments.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/tabler-vendors.min.css?1692870487" rel="stylesheet"/>
    <link href="../Content/dist/css/demo.min.css?1692870487" rel="stylesheet"/>
    <link rel="icon" type="image/x-icon" href="../Content/static/new-favicon.png" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@tabler/icons-webfont@latest/dist/tabler-icons.min.css" />
    <style>
        .dropdown-toggle::after {display: none;}
    </style>
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.17.2/dist/sweetalert2.min.css" rel="stylesheet">
</head>
<body class=" d-flex flex-column">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server">
            <Scripts>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="respond" />
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
                <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
                <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
                <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
                <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
                <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
                <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
                <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
                <asp:ScriptReference Name="WebFormsBundle" />


                <asp:ScriptReference Path="~/Content/dist/js/demo-theme.min.js?1692870487" />
                <asp:ScriptReference Path="~/Content/dist/js/tabler.min.js?1692870487" />
                <asp:ScriptReference Path="~/Content/dist/js/demo.min.js?1692870487" />
                <asp:ScriptReference Path="~/Content/dist/js/demo-theme.min.js?1692870487" />
                <asp:ScriptReference Path="~/Scripts/jquery-3.7.0.min.js?1692870487" />
                <asp:ScriptReference Path="https://cdn.jsdelivr.net/npm/sweetalert2@11" />
            </Scripts>
        </asp:ScriptManager>

        <div class="page page-center">
            <div class="container mt-3">
                <div class="d-none d-md-flex float-end">
                    <a href="?theme=dark" class="nav-link px-0 hide-theme-dark" title="Enable dark mode"
                        data-bs-toggle="tooltip" data-bs-placement="bottom">
                        <i class="ti ti-moon fs-2" width="24" height="24"></i>
                    </a>
                    <a href="?theme=light" class="nav-link px-0 hide-theme-light" title="Enable light mode"
                        data-bs-toggle="tooltip" data-bs-placement="bottom">
                        <i class="ti ti-sun fs-2" width="24" height="24"></i>
                    </a>
                </div>
            </div>

            <div class="container container-tight py-4">

                <div class="mb-4 d-flex justify-content-center align-items-center">
                    <h1>ORDER BLINDS</h1>
                </div>

                <div class="card card-md">
                    <div class="card-body">
                        <h2 class="h2 text-center mb-4">Login to your account</h2>
                        <div class="mb-3">
                            <label class="form-label">Username</label>
                            <asp:TextBox runat="server" ID="txtUserLogin" CssClass="form-control " placeholder="User Name" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="mb-4">
                            <label class="form-label" runat="server" visible="true">
                                Password
                                <span class="form-label-description">
                                    <a runat="server" href="~/account/forgot">FORGOT PASSWORD</a>
                                </span>
                            </label>
                            
                            <div class="">
                                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control " placeholder="Your password" autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                        <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
                            <div class="d-flex">
                                <div>
                                    <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                </div>
                                <div>
                                    <span runat="server" id="msgError">I'm so sorry…</span>
                                </div>
                            </div>
                            <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                        </div>
                        <div class="form-footer">
                            <asp:Button runat="server" ID="btnLogin" CssClass="btn btn-primary w-100 " Text="Log In" OnClick="btnLogin_Click" />
                        </div>
                    </div>
                </div>

                <div class="text-center text-secondary mt-3">
                   Want to order a panorama shutter?, <a href="https://shutters.onlineorder.au/" tabindex="-1">click here.</a>
                </div>
            </div>
        </div>
        <script>
            document.querySelectorAll(".form-control, .form-select").forEach((el) => {
                el.addEventListener("input", (e) => {
                    e.target.classList.remove("is-invalid");
                })
            })

            const colourTablerTheme = () => {
                const theme = localStorage.getItem("tablerTheme") == "dark";
                return theme;
            }

            const isDark = colourTablerTheme();
            const showMessageError = (msg, input) => {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    html: msg,
                    customClass: {
                        popup: isDark ? "bg-dark text-white" : "bg-white text-dark"
                    }
                }).then(() => {
                    document.getElementById(input).focus();
                })

            }
        </script>
    </form>

</body>
</html>

