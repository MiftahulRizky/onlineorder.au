<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Password.aspx.vb" Inherits="Account_Password" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Change Password" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Change Password</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-6 col-sm-12 col-md-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" runat="server" id="cardTitle">Password Form</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-4 col-form-label required">New Password</label>
                                <div class="col-lg-8 col-sm-12 col-md-12">
                                    <asp:TextBox runat="server" ID="txtNewPass" TextMode="Password" CssClass="form-control" placeholder="Password ....."></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-4 col-form-label required">Confirm New Password</label>
                                <div class="col-lg-8 col-sm-12 col-md-12">
                                    <asp:TextBox runat="server" ID="txtCNewPass" TextMode="Password" CssClass="form-control" placeholder="Confirm New Password ....."></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <div class="col-lg-8 offset-lg-4">
                                    <label for="chkShowPass" style="cursor: pointer;">
                                        <input type="checkbox" id="chkShowPass" onclick="togglePassword()"> Show Password
                                    </label>
                                </div>
                            </div>

                            <div class="row" runat="server" id="divError">
                                <div class="col-12">
                                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
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
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 col-sm-12 col-md-12">

                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblLoginId"></asp:Label>
        <asp:Label runat="server" ID="lblPasswordHash"></asp:Label>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE CustomerLogins SET Password=@Password, Reset=0 WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblLoginId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPasswordHash" Name="Password" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>

    <div class="modal modal-blur fade" id="modalSuccess" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-green icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" /><path d="M9 12l2 2l4 -4" /></svg>
                    <h3>Successfully</h3>
                    <div class="text-secondary">PASSWORD SUCCESSFULLY UPDATED</div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="javascript:void(0);" id="vieworder" class="btn btn-success w-100" data-bs-dismiss="modal">CLOSE</a></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            loaderFadeOut();
        })
        document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
            document.activeElement.blur();
            document.body.focus();
        });
        function showSuccess() {
            $('#modalSuccess').modal('show');
        }

        $(document).on('hidden.bs.modal', '#modalSuccess', function () {
            window.location.href = "/";
        });

        $("#vieworder").on("click", () => window.location.href = "/");

        function togglePassword() {
            var newPass = document.getElementById('<%= txtNewPass.ClientID %>');
            var confirmPass = document.getElementById('<%= txtCNewPass.ClientID %>');
            var checkBox = document.getElementById('chkShowPass');

            if (checkBox.checked) {
                newPass.type = "text";
                confirmPass.type = "text";
            } else {
                newPass.type = "password";
                confirmPass.type = "password";
            }
        }
    </script>
</asp:Content>
